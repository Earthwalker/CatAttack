using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Text JoinClientName;
	public Text JoinServerInfo;
	public Text HostClientName;
	public Text HostServerInfo;

	/// <summary>
	/// Starts the game
	/// </summary>
	public void Play()
	{
		// lock the cursor
		Screen.lockCursor = true;

		// hide the menu
		GetComponent<Canvas>().enabled = false;

		// enable pausing
		GameObject.Find("PauseCanvas").GetComponent<PauseMenu>().enabled = true;

		// set server/client variables
		Multiplayer multiplayer = GameObject.FindGameObjectWithTag("GameController").GetComponent<Multiplayer>();
		multiplayer.ClientName = JoinClientName.text;
		multiplayer.ServerInfo = JoinServerInfo.text;
		multiplayer.Connect(false);
	}

	public void Host()
	{
		// lock the cursor
		Screen.lockCursor = true;

		Debug.Log(Screen.lockCursor.ToString());

		// hide the menu
		GetComponent<Canvas>().enabled = false;

		// enable pausing
		GameObject.Find("PauseCanvas").GetComponent<PauseMenu>().enabled = true;

		// set server/client variables
		Multiplayer multiplayer = GameObject.FindGameObjectWithTag("GameController").GetComponent<Multiplayer>();
		multiplayer.ClientName = HostClientName.text;
		multiplayer.ServerInfo = HostServerInfo.text;
		multiplayer.Connect(true);
	}

	/// <summary>
	/// Exits the game
	/// </summary>
	public void Exit()
	{
		Application.Quit();

#if UNITY_EDITOR
		if (Application.isEditor)
			UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}