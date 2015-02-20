using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Draws a 3D laser from out position pointing in the direction we are facing
/// 
/// Attached to a networked object
/// </summary>
public class Laser : NetworkMonoBehaviour
{
	#region inspector

	public GameObject HitMarkerPrefab;

	#endregion

	/// <summary>
	/// Maximum length the laser can be
	/// </summary>
	public int MaximumLength;

	/// <summary>
	/// Color of the laser
	/// </summary>
	public Color color = Color.red;

	/// <summary>
	/// Size of the laser
	/// </summary>
	public float Size = 0.5f;

	/// <summary>
	/// Hit marker
	/// </summary>
	GameObject HitMarker;

	/// <summary>
	/// End of our laser
	/// </summary>
	public Vector3 EndPosition;

	/// <summary>
	/// The line renderer
	/// </summary>
	LineRenderer line;

	/// <summary>
	/// Whether the laser should be visible
	/// </summary>
	bool visible;

	/// <summary>
	/// Whether the laser should be visible
	/// </summary>
	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;

			if (Local)
				RPC("UpdateLaserVisible", RPCMode.OthersBuffered, visible);

			// line renderer
			if (line)
				line.renderer.enabled = visible;
		}
    }

	/// <summary>
	/// Called at the start of the game
	/// </summary>
	void Awake()
	{
		// create our line renderer
		line = gameObject.AddComponent<LineRenderer>();
		line.material = new Material(Shader.Find("Particles/Additive"));
		line.SetColors(color, color);
		line.SetWidth(Size, Size);
		line.renderer.enabled = false;

		// create out hit marker
		if (HitMarkerPrefab != null)
		{
			HitMarker = (GameObject)Instantiate(HitMarkerPrefab);
			HitMarker.renderer.enabled = false;
		}
    }

	/// <summary>
	/// Local update
	/// </summary>
	/// <returns></returns>
	public override float LocalUpdate()
	{
		// make sure the laser is visible
		if (!Visible)
		{
			// make sure the hit marker is disabled
			if (HitMarker)
				HitMarker.renderer.enabled = visible;

			return 0;
		}

		// ray cast to see if we hit an object
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position, transform.forward, out hitInfo, MaximumLength))
		{
			// set the end position at the collision point
			EndPosition = hitInfo.point;

			// adjust the hit marker if we are using one
			if (HitMarker)
			{
				// if the laser changes position, update across network
				if (HitMarker.transform.position != EndPosition)
					RPC("UpdateLaserPosition", RPCMode.OthersBuffered, EndPosition);

				HitMarker.transform.position = EndPosition;
				HitMarker.renderer.enabled = true;
			}
		}
		else
		{
			// set the end position based on the direction and the length
			EndPosition = transform.forward * MaximumLength;

			// disable the hit marker
			if (HitMarker)
				HitMarker.renderer.enabled = false;
		}

		// set the renderer's positions
		line.SetPosition(0, transform.position);
		line.SetPosition(1, EndPosition);

		line.renderer.enabled = true;

		return 0;
    }

	/// <summary>
	/// Remote update
	/// </summary>
	/// <returns></returns>
	public override float RemoteUpdate()
	{
		// make sure the laser is visible
		if (!Visible)
			return 0;

		// set the renderer's positions
		line.SetPosition(0, transform.position);
		line.SetPosition(1, EndPosition);

		return 0;	
	}

	/// <summary>
	/// Called in the editor when selected
	/// </summary>
	void OnDrawGizmosSelected()
	{
		Gizmos.color = color;
		Gizmos.DrawLine(transform.position, transform.forward * MaximumLength);
    }

	#region RPCs

	/// <summary>
	/// RPC to update laser visibility
	/// </summary>
	/// <param name="visible"></param>
	[RPC]
	public void UpdateLaserVisible(bool visible)
	{
		Visible = visible;
	}

	/// <summary>
	/// RPC to update laser end position
	/// </summary>
	/// <param name="endPosition"></param>
	[RPC]
	public void UpdateLaserPosition(Vector endPosition)
	{
		EndPosition = endPosition.ToVector3();
	}

	#endregion
}