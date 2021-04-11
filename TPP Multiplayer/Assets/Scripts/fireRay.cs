using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class fireRay : MonoBehaviourPun
{
    [SerializeField]
    public bool fire;

    public float damage = 3f;

    public float headShot_damage = 40f;

    [SerializeField]
    private GameObject crosshair;

    public GameObject rigidBody;

    public float fireRate = 15f;

    public GameObject impactEffect;

    private float nextTimetoFire = 0f;
    public Text Reload_text; 
    public int bullets = 30;

    public static bool ScopeIn;

    public bool Fsm_scope_indicator;

    public bool Reload = false;
    private Animator anim;

    public Transform target;
     private Vector3 old_pos;
    private Quaternion old_rot;

    public bool headshot;

    private string MyPlayerName;

    private string MyTeam;
    void Start(){
        if(!photonView.IsMine){
            return;
        }

        anim = rigidBody.GetComponent<Animator>();
        old_pos = target.localPosition; 
        //old_rot = target.localRotation;
        MyTeam = (string) PhotonNetwork.LocalPlayer.CustomProperties["pos"];
    }
    void Update()
    {
        if(!photonView.IsMine){
            return;
        }

        Fsm_scope_indicator = ScopeIn;

        if(ScopeIn){
            anim.SetBool("ScopeIn", true);
            target.localRotation = Quaternion.Euler(-2.842f, 0.793f, 4.351f);
            target.localPosition = new Vector3(-0.094f, 0.458f, 1.809f);
           
        }
        else{
            anim.SetBool("ScopeIn", false);
            target.localPosition = old_pos;
        }

        Reload_text.text = "" + bullets;

        if(fire && Time.time >= nextTimetoFire && bullets>0){
            bullets-=1;
            nextTimetoFire = Time.time + 1f/fireRate;
            RaycastHit hit;
            
            print("Fired");
            if(Physics.Raycast(crosshair.transform.position, Camera.main.transform.forward, out hit, 25f)) { 
                Debug.DrawRay(crosshair.transform.position, Camera.main.transform.forward * 25f, Color.green);
                if(hit.transform.tag == "Player"){
                    if(hit.collider.transform.tag == "Head"){
                        print("Hit Head");
                        headshot = true;
                        hit.transform.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, headShot_damage, MyPlayer.MyPlayerName, MyTeam);
                    }
                    else{
                        print("Hit Body");
                        hit.transform.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.AllBuffered, damage, MyPlayer.MyPlayerName, MyTeam);
                    }
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
 
        }
    }

    
}
