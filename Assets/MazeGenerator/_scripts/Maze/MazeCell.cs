using UnityEngine;
using System.Collections;

/// <summary>
/// GridCell defines a single cell in a Grid.
/// </summary>
public class MazeCell : IGridCell
{	
	#region Fields
	
	/// <summary>
	/// The linear index of the cell.
	/// </summary>
	private int m_index;
	public int Index
	{
		get { return m_index; }
		set { m_index = value; }
	}
	
	/// <summary>
	/// The location of the cell in the grid.
	/// </summary>
	private GridLocation m_gridLocation;
	public GridLocation Location
	{
		get { return m_gridLocation; }
		set { m_gridLocation = value; }
	}
	
	/// <summary>
	/// The neighboring cell to the north (+y).
	/// </summary>
	private IGridCell m_north;
	public IGridCell North
	{
		get { return m_north; }
		set { m_north = value; }
	}
	
	/// <summary>
	/// The neighboring cell to the south (-y).
	/// </summary>
	private IGridCell m_south;
	public IGridCell South
	{
		get { return m_south; }
		set { m_south = value; }
	}
	
	/// <summary>
	/// The neighboring cell to the east (+x).
	/// </summary>
	private IGridCell m_east;
	public IGridCell East
	{
		get { return m_east; }
		set { m_east = value; }
	}
	
	/// <summary>
	/// The neighboring cell to the west (-x).
	/// </summary>
	private IGridCell m_west;
	public IGridCell West
	{
		get { return m_west; }
		set { m_west = value; }
	}
	
	/// <summary>
	/// The exits from this cell.
	/// </summary>
	public MazeCellExits Exits = MazeCellExits.None;
	
	/// <summary>
	/// Gets the number of exits from this cell.
	/// </summary>
	public int NumberOfExits
	{
		get
		{
			int result = 0;
			if (ExitNorth) result++;
			if (ExitSouth) result++;
			if (ExitEast) result++;
			if (ExitWest) result++;
			return result;
		}
	}
	
	/// <summary>
	/// Gets or sets a bool indicating whether this tile has no exits.
	/// </summary>
	public bool NoExits
	{
		get { return Exits == MazeCellExits.None; }
		set { Exits = MazeCellExits.None; }
	}
	
	/// <summary>
	/// Gets or sets a bool indicating whether this tile has an exit to the North.
	/// </summary>
	public bool ExitNorth
	{
		get { return (Exits & MazeCellExits.North) == MazeCellExits.North; }
		set { Exits = Exits | MazeCellExits.North; }
	}
	
	/// <summary>
	/// Gets or sets a bool indicating whether this tile has an exit to the South.
	/// </summary>
	public bool ExitSouth
	{
		get { return (Exits & MazeCellExits.South) == MazeCellExits.South; }
		set { Exits = Exits | MazeCellExits.South; }
	}
		
	/// <summary>
	/// Gets or sets a bool indicating whether this tile has an exit to the East.
	/// </summary>
	public bool ExitEast
	{
		get { return (Exits & MazeCellExits.East) == MazeCellExits.East; }
		set { Exits = Exits | MazeCellExits.East; }
	}
	
	/// <summary>
	/// Gets or sets a bool indicating whether this tile has an exit to the West.
	/// </summary>
	public bool ExitWest
	{
		get { return (Exits & MazeCellExits.West) == MazeCellExits.West; }
		set { Exits = Exits | MazeCellExits.West; }
	}
	
	/// <summary>
	/// An arbitrary weighting value that indicates the cell's distance
	/// from the origin cell.
	/// </summary>
	public int CrawlDistance = 0;
	
	/// <summary>
	/// A normalized weighting value that indicates the cell's distance
	/// from the origin cell in relation to the rest of the maze.
	/// </summary>
	public float NormalizedDistance = 0f;
	
	/// <summary>
	/// True if this cell is the start cell.
	/// </summary>
	public bool IsStartCell = false;
	
	/// <summary>
	/// True if this cell is a dead end.
	/// </summary>
	public bool IsDeadEnd = false;
	
	/// <summary>
	/// Generic boolean property useful for any specific flagging purposes.
	/// </summary>
	public bool Flagged = false;
	
	#endregion
}
