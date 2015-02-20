using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour
{
	#region Fields
	
	/// <summary>
	/// True if the maze should wrap across borders.
	/// </summary>
	public bool Wrap = false;

	/// <summary>
	/// The width of the maze grid.
	/// </summary>
	public int Width = 32;
	
	/// <summary>
	/// The height of the maze grid.
	/// </summary>
	public int Height = 32;

	/// <summary>
	/// Wall prefab to create
	/// </summary>
	public GameObject WallPrefab;

	/// <summary>
	/// Spacing between wall objects
	/// </summary>
	public int Spacing;
	
	#endregion
	
	#region Methods
	
	/// <summary>
	/// Generates a maze to use as a basis for the map.
	/// </summary>
	public Grid<MazeCell> GenerateMaze()
	{	
		// Initialize variables
		Stack<GridLocation> cellsVisited = new Stack<GridLocation>();
		Grid<MazeCell> grid = new Grid<MazeCell>(Width, Height);
		int crawlDistance = 0;
		int maxCrawlDistance = 0;
		
		// Select initial cell position, flag it as the initial cell,
		// and push it onto the stack.
		GridLocation cellPos = grid.WrapCoordinates((int)transform.position.x, (int)transform.position.y);
		grid.GetCellAt(cellPos).IsStartCell = true;
		cellsVisited.Push(cellPos);
		
		// Recursively crawl the maze.
		while (cellsVisited.Count > 0)
		{	
			// Flag the cell as visited.
			MazeCell cell = grid.GetCellAt(cellPos);
			cell.Flagged = true;
			cell.CrawlDistance = crawlDistance;
			
			// Calculate valid exits from the current cell position.
			MazeCellExits validExits = MazeCellExits.None;
			if ((Wrap || cellPos.x != 0)			&& !grid.GetCellAt(cellPos.x - 1, cellPos.y).Flagged) { validExits = validExits | MazeCellExits.West; }
			if ((Wrap || cellPos.x != Width - 1) 	&& !grid.GetCellAt(cellPos.x + 1, cellPos.y).Flagged) { validExits = validExits | MazeCellExits.East; }
			if ((Wrap || cellPos.y != 0) 			&& !grid.GetCellAt(cellPos.x, cellPos.y - 1).Flagged) { validExits = validExits | MazeCellExits.North; }
			if ((Wrap || cellPos.y != Height - 1)	&& !grid.GetCellAt(cellPos.x, cellPos.y + 1).Flagged) { validExits = validExits | MazeCellExits.South; }
			
			// When valid exits are found, flag the tile with a random
			// exit and select the next tile. Otherwise backtrack through the
			// stack looking for the most recently visited tile with valid
			// exits.
			if (validExits != MazeCellExits.None)
			{
				// Increment crawlDistance
				crawlDistance++;
				
				// Add the cell to the stack so we can return to it later for
				// recursive exit checking.
				cellsVisited.Push(cellPos);
				
				// Choose a random exit from the available exits.
				MazeCellExits exit = GetRandomExit(validExits);
				cell.Exits = cell.Exits | exit;
				
				// Select the next tile.
				if (exit == MazeCellExits.North)
				{
					cellPos = new GridLocation(cellPos.x, cellPos.y - 1);
					exit = MazeCellExits.South;
				}
				else if (exit == MazeCellExits.South)
				{
					cellPos = new GridLocation(cellPos.x, cellPos.y + 1);
					exit = MazeCellExits.North;
				}
				else if (exit == MazeCellExits.West)
				{
					cellPos = new GridLocation(cellPos.x - 1, cellPos.y);
					exit = MazeCellExits.East;
				}
				else if (exit == MazeCellExits.East)
				{
					cellPos = new GridLocation(cellPos.x + 1, cellPos.y);
					exit = MazeCellExits.West;
				}
				
				// Create an exit back to the previous tile.
				cell = grid.GetCellAt(cellPos);
				cell.Exits = cell.Exits | exit;
			}
			else
			{
				// Update max crawl distance.
				if (maxCrawlDistance < crawlDistance)
					maxCrawlDistance = crawlDistance;
				// Decrement crawlDistance
				crawlDistance--;
				
				if (cell.NumberOfExits == 1)
					cell.IsDeadEnd = true;
				
				// No valid exits so backtrack through the stack.
				cellPos = cellsVisited.Pop();
			}
		}
		
		foreach (MazeCell cell in grid.CellArray)
			cell.NormalizedDistance = (float)cell.CrawlDistance / (float)maxCrawlDistance;

		// draw maze
		DrawMaze(grid);

		return grid;
	}
	
	/// <summary>
	/// Gets a random cardinal direction from the specified directions.
	/// </summary>
	private MazeCellExits GetRandomExit(MazeCellExits validExits)
	{
		List<MazeCellExits> exits = new List<MazeCellExits>();
		if ((validExits & MazeCellExits.North) 	== MazeCellExits.North) exits.Add(MazeCellExits.North);
		if ((validExits & MazeCellExits.South) 	== MazeCellExits.South) exits.Add(MazeCellExits.South);
		if ((validExits & MazeCellExits.East) 	== MazeCellExits.East) 	exits.Add(MazeCellExits.East);
		if ((validExits & MazeCellExits.West) 	== MazeCellExits.West) 	exits.Add(MazeCellExits.West);
		
		int randIndex = (int)(Random.value * exits.Count);
		if (randIndex == exits.Count) randIndex--;
		
		return exits[randIndex];
	}
	
	/// <summary>
	/// Generates the debug render.
	/// </summary>
	private void DrawMaze(Grid<MazeCell> grid)
	{
		// destroy any walls already existing
		for (int i = 0; i < transform.childCount; i++)
		{
			// get any child with the tag Wall and destroy it
			if (transform.GetChild(i).CompareTag("Wall"))
				Destroy(transform.GetChild(i).gameObject);
        }

		foreach (MazeCell cell in grid.CellArray)
		{
			if (!cell.ExitNorth)
				CreateWall(new Vector2(cell.Location.x * Spacing, cell.Location.y * Spacing - (Spacing/2)), 0);
			if (!cell.ExitEast)
				CreateWall(new Vector2(cell.Location.x * Spacing + (Spacing / 2), cell.Location.y * Spacing), 90);
			if (!cell.ExitSouth)
				CreateWall(new Vector2(cell.Location.x * Spacing, cell.Location.y * Spacing + (Spacing / 2)), 180);
			if (!cell.ExitWest)
				CreateWall(new Vector2(cell.Location.x * Spacing - (Spacing / 2), cell.Location.y * Spacing), 270);
		}
	}

	/// <summary>
	/// Create a wall if no wall is present at the position
	/// </summary>
	/// <param name="position"></param>
	/// <param name="angle"></param>
	private void CreateWall(Vector2 position, int angle)
	{
		position += new Vector2(transform.position.x, transform.position.z);

		if (!Utility.FindAtPositionWithTag(position, "Wall"))
		{
			var wall = (GameObject)Instantiate(WallPrefab);
			wall.transform.Translate(position.x, 0, position.y);
			wall.transform.eulerAngles = new Vector3(0, angle, 0);
			wall.transform.SetParent(transform);
		}
	}

	#endregion
}