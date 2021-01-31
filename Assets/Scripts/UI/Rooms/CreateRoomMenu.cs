using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] Text _roomName = null;

    public static void CreateRoom(string name)
    {
        Debug.Log($"Getting into room \"{name}\"...");
        PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions() { MaxPlayers = 3 }, TypedLobby.Default);
        Debug.Log($"Done trying to get into room \"{name}\".");
    }

    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        CreateRoom(_roomName.text);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log($"Created room successfully.", this);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.LogError($"Failed to create room. {returnCode} : {message}", this);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined room.", this);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.LogError($"Failed to join room. {returnCode} : {message}", this);
    }
}
