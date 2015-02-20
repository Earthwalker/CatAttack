using UnityEngine;
using System.Collections.Generic;

namespace CatAttack
{
	public class Cat : NetworkObject
	{
		#region inspector

		/// <summary>
		/// How far we can see
		/// </summary>
		public float SightRange;

		#endregion


		Vector3 target;

		/// <summary>
		/// The point we are heading to
		/// </summary>
		public Vector3 Target
		{
			get
			{
				return target;
			}
			set
			{
				target = Utility.Snap(value, GetComponent<Movement>().GridSize);
			}
		}

		/// <summary>
		/// Where our eyes are compared to our position
		/// </summary>
		public Vector3 EyeOffset;

		/// <summary>
		/// Called at the start of the game
		/// </summary>
		public override void Start()
		{
			base.Start();

			if (Local)
				TeleportToRandomEdge();
		}

		/// <summary>
		/// Local update
		/// </summary>
		/// <returns></returns>
		public override float LocalUpdate()
		{
			float distance = 9999;
			RaycastHit hitInfo;

			// check if any visible lasers are in sight
			foreach (var laser in GameObject.FindGameObjectsWithTag("Laser"))
			{
				if (laser.renderer.enabled)
				{
					if (!Physics.Linecast(transform.position + EyeOffset, laser.transform.position, out hitInfo))
					{
						if (hitInfo.distance < distance)
							Target = laser.transform.position;
					}
				}
			}

			// check if any players are in sight
			foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
			{
				if (!Physics.Linecast(transform.position + EyeOffset, player.transform.position, out hitInfo))
				{
					if (player.GetComponent<Player>().IsAlive)
					{
						if (hitInfo.distance < distance)
							Target = player.transform.position;
					}
				}
			}

			// check if we need to move
			if (Target != transform.position)
				MoveTowardsTarget();

			return 0.1f;
		}

		void MoveTowardsTarget()
		{
			// keep the cat level headed
			//Target.Set(Target.x, transform.position.y, Target.z);

			transform.LookAt(Target);
			GetComponent<FreeLook>().LookAngle = transform.eulerAngles;

			if (GetComponent<Movement>().Move(Vector3.forward) != Vector3.zero)
				DestroyObstacles();
		}

		void DestroyObstacles()
		{
			var hitInfos = Physics.RaycastAll(transform.position - EyeOffset, -transform.forward, GetComponent<Movement>().GridSize);
			Debug.DrawRay(transform.position - EyeOffset, -transform.forward, Color.green, GetComponent<Movement>().GridSize);

			// check if moving to this position hits something
			if (hitInfos.Length > 0)
			{
				// go through each hit to see if we need to do anything
				foreach (var hit in hitInfos)
				{
					if (hit.transform.CompareTag("Player"))
						hit.transform.GetComponent<Player>().IsAlive = false;
					else if (hit.transform.CompareTag("Wall"))
						Destroy(hit.transform.gameObject);
				}
			}
		}

		void OnTriggerEnter(Collider other)
		{
			Debug.Log("hit");

			if (!Local)
				return;

			if (other.CompareTag("Player"))
				other.GetComponent<Player>().IsAlive = false;
			else if (other.CompareTag("Wall"))
				Destroy(other.transform.gameObject);
		}

		/// <summary>
		/// Teleports to a random edge
		/// </summary>
		void TeleportToRandomEdge()
		{
			Movement movement = GetComponent<Movement>();

			// choose a random edge
			if (Random.Range(0, 2) == 0)
			{
				if (Random.Range(0, 2) == 0)
				{
					movement.GridPosition = new Vector3(-1, 1, Random.Range(0, (int)movement.WorldSize.z));
					Target = (movement.GridPosition + Vector3.right) * movement.GridSize;
				}
				else
				{
					movement.GridPosition = new Vector3((int)movement.WorldSize.x, 1, Random.Range(0, (int)movement.WorldSize.z));
					Target = (movement.GridPosition + Vector3.left) * movement.GridSize;
				}
			}
			else
			{
				if (Random.Range(0, 2) == 0)
				{
					movement.GridPosition = new Vector3(Random.Range(0, (int)movement.WorldSize.x), 1, -1);
					Target = (movement.GridPosition + Vector3.forward) * movement.GridSize;
				}
				else
				{
					movement.GridPosition = new Vector3(Random.Range(0, (int)movement.WorldSize.x), 1, (int)movement.WorldSize.z);
					Target = (movement.GridPosition + Vector3.back) * movement.GridSize;
				}
			}

			MoveTowardsTarget();

			Debug.Log("Teleported to space: " + movement.GridPosition.ToString() + ":" + (Target / movement.GridSize).ToString());
		}
	}
}