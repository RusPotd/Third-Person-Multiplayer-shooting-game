using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class event_trigger_shooting : MonoBehaviour
{
    public static bool is_firing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void pointerUp(){
        is_firing = false;
    }

    public void pointerDown(){
        is_firing = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
