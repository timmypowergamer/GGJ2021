using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using Photon.Realtime;

public class TitleScript : MonoBehaviourPunCallbacks
{
    public InputField _roomCodeInput;
    public Text _lobbyRoomCode;
    public string newGameScene;
	public GameObject TitleCanvasGameObject;
	public GameObject SecondCanvasGameObject;	
	public GameObject LobbyCanvasGameObject;
    public GameObject Player1Panel;
    public GameObject Player2Panel;
    public GameObject Player3Panel;
    public GameObject Player1Model;
	public GameObject Player2Model;
	public GameObject Player3Model;

	// Start is called before the first frame update
    void Start()
    {
        TitleCanvasGameObject.SetActive(true);
		SecondCanvasGameObject.SetActive(false);
		LobbyCanvasGameObject.SetActive(false);		
    }

	public void NewGame()
    {
        TitleCanvasGameObject.SetActive(false);
		SecondCanvasGameObject.SetActive(true);
		LobbyCanvasGameObject.SetActive(false);	
    }

    static string createRoomCode()
    {
        return new string(Enumerable.Range(0, 4).Select(_ => (char)(Random.Range(0, 26) + 'A')).ToArray());
    }

    public void HostGame()
    {
        _roomCodeInput.text = createRoomCode();
        PhotonNetwork.CreateRoom(_roomCodeInput.text, new RoomOptions() { MaxPlayers = 3 }, TypedLobby.Default);
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(_roomCodeInput.text);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        _lobbyRoomCode.text = _roomCodeInput.text;
        Player3Panel.SetActive(PhotonNetwork.CurrentRoom.PlayerCount > 2);
        Player2Panel.SetActive(PhotonNetwork.CurrentRoom.PlayerCount > 1);
        TitleCanvasGameObject.SetActive(false);
        SecondCanvasGameObject.SetActive(false);
        LobbyCanvasGameObject.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        TitleCanvasGameObject.SetActive(false);
        SecondCanvasGameObject.SetActive(true);
        LobbyCanvasGameObject.SetActive(false);
    }

    public void GoBack()
    {
        TitleCanvasGameObject.SetActive(true);
		SecondCanvasGameObject.SetActive(false);
		LobbyCanvasGameObject.SetActive(false);		
    }
	
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"{newPlayer.NickName} entered.");
        Player3Panel.SetActive(PhotonNetwork.CurrentRoom.PlayerCount > 2);
        Player2Panel.SetActive(PhotonNetwork.CurrentRoom.PlayerCount > 1);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"{otherPlayer.NickName} left.");
        Player3Panel.SetActive(PhotonNetwork.CurrentRoom.PlayerCount > 2);
        Player2Panel.SetActive(PhotonNetwork.CurrentRoom.PlayerCount > 1);
    }
}
