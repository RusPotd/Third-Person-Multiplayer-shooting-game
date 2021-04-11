using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class characteraiming : MonoBehaviourPunCallbacks
{
    public float turnSpeed = 15;
    bool fireClick = false;

    [SerializeField]
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;  //GameObject.FindGameObjectWithTag("sceneCamera").GetComponent<Camera>() as Camera
        //myPhotonView = (PhotonView)player_setup.GetComponent<PhotonView>();
        fireClick = event_trigger_shooting.is_firing;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fireClick = event_trigger_shooting.is_firing;
        float yCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yCamera, 0), turnSpeed * Time.fixedDeltaTime);

        /*if(fireClick && !fireRay.ScopeIn){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yCamera+onFireCameraRotation, 0), turnSpeed * Time.fixedDeltaTime);
        }   
        else{
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yCamera, 0), turnSpeed * Time.fixedDeltaTime);
        }*/
        
    }
}
