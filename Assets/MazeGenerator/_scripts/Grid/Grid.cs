using UnityEngine;
using System.Collections;

/// <summary>
/// A grid map.
/// </summary>
public class Grid<T> where T : IGridCell, new()
{
	#region Constructors
	
	/// <summary>
	/// Initializes a new instance of the GridMap class.
	/// </summary>
	public Grid(int width, int height)
	{
		Width = width;
		Height = height;
		CellArray = new T[width * height];
		InitializeGrid();
	}
	
	#endregion
	
	#region Fields
	
	/// <summary>
	/// The width of the grid.
	/// </summary>
	private int m_width;
	public int Width
	{
		get { return m_width; }
		private set { m_width = value; }
	}
	
	/// <summary>
	/// The height of the grid.
	/// </summary>
	private int m_height;
	public int Height
	{
		get { return m_height; }
		private set { m_height = value; }
	}
	
	/// <summary>
	/// Gets the area of the grid.
	/// </summary>
	public int Area
	{
		get { return Width * Height; }
	}
	
	/// <summary>
	/// The array of grid cells.
	/// </summary>
	public T[] CellArray;
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Initializes the grid.
	/// </summary>
	private void InitializeGrid()
	{
		// Create cells
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				int index = GridToCellIndex(x, y);
				T cell = new T();
				cell.Location = new GridLocation(x, y);
				cell.Index = index;
				CellArray[index] = cell;
			}
		}
		
		// Link cells
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				T cell = CellArray[GridToCellIndex(x, y)];
				cell.North = GetCellAt(x, y + 1);
				cell.South = GetCellAt(x, y - 1);
				cell.East = GetCellAt(x + 1, y);
				cell.West = GetCellAt(x - 1, y);
			}
		}
	}
	
	/// <summary>
	/// Gets the cell at grid position <x,y>.
	/// </summary>
	public T GetCellAt(int x, int y)
	{
		return CellArray[GridToCellIndex(x, y)];
	}
	
	/// <summary>
	/// Gets the cell at the specified GridLocation.
	/// </summary>
	public T GetCellAt(GridLocation location)
	{
		return CellArray[GridToCellIndex(location.x, location.y)];
	}
	
	/// <summary>
	/// Convert an <x,y> coordinate to a cell index.
	/// </summary>
	private int GridToCellIndex(int x, int y)
	{
		x = (x % Width + Width) % Width;
		y = (y % Height + Height) % Height;
		return x + y * Width;
	}
	
	/// <summary>
	/// Wraps the specified coordinates to fit within the grid.
	/// </summary>
	public GridLocation WrapCoordinates(int x, int y)
	{
		x = x % Width;
		y = y % Height;
		return new GridLocation(x, y);
	}
	
	#endregion
}
