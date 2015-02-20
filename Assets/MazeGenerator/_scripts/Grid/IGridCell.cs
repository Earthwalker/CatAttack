using UnityEngine;
using System.Collections;

/// <summary>
/// Interface used by the Grid<T> generic collection type.
/// </summary>
public interface IGridCell
{
	#region Fields
	
	/// <summary>
	/// The linear index of the cell.
	/// </summary>
	int Index { get; set; }
	
	/// <summary>
	/// The location of the cell in the grid.
	/// </summary>
	GridLocation Location { get; set; }
	
	/// <summary>
	/// The neighbouring cell to the north (+y).
	/// </summary>
	IGridCell North { get; set; }
	
	/// <summary>
	/// The neighbouring cell to the south (-y).
	/// </summary>
	IGridCell South { get; set; }
	
	/// <summary>
	/// The neighbouring cell to the east (+x).
	/// </summary>
	IGridCell East { get; set; }
	
	/// <summary>
	/// The neighbouring cell to the west (-x).
	/// </summary>
	IGridCell West { get; set; }
	
	#endregion
}
