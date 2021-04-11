using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputController : MonoBehaviour
{
    [SerializeField] private Camera camera1;

    [SerializeField] private Transform player;

    [SerializeField] private Transform SCelton;

    private Vector2 vector;
    [SerializeField] public FixedTouchField TouchField;
    public float camSpeed = 0.2f;
    // Start is called before the first frame update
    //public Vector3 setUp;

    private Vector3 previousPosition;
    void Start()
    {
       camera1.transform.position = player.position; // - setUp; 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        vector.x = TouchField.TouchDist.x * camSpeed;
        vector.y = TouchField.TouchDist.y * camSpeed;

        previousPosition = camera1.ScreenToViewportPoint(vector);
        
        player.transform.Rotate(new Vector3(1,0,0), -previousPosition.y * 180);
        player.transform.Rotate(new Vector3(0,1,0), previousPosition.x * 180, Space.World);
    }
}
