using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모노 행성에서 플레이어는 네온 조각들을 모아 네온사인을 완성시켜야 한다.
// 플레이어가 네온 조각을 클릭하면 네온 조각은 사라진다.
// 플레이어가 클릭해서 모은 해당 네온 조각이 컬러팔레트에 추가된다.
public class CollectNeonPiece : MonoBehaviour {


    Ray ray;
    RaycastHit hitInfo;
    [Header("Prefab / 네온조각 클릭 파티클")]
    public GameObject clickNeonParticle;

    void Update()
    {
        //ClickNeonPiece();
    }

    // 네온 피스는 가까이 가야 얻을 수 있음.
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "NEONPIECE")
        {
            // 클릭 파티클 생성
            GameObject clickParticle = Instantiate(clickNeonParticle);
            clickParticle.transform.position = other.transform.position;
            // 컬러팔레트 네온리스트에 추가
            ColorPalette.Instance.InputNeon(other.gameObject.name.ToString());
            Destroy(other.gameObject);
        }
    }

    //void ClickNeonPiece()
    //{
    //    // 마우스 좌클릭시
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // 마우스 누르는 포지션의 값을 저장
    //        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //        if (Physics.Raycast(ray, out hitInfo))
    //        {
    //            GameObject clickObject = hitInfo.transform.gameObject;
    //            // 마우스로 누른 오브젝트의 태그가 NEONPIECE인 경우에만
    //            if (clickObject.tag == "NEONPIECE")
    //            {
    //                // 클릭 파티클 생성
    //                GameObject clickParticle = Instantiate(clickNeonParticle);
    //                clickParticle.transform.position = clickObject.transform.position;

    //                // 컬러팔레트 구름리스트에 추가
    //                ColorPalette.Instance.InputNeon(clickObject.name.ToString());
    //                // 클릭한 오브젝트 삭제
    //                Destroy(hitInfo.transform.gameObject);
    //            }
    //        }
    //    }
    //}

}
