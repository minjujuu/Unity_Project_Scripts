using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VIVID 방에서 모은 색깔로 COLORLESS 방에 있는 그림을 채운다.
// 1. 플레이어의 팔레트에 있는 색 (ColorPalette.cs)
// 2. 그림을 색칠할 때 필요한 색 
// - 1, 2번을 비교하여 같은 색이 있으면 그림이 색칠된다.
// < 색칠방법 >
// 스케치에 필요한 색은 빨, 초, 파로 가정한다.
// 마우스로 그림을 클릭했을 때,
// 팔레트에 빨, 초, 파 색깔이 있으면
// 해당 색이 필요한 부분에 색칠이 된다.
public class PaintPicture : MonoBehaviour {
    //  그림에 필요한 색 : 머리 / 얼굴 / 옷 / 배경
    //1. 머리
    //2. 머리 + 얼굴
    //3. 머리 + 얼굴 + 옷
    //4. 머리 + 얼굴 + 옷 + 배경(웃는 얼굴)
    //5. 얼굴 
    //6. 얼굴 + 옷 
    //7. 옷
    //8. 옷 + 머리

    // 빨강 : 머리
    // 초록 : 얼굴
    // 파랑 : 몸

    // ColorPalette의 palette 리스트를 담을 리스트
    List<string> playerColorPalette = new List<string>();

    // 머티리얼이 변경될 스케치 오브젝트
    [Header("컬러리스 행성에 있는 그림 오브젝트")]
    public GameObject sketch;
    // 스케치의 메쉬렌더러
    MeshRenderer sketchMr;
    // 스케치의 머터리얼들
    Material[] sketchMts;
    //// 머리 칠해진 그림
    //public Material hairMaterial;
    //// 얼굴 칠해진 그림
    //public Material faceMaterial;
    //// 몸 칠해진 그림
    //public Material bodyMaterial;
    

    // 마우스 클릭을 위한 변수
    Ray ray;
    RaycastHit hitInfo;

    void Start () {
        sketchMr = sketch.GetComponent<MeshRenderer>();
        sketchMts = sketch.GetComponent<MeshRenderer>().materials;
        
        sketchMr.material = sketchMts[0];

        
    }

    public Camera sketchCamera;
    Ray sketchCameraRay;
	void Update () {
        sketchCameraRay = sketchCamera.ScreenPointToRay(Input.mousePosition);
        // 플레이어가 마우스로 그림을 클릭하면
        // 플레이어의 팔레트를 확인해서 그림을 해당 색이 있는 머티리얼로 바꾼다.
        // 마우스 좌클릭시
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(sketchCameraRay, out hitInfo))
            {
                // 마우스로 누른 오브젝트의 태그가 SKETCH인 경우에만
                if (hitInfo.transform.gameObject.tag == "SKETCH")
                {
                    print("SKETCH 클릭");
                    // 플레이어 팔레트 복사
                    foreach (string col in ColorPalette.Instance.cocktailList)
                    {
                        playerColorPalette.Add(col);
                    }
                    PaintSketchAll();
                    //checkPalette();
                }
            }
        }

    }


    //// 플레이어의 팔레트를 확인하는 메서드
    //void checkPalette()
    //{
    //    // 1. 팔레트에 빨간색이 있으면
    //    if(playerColorPalette.Contains("Hair"))
    //    {
    //        // 빨간색이 칠해진 그림으로 바꿈.
    //        sketchMr.material = hairMaterial;
    //        // 2. 만약 빨간색도 있고 초록색도 있으면
    //        if (playerColorPalette.Contains("Hair") && playerColorPalette.Contains("Face"))
    //        {
    //            // 빨간색도 있고 초록색도 있는 머티리얼로 변경
    //            print("빨 + 초");
    //        }

    //        // 3. 만약 빨강, 초록, 파랑 다 있으면
    //        if(playerColorPalette.Contains("Hair") && playerColorPalette.Contains("Face") && playerColorPalette.Contains("Body"))
    //        {
    //            // 빨, 초, 파 다 있는 머티리얼로 변경
    //        }

    //        // 4. 만약, 모든 색을 가지고 있고 폐의 방에 꽃을 다 채웠으면
    //        // 완성된 그림으로 변경
    //    }
    //    // 5. 팔레트에 초록색이 있으면
    //    if(playerColorPalette.Contains("Face"))
    //    {
    //        // 초록색이 칠해진 그림으로 바꿈.
    //        sketchMr.material = faceMaterial;
    //        // 6. 만약 초록, 파랑 다 있으면
    //        if (playerColorPalette.Contains("Face") && playerColorPalette.Contains("Body"))
    //        {
    //            // 초록 파랑이 있는 머티리얼로 변경
    //        }
    //    }
    //    // 팔레트에 파란색이 있으면
    //    if(playerColorPalette.Contains("Body"))
    //    {
    //        // 파란색이 칠해진 그림으로 바꿈.
    //        sketchMr.material = bodyMaterial;
    //        // 7. 만약 파랑, 빨강이 있으면
    //        if(playerColorPalette.Contains("Body") && playerColorPalette.Contains("Hair"))
    //        {
    //            // 파랑, 빨강 있는 그림으로 변경
    //        }
    //    }
    //}

    void PaintSketchAll()
    {
        if(playerColorPalette.Contains("Hair") && playerColorPalette.Contains("Face") 
            && playerColorPalette.Contains("Body"))
        {
            sketchMr.material = sketchMts[1];

        }
    }
}
