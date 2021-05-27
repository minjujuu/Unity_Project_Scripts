using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 컬러팔레트 : 사용자가 모은 색깔을 확인할 수 있다.
// 키보드 Q를 누르고 있는 동안 컬러팔레트 창을 띄운다.
// ex) 컬러팔레트 배열을 확인 후 있는 색깔을 컬러팔레트 창에 띄운다.
// 컬러팔레트 배열과 UI 배열이 필요하다.
public class ColorPalette : MonoBehaviour {

    [Header("UI / 컬러팔레트 UI Panel")]
    public GameObject colorPalettePanel;

    [Header("GameObject / Go컬러리스 그림자길")]
    public GameObject shadowRoad; 
    [Header("GameObject / 그림자괴물")]
    public GameObject shadowMonster;

    // 비비드 행성을 클리어하면 생기는 것들
    [Header("GameObjet / Go파스텔 피아노 건반길")]
    public GameObject pianoRoad;

    // 파스텔 행성을 클리어하면 생기는 것들

    [Header("GameObject / 구름기구 파티클")]
    public GameObject cloudPangParticle;
    [Header("GameObject / 구름기구 ")]
    public GameObject cloudAttraction;


    // 칵테일피아노 / 구름미로 / 네온조각 / 그림자괴물 스크립트
    CocktailPiano cocktailPianoScript;
    CloudMaze cloudMazeScript;
    CollectNeonPiece collectNeonScript;

    // ColorPalette Panel 의 CanvasRenderer 
    CanvasRenderer panelRenderer;
    // 칵테일, 구름, 네온사인 UI들의 CanvasRenderer
    CanvasRenderer[] colorUiRenderers;

    // 컬러팔레트 배열에 모은 색을 저장한다.
    // - 다른 스크립트에서 컬러팔레트 배열에 접근할 수 있도록
    //   Singleton 패턴 사용
    private static ColorPalette _instance = null;
    public static ColorPalette Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType(typeof(ColorPalette)) as ColorPalette;

                if(_instance == null)
                {
                    Debug.LogError("ColorPalette 오브젝트 비활성화 상태");
                }
            }
            return _instance;
        }
    }

    // ========= 각 스크립트에서 접근할 수 있는 리스트들 =========
    // - 칵테일, 구름, 네온조각을 모으는 순간 각 리스트에 추가된다.
    // 컬러칵테일 리스트
    [HideInInspector]
    public List<string> cocktailList = new List<string>();
    // 컬러구름 리스트
    [HideInInspector]
    public List<string> cloudList = new List<string>();
    // 컬러네온 리스트
    [HideInInspector]
    public List<string> neonList = new List<string>();

    // =============== <Input List> 함수 =========
    // InputCocktail : 외부스크립트에서 접근 가능한 함수
    // 사용자가 모은 칵테일을 cocktailList 에 저장한다.
    public void InputCocktail(string cocktail)
    {
        cocktailList.Add(cocktail);
        if (cocktailList.Contains("Hair") && cocktailList.Contains("Face") && cocktailList.Contains("Body"))
        {
            ClearVividPlanet();
            SoundManager.Instance.PlayPastelSound();
        }
    }

    // InputCloud : 외부스크립트에서 접근 가능한 함수
    // 사용자가 모은 구름을 cloudList 에 저장한다.
    public void InputCloud(string cloud)
    {
        cloudList.Add(cloud);
        if (RightTenClouds() == true)
        {
            ClearPastelPlanet();
            SoundManager.Instance.PlayMonoSound();
        }
    }

    // InputNeon : 외부스크립트에서 접근 가능한 함수
    // 사용자가 모은 네온을 neonList에 저장한다.
    public void InputNeon(string neonPiece)
    {
        print(neonPiece);
        neonList.Add(neonPiece);
        if(neonList.Count == 3)
        {
            ClearMonoPlanet();
            SoundManager.Instance.PlayMainSound();
        }
    }


    // 사용자가 모은 구름의 개수가 10개면 true를 반환하는 메서드
    public bool RightTenClouds()
    {
        if(cloudList.Count == 10)
        {
            return true;
        }
        return false;
    }

    void Start()
    {
        goalCameraScript.SetActive(false);
        moodText.SetAlpha(0);
        // 그림자 괴물은 파스텔 행성을 클리어해야 생긴다.
        shadowMonster.SetActive(false);
        
        cocktailPianoScript = GetComponent<CocktailPiano>();
        // cloudMazeScript는 비비드 행성을 클리어한 후에 활성화된다.
        cloudMazeScript = GetComponent<CloudMaze>();
        cloudMazeScript.enabled = false;
        // shadowMonsterScript는 파스텔 행성을 클리어한 후에 활성화된다.
        collectNeonScript = GetComponent<CollectNeonPiece>();
        collectNeonScript.enabled = false;

        // Color Palette Panel은 Q를 눌렀을 때만 눈에 보여야 한다.
        // - 시작하자마자 보이지 않게 설정.
        panelRenderer = colorPalettePanel.GetComponent<CanvasRenderer>();
        panelRenderer.SetAlpha(0);

        // 색 UI들도 마찬가지로 Q를 눌렀을 때만 눈에 보여야 한다.
        // - 시작하자마자 보이지 않게 설정       
        colorUiRenderers = colorPalettePanel.GetComponentsInChildren<CanvasRenderer>();
        foreach(CanvasRenderer cr in colorUiRenderers)
        {
            cr.SetAlpha(0);
        }
  
    }
    void Update()
    {
        // 만약 Q키를 누르고 있다면 
        if (Input.GetKey(KeyCode.Q)) 
        {
            // 어떤 칵테일을 모았는지 확인
            CheckCocktail();
            // 어떤 구름을 모았는지 확인
            CheckCloud();
            // 어떤 네온조각을 모았는지 확인
            CheckNeonPiece();
            // 컬러팔레트를 확인할 수 있다.
            panelRenderer.SetAlpha(100.0f);
            // "My Color Palette" Text 는 보여야 하므로
            colorUiRenderers[1].SetAlpha(100.0f);
        }
        else
        { // Q키에서 손을 떼면 컬러팔레트가 사라진다.
            panelRenderer.SetAlpha(0);
            foreach (CanvasRenderer cr in colorUiRenderers)
            {
                cr.SetAlpha(0);
            }
        }
    }

    // CheckCocktail() : 플레이어가 어떤 색을 모았는지 확인하는 메서드
    // - InputColor() 메서드를 통해 다른 스크립트에서 넘어온 색을 palette 배열을 통해 확인한다.
    // - palette 배열에 있는 색 확인
    // ex) red가 있다면, red UI 띄움.
    void CheckCocktail()
    {
        if (cocktailList.Contains("Hair"))
        {
            // 머리색 칵테일 보이게
            colorUiRenderers[2].SetAlpha(100);
        }
        if (cocktailList.Contains("Face"))
        {
            // 얼굴색 칵테일 보이게
            colorUiRenderers[3].SetAlpha(100);
        }
        if (cocktailList.Contains("Body"))
        {
            // 몸색 칵테일 보이게
            colorUiRenderers[4].SetAlpha(100);
        }

    }
    [Header("Particle / 컬러리스 행성 비파티클")]
    public GameObject rainfallParticle;
    // =============== 각 행성을 클리어했을 때 호출되는 메서드들 =========
    // 칵테일 세 개를 모두 모았을 때 실행되는 메서드
    // - 카메라 페이드인 이펙트를 더이상 사용하지 않는다. 
    // - 피아노 카메라를 사용하지 않는다.
    // - 카메라 충돌체를 사용하지 않는다.
    // - 파스텔 행성으로 가는 길이 생긴다
    // - 꽃이 생긴다
    // 명대사를 보여준다
    public CanvasRenderer moodText;
    void ClearVividPlanet()
    {
        moodText.SetAlpha(100);
        // 폐 UI 를 건강하게 바꾼다
        LungsUiManager.Instance.ShowHealthLungs();
        // 컬러리스 행성의 비 파티클을 비활성화시킨다.
        rainfallParticle.SetActive(false);
        // 길이 생긴다.
        pianoRoad.SetActive(true);
        // 피아노에 가까이 가도 아무일이 일어나지 않도록 
        // - 피아노 충돌체 비활성화
        //pianoZone.SetActive(false);
        // -------- 파스텔 행성과 스크립트 활성화 -------
        // 파스텔행성의 CloudMaze 스크립트 활성화
        cloudMazeScript.enabled = true;
        ShowCrossHead.Instance.ShowHead();
    }

    // 구름을 모두 모았을 때 실행되는 메서드
    // - 모노행성으로 가는 그림자 길이 생긴다.
    void ClearPastelPlanet()
    {
        // 건강이 갑자기 안좋아진 폐를 보여준다
        LungsUiManager.Instance.ShowBadLungs();
        // 그림자 길 보이도록.
        // 파티클 생기도록.
        cloudPangParticle.SetActive(true);
        // -------- 비비드 행성 스크립트 비활성화 -------
        // 칵테일피아노 스크립트 비활성화
        cocktailPianoScript.enabled = false;
        // 파스텔 행성의 CloudMaze 스크립트 비활성화
        cloudMazeScript.enabled = false;
        // 모노행성의 Shadow Monster 스크립트 활성화
        collectNeonScript.enabled = true;
        shadowMonster.SetActive(true);
        ShowCrossHead.Instance.HideHead();

    }

    public void ShowCloudAttraction()
    {
        cloudAttraction.SetActive(true);
    }

    // 모노 행성에서 네온 조각을 모두 모았을 때 실행되는 메서드
    // 모노행성에 도착했을 때 모든 행성을 비활성화 했었으므로,
    // 다시 모든 행성을 활성화
    // * 컬러리스 행성
    //  - 그림을 보호하고 있던 보호막을 비활성화
    //  - 비가 아닌 새로운 파티클 활성화
    [Header("GameObject / 그림 보호막")]
    public GameObject lockSketch;
    [Header("Particle / 모든 행성 클리어 파티클")]
    public GameObject goalParticle;
    public GameObject goalCameraScript;
    void ClearMonoPlanet()
    {

        lockSketch.SetActive(false);

        // 모든 행성을 클리어 성공 파티클이 보인다.
        goalParticle.SetActive(true);
        // 모든행성을 활성화
        //GoalManager.Instance.ShowAllPlanet();

        PlayUiManager.Instance.ShowUI((int)PlayUiManager.textUI.monoclear);

        goalCameraScript.SetActive(true);
        
        Camera.main.GetComponent<AudioListener>().enabled = false;
        // 건강이 좋아진 폐 UI를 보여준다.
        LungsUiManager.Instance.ShowHealthLungs();
        // 컬러리스 행성으로 가는 길이 생긴다.
        shadowRoad.SetActive(true);
        //// 모노행성의 Shadow Monster 스크립트 비활성화
        //collectNeonScript.enabled = false;

        // 모노 행성을 클리어하면
        // 스케치을 감싸고 있던 막이 없어진다.

    }


    // =============== 어떤 구름을 모았는지 확인하여 UI를 띄우는 메서드 =========
    void CheckCloud()
    {
        if(cloudList.Contains("Cloud1"))
        {
            colorUiRenderers[5].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud2"))
        {
            colorUiRenderers[6].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud3"))
        {
            colorUiRenderers[7].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud4"))
        {
            colorUiRenderers[8].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud5"))
        {
            colorUiRenderers[9].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud6"))
        {
            colorUiRenderers[10].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud7"))
        {
            colorUiRenderers[11].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud8"))
        {
            colorUiRenderers[12].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud9"))
        {
            colorUiRenderers[13].SetAlpha(100);
        }
        if (cloudList.Contains("Cloud10"))
        {
            colorUiRenderers[14].SetAlpha(100);
        }
    }

    // =============== 어떤 네온조각을 모았는지 확인하여 UI를 띄우는 메서드 =========
    void CheckNeonPiece()
    {
        if (neonList.Contains("NeonPiece1"))
        {
            // 첫 번째 네온사인 조각 보이게
            colorUiRenderers[15].SetAlpha(100);
        }
        if (neonList.Contains("NeonPiece2"))
        {
            // 두 번째 네온사인 조각 보이게
            colorUiRenderers[16].SetAlpha(100);
        }
        if (neonList.Contains("NeonPiece3"))
        {
            // 세 번째 네온사인 조각 보이게
            colorUiRenderers[17].SetAlpha(100);
        }
    }
}
