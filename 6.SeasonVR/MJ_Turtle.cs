using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 거북이
// 1. 지정해준 waypoint를 따라서 움직인다.

public class MJ_Turtle : MonoBehaviour {

    public Transform turtlePointParent;
    public Transform[] turtleWayPoints;

    public float moveSpeed = 2;
    int curIndex = 1;
    void Start () {
        turtleWayPoints = turtlePointParent.GetComponentsInChildren<Transform>();
        gameObject.transform.position = turtleWayPoints[curIndex].position;
	}

	void Update () {
        print(curIndex);
        // 가고자하는 방향(waypoints)으로 움직인다.
        Vector3 dir = turtleWayPoints[curIndex].position - transform.position;
        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 5 * Time.deltaTime);

        if(Vector3.Distance(transform.position, turtleWayPoints[curIndex].position) < 0.5f)
        {
            transform.position = turtleWayPoints[curIndex].position;
            if(turtleWayPoints.Length -1 > curIndex)
            {
                curIndex++;
            }
            else
            {
                gameObject.transform.position = turtleWayPoints[0].position;
                curIndex = 1;
            }
            
        }
	}
}
