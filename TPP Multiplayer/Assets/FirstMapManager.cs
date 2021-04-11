using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FirstMapManager : MonoBehaviour
{
    public Camera sceneCam;

    public AudioListener sceneCamAudio;

    public GameObject player;

    [SerializeField]
    public Transform playerSpawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        sceneCam.enabled = false;
        sceneCamAudio.enabled = false;

        PhotonNetwork.Instantiate(player.name, playerSpawnPosition.position, playerSpawnPosition.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
