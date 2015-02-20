using UnityEngine;
using System;

namespace CatAttack
{
	/// <summary>
	/// Handles player controls
	/// </summary>
	public class Controls : NetworkMonoBehaviour
	{
		/// <summary>
		/// Local update
		/// </summary>
		/// <returns></returns>
		public override float LocalUpdate()
		{
			// look
			FreeLook freeLook = GetComponentInChildren<FreeLook>();

			freeLook.Look(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")));

			if (!GetComponent<Player>().IsAlive)
				return 0;

			// check if our camera moved since last time
			if (freeLook.transform.localEulerAngles.y != 0)
			{
				// rotate along the y axis to face the same direction as our camera
				transform.Rotate(Vector3.up, freeLook.transform.localEulerAngles.y);

				// reset the y angle since we are now matched with it
				freeLook.transform.localEulerAngles = new Vector3(freeLook.transform.localEulerAngles.x, 0, freeLook.transform.localEulerAngles.z);
            }

			// movement
			GetComponent<Movement>().Move(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));

			// left click toggles the laser on or off
			if (Input.GetMouseButtonDown(0))
				GetComponentInChildren<Laser>().Visible = !GetComponentInChildren<Laser>().Visible;

			return 0;
		}
	}
}