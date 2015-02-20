using UnityEngine;
using System;
using System.Collections.Generic;

public class Multiplayer : MonoBehaviour
{
	#region inspector

	/// <summary>
	/// Player object to create
	/// </summary>
	public GameObject PlayerPrefab;

	/// <summary>
	/// The name we use for our player
	/// </summary>
	public string ClientName;

	/// <summary>
	/// The name or IP of the server we are connected to
	/// </summary>
	public string ServerInfo;

	/// <summary>
	/// Maximum number of connections we can have
	/// </summary>
	public int Connections;

	/// <summary>
	/// Port to use
	/// </summary>
	public int Port;

	/// <summary>
	/// Whether we should use NAT punch through
	/// </summary>
	public bool UseNat;

	#endregion

	/// <summary>
	/// Whether we have started the server or joined one
	/// </summary>
	public bool Connected { get; private set; }

	/// <summary>
	/// If we are using unofficial resources or configurations
	/// </summary>
	public bool Custom { get; private set; }

	public List<GameObject> Players { get; private set; }

	/// <summary>
	/// Initialize the networking
	/// </summary>
	public void Connect(bool isServer)
	{
		Players = new List<GameObject>();

		try
		{
			if (isServer)
				Network.InitializeServer(Connections, Port, UseNat);
			else
				Network.Connect(ServerInfo, Port);
        }
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	/// <summary>
	/// Shuts down the networking
	/// </summary>
	public void Shutdown()
	{
		Debug.Log("Shutting network down...");

		Connected = false;

		// disconnect from peers
		Network.Disconnect(500);
	}

	/// <summary>
	/// Called on the server whenever a Network.InitializeServer was invoked and has completed
	/// </summary>
	void OnServerInitialized()
	{
		if (!Connected)
		{
			Debug.Log("Server initialized");

			// update clients about ourself when they connect
			networkView.RPC("UpdateServerInfo", RPCMode.OthersBuffered, ServerInfo, Custom);

			// create the new player across the network
			networkView.RPC("NewPlayer", RPCMode.AllBuffered, ClientName, Network.AllocateViewID());

			Connected = true;
		}
	}

	/// <summary>
	/// Called on the server whenever a new player has successfully connected
	/// </summary>
	/// <param name="player"></param>
	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log("New player connecting from " + player.ipAddress + ":" + player.port + "...");
	}

	/// <summary>
	/// Called on the client when you have successfully connected to a server
	/// </summary>
	void OnConnectedToServer()
	{
		if (!Connected)
		{
			Debug.Log("Connecting to server...");

			// tell the server out name
			networkView.RPC("UpdateClientInfo", RPCMode.Server, ClientName, Network.AllocateViewID());
		}
	}

	/// <summary>
	/// Called on the server whenever a player disconnected from the server
	/// </summary>
	/// <param name="player"></param>
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		// check if this player is connected already
		if (!Players.Exists(p => p.networkView.owner == player))
			return;

		// find out the name of the player disconnecting
		string name = Players.Find(p => p.networkView.owner == player).name;

		// remove the player from our player list
		Players.Remove(Players.Find(p => p.name == name));

		// clean up after the disconnected player
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);

		// notify the network of the player who is disconnecting
		networkView.RPC("DestroyPlayer", RPCMode.AllBuffered, name, player);
	}

	/// <summary>
	/// Called on the client when the connection was lost or you disconnected from the server
	/// </summary>
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if (Connected)
		{
			Debug.Log("Disconnected from server");

			Shutdown();

			// restart the game
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	#region RPCs

	/// <summary>
	/// RPC to send server info to clients when they connect
	/// </summary>
	/// <param name="serverName"></param>
	/// <param name="custom"></param>
	[RPC]
	public void UpdateServerInfo(string serverName, bool custom)
	{
		ServerInfo = serverName;
		Custom = custom;

		Connected = true;

		Debug.Log("Connected to server " + serverName);
	}

	/// <summary>
	/// RPC to send client info to the server
	/// </summary>
	/// <param name="clientName"></param>
	[RPC]
	public void UpdateClientInfo(string clientName, NetworkViewID viewID)
	{
		// create the new player across the network
		networkView.RPC("NewPlayer", RPCMode.AllBuffered, clientName, viewID);

		Debug.Log(clientName + " connected");
	}

	/// <summary>
	/// RPC to create a new player
	/// </summary>
	/// <param name="name"></param>
	/// <param name="viewID"></param>
	[RPC]
	public void NewPlayer(string name, NetworkViewID viewID)
	{
		// create the new player
		var player = (GameObject)Instantiate(PlayerPrefab);
		player.name = name;
		player.networkView.viewID = viewID;

		// add the new player to our player list
			Players.Add(player);

        Debug.Log(name + " joined");
	}

	/// <summary>
	/// RPC to destroy a player
	/// </summary>
	/// <param name="name"></param>
	/// <param name="viewID"></param>
	[RPC]
	public void DestroyPlayer(string name, NetworkPlayer player)
	{
		// remove the player from our player list
		Players.Remove(Players.Find(p => p.name == name));

		// clean up after the disconnected player
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);

		Debug.Log(name + " disconnected");
	}

	#endregion
}