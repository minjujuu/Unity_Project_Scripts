using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] MapPrefabs;
    // 라이트
    public GameObject[] mapLights;

    // 인스턴스 2개 생성해서 반복
    GameObject mapInstance_1;
    GameObject mapInstance_1_background_1;
    GameObject mapInstance_1_background_2;

    GameObject mapInstance_2;
    GameObject mapInstance_2_background_1;
    GameObject mapInstance_2_background_2;

    // 첫 번째 인스턴스의 attack point 
    Transform attachPoint_1L;
    Transform attachPoint_1R;
    
    Transform attachPoint_back1_1L;
    Transform attachPoint_back1_1R;

    Transform attachPoint_back2_1L;
    Transform attachPoint_back2_1R;

    // 두 번째 인스턴스의 attack point 
    Transform attachPoint_2L;
    Transform attachPoint_2R;

    Transform attachPoint_back1_2L;
    Transform attachPoint_back1_2R;

    Transform attachPoint_back2_2L;
    Transform attachPoint_back2_2R;

    
    int randMapIdx;
    void Start()
    {
        SetMapInstance();
    }


    void SetMapInstance()
    {
        // 여러 개 맵 중 하나를 랜덤으로 정함 
        randMapIdx = Random.Range(0, MapPrefabs.Length);
        // 선택된 맵에 맞게 사전에 결정된 Fog Setting을 적용함
        SetFogSetting(randMapIdx);
        // 맵을 위한 light를 생성함
        GameObject light = Instantiate(mapLights[randMapIdx]);
        // 플레이어가 움직여도 light가 적용되도록 하기 위해 light의 parent를 Camera로 설정함 
        light.transform.SetParent(GameObject.FindObjectOfType<Camera>().transform);

        // 모든 맵 프리팹은 세 개의 자식을 가지고 있음
        // Tile, Background_Front, Background_Back
        /* Setting Instance No.1*/
        GameObject Map1 = Instantiate(MapPrefabs[randMapIdx]);
        GameObject MapSets_1 = new GameObject("MapSets_1");
        MapSets_1.name = "MapSets_1";
        
        // 1) Tile에 대한 설정
        mapInstance_1 = Map1.transform.GetChild(0).gameObject;
        mapInstance_1.name = "Map_Instance_1_Field";
        mapInstance_1.transform.SetParent(MapSets_1.transform);

        // 맵 확장 시 추가 인스턴스를 붙일 위치를 받아옴 
        attachPoint_1L = mapInstance_1.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_1R = mapInstance_1.transform.Find("ExpandMapTrigger_right").GetChild(0);

        // 2) Background_Front에 대한 설정  
        mapInstance_1_background_1 = Map1.transform.GetChild(0).gameObject;
        mapInstance_1_background_1.name = "Map_Instance_1_background_1";
        mapInstance_1_background_1.transform.SetParent(MapSets_1.transform);
        // 맵 확장 시 추가 인스턴스를 붙일 위치를 받아옴
        attachPoint_back1_1L = mapInstance_1_background_1.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_back1_1R = mapInstance_1_background_1.transform.Find("ExpandMapTrigger_right").GetChild(0);

        // 2) Background_Back에 대한 설정
        mapInstance_1_background_2 = Map1.transform.GetChild(0).gameObject;
        mapInstance_1_background_2.name = "Map_Instance_1_background_2";
        mapInstance_1_background_2.transform.SetParent(MapSets_1.transform);
        // 맵 확장 시 추가 인스턴스를 붙일 위치를 받아옴
        attachPoint_back2_1L = mapInstance_1_background_2.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_back2_1R = mapInstance_1_background_2.transform.Find("ExpandMapTrigger_right").GetChild(0);

        mapInstance_1.SetActive(true); // 첫번째 맵 인스턴스를 우선 활성화
        mapInstance_1_background_1.SetActive(true);
        mapInstance_1_background_2.SetActive(true);


        /* Setting Instance No.2*/
        GameObject Map2 = Instantiate(MapPrefabs[randMapIdx]);
        GameObject MapSets_2 = new GameObject("MapSets_2");
        MapSets_2.name = "MapSets_2";

        mapInstance_2 = Map2.transform.GetChild(0).gameObject;
        mapInstance_2.name = "Map_Instance_2_Field";
        mapInstance_2.transform.SetParent(MapSets_2.transform);

        attachPoint_2L = mapInstance_2.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_2R = mapInstance_2.transform.Find("ExpandMapTrigger_right").GetChild(0);

        mapInstance_2_background_1 = Map2.transform.GetChild(0).gameObject;
        mapInstance_2_background_1.name = "Map_Instance_2_background_1";
        mapInstance_2_background_1.transform.SetParent(MapSets_2.transform);

        attachPoint_back1_2L = mapInstance_2_background_1.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_back1_2R = mapInstance_2_background_1.transform.Find("ExpandMapTrigger_right").GetChild(0);

        mapInstance_2_background_2 = Map2.transform.GetChild(0).gameObject;
        mapInstance_2_background_2.name = "Map_Instance_2_background_2";
        mapInstance_2_background_2.transform.SetParent(MapSets_2.transform);

        attachPoint_back2_2L = mapInstance_2_background_2.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_back2_2R = mapInstance_2_background_2.transform.Find("ExpandMapTrigger_right").GetChild(0);

        mapInstance_2.SetActive(false); // 두번째 프리팹은 우선 비활성화
        mapInstance_2_background_1.SetActive(false);
        mapInstance_2_background_2.SetActive(false);
    }


    // ExpandMapTrigger에서 호출되는 함수 
    // 어느 Trigger에서 호출되었는지에 따라 맵 인스턴스 위치를 정해서 맵을 확장해줌
    public void SendMapTriggerSignal(int instanceNum, int dir)
    {
        // 만약 instanceNum이 1이고, dir이 0이면 trigger_1L로부터 온 신호
        // 만약 instanceNum이 1이고, dir이 1이면 trigger_1R로부터 온 신호
        if(instanceNum == 1)
        {
            if(dir == 0)
            {
                //Debug.Log("FIELD :: trigger_1L로부터 온 신호");
                mapInstance_2.transform.position = attachPoint_1L.position;
                mapInstance_2.transform.position = new Vector3(mapInstance_2.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("FIELD :: trigger_1R로부터 온 신호");
                mapInstance_2.transform.position = attachPoint_1R.position;
                mapInstance_2.transform.position = new Vector3(mapInstance_2.transform.position.x -25, 0, 0);
            }
            mapInstance_2.SetActive(true);
        }
        
        if(instanceNum == 2)
        {
            if(dir == 0)
            {
                //Debug.Log("FIELD :: trigger_2L로부터 온 신호");
                mapInstance_1.transform.position = attachPoint_2L.position;
                mapInstance_1.transform.position = new Vector3(mapInstance_1.transform.position.x +25, 0, 0);
            }
            else
            {
                //Debug.Log("FIELD :: trigger_2R로부터 온 신호");
                mapInstance_1.transform.position = attachPoint_2R.position;
                mapInstance_1.transform.position = new Vector3(mapInstance_1.transform.position.x -25, 0, 0);
            }
        }

        if(instanceNum == 3)
        {
            if (dir == 0)
            {
                //Debug.Log("BACK1 :: trigger_1L로부터 온 신호");
                mapInstance_2_background_1.transform.position = attachPoint_back1_1L.position;
                mapInstance_2_background_1.transform.position = new Vector3(mapInstance_2_background_1.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK1 :: trigger_1R로부터 온 신호");

                mapInstance_2_background_1.transform.position = attachPoint_back1_1R.position;
                mapInstance_2_background_1.transform.position = new Vector3(mapInstance_2_background_1.transform.position.x - 25, 0, 0);
            }
            mapInstance_2_background_1.SetActive(true);
        }

        if (instanceNum == 4)
        {
            if (dir == 0)
            {
                //Debug.Log("BACK1 :: trigger_L로부터 온 신호");

                mapInstance_1_background_1.transform.position = attachPoint_back1_2L.position;
                mapInstance_1_background_1.transform.position = new Vector3(mapInstance_1_background_1.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK1 :: trigger_R로부터 온 신호");
                mapInstance_1_background_1.transform.position = attachPoint_back1_2R.position;
                mapInstance_1_background_1.transform.position = new Vector3(mapInstance_1_background_1.transform.position.x - 25, 0, 0);
            }
        }

        if (instanceNum == 5) //  Map Instance 1의 Back2
        {
            if (dir == 0)
            {
                //Debug.Log("BACK2 :: trigger_L로부터 온 신호");
                mapInstance_2_background_2.transform.position = attachPoint_back2_1L.position;
                mapInstance_2_background_2.transform.position = new Vector3(mapInstance_2_background_2.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK2 :: trigger_R로부터 온 신호");

                mapInstance_2_background_2.transform.position = attachPoint_back2_1R.position;
                mapInstance_2_background_2.transform.position = new Vector3(mapInstance_2_background_2.transform.position.x - 25, 0, 0);
            }
            mapInstance_2_background_2.SetActive(true);
        }

        if (instanceNum == 6) //  Map Instance 2의 Back2
        {
            if (dir == 0)
            {
                //Debug.Log("BACK2 :: trigger_2L로부터 온 신호");

                mapInstance_1_background_2.transform.position = attachPoint_back2_2L.position;
                mapInstance_1_background_2.transform.position = new Vector3(mapInstance_1_background_2.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK2 :: trigger_2R로부터 온 신호");
                mapInstance_1_background_2.transform.position = attachPoint_back2_2R.position;
                mapInstance_1_background_2.transform.position = new Vector3(mapInstance_1_background_2.transform.position.x - 25, 0, 0);
            }
        }

    }

    public Color fogColor_Map2;
    public Color fogColor_Map3;
    public Color fogColor_Map4; 

    //bool bConverted = ColorUtility.TryParseHtmlString("#4CA7E6FF", out newCol);

    //Color m4_fogColor;
    //bool m4Converted = ColorUtility.TryParseHtmlString("#42748CFF", out m4_fogColor);

    // 이번에 사용할 맵에 따라 기존에 정해진 fog setting을 설정해줌 
    void SetFogSetting(int idx)
    {
        switch (idx)
        {
            case 0: // MAP1
                RenderSettings.fog = false;
                break;
            case 1: // MAP2
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                //fogColor_Map2 = new Color(185, 138, 62);
                RenderSettings.fogColor = fogColor_Map2;
                //RenderSettings.fogColor = new Color(185, 138, 62);
                RenderSettings.fogStartDistance = 14.7f;
                RenderSettings.fogEndDistance = 231.9f;
                break;
            case 2: // MAP3
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                //fogColor_Map3 = new Color(103, 112, 115);
                RenderSettings.fogColor = fogColor_Map3;
                //RenderSettings.fogColor = new Color(103, 112, 115);
                RenderSettings.fogStartDistance = 14.7f;
                RenderSettings.fogEndDistance = 231.9f;
                break;
            case 3: // MAP4
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                //fogColor_Map4 = new Color(66, 116, 140);
                RenderSettings.fogColor = fogColor_Map4;
                //RenderSettings.fogColor = new Color(66, 116, 140);
                RenderSettings.fogStartDistance = 29f;
                RenderSettings.fogEndDistance = 1554.2f;
                break;

        }

    }
}
