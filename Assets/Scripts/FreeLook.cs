using UnityEngine;

public class FreeLook : NetworkMonoBehaviour
{
	/// <summary>
	/// The angle we are currently looking at
	/// </summary>
	Vector3 lookAngle;

	/// <summary>
	/// The angle we are currently looking at
	/// </summary>
	public Vector3 LookAngle
	{
		get
		{
			return lookAngle;
		}
		set
		{
			Vector3 angle = value;

            if (LockHorizontal)
				angle.Set(angle.x, 0, angle.z);
			if (LockVertical)
				angle.Set(0, angle.y, 0);

			lookAngle = angle;

			transform.rotation = Quaternion.Euler(lookAngle);

			// update the look angle across the network
			if (Local)
				RPC("UpdateLookAngle", RPCMode.OthersBuffered, lookAngle);
		}
	}

	/// <summary>
	/// Look sensitivity
	/// </summary>
	public Vector2 Sensitivity = new Vector2(100, 100);

	/// <summary>
	/// The range of horizontal values we can have
	/// </summary>
	public Vector2 HorizontalRange = new Vector2(-360, 360);

	/// <summary>
	/// The range of horizontal values we can have
	/// </summary>
	public Vector2 VerticalRange = new Vector2(-60, 60);

	/// <summary>
	/// Whether we should not allows changes to our horizontal rotation (y axis)
	/// </summary>
	public bool LockHorizontal;

	/// <summary>
	/// Whether we should not allows changes to our vertical rotation (x axis)
	/// </summary>
	public bool LockVertical;

	/// <summary>
	/// Changes the viewing angle based on the given inputs
	/// </summary>
	/// <param name="input"></param>
	public bool Look(Vector2 input)
	{
		// make sure our inputs aren't empty
		if (input == Vector2.zero)
			return false;

		// don't move if the cursor is not locked
		if (!Screen.lockCursor)
			return false;

		// start from our current look angle
		Vector3 angle = LookAngle;

		// only change if this axis is unlocked
		if (!LockVertical)
		{
			// changing the vertical view affects the x axis
			angle.x -= input.y * Sensitivity.y * Time.deltaTime;

			// keep the angle between -360 and 360
			//angle.x = Mathf.Repeat(angle.x, 720) - 360;

			// now clamp the angle according to our ranges
			angle.x = Mathf.Clamp(angle.x, VerticalRange.x, VerticalRange.y);
			//Debug.Log("angle x: " + angle.x.ToString());
        }

		// only change if this axis is unlocked
		if (!LockHorizontal)
		{
			// changing the horizontal view affects the y axis
			angle.y += input.x * Sensitivity.x * Time.deltaTime;

			// keep the angle between -360 and 360
			angle.y = Mathf.Repeat(angle.y, 720) - 360;

			// now clamp the angle according to our ranges
			angle.y = Mathf.Clamp(angle.y, HorizontalRange.x, HorizontalRange.y);
		}

		// check if we need to rotate the camera
		if (angle != LookAngle)
		{
			LookAngle = angle;
            return true;
		}

		return false;
	}

	#region RPCs

	/// <summary>
	/// RPC to update the look angle
	/// </summary>
	/// <param name="rotation">Rotation</param>
	[RPC]
	public void UpdateLookAngle(Vector rotation)
	{
		LookAngle = rotation.ToVector3();
    }

	#endregion
}
