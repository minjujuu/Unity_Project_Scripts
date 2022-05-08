using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// < 폐 UI를 관리하는 스크립트 >
// 1) 1분에 한 개씩 눈꽃이 생긴다. 
//   - OneMinuteOneSnow()
// 2) 
public class LungsUiManager : MonoBehaviour {

    private static LungsUiManager _instance = null;
    public static LungsUiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(LungsUiManager)) as LungsUiManager;

                if (_instance == null)
                {
                    Debug.LogError("LungsUiManager 오브젝트 비활성화 상태");
                }
            }
            return _instance;
        }
    }

    public CanvasRenderer[] lungsSnowRenderers;
    public GameObject SnowsGroup;

    // 건강한 폐 UI
    public CanvasRenderer HealthLungs;
    // 나빠져가는 폐 UI
    public CanvasRenderer MidLungs;
    // 안좋은 폐UI
    public CanvasRenderer blackLungs;
    // Use this for initialization
    void Start () {
        HealthLungs.SetAlpha(0);
        MidLungs.SetAlpha(0);
        // 처음에는 모든 눈꽃을 보이지 않게 한다.
        lungsSnowRenderers = SnowsGroup.GetComponentsInChildren<CanvasRenderer>();
        foreach (CanvasRenderer cr in lungsSnowRenderers)
        {
            cr.SetAlpha(0);
        }
    }

    bool IsHealthLungs = false;
    float currentTime = 0;
    int order = 0;
    void Update () {
        if(IsHealthLungs == true)
        {
            // 시간이 흐르게 한다.
            currentTime += Time.deltaTime;
            // 1분에 한개씩 snow가 생긴다.
            if (currentTime > 50.0f)
            {
                OneMinuteOneSnow(order);
                order++;
                if (order == 3)
                {
                    ShowMidLungs();
                }
            }
        }
	}

    // 눈꽃 다섯개의 캔버스 렌더러들을 배열에 담는다. 
    // 1분마다 하나의 눈꽃 캔버스렌더러의 알파를 100으로 한다.
    
    void OneMinuteOneSnow(int order)
    {
        if(order < 6)
        {
            lungsSnowRenderers[order].SetAlpha(100);
            currentTime = 0;
        }

    }

    public void ShowHealthLungs()
    {
        HealthLungs.SetAlpha(100);
        IsHealthLungs = true;
    }

    public void ShowMidLungs()
    {
        MidLungs.SetAlpha(100);
    }

    public void ShowBadLungs()
    {
        blackLungs.SetAlpha(100);
    }
}
