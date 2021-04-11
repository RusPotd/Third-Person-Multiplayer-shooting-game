using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Android;
using Photon.Pun;

public class Firebase_login : MonoBehaviour
{

    [Header("---UI Screens---")]

    public GameObject SetUpUI;

    public GameObject roomUI;

    [Header("---UI InputFields---")]

    public InputField Username;

    [Header("---UI Texts---")]

    public Text usernameText;

    public static string playerName;

    public static bool auth;


    void Awake(){
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        Directory.CreateDirectory(Application.persistentDataPath + "/User_Logs/");
    }

    public void enter_UserName_Btn(){
        string path = Application.persistentDataPath + "/User_Logs/" +"username"+".txt";
        if(!File.Exists(path)){
            if(Username.text!=""){
                File.WriteAllText(path, Username.text);
                usernameText.text = Username.text;
                auth = true;
                SetUpUI.SetActive(false);
                if(LobbyManager.connectedSuccess){
                    roomUI.SetActive(true);
                }
                //File.AppendAllText(path, Contect);
            }
        }
    }

    void Start(){
        string path = Application.persistentDataPath + "/User_Logs/" +"username"+".txt";  //C:\Users\Rushikesh Potdar\AppData\LocalLow\Rushikesh Potdar\TPP Multiplayer\User_Logs in unity
        if(File.Exists(path)){
            auth = true;
            playerName = (string)File.ReadAllText(path);
            print(playerName);
            usernameText.text = playerName;
            SetUpUI.SetActive(false);
            if(LobbyManager.connectedSuccess){
                    roomUI.SetActive(true);
                }
        }
        else{
            roomUI.SetActive(false);
            SetUpUI.SetActive(true);
        }
    }
   
}
