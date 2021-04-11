using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class timeManager : MonoBehaviour, IPunObservable
{
    public bool is_master;

    public static int time_sec_left;
    public static bool time_waitFlag;
    public int secondsLeft;
    public bool waitFlag;

    public bool IMmaster;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient){
                IMmaster = true;
                time_waitFlag = GameManager.waitFlag;
                time_sec_left = GameManager.secondsLeft;
            }
        else{
             IMmaster = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){ 
     
        if(stream.IsWriting){
            if(PhotonNetwork.IsMasterClient){
                stream.SendNext(time_waitFlag);
                stream.SendNext(time_sec_left);
            }
        }
        else{
            if(!PhotonNetwork.IsMasterClient){
                waitFlag = (bool)stream.ReceiveNext();
                GameManager.waitFlag = waitFlag;
                secondsLeft = (int)stream.ReceiveNext();
                GameManager.secondsLeft = secondsLeft;
            }
        }
    }
}
