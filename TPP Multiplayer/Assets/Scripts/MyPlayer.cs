using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.CrossPlatformInput;
using System.IO;
using TMPro;

public class MyPlayer : MonoBehaviourPun, IPunObservable
{
    public static bool isNotMine;
    public bool IsnotMine;
    public float MoveSpeed = 3f;
    private float copy_move_speed;
    public float smoothRotationTime = 0.12f;
    public float JumpForce = 350f;
    public bool enableMobileInputs = false;
    float currentVeclocity;
    float currentSpeed;
    float speedVelocity;
    private FixedJoystick joystick;
    private Animator anim;
    private Transform cameraTransform;
    public bool getHit = false;

    [SerializeField]
    public AudioSource ShootSound;

    [SerializeField]
    public static bool dead = false;

    public bool AtStartImmortal;

    Vector3 p1 = new Vector3(-24f, -0.23f, 17.68f);
    Vector3 p2 = new Vector3(25.7f, -0.67f, 1.47f);

    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;

    [SerializeField]
    public float playerHealth = 100f;

    [SerializeField] bool Pressed;  

     [SerializeField] bool Crouch = false;  

    protected CapsuleCollider CapsuleCollider;
    protected Rigidbody rigidbody;

    [SerializeField]
    public bool fireClick = false;      
    public bool m_IsGrounded;
    float m_GroundCheckDistance = 0.1f;

    [SerializeField] int Red;
    [SerializeField] int Blue;

    public bool someVar;
    private float distToGround;
    public bool Match_start; 

    public static bool initialise = false;

    public Text KillText;
    public static string MyPlayerName;

    public TextMeshProUGUI MyPlayerNameText;

    private string killName = "";

    private void Awake()
    {
        if(!photonView.IsMine){
            isNotMine = true;
            IsnotMine = true;
            return;
        }
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        try{
            MyPlayerName = (string)File.ReadAllText(Application.persistentDataPath + "/User_Logs/" +"username"+".txt");
        }
        catch(Exception e){
            MyPlayerName = e.ToString();
        }
        //photonView.Owner.NickName = MyPlayerName;
    }
    private void Start()
    {
         if(!photonView.IsMine){
            MyPlayerNameText.text = photonView.Owner.NickName;
           
            if(PhotonNetwork.LocalPlayer.CustomProperties["pos"]!=photonView.Owner.CustomProperties["pos"]){
                GetComponent<Outline>().enabled = true;
                MyPlayerNameText.color = Color.red;
            }
            else{
                MyPlayerNameText.color = Color.yellow;
            }
            isNotMine = true;
            IsnotMine = true;
            return;
        }
        else{
            MyPlayerNameText.text = ""; //PhotonNetwork.NickName;
        }
        anim = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        CapsuleCollider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        fireClick = event_trigger_shooting.is_firing;
        someVar = false;
        m_IsGrounded = false;
        copy_move_speed = MoveSpeed;
        
    }
    
    void Update(){

         if(!photonView.IsMine){
            return;
        }

        fireClick = event_trigger_shooting.is_firing;
        Match_start = GameManager.Match_start;

        if(CrossPlatformInputManager.GetButtonDown("Jump")){
            if(Crouch){
                Crouching();
            }
            if(IsGrounded()){
              rigidbody.AddForce(Vector2.up * JumpForce);
              anim.SetTrigger("Jump");
            }
            
        }

        if(CrossPlatformInputManager.GetButtonDown("crouch")){
            Crouching();
        }

        if(dead==true){
            float step =  10f * Time.deltaTime; // calculate distance to move
            dead = false;
            AtStartImmortal = true;
            if (PhotonNetwork.LocalPlayer.CustomProperties["pos"]=="1")
            {
                //run rpc
                photonView.RPC("RPC_PlayerKilled", RpcTarget.All, 1);
                transform.position = p1;
            }
            else{
                //run rpc
                photonView.RPC("RPC_PlayerKilled", RpcTarget.All, 2);
                transform.position = p2;
            }
            
        }

        if(initialise){    //transform position at start
            initialise = false;
            AtStartImmortal = true;
            if (PhotonNetwork.LocalPlayer.CustomProperties["pos"]=="1")
            {
                MyPlayerNameText.color = Color.red;
                transform.position = p1;
            }
            else{
                MyPlayerNameText.color = Color.blue;
                transform.position = p2;
            }
        }

        m_IsGrounded = IsGrounded();
    }

    void FixedUpdate()
    {
         if(!photonView.IsMine){
            return;
        }

        Vector2 input = Vector2.zero;
        input = new Vector2(joystick.Horizontal, joystick.Vertical);

        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);

        if(Crouch){   //slow down if croch
            if(input.y >= 0.9 || input.y <= -0.9){
                MoveSpeed = copy_move_speed - 1f;
            }
            else{
                MoveSpeed = copy_move_speed - 0.5f;
            }
        }

        transform.Translate(input.x * MoveSpeed * Time.deltaTime, 0, input.y * MoveSpeed * Time.deltaTime);

        if(fireClick){
            anim.SetBool("NetFire", true);
        }
        else{
            anim.SetBool("NetFire", false);
        }

    }

    [PunRPC]
    public void GetDamage(float amount, string EnemyPlayer, string EnemyTeam){
        if(Match_start && PhotonNetwork.LocalPlayer.CustomProperties["pos"]!=EnemyTeam){
            playerHealth -= amount;
            getHit = true;
            if(playerHealth<=0){
                killName = MyPlayerName+"->"+EnemyPlayer;
                print("Damage by : "+ killName);
                DisplayPlayersName(killName);
            }
        }
    }

    [PunRPC]
    public void RPC_PlayerKilled(int team){
        if(team == 1){
            GameManager.Blue++;
        }
        else{
            GameManager.Red++;
        }
    }

    public void DisplayPlayersName(string s){    //locally only ??
        KillText.text = s;
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(3);
        KillText.text = "";
        killName = "";
    }

    void Crouching(){

        //Debug.DrawRay(transform.position, Vector3.up * 2f, Color.green);

        if(Crouch==false){
                CapsuleCollider.height = 0.8f;
                CapsuleCollider.center = new Vector3(CapsuleCollider.center.x, 0.49f, CapsuleCollider.center.z);
                Crouch = true;
                anim.SetBool("cruch", true);
            }
            else{
                MoveSpeed = copy_move_speed;
                CapsuleCollider.height = 1f;
                CapsuleCollider.center = new Vector3(CapsuleCollider.center.x, 0.5f, CapsuleCollider.center.z);
                Crouch = false;
                anim.SetBool("cruch", false);
                
                /*var cantstandup = Physics.Raycast(transform.position, Vector3.up, 2f);

                if(!cantstandup){} */
            }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(fireClick);
        }
        else{
            if((bool)stream.ReceiveNext()){
                MuzzleFlash();
            }
            else{
                FireUp();
            }
        }
    }

    private void FireUp()
    {
         if(photonView.IsMine){
            anim.SetBool("NetFire", false);
        }
        someVar = false;
        fireClick = false;        
        //muzzle.Stop();
        ShootSound.loop = false;
        ShootSound.Stop();
      
    }

    private void MuzzleFlash()
    {
        if(photonView.IsMine){
            anim.SetBool("NetFire", true);
        }
        someVar = true;
        ShootSound.loop = true;
        ShootSound.Play();
    }

    bool IsGrounded()  {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround - 0.3f);
    }
}
