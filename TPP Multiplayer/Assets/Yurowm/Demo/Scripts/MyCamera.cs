using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyCamera : MonoBehaviourPun
{
    public PhotonView playerPhotonView;
    public float Yaxis;
    public float Xaxis;

    public Vector3 pos;
    public float RotationSensitivity = 8f;
    private Transform target;

    float RotationMin = -40f;
    float RotationMax = 80f;
     float smoothTime = 0.12f;

    Vector3 targetRotation;
    Vector3 currentVel;
    public bool enableMobileInputs = false;
    private FixedTouchField touchField;

    protected Transform localPlayer;

    void Awake(){
        if(playerPhotonView.IsMine){
            //transform.parent = GameObject.Find("CamHolder").transform;
            touchField = GameObject.Find("TouchPanel").GetComponent<FixedTouchField>();
            target = GetLocalPlayer().transform.GetChild(3);
            localPlayer = GetLocalPlayer().transform;
        }
        else{
            this.gameObject.SetActive(false);
        }
        
    }

    void Update()
    {
        if(!playerPhotonView.IsMine){
            return;
        }

        if (enableMobileInputs)
        {
            Yaxis += touchField.TouchDist.x * RotationSensitivity;
            Xaxis -= touchField.TouchDist.y * RotationSensitivity;
        }
        else { 
         Yaxis += Input.GetAxis("Mouse X")* RotationSensitivity;
         Xaxis -= Input.GetAxis("Mouse Y")* RotationSensitivity;
        }
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);

        if(!fireRay.ScopeIn){
            transform.eulerAngles = targetRotation;
        }
        else{
            localPlayer.eulerAngles = targetRotation;
        }
        transform.position = target.position;   // + transform.forward * 2f;
       
    }

    GameObject GetLocalPlayer(){
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players){
            if(player.GetComponent<PhotonView>().IsMine){
                return player;
            }
        }
        return null;
    }
}
