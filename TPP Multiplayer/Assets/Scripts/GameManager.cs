using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviourPun
{
    public Camera sceneCam;

    public AudioListener sceneCamAudio;
    public GameObject player;

    [SerializeField]
    public Transform playerSpawnPosition;
    // Start is called before the first frame update

    [SerializeField]
    public Transform playerSpawnPosition2;  

    public AudioSource WinSound;

    [SerializeField]
    public int players;

    [SerializeField] public static int Red = 0;
    [SerializeField] public static int Blue = 0;

    [SerializeField] public int Limit = 2;

    [SerializeField] private String winner = "Ongoing";

    [SerializeField] protected Image victory_bar;
    [SerializeField] protected Text victory_team;

    [SerializeField] protected Text Timer;

    [SerializeField] protected Text no_of_players;

     [SerializeField] public Text RedTeam;
    [SerializeField] public Text BlueTeam;

    public static int secondsLeft = 0;

    public int disRoom = 0;
    public int afterWait = 0;
    public bool takingAway;

    public static bool waitFlag = false;

    public Text test_flag;

    public Text match_start_check;

    public static bool Match_start = false;

    void Awake(){
        victory_bar.enabled = false;
        victory_team.enabled = false;
        Timer.enabled = true;
    }
    void Start()
    {
        sceneCam.enabled = false;
        sceneCamAudio.enabled = false;

        players = PhotonNetwork.CurrentRoom.PlayerCount;

        //string MyPlayerName = (string)File.ReadAllText(Application.persistentDataPath + "/User_Logs/" +"username"+".txt");
        //PhotonNetwork.NickName = MyPlayerName;
        
        if(players%2 == 0){
            PhotonNetwork.Instantiate(player.name, playerSpawnPosition.position, playerSpawnPosition.rotation);
            try{
                PhotonNetwork.LocalPlayer.CustomProperties.Add("pos", "1");
            }
            catch{
                PhotonNetwork.LocalPlayer.CustomProperties["pos"] = "1";
            }
            
        }
        else{
            PhotonNetwork.Instantiate(player.name, playerSpawnPosition2.position, playerSpawnPosition2.rotation);
            try{
                PhotonNetwork.LocalPlayer.CustomProperties.Add("pos", "2");
            }
            catch{
                PhotonNetwork.LocalPlayer.CustomProperties["pos"] = "2";
            }
        }

        RedTeam.text = Red.ToString();
        BlueTeam.text = Blue.ToString();
        Timer.GetComponent<Text>().text = "00:" + secondsLeft;
    }

    void Update(){

        players = PhotonNetwork.CurrentRoom.PlayerCount;
        no_of_players.text = "Players : "+players;

        RedTeam.text = Red.ToString();
        BlueTeam.text = Blue.ToString();

        if(afterWait==1){
            print(""+afterWait);
            runRoomDisconnection();
            afterWait=0;
            disRoom=0;
        }

        //match_start_check.text = "" + Match_start;

        /*if(waitFlag){
            test_flag.text = "waitFlag = True";
        }
        else{
            test_flag.text = "waitFlag = False";
        }*/

        if(takingAway == false && secondsLeft > 0){
            Timer.enabled = true;
            StartCoroutine(TimeTaker());
        }
        if(secondsLeft==0){
            Timer.enabled = false;
        }

        waitingForPlayers();

        if(Match_start){
            if(Red == Limit){
                winner = "Red Victory";
                afterWin("Red");
            }
            else if(Blue == Limit){
                winner = "Blue Victory";
                afterWin("Blue");
            }
            else if(secondsLeft==0){
                if(Red > Blue){
                    winner = "Red Victory";
                    afterWin("Red");
                }
                else{
                    winner = "Blue Victory";
                    afterWin("Blue");
                }
            }
        }
    }

    private void waitingForPlayers()
    {
        if(players < 2 && waitFlag==false){
            waitFlag = true;
            victory_team.enabled = true;
            secondsLeft = 60;
            victory_team.text = "Waiting for Players";
        }
        else if(players > 1 && waitFlag){

            if(secondsLeft==0){   //timer ended lets start match
                waitFlag = false;
                victory_team.enabled = false;
                Match_start = true;
                StartMatch();
            }
            else if(secondsLeft<5){
                victory_team.text = "Starting Match...";
            }
            else{       //other players than host are waiting to start match
                victory_team.enabled = true;
                victory_team.text = "Waiting for Players";
                Match_start = false;
            }

            
        }
        else if(waitFlag && players <= 1 && secondsLeft==0){  //No players found
            victory_team.text = "No Players Found";
            disRoom = 1;
        }
        else if(!waitFlag && !Match_start && disRoom!=1){
            victory_team.enabled = false;
            Match_start = true;
            Red=0;
            Blue=0;
        }
    }

    void StartMatch(){
        secondsLeft = 600;
        waitFlag = false;
        victory_team.enabled = false;
        Match_start = true;
        Red=0;
        Blue=0;
        MyPlayer.initialise = true;
    }

    void afterWin(String s){
            Match_start = false;
            victory_bar.enabled = true;
            victory_team.enabled = true;
            victory_team.text = s + " Team Victory";
            if(s=="Red"){
                victory_team.color = Color.red;
            }
            else{
                victory_team.color = Color.blue;
            }
            disRoom = 1;
            WinSound.Play();
    }

    void runRoomDisconnection(){
            print("Room Disconnected");
            Red=0;
            Blue=0;
            PhotonNetwork.CurrentRoom.ClearExpectedUsers();
            PhotonNetwork.Disconnect(); 
            SceneManager.LoadScene("Lobby");
    }

    IEnumerator TimeTaker(){
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondsLeft-=1;
        int min = secondsLeft / 60;
        int sec = 0;
        if(min > 0){
            sec = secondsLeft % 60;
        }
        else{
            sec = secondsLeft;
        }
        if(sec<10){
            Timer.GetComponent<Text>().text = "0" + min + ":0" + sec;
        }
        else{
            Timer.GetComponent<Text>().text = "0" + min + ":" + sec;
        }
        takingAway = false;
    }
}
