using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 여러개의 Spawn Point에서 바보물고기가 Spawn 된다.
// - 랜덤으로 하나의 Spawn Point에서 Spawn
// 1. SpawnPoint 배열
// 2. 바보물고기 프리팹
// 3. Spawn 시간이 텀
public class SpawnFoolFish : MonoBehaviour {

    public Transform spawnParentObject;
    Transform[] spawnPoints;
    public GameObject foolFish;
    public float delayTime = 10.0f;


	void Start () {
        spawnPoints = spawnParentObject.GetComponentsInChildren<Transform>();

        // delayTime마다 한번씩 랜덤의 Spawn Point에서 한 마리의 바보물고기가 Spawn 된다.
        Invoke("SpawnFish", delayTime);

    }

    void SpawnFish()
    {
        ////print("=============바보 물고기 생성============");
        // 랜덤값을 정한다.
        int ran = Random.Range(0, spawnPoints.Length);
        // 바보불고기를 생성한다.
        GameObject fish = Instantiate(foolFish);
        // 랜덤으로 정해진 spawnPoint의 위치를 바보물고기의 위치로 지정한다.
        fish.transform.position = spawnPoints[ran].position;
        fish.transform.rotation = spawnPoints[ran].rotation;

        // 재귀 호출
        Invoke("SpawnFish", delayTime);
    }

}
