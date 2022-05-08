using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject[] MapPrefabs;
    // ����Ʈ
    public GameObject[] mapLights;

    // �ν��Ͻ� 2�� �����ؼ� �ݺ�
    GameObject mapInstance_1;
    GameObject mapInstance_1_background_1;
    GameObject mapInstance_1_background_2;

    GameObject mapInstance_2;
    GameObject mapInstance_2_background_1;
    GameObject mapInstance_2_background_2;

    // ù ��° �ν��Ͻ��� attack point 
    Transform attachPoint_1L;
    Transform attachPoint_1R;
    
    Transform attachPoint_back1_1L;
    Transform attachPoint_back1_1R;

    Transform attachPoint_back2_1L;
    Transform attachPoint_back2_1R;

    // �� ��° �ν��Ͻ��� attack point 
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
        // ���� �� �� �� �ϳ��� �������� ���� 
        randMapIdx = Random.Range(0, MapPrefabs.Length);
        // ���õ� �ʿ� �°� ������ ������ Fog Setting�� ������
        SetFogSetting(randMapIdx);
        // ���� ���� light�� ������
        GameObject light = Instantiate(mapLights[randMapIdx]);
        // �÷��̾ �������� light�� ����ǵ��� �ϱ� ���� light�� parent�� Camera�� ������ 
        light.transform.SetParent(GameObject.FindObjectOfType<Camera>().transform);

        // ��� �� �������� �� ���� �ڽ��� ������ ����
        // Tile, Background_Front, Background_Back
        /* Setting Instance No.1*/
        GameObject Map1 = Instantiate(MapPrefabs[randMapIdx]);
        GameObject MapSets_1 = new GameObject("MapSets_1");
        MapSets_1.name = "MapSets_1";
        
        // 1) Tile�� ���� ����
        mapInstance_1 = Map1.transform.GetChild(0).gameObject;
        mapInstance_1.name = "Map_Instance_1_Field";
        mapInstance_1.transform.SetParent(MapSets_1.transform);

        // �� Ȯ�� �� �߰� �ν��Ͻ��� ���� ��ġ�� �޾ƿ� 
        attachPoint_1L = mapInstance_1.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_1R = mapInstance_1.transform.Find("ExpandMapTrigger_right").GetChild(0);

        // 2) Background_Front�� ���� ����  
        mapInstance_1_background_1 = Map1.transform.GetChild(0).gameObject;
        mapInstance_1_background_1.name = "Map_Instance_1_background_1";
        mapInstance_1_background_1.transform.SetParent(MapSets_1.transform);
        // �� Ȯ�� �� �߰� �ν��Ͻ��� ���� ��ġ�� �޾ƿ�
        attachPoint_back1_1L = mapInstance_1_background_1.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_back1_1R = mapInstance_1_background_1.transform.Find("ExpandMapTrigger_right").GetChild(0);

        // 2) Background_Back�� ���� ����
        mapInstance_1_background_2 = Map1.transform.GetChild(0).gameObject;
        mapInstance_1_background_2.name = "Map_Instance_1_background_2";
        mapInstance_1_background_2.transform.SetParent(MapSets_1.transform);
        // �� Ȯ�� �� �߰� �ν��Ͻ��� ���� ��ġ�� �޾ƿ�
        attachPoint_back2_1L = mapInstance_1_background_2.transform.Find("ExpandMapTrigger_left").GetChild(0);
        attachPoint_back2_1R = mapInstance_1_background_2.transform.Find("ExpandMapTrigger_right").GetChild(0);

        mapInstance_1.SetActive(true); // ù��° �� �ν��Ͻ��� �켱 Ȱ��ȭ
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

        mapInstance_2.SetActive(false); // �ι�° �������� �켱 ��Ȱ��ȭ
        mapInstance_2_background_1.SetActive(false);
        mapInstance_2_background_2.SetActive(false);
    }


    // ExpandMapTrigger���� ȣ��Ǵ� �Լ� 
    // ��� Trigger���� ȣ��Ǿ������� ���� �� �ν��Ͻ� ��ġ�� ���ؼ� ���� Ȯ������
    public void SendMapTriggerSignal(int instanceNum, int dir)
    {
        // ���� instanceNum�� 1�̰�, dir�� 0�̸� trigger_1L�κ��� �� ��ȣ
        // ���� instanceNum�� 1�̰�, dir�� 1�̸� trigger_1R�κ��� �� ��ȣ
        if(instanceNum == 1)
        {
            if(dir == 0)
            {
                //Debug.Log("FIELD :: trigger_1L�κ��� �� ��ȣ");
                mapInstance_2.transform.position = attachPoint_1L.position;
                mapInstance_2.transform.position = new Vector3(mapInstance_2.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("FIELD :: trigger_1R�κ��� �� ��ȣ");
                mapInstance_2.transform.position = attachPoint_1R.position;
                mapInstance_2.transform.position = new Vector3(mapInstance_2.transform.position.x -25, 0, 0);
            }
            mapInstance_2.SetActive(true);
        }
        
        if(instanceNum == 2)
        {
            if(dir == 0)
            {
                //Debug.Log("FIELD :: trigger_2L�κ��� �� ��ȣ");
                mapInstance_1.transform.position = attachPoint_2L.position;
                mapInstance_1.transform.position = new Vector3(mapInstance_1.transform.position.x +25, 0, 0);
            }
            else
            {
                //Debug.Log("FIELD :: trigger_2R�κ��� �� ��ȣ");
                mapInstance_1.transform.position = attachPoint_2R.position;
                mapInstance_1.transform.position = new Vector3(mapInstance_1.transform.position.x -25, 0, 0);
            }
        }

        if(instanceNum == 3)
        {
            if (dir == 0)
            {
                //Debug.Log("BACK1 :: trigger_1L�κ��� �� ��ȣ");
                mapInstance_2_background_1.transform.position = attachPoint_back1_1L.position;
                mapInstance_2_background_1.transform.position = new Vector3(mapInstance_2_background_1.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK1 :: trigger_1R�κ��� �� ��ȣ");

                mapInstance_2_background_1.transform.position = attachPoint_back1_1R.position;
                mapInstance_2_background_1.transform.position = new Vector3(mapInstance_2_background_1.transform.position.x - 25, 0, 0);
            }
            mapInstance_2_background_1.SetActive(true);
        }

        if (instanceNum == 4)
        {
            if (dir == 0)
            {
                //Debug.Log("BACK1 :: trigger_L�κ��� �� ��ȣ");

                mapInstance_1_background_1.transform.position = attachPoint_back1_2L.position;
                mapInstance_1_background_1.transform.position = new Vector3(mapInstance_1_background_1.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK1 :: trigger_R�κ��� �� ��ȣ");
                mapInstance_1_background_1.transform.position = attachPoint_back1_2R.position;
                mapInstance_1_background_1.transform.position = new Vector3(mapInstance_1_background_1.transform.position.x - 25, 0, 0);
            }
        }

        if (instanceNum == 5) //  Map Instance 1�� Back2
        {
            if (dir == 0)
            {
                //Debug.Log("BACK2 :: trigger_L�κ��� �� ��ȣ");
                mapInstance_2_background_2.transform.position = attachPoint_back2_1L.position;
                mapInstance_2_background_2.transform.position = new Vector3(mapInstance_2_background_2.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK2 :: trigger_R�κ��� �� ��ȣ");

                mapInstance_2_background_2.transform.position = attachPoint_back2_1R.position;
                mapInstance_2_background_2.transform.position = new Vector3(mapInstance_2_background_2.transform.position.x - 25, 0, 0);
            }
            mapInstance_2_background_2.SetActive(true);
        }

        if (instanceNum == 6) //  Map Instance 2�� Back2
        {
            if (dir == 0)
            {
                //Debug.Log("BACK2 :: trigger_2L�κ��� �� ��ȣ");

                mapInstance_1_background_2.transform.position = attachPoint_back2_2L.position;
                mapInstance_1_background_2.transform.position = new Vector3(mapInstance_1_background_2.transform.position.x + 25, 0, 0);
            }
            else
            {
                //Debug.Log("BACK2 :: trigger_2R�κ��� �� ��ȣ");
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

    // �̹��� ����� �ʿ� ���� ������ ������ fog setting�� �������� 
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
