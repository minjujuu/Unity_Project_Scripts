using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 아이템은 회전한다.
// 2. 아이템이 있는 곳에는 파티클이 있다.
//  - 플레이어에게 "아이템은 여기있다" 고 알려줄 수 있는 파티클.
// 3. 아이템을 먹으면 파티클은 사라진다.
public class MJ_EffectOfItem : MonoBehaviour {

    //public GameObject itemParticle;

    private void Start()
    {
        //GameObject particle = Instantiate(itemParticle);
        //particle.transform.position = transform.position;
    }
    void Update () {
        RotatingItem();

	}

    // 아이템을 회전시키는 
    void RotatingItem()
    {
        transform.Rotate(new Vector3(30, 45, 60) * Time.deltaTime);
    }
}
