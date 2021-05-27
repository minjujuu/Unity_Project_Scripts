using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CocktailPiano : MonoBehaviour
{
    AudioSource[] audios;
    // 머리색 획득을 알리는 칵테일
    [Header("Prefab / 머리색 칵테일")]
    public GameObject hairCocktail;
    // 얼굴색 획득을 알리는 칵테일
    [Header("Prefab / 얼굴색 칵테일")]
    public GameObject faceCocktail;
    // 몸색 획득을 알리는 칵테일
    [Header("Prefab / 몸색 칵테일")]
    public GameObject bodyCocktail;

    // 연주 슬라이드바
    [Header("UI / 연주슬라이드바")]
    public Scrollbar playBar;
    // 칵테일 생성 위치
    [Header("Empty / 칵테일 생성위치")]
    public Transform table;
    // hit 파티클
    [Header("Prefab / 연주 파티클")]
    public GameObject pianoPlayParticle;

    // 마우스 클릭을 위한 변수
    Ray pianoCameraRay;
    Ray mainCameraRay;
    RaycastHit pianoHitInfo;
    RaycastHit mainHitInfo;

    // 색깔건반 배열
    List<string> colorKey = new List<string>();

    // 악보정답 배열
    // 머리색 악보
    List<string> hairAnswerKey = new List<string>();
    // 얼굴색 악보
    List<string> faceAnswerKey = new List<string>();
    // 옷색 악보
    List<string> bodyAnswerKey = new List<string>();


    // ------------------- 건반 애니메이션 
    public Animator[] colorKeyAnims;
    public GameObject pianoColorKeys;
    // 애니메이터 컴포넌트를 가져오기 위한 변수들
    //public GameObject RedKey;
    //public GameObject OrangeKey;
    //public GameObject YellowKey;
    //public GameObject GreenKey;
    //public GameObject BlueKey;
    //public GameObject DeepBlueKey;
    //public GameObject PurpleKey;

    // 건반 소리가 7개 있다.
    // 빨 주 노 초 파 남 보
    // AudioSource[1] 를 이용한다.
    public enum colorKeySounds
    {
        RedSound,
        OrangeSound,
        YellowSound,
        GreenSound,
        BlueSound,
        DeepBlueSound,
        PurpleSound
    }

    private void Awake()
    {
        playBar = GameObject.Find("PlayBar").GetComponent<Scrollbar>();
        table = GameObject.Find("Table").GetComponent<Transform>();
        PianoCamera = GameObject.Find("PianoCamera").GetComponent<Camera>();
        mainCamera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
    }
    void Start()
    {

        audios = GetComponents<AudioSource>();
        ChoiceMainCamera();
        mainScreenCircle.Size = 0.0f;

        // 컬러건반 애니메이터 컴포넌트 가져오기
        colorKeyAnims = pianoColorKeys.GetComponentsInChildren<Animator>();

        // 악보정답 배열에 값 설정
        // 머리색 악보 정답 : 파보남빨초노주
        hairAnswerKey.Add("Blue");
        hairAnswerKey.Add("Purple");
        hairAnswerKey.Add("DeepBlue");
        hairAnswerKey.Add("Red");
        hairAnswerKey.Add("Green");
        hairAnswerKey.Add("Yellow");
        hairAnswerKey.Add("Orange");

        // 얼굴색 악보 정답 : 초주남보노파빨
        faceAnswerKey.Add("Green");
        faceAnswerKey.Add("Orange");
        faceAnswerKey.Add("DeepBlue");
        faceAnswerKey.Add("Purple");
        faceAnswerKey.Add("Yellow");
        faceAnswerKey.Add("Blue");
        faceAnswerKey.Add("Red");

        // 몸색 악보 정답 : 남초빨보파노주 
        bodyAnswerKey.Add("DeepBlue");
        bodyAnswerKey.Add("Green");
        bodyAnswerKey.Add("Red");
        bodyAnswerKey.Add("Purple");
        bodyAnswerKey.Add("Blue");
        bodyAnswerKey.Add("Yellow");
        bodyAnswerKey.Add("Orange");

        playBar.size = 0;

    }

    void Update()
    {
        TouchButton();
    }

    RaycastHit hitInfo;
    string colorKeyName;
    string cocktailName;
    void TouchButton()
    {
        // 마우스 좌클릭시
        if (Input.GetMouseButtonDown(0))
        {
            // 특정레이어(pianoKey)만 충돌하게 한다.
            // 플레이어 레이어만 눌리지 않게 한다.
            int playerLayer = LayerMask.NameToLayer("Player");
            int layerMask = 1 << playerLayer;

            pianoCameraRay = PianoCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(pianoCameraRay, out pianoHitInfo, 100, ~layerMask))
            {
                // 마우스로 누른 오브젝트의 태그가 COLORKEY인 경우에만
                if (pianoHitInfo.transform.CompareTag("COLORKEY"))
                {
                    colorKeyName = pianoHitInfo.transform.gameObject.name;
                    print(pianoHitInfo.transform.gameObject.name);
                    // 해당 색의 오디오 재생
                    SetColorKeySound(colorKeyName);
                    // 해당 색 건반의 애니메이션 실행
                    PushColorKeyAnim(colorKeyName);
                    // 색깔건반 배열에 마우스로 클릭한 건반색의 이름을 저장
                    colorKey.Add(colorKeyName);

                    // 클릭 파티클 생성
                    CreateEffect();

                    // 플레이바 채우기
                    FillPlayBar();
                    

                    CheckColorKeys();
                }
                // 마우스로 누른 오브젝트의 태그가 COCKTAIL인 경우에만
                if (pianoHitInfo.transform.gameObject.tag == "COCKTAIL")
                {
                    cocktailName = pianoHitInfo.transform.gameObject.name;
                    // 칵테일을 클릭하면 팔레트에 칵테일 UI가 추가 되도록 !! 
                    ClickCocktail(cocktailName);
                    // 클릭한 칵테일은 사라지도록 
                    Destroy(pianoHitInfo.transform.gameObject);
                }
            }
        }
    }

    // 컬러키의 Push Animation 을 재생하는 메서드
    // - 사용자가 누른 컬러키 건반이 Red이면, Red 애니메이션을 재생한다. 
    void PushColorKeyAnim(string name)
    {
        switch (name)
        {
            case "Red":
                colorKeyAnims[0].SetTrigger("PushRedKey");
                break;
            case "Orange":
                colorKeyAnims[1].SetTrigger("PushOrangeKey");
                break;
            case "Yellow":
                colorKeyAnims[2].SetTrigger("PushYellowKey");
                break;
            case "Green":
                colorKeyAnims[3].SetTrigger("PushGreenKey");
                break;
            case "Blue":
                colorKeyAnims[4].SetTrigger("PushBlueKey");
                break;
            case "DeepBlue":
                colorKeyAnims[5].SetTrigger("PushDeepBlueKey");
                break;
            case "Purple":
                colorKeyAnims[6].SetTrigger("PushPurpleKey");
                break;

        }
            
    }

    public AudioClip[] colorSoundArray;

    void SetColorKeySound(string colorKeyName)
    {
        switch(colorKeyName)
        {
            case "Red":
                audios[1].clip = colorSoundArray[(int)colorKeySounds.RedSound];
                break;
            case "Orange":
                audios[1].clip = colorSoundArray[(int)colorKeySounds.OrangeSound];
                break;
            case "Yellow":
                audios[1].clip = colorSoundArray[(int)colorKeySounds.YellowSound];
                break;
            case "Green":
                audios[1].clip = colorSoundArray[(int)colorKeySounds.GreenSound];
                break;
            case "Blue":
                audios[1].clip = colorSoundArray[(int)colorKeySounds.BlueSound];
                break;
            case "DeepBlue":
                audios[1].clip = colorSoundArray[(int)colorKeySounds.DeepBlueSound];
                break;
            case "Purple":
                audios[1].clip = colorSoundArray[(int)colorKeySounds.PurpleSound];
                break;
        }
        audios[1].Play();
    }

    void CreateEffect()
    {
        // 파티클 생성
        GameObject par = Instantiate(pianoPlayParticle);
        // 파티클 위치를 마우스 포인트로 설정
        par.transform.position = pianoHitInfo.transform.position;
        // 3초 후에 파티클 삭제
        Destroy(par, 3.0f);
    }

    void FillPlayBar()
    {

        // 칵테일 피아노에 붙어있는 PlayerBar UI ++
        playBar.size += 0.15f;
    }


    // 두 리스트의 값 비교를 위한 boolList
    // 모든 값이 true이면 칵테일 생성
    List<bool> boolList = new List<bool>();

    void CheckColorKeys()
    {
        // 연주를 다했으면 어떤 악보를 연주했는지 확인
        if (colorKey.Count == 7)
        {
            playBar.size = 0;
            // 막누른 게 아니고 악보를 연주하려 했다는 조건
            if (colorKey[0].Equals("Blue") || colorKey[0].Equals("Green") || colorKey[0].Equals("DeepBlue"))
            {
                // 악보들 중 하나라도 맞게 건반을 클릭했는지 확인하는 함수
                // 머리색이면 리스트 0 번째가 파란색
                if (colorKey[0].Equals("Blue"))
                {
                    print("===============머리키와 맞나 CheckColorKeys");
                    CheckColorType(ColorType.Hair);
                }
                else if (colorKey[0].Equals("Green"))
                {
                    print("=============얼굴키와 맞나========");
                    CheckColorType(ColorType.Face);
                }
                else if (colorKey[0].Equals("DeepBlue"))
                {
                    print("=============몸키와 맞나========");
                    CheckColorType(ColorType.Body);
                }
            }
            else
            {
                // 플레이어가 막눌렀을 경우
                // 다시 연주할 수 있도록 colorKey 리스트 리셋
                colorKey.Clear();
            }

        }
    }

    // 다시 연주할 수 있도록 셋팅하는 메서드
    void ResetList()
    {
        // 연주배열 초기화 ( 다시 연주할 수 있도록)
        colorKey.Clear();

        // 마찬가지로 비교배열도 초기화
        boolList.Clear();
        //colorOrder = 0;

    }

    enum ColorType
    {
        Hair,
        Face,        
        Body
    }

    // 컬러타입에 따라 맞는 악보인지 체크하고 칵테일 프리팹을 결정한다.
    void CheckColorType(ColorType type)
    {
        List<string> keys;
        GameObject cocktailPrefab;
        if(type == ColorType.Hair)
        {
            keys = hairAnswerKey;
            cocktailPrefab = hairCocktail;
        }
        else if(type  == ColorType.Face)
        {
            keys = faceAnswerKey;
            cocktailPrefab = faceCocktail;
        }
        else
        {
            keys = bodyAnswerKey;
            cocktailPrefab = bodyCocktail;
        }
        // 연주한 건반(colorKey)과 머리색악보와 일치하는지 확인
        for (int h = 0; h < keys.Count; h++)
        {
            // 비교값을 저장
            bool boolValue = colorKey[h].Equals(keys[h]);
            boolList.Add(boolValue);
            // 비교값이 맞으면
            if (boolValue == true)
            {
                // 모든 값 비교가 끝났는데 모두 일치하면 
                // - 칵테일 프리팹 생성
                // - 팔레트에 색 추가
                if (boolList.Count == 7 && !boolList.Contains(false))
                {
                    // 칵테일 생성
                    GameObject colorCocktail = Instantiate(cocktailPrefab);
                    // 칵테일 위치 지정
                    colorCocktail.transform.position = table.position;

                    // 다시 연주할 수 있도록 리스트를 리셋
                    ResetList();
                }
                // 아직 비교를 다 안했으면 반복을 계속
                else
                {
                    continue;
                }
            }
            else
            {
                ResetList();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PIANOZONE")
        {
            StartCoroutine(this.SceneFadeIn());

            PlayUiManager.Instance.HideUI((int)PlayUiManager.textUI.vivid1);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PIANOZONE")
        {
            ChoicePianoCamera();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PIANOZONE")
        {
            ChoiceMainCamera();
            mainScreenCircle.enabled = true;
            StartCoroutine(this.SceneFadeOut());
        }
    }


    // 플레이어가 피아노 ZONE 안에 들어오면
    // - 플레이어에 붙은 메인카메라 대신에
    // - Piano카메라가 활성화 되었으면 좋겠다.
    public Camera PianoCamera;
    public Camera mainCamera;
    void ChoicePianoCamera()
    {
        PianoCamera.depth = 1;
        mainCamera.depth = 0;
    }

    void ChoiceMainCamera()
    {
        mainScreenCircle.enabled = false;
        PianoCamera.depth = 0;
        mainCamera.depth = 1;
    }

    void ClickCocktail(string name)
    {
        if (name.Contains("HairCocktail"))
        {
            if (ColorPalette.Instance != null)
            {
                // 팔레트에 머리색 추가
                ColorPalette.Instance.InputCocktail("Hair");
            }
        }
        if(name.Contains("FaceCocktail"))
        {
            if (ColorPalette.Instance != null)
            {
                // 팔레트에 얼굴색 추가
                ColorPalette.Instance.InputCocktail("Face");
            }
        }
        if(name.Contains("BodyCocktail"))
        {
            if (ColorPalette.Instance != null)
            {
                // 팔레트에 몸색 추가
                ColorPalette.Instance.InputCocktail("Body");
            }
        }
        
    }

    
    // ============= 카메라 페이드인 페이드아웃 ==============
    public CameraFilterPack_TV_WideScreenCircle pianoScreenCircle;

    // 피아노카메라로 바뀔 때 실행되는 코루틴함수
    // 화면이 점점 밝아진다.
    // 화면 테두리는 어두운 상태이다.
    private IEnumerator SceneFadeIn()
    {
        pianoScreenCircle.Size = 0.8f;
        pianoScreenCircle.Smooth = 0.4f;
        float delay = 3f, m_time = 0.0f;
        float increaseValue = delay * 0.01f;
        while (pianoScreenCircle.Size >= 0.7f)
        {
            pianoScreenCircle.Size -= 0.008f;
            pianoScreenCircle.Smooth -= 0.004f;
            m_time += increaseValue;
            yield return new WaitForSeconds(increaseValue);
        }
       // screenCircle.enabled = false;

        yield return null;

    }

    float currentTime = 0;

    // 피아노카메라에서 메인카메라로 전환될 때 실행된다.
    public CameraFilterPack_TV_WideScreenCircle mainScreenCircle;
    private IEnumerator SceneFadeOut()
    {
        mainScreenCircle.Size = 0.7f;
        mainScreenCircle.Smooth = 0.4f;
        float delay = 2f, m_time = 0.0f;
        float increaseValue = delay * 0.01f;
        while (mainScreenCircle.Size <= 0.8f && mainScreenCircle.Smooth > 0.1)
        {
            mainScreenCircle.Size += 0.005f;
            mainScreenCircle.Smooth -= 0.005f;
            m_time += increaseValue;
            yield return new WaitForSeconds(increaseValue);

        }
        yield return null;

    }

}
