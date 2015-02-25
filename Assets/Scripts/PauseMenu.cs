using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Menu shown when the game is paused or focus is lost
/// Attached to PauseCanvas
/// </summary>
public class PauseMenu : MonoBehaviour
{
	/// <summary>
	/// Event called when we pause
	/// </summary>
	public event PauseHandler OnPause;

	/// <summary>
	/// Event raised when we resume
	/// </summary>
	public event PauseHandler OnResume;

	public delegate void PauseHandler(Component menu);

	/// <summary>
	/// Pauses the game
	/// </summary>
	public void Pause()
	{
		// don't do anything if we are already paused
		if (IsPaused())
			return;

		// show the canvas
		GetComponent<Canvas>().enabled = true;

		// make sure the cursor is unlocked so we can interact with the menu
        Screen.lockCursor = false;

		// raise our paused event
		if (OnPause != null)
			OnPause(this);

		Debug.Log("Paused");
	}

	/// <summary>
	/// Resumes the game
	/// </summary>
	public void Resume()
	{
		// don't do anything if we are not paused
		if (!IsPaused())
			return;

		// hide the canvas
		GetComponent<Canvas>().enabled = false;

		// lock the cursor once more
		Screen.lockCursor = true; //TODO: restore the lock state of the cursor before we paused

		// raise our resumed event
		if (OnResume != null)
			OnResume(this);

		Debug.Log("Resumed");
	}

	/// <summary>
	/// Leaves the current match
	/// </summary>
	public void Leave()
	{
		Debug.Log("Leaving match...");

		// restart the game
		Application.LoadLevel(Application.loadedLevel);
	}

	/// <summary>
	/// Returns whether the game is currently paused
	/// </summary>
	/// <returns></returns>
	public bool IsPaused()
	{
		return !enabled || GetComponent<Canvas>().enabled;
    }

	/// <summary>
	/// Called at every frame
	/// </summary>
	void Update()
	{
		// toggle the pause menu when escape is pressed
		if (Input.GetKeyDown(KeyCode.Escape))
        {
			// resume the game if the pause menu is already up
			if (GetComponent<Canvas>().enabled)
				Resume();
			else
				Pause();
		}
    }

	/// <summary>
	/// Executes when our game changes focus
	/// </summary>
	/// <param name="focusStatus">Whether the game is in focus</param>
	void OnApplicationFocus(bool focusStatus)
	{
		if (!focusStatus)
			Pause();
    }
}