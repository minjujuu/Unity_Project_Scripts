using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 파스텔 행성 :: 구름미로
// 1. 플레이어는 미로내에서 구름조각 10개를 모아 출구에 도착해야한다.
// 2. 미로의 끝에는 무지개블랙홀이 있다.
// 3. 구름조각 10개를 모은 채로 무지개블랙홀을 클릭하면
//    구름기구가 나타난다.
//   - 구름기구가 나타나면 모노행성으로 향하는 길이 나타난다.

// - 구름조각 10개는 [파스텔 색 10개]로 되어있다.
// - 각 구름조각에는 [색깔태그]가 있다.
// 사용자가 [마우스로 클릭]한 조각의 태그를 구분한다.
// 해당 색깔을 컬러팔레트.cs의 CloudList에 입력한다.
// 컬러팔레트에서 자신이 모은 구름조각을 확인할 수 있다.

public class CloudMaze : MonoBehaviour {

    Ray ray;
    RaycastHit hitInfo;
    [Header("Prefab / 구름 파티클")]
    public GameObject clickCloudParticle;


    void Start () {

	}

	void Update () {
        ClickCloud();
	}

    void ClickCloud()
    {
        // 마우스 좌클릭시
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 누르는 포지션의 값을 저장
            ray = new Ray(Camera.main.transform.position,
                Camera.main.transform.forward);
            hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo))
            {
                GameObject clickObject = hitInfo.transform.gameObject;
                // 마우스로 누른 오브젝트의 태그가 CLOUD인 경우에만
                if (clickObject.tag == "CLOUD")
                {
                    // 클릭 파티클 생성
                    GameObject clickParticle = Instantiate(clickCloudParticle);
                    clickParticle.transform.position = clickObject.transform.position;

                    // 컬러팔레트 구름리스트에 추가
                    ColorPalette.Instance.InputCloud(clickObject.name.ToString());
                    // 클릭한 오브젝트 삭제
                    Destroy(hitInfo.transform.gameObject);

                }
            }
        }
    }

    // 구름을 다 모아 미로를 탈출하면
    // ColorPalette의 ShowCloudAttraction()메서드 호출
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ClearPastelCollider")
        {
            ColorPalette.Instance.ShowCloudAttraction();
        }
    }

}
