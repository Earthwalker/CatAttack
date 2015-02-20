using UnityEngine;
using System.Collections;
using CatAttack;

public class Movement : NetworkMonoBehaviour
{
	/// <summary>
	/// Size of the grid we snap to
	/// </summary>
	public float GridSize;

	public float MoveDelay;

	/// <summary>
	/// Size of the world in grid spaces
	/// </summary>
	public Vector3 WorldSize;

	public string[] CollisionTags;

	/// <summary>
	/// The current grid position
	/// </summary>
	Vector3 gridPosition;

	/// <summary>
	/// The current grid position
	/// </summary>
	public Vector3 GridPosition
	{
		get
		{
			return gridPosition;
		}
		set
		{
            gridPosition = value;

			transform.position = gridPosition * GridSize;

			// update the grid position across the network
			if (Network.isServer)
				RPC("UpdateMovement", RPCMode.OthersBuffered, gridPosition);
		}
	}

	bool justMoved;

	/// <summary>
	/// Whether we just moved
	/// </summary>
	public bool JustMoved
	{
		get
		{
			return justMoved;
		}
		private set
		{
			justMoved = value;

			if (justMoved)
				StartCoroutine(MoveTimer());
		}
	}

	IEnumerator MoveTimer()
	{
		for (float i = 0; i <= MoveDelay; i += Time.deltaTime)
			yield return 0;

		JustMoved = false;
	}

	/// <summary>
	/// Moves the object based on the inputs given
	/// </summary>
	/// <param name="input"></param>
	public Vector3 Move(Vector3 input)
	{
		// make sure our inputs aren't empty
		if (input == Vector3.zero)
			return Vector3.zero;

		if (JustMoved)
			return Vector3.zero;

		// for the time being, we only allow digital input
		input = new Vector3(System.Math.Sign(input.x), System.Math.Sign(input.y), System.Math.Sign(input.z));

		// if we're the server, we can directly move
		if (Network.isServer)
		{
			// holds our new velocity
			Vector3 velocity = new Vector3(input.x, 0, input.z);

			// we can't move diagonally
			if (velocity.z != 0)
				velocity.x = 0;

			// calculate the velocity based on the direction we're facing
			velocity = Quaternion.Euler(velocity.x, Utility.Snap(transform.eulerAngles.y, 90), 0) * velocity;
			//velocity = Quaternion.Euler(velocity.x, transform.eulerAngles.y, 0) * velocity;

			// check if the new position is clear of objects
			if (CheckCollision((GridPosition + velocity) * GridSize))
				return Vector3.zero;

			// make sure the new position is in the world
			//if (CheckOutside(GridPosition + velocity))
			//	return Vector3.zero;

			// add the new velocity
			GridPosition += velocity;

			// update across the network
			RPC("UpdateMovement", RPCMode.OthersBuffered, GridPosition);

			input = Vector3.zero;

			JustMoved = true;

			return velocity;
		}
		else
		{
			// if we have input, tell the server so we can move
			RPC("UpdateMovement", RPCMode.Server, input);
			input = Vector3.zero;
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Teleports to a random free space
	/// </summary>
	public void TeleportToRandomSpace()
	{
		// holds our new position
		Vector3 newPosition;

		do
		{
			// pick a random space in the world (we have a border of one everywhere so we can exclude those positions)
			newPosition = new Vector3(Random.Range(1, (int)WorldSize.x) * GridSize, transform.position.y, Random.Range(1, (int)WorldSize.z) * GridSize);
		}
		// make sure there is nothing in the way
		while (Utility.FindAtPositionWithTag(newPosition, CollisionTags));

		// set our grid position to our new one
		GridPosition = newPosition / GridSize;

		Debug.Log("Teleported to space: " + GridPosition.ToString());
	}

	/// <summary>
	/// Check colliding instances at the grid location
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public bool CheckCollision(Vector3 position)
	{
		RaycastHit hitInfo;

		// check if moving to this position hits something
		if (Physics.Linecast(GridPosition * GridSize, position, out hitInfo))
		{
			// make sure we should collide with this object
			foreach (var collisionTag in CollisionTags)
			{
				if (hitInfo.transform.CompareTag(collisionTag))
					return true;
            }
		}

		return false;
    }

	/// <summary>
	/// Check if we are outside world bounds
	/// </summary>
	/// <param name="gridPos"></param>
	/// <returns></returns>
	bool CheckOutside(Vector3 gridPos)
	{
		return (gridPos.x < 0 || gridPos.z < 0 ||
				gridPos.x >= WorldSize.x || gridPos.z >= WorldSize.z);
    }

	#region RPCs

	/// <summary>
	/// RPC to update the movement, inputs for the server and positions for the clients
	/// </summary>
	/// <param name="movement"></param>
	[RPC]
	public void UpdateMovement(Vector movement)
	{
		// the server receives inputs from the client
		if (Network.isServer)
			Move(movement.ToVector3());
		else
			GridPosition = movement.ToVector3();
	}

	#endregion
}
