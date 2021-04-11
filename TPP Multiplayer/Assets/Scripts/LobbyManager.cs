using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("---UI Screens---")]
    public GameObject roomUI;
    public GameObject connectUI;

    [Header("---UI Text---")]
    public Text statusText;
    public Text connectingText;

    public Text UserName;

    [Header("---UI InputFields---")]
    public InputField createRoom;
    public InputField joinRoom;

    public int levelNumber;

    public static bool connectedSuccess;

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        connectingText.text = "Joining Lobby...";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
       
    }
    public override void OnJoinedLobby()
    {
        connectedSuccess = true;
        connectUI.SetActive(false);
        if(Firebase_login.auth){
             roomUI.SetActive(true);
             PhotonNetwork.NickName = UserName.text;
        }
        statusText.text = "Joined To Lobby";
    }

    public override void OnJoinedRoom()
    {
        if(levelNumber==1){
            PhotonNetwork.LoadLevel(1);
        }
        else if(levelNumber==2){
            PhotonNetwork.LoadLevel(2);
        }
        
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        connectUI.SetActive(true);
        connectingText.text = "Disconnected... "+cause.ToString();
        roomUI.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int roomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomName.ToString(), roomOptions, TypedLobby.Default, null);
    }
    #region ButtonClicks

    public void Onclick_CreateBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createRoom.text, roomOptions, TypedLobby.Default,null);
    }

    public void Onclick_JoinBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(joinRoom.text, roomOptions, TypedLobby.Default);
    }
    public void OnClick_PlayNow()
    {
        PhotonNetwork.JoinRandomRoom();
        statusText.text = "Creating Room... Please Wait...";
        levelNumber = 1;

    }

    public void OnClickLobbyNow(){
        PhotonNetwork.JoinRandomRoom();
        statusText.text = "Joining Practise room...";
        levelNumber = 2;
    }
    #endregion
}