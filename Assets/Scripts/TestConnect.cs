using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to server...", this);
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log($"Connected to server as {PhotonNetwork.LocalPlayer.NickName}.", this);

        Debug.Log("Joining lobby...", this);
        PhotonNetwork.JoinLobby();
        Debug.Log("Joined the lobby.", this);

        Invoke("createRoom", 5);
    }

    private void createRoom()
    {
        CreateRoomMenu.CreateRoom($"test{Random.Range(0, 9999)}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log($"Disconnected for reason: {cause}.", this);
    }
}
