using UnityEngine;
using System.Collections;

/// <summary>
/// Flags for the available exits from each map tile.
/// </summary>
[System.Flags]
public enum MazeCellExits
{
	None  = 0,
	North = 1,
	South = 2,
	East  = 4,
	West  = 8
}
