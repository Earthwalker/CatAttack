using UnityEngine;
using System.Collections;

namespace CatAttack
{
	public enum Phase
	{
		Setup,
		Prepare,
		Attack,
		Endgame,
		Number
	}

	public class Match : NetworkMonoBehaviour
	{
		#region inspector

		/// <summary>
		/// Minimum amount of players to join before starting
		/// </summary>
		public int MinimumPlayers;

		/// <summary>
		/// Prepare Phase length
		/// </summary>
		public int PrepareTime = 30;

		/// <summary>
		/// Endgame Phase length
		/// </summary>
		public int EndgameTime = 30;

		/// <summary>
		/// Rate cats spawn during the attack phase
		/// </summary>
		public float CatSpawnRate;

		/// <summary>
		/// Cat prefab
		/// </summary>
		public GameObject CatPrefab;

		#endregion

		/// <summary>
		/// Number of players still alive
		/// </summary>
		public int PlayersAlive { get; private set; }

		/// <summary>
		/// Current time of the phase
		/// </summary>
		public int CurrentTime { get; private set; }

		/// <summary>
		/// Total time since the match started
		/// </summary>
		public int TotalTime { get; private set; }

		/// <summary>
		/// Current phase we are in
		/// </summary>
		public Phase CurrentPhase { get; private set; }

		/// <summary>
		/// Whether we are ready for match events
		/// </summary>
		bool readyToStart;

		/// <summary>
		/// Called when the serve is initialized
		/// </summary>
		void OnServerInitialized()
		{
			Initialize(networkView);

			// start the match
			NewMatch();
		}

		/// <summary>
		/// Local update
		/// </summary>
		/// <returns></returns>
		public override float LocalUpdate()
		{
			if (!GetComponent<Multiplayer>().Connected)
			{
				readyToStart = false;
				CurrentPhase = Phase.Setup;
				Stop();

				return 0;
			}

            if (!readyToStart)
				return 0;

			// do match events
			MatchEvents();

			if (TotalTime > 0)
			{
				TotalTime++;
				CurrentTime++;
			}

			return 1.0f;
		}

		/// <summary>
		/// Match events
		/// </summary>
		void MatchEvents()
		{
			// do different events for each phase
			switch (CurrentPhase)
			{
				// waiting for players
				case Phase.Setup:
					// check if we have enough players to start the match
					if (Network.connections.Length >= MinimumPlayers)
					{
						NextPhase();
						TotalTime = 1;
					}
                    break;
				// prepare before the attack
				case Phase.Prepare:
					if (CurrentTime > PrepareTime)
						NextPhase();
					break;
				// cats start attacking at regular intervals
				case Phase.Attack:
					PlayersAlive = 0;

					foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
					{
						if (player.GetComponent<Player>().IsAlive)
							PlayersAlive++;
                    }

					// if there is only one player or no players left, the match is done
					if (PlayersAlive < 1) // TODO: one or zero survivors?
						NextPhase();

					break;
				// show scores
				case Phase.Endgame:
					if (CurrentTime > EndgameTime)
						NextPhase();
					break;
			}
		}

		/// <summary>
		/// Cat spawner
		/// </summary>
		/// <returns></returns>
		IEnumerator SpawnCatTimer()
		{
			while (CurrentPhase == Phase.Attack)
			{
				Network.Instantiate(CatPrefab, Vector3.zero, Quaternion.identity, 0);

				Debug.Log("Spawning cat...");

				for (float i = 0; i <= CatSpawnRate; i += Time.deltaTime)
					yield return 0;
			}
		}

		/// <summary>
		/// Switch to the next phase
		/// </summary>
		void NextPhase()
		{
			CurrentPhase++;
			CurrentTime = 1;

			// if we are on the last phase then start over
			if (CurrentPhase == Phase.Number)
			{
				NewMatch();
				return;
			}

			// if this is the attack phase, start the cat spawner coroutine
			if (CurrentPhase == Phase.Attack)
				StartCoroutine(SpawnCatTimer());

			Debug.Log("Changing phase to: " + CurrentPhase.ToString());
		}

		/// <summary>
		/// Start a new match
		/// </summary>
		public void NewMatch()
		{
			// only the server can restart the match
			if (Network.isClient)
				return;

			Debug.Log("Starting new match...");

			readyToStart = false;

			// reset the clocks
			TotalTime = 0;
			CurrentTime = 0;

			// destroy all the cats
			foreach (var cat in GameObject.FindGameObjectsWithTag("Cat"))
				Network.DestroyPlayerObjects(cat.networkView.owner);

			Debug.Log("Destroyed all cats");

			// set the seed
			Random.seed = Time.frameCount;

			// generate maze
			GetComponent<Maze>().GenerateMaze();

			Debug.Log("Generated maze with seed: " + Random.seed.ToString());

			// tell clients to generate the maze
			RPC("GenerateMaze", RPCMode.OthersBuffered, Random.seed);

			// assign random positions to players
			foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
			{
				if (player.networkView)
				{
					// revive all the players
					player.GetComponent<Player>().IsAlive = true;

					// move the player to a random space
					player.GetComponent<Movement>().TeleportToRandomSpace();
				}
			}

			Debug.Log("Reset all players");

			readyToStart = true;

			Debug.Log("Finished match setup");
		}

		/// <summary>
		/// RPC to tell clients to generate the maze
		/// </summary>
		/// <param name="seed"></param>
		[RPC]
		public void GenerateMaze(int seed)
		{
			Random.seed = seed;
            GetComponent<Maze>().GenerateMaze();

			Debug.Log("Generated maze with seed: " + Random.seed.ToString());
		}
    }
}