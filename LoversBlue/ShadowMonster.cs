using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모노행성 : 그림자 괴물
// 1. 플레이어가 행성에 도착하면 벽이나 허공에 그림자괴물이 있다.
//  - 그림자괴물은 플레이어와 마주보고 있다.
// 2. 그림자괴물에게서 공격을 받는다.
//  - 빨간 조명으로 위협을 느끼게 하는 공격이다. 
//  - 공격을 받으면 안개파티클이 하나씩 늘어나서 시야가 흐려진다.
//  - 그러면 색을 칠해야하는 네온사인을 찾기 어려워진다.
// 3. 플레이어는 그림자 괴물의 공격을 피해야한다.

// 모노행성 : 네온사인
// 1. 파스텔행성에서 모은 구름조각들로 네온사인을 칠할 수 있다.
//   - 색깔 매칭 미정
// 2. 네온사인을 완성시키면 
//  - 주변의 모든 색깔행성들의 색이 사라진다.
//  - 무색(흰색)으로 변한다.
//  - 행성 주변을 감싸고 있던 안개 위에 그림자로 된 길이 나타난다.
public class ShadowMonster : MonoBehaviour {

    public RaycastHit hit;
    public Transform RayPos;

    [Header("Prefab / 빨간 불에 맞았을 때 파티클")]
    public GameObject smokeParitcle;

    [Header("Transform / 스모크파티클 생기는 장소_1")]
    public Transform smokePot1;
    public Transform smokePot2;
    public Transform smokePot3;

    int count;
    void Start () {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(RayPos.position, RayPos.forward * 10.0f, Color.red);

        if (Physics.Raycast(RayPos.position, RayPos.forward, out hit, 10.0f))
        {
            if(hit.collider.tag == "Player")
            {
                count++;
                GameObject smoke = Instantiate(smokeParitcle);
                if (count == 1)
                {
                    smoke.transform.position = smokePot1.transform.position;
                }
                else if (count == 2)
                {
                    smoke.transform.position = smokePot2.transform.position;
                }
                else if (count ==3)
                {
                    smoke.transform.position = smokePot3.transform.position;
                }
                
            }
        }

    }
}
