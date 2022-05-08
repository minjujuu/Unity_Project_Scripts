using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_PingPongMove : MonoBehaviour
{
    public float pSpeed = 1f;
    public float magnitude = 3;


    void Update()
    {
        transform.position += Vector3.up * Mathf.Sin(Time.time * pSpeed) * magnitude;
        //float per = Mathf.PingPong(Time.time, magnitude);
        //transform.position += Vector3.up * (per - 0.5f) * pSpeed * 0.5f;
    }
}
