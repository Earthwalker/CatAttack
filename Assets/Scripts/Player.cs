using UnityEngine;

namespace CatAttack
{
	public class Player : NetworkObject
	{
		[SerializeField]
		bool isAlive;

		public bool IsAlive
		{
			get
			{
				return isAlive;
			}
			set
			{
				isAlive = value;

				GetComponent<Movement>().enabled = isAlive;
				renderer.enabled = isAlive;
				collider.enabled = isAlive;
				
				if (Local)
					GetComponentInChildren<Camera>().enabled = isAlive;

				// update alive state across the network
				if (Network.isServer)
					RPC("UpdateAliveState", RPCMode.OthersBuffered, isAlive);
			}
		}

		/// <summary>
		/// Called at the start of the game
		/// </summary>
		public override void Start()
		{
			base.Start();

			if (Network.isServer)
			{
				IsAlive = GameObject.FindGameObjectWithTag("GameController").GetComponent<Match>().CurrentPhase == Phase.Setup;

				// move the player to a random space
				if (IsAlive)
					GetComponent<Movement>().TeleportToRandomSpace();
			}
		}

		/// <summary>
		/// RPC to update whether we are alive
		/// </summary>
		/// <param name="isAlive"></param>
		[RPC]
		public void UpdateAliveState(bool isAlive)
		{
			IsAlive = isAlive;
		}
	}
}