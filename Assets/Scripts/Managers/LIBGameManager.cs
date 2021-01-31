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
				//StartGame();
			}
			else
			{
				// not all players loaded yet. wait:
				Debug.Log("! ");
			}
		}

	}

	#endregion


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
