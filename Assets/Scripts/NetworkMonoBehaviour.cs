using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NetworkMonoBehaviour : MonoBehaviour
{
	/// <summary>
	/// Whether the script has been initialized
	/// </summary>
	public bool Initialized;

	/// <summary>
	/// Whether this script is attached to a local object
	/// </summary>
	public bool Local;

	/// <summary>
	/// Network view
	/// </summary>
	NetworkView MyNetworkView { get; set; }

	/// <summary>
	/// Initializes the script
	/// </summary>
	/// <param name="local"></param>
	public void Initialize(NetworkView view)
	{
		Local = view.isMine;
		MyNetworkView = view;

		// start the update coroutine
		StartCoroutine(ManagedUpdate());

		Initialized = true;
    }

	/// <summary>
	/// Can be called to stop updates
	/// </summary>
	public virtual void Stop()
	{
		StopCoroutine(ManagedUpdate());
	}

	/// <summary>
	/// Managed update loop
	/// </summary>
	/// <returns></returns>
	IEnumerator ManagedUpdate()
	{
		// continue until we are stopped
		while (true)
		{
			// how much time before the next update
			float time;
			
			// go to the appropriate update function
			if (Local)
				time = LocalUpdate();
			else
				time = RemoteUpdate();

			for (float i = 0; i <= time; i += Time.deltaTime)
				yield return 0;
		}
	}

	/// <summary>
	/// Update loop for local objects
	/// </summary>
	/// <returns>How much time before the next update</returns>
	public virtual float LocalUpdate()
	{
		return 0;
	}

	/// <summary>
	/// Update loop for remote objects
	/// </summary>
	/// <returns>How much time before the next update</returns>
	public virtual float RemoteUpdate()
	{
		return 0;
	}

	public void RPC(string name, RPCMode mode, params object[] args)
	{
		if (!Initialized)
			return;

		for (int i=0; i<args.Length; i++)
		{
			if (args[i].GetType() == typeof(Vector3))
			{
				Vector3 vector = (Vector3)args[i];
				args[i] = new Vector(vector.x, vector.y, vector.z);
			}
		}

        InheritableRPCExtensions.RPCEx(MyNetworkView, name, mode, args);
    }
}