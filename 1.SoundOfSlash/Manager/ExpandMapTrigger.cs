using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandMapTrigger : MonoBehaviour
{
    MapManager mapManager;
    public int dirIdx; // 왼쪽트리거면 0, 오른쪽 트리거면 1
    int myInstanceNumber;
    private void Start()
    {
        mapManager = GameObject.FindObjectOfType<MapManager>();

        switch(transform.parent.name)
        {
            case "Map_Instance_1_Field":
                myInstanceNumber = 1;
                break;
            case "Map_Instance_2_Field":
                myInstanceNumber = 2;
                break;
            case "Map_Instance_1_background_1":
                myInstanceNumber = 3;
                break;
            case "Map_Instance_2_background_1":
                myInstanceNumber = 4;
                break;
            case "Map_Instance_1_background_2":
                myInstanceNumber = 5;
                break;
            case "Map_Instance_2_background_2":
                myInstanceNumber = 6;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(dirIdx == 0)
            {
                mapManager.SendMapTriggerSignal(myInstanceNumber, 0);
            }
            else
            {
                mapManager.SendMapTriggerSignal(myInstanceNumber, 1);
            }
        }
    }
}
