using System.Collections;

using UnityEngine;
//using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LIBGameManager : MonoBehaviourPunCallbacks
{
	public const string PLAYER_LOADED_LEVEL = "player_loaded_level";

	public static LIBGameManager Instance { get; private set; }

	[System.Serializable]
	public struct SpawnGroup
	{
		public enum SpawnType
		{
			LOVER = 0,
			LOVER2 = 1,
			PREDATOR = 2,
			WEAPON = 3,
			EXIT = 4
		}

		[System.Serializable]
		public struct SpawnList
		{
			public SpawnType Type;
			public Transform[] Positions;
		}

		public SpawnList[] Spawns;

		public Transform GetSpawnPoint(SpawnType spawnType, int index)
		{
			foreach(SpawnList spawn in Spawns)
			{
				if(spawn.Type == spawnType)
				{
					return spawn.Positions[index];
				}
			}
			return null;
		}

		public int[] GetSpawnPositions()
		{
			int[] posArray = new int[Spawns.Length + 1];
			for (int i = 0; i < Spawns.Length; i++)
			{
				posArray[i] = Random.Range(0, Spawns[i].Positions.Length);
			}
			return posArray;
		}
	}

	[SerializeField] private SpawnGroup[] SpawnGroups;

	private int _currentSpawnGroup;
	private int[] _currentSpawnPositions;

	[SerializeField] private GameObject LoverPrefab;
	[SerializeField] private GameObject PredatorPrefab;
	[SerializeField] private GameObject WeaponPrefab;
	[SerializeField] private GameObject ExitPrefab;

	[SerializeField] private Transform _waitingPosition;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.LogError("Should not have 2 LIBGameManagers in scene!");
			Destroy(gameObject);
		}
	}

	public void Start()
	{
		Debug.Log("LOcal Player = " + PhotonNetwork.LocalPlayer.ToString());
		Debug.Log("custom props = " + PhotonNetwork.LocalPlayer.CustomProperties.ToString());
		Debug.Log("player_index = " + PhotonNetwork.LocalPlayer.CustomProperties["player_index"]);
		int index = (int)PhotonNetwork.LocalPlayer.CustomProperties["player_index"];

		if (index < 2)
		{
			PhotonNetwork.Instantiate(LoverPrefab.name, _waitingPosition.position, _waitingPosition.rotation);
		}
		else
		{
			PhotonNetwork.Instantiate(PredatorPrefab.name, _waitingPosition.position, _waitingPosition.rotation);
		}
		Hashtable props = new Hashtable
			{
				{PLAYER_LOADED_LEVEL, true}
			};
		PhotonNetwork.LocalPlayer.SetCustomProperties(props);
	}

	#region PUN CALLBACKS

	public override void OnDisconnected(DisconnectCause cause)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
	}

	public override void OnLeftRoom()
	{
		PhotonNetwork.Disconnect();
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{

	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}


		// if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
		int startTimestamp;
		bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

		if (changedProps.ContainsKey(PLAYER_LOADED_LEVEL))
		{
			if (CheckAllPlayerLoadedLevel())
			{
				int group = GetRandomSpawnGroup();
				int[] positions = SpawnGroups[group].GetSpawnPositions();
				photonView.RPC("StartGame", RpcTarget.All, group, positions);
			}
			else
			{
				// not all players loaded yet. wait:
				Debug.Log($"Waiting for all players to load!");
			}
		}

	}

	#endregion

	[PunRPC]
	private void StartGame(int _spawnGroup, int[] _spawnPositions, PhotonMessageInfo info)
	{
		Debug.Log("Start Game called! group = " + _spawnGroup + ", _positions = " + _spawnPositions.ToString());
		_currentSpawnGroup = _spawnGroup;
		_currentSpawnPositions = _spawnPositions;

		if (PlayerController.LocalPlayerInstance != null)
		{
			Transform spawnPos = SpawnGroups[_currentSpawnGroup].GetSpawnPoint((SpawnGroup.SpawnType)PlayerController.LocalPlayerInstance.PlayerIndex, _currentSpawnPositions[PlayerController.LocalPlayerInstance.PlayerIndex]);
			Debug.Log("Spawn pos = " + spawnPos.name);
			PlayerController.LocalPlayerInstance.StartGame(spawnPos);
		}
		if(PhotonNetwork.IsMasterClient)
		{
			Transform weaponSpawnPos = SpawnGroups[_currentSpawnGroup].GetSpawnPoint(SpawnGroup.SpawnType.WEAPON, _currentSpawnPositions[3]);
			PhotonNetwork.Instantiate(WeaponPrefab.name, weaponSpawnPos.position, weaponSpawnPos.rotation);
			Transform exitSpawnPos = SpawnGroups[_currentSpawnGroup].GetSpawnPoint(SpawnGroup.SpawnType.EXIT, _currentSpawnPositions[4]);
			PhotonNetwork.Instantiate(ExitPrefab.name, exitSpawnPos.position, exitSpawnPos.rotation);
		}
	}

	public int GetRandomSpawnGroup()
	{
		int randIndex = Random.Range(0, SpawnGroups.Length);
		return randIndex;
	}

	private bool CheckAllPlayerLoadedLevel()
	{
		foreach (Player p in PhotonNetwork.PlayerList)
		{
			object playerLoadedLevel;

			if (p.CustomProperties.TryGetValue(PLAYER_LOADED_LEVEL, out playerLoadedLevel))
			{
				if ((bool)playerLoadedLevel)
				{
					continue;
				}
			}

			return false;
		}

		return true;
	}
}
