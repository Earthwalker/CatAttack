using UnityEngine;

/// <summary>
/// Spectates a match in various modes
/// </summary>
public class SpectatorCamera : MonoBehaviour
{
	//target player
	GameObject target;

	//camera modes
	bool freecam = true;
	bool follow = false;
	bool firstPerson = true;

	/// <summary>
	/// Update loop
	/// </summary>
	void Update()
	{
		//use the jump button to switch camera modes
		if (Input.GetButtonDown("Jump"))
		{
			if (freecam)
			{
				freecam = false;
				follow = true;
				firstPerson = true;
			}
			else if (follow)
			{
				if (firstPerson)
					firstPerson = false;
				else
				{
					follow = false;
					freecam = true;
					target = null;
				}
			}
		}

		// free cam allows free movement
		if (freecam)
		{
			MoveForward(Input.GetAxisRaw("Vertical"));
			Strafe(Input.GetAxisRaw("Horizontal"));
		}

		// follow camera follows a specific player in third person or first
		if (follow)
		{
			// go to next player
			if (Input.GetMouseButtonDown(0))
				FindNextTarget();

			if (target != null)
			{
				if (firstPerson)
				{
					transform.position = target.transform.position;
					transform.rotation = target.transform.rotation;
				}
				else
				{
					//TODO: test to make sure third person works as intended
					Vector3 pos = target.transform.position;
					pos.y += 2;
					transform.position = pos;
					transform.rotation = target.transform.rotation;
				}
			}
			else
				FindNextTarget();
		}
	}

	/// <summary>
	/// Moves the camera 
	/// </summary>
	/// <param name="val">Value</param>
	void MoveForward(float val)
	{
		transform.position += val * transform.forward.normalized;
	}

	/// <summary>
	/// Strafes the camera
	/// </summary>
	/// <param name="val">Value</param>
	void Strafe(float val)
	{
		transform.position += val * transform.right.normalized;
	}

	/// <summary>
	/// Finds the next target to spec
	/// </summary>
	void FindNextTarget()
	{
		GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");

		for (int i = 0; i < playerArray.Length; i++)
		{
			if (target == null)
				target = playerArray[i];
			else if (target == playerArray[i])
			{
				if (i != playerArray.Length - 1)
					target = playerArray[i];
				else
					target = playerArray[0];
			}
		}
	}
}