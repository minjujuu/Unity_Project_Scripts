using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어와 상호작용하는 Fish들을 Spawn 하는 스크립트
// 1. 10 마리 Spawn 한다.
// 2. 여러 개의 spawn point 중 플레이어와 가까운 곳에서 spawn 하도록 한다.
public class MJ_FishTank : MonoBehaviour {

    public GameObject MJ_fish;
    static int fishNum = 10;
    public GameObject[] fishs = new GameObject[fishNum];
    public Transform spawnParent;
    Transform playerTr;
    Transform[] points;

    public int delayTime = 5;
	void Start () {
        points = spawnParent.GetComponentsInChildren<Transform>();
        playerTr = Camera.main.transform;
        // 10초에 한 번씩 플레이어와 가까운 point를 탐색한다.
        Invoke("SearchPoint", delayTime);
    }
	
    Vector3 nearPoint;
    void SearchPoint()
    {
        for(int i = 0; i < points.Length; i++)
        {
            nearPoint = points[0].position;
            // 만약 가장 가까운 포인트보다 그 다음 i 포인트의 거리가 더 작으면 nearPoint는 그 다음 포인트로 변경.
            if(Vector3.Distance(nearPoint, playerTr.position) > Vector3.Distance(points[i].position, playerTr.position))
            {
                nearPoint = points[i].position;
                
            }
        }
        
        // 비교가 끝나면 그 nearPoint 주위에 물고기들을 spawn 한다.
        for (int i = 0; i < fishNum; i++)
        {
            Vector3 pos = new Vector3(Random.Range(nearPoint.x, (nearPoint.x - 5)),
                                      Random.Range(nearPoint.y, (nearPoint.y - 5)),
                                      Random.Range(nearPoint.z, (nearPoint.z - 5)));
            fishs[i] = (GameObject)Instantiate(MJ_fish, pos, Quaternion.identity);
        }
        Invoke("SearchPoint", delayTime);
    }


}
