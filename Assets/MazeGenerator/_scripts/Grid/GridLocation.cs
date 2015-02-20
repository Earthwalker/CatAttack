using UnityEngine;
using System.Collections;

/// <summary>
/// GridLocation structure.
/// </summary>
public struct GridLocation
{
	#region Constructors
	
	/// <summary>
	/// Initializes a new instance of the GridLocation struct.
	/// </summary>
	public GridLocation(int px, int py)
	{
		x = px;
		y = py;
	}
	
	#endregion
	
	#region Fields
	
	/// <summary>
	/// The x-position.
	/// </summary>
	public int x;
	
	/// <summary>
	/// The y-position.
	/// </summary>
	public int y;
	
	#endregion
}
