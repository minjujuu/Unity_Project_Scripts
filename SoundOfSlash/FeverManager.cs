using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RhythmGameStarter;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using static UnityEngine.Random;
using static MonsterPatternGenerator;

public class FeverManager : MonoBehaviour
{
    [Header("Custom AudioSources")]
    public AudioSource songSource = null;
    public AudioSource feverSource = null;

    [Header("Sub Custom AudioSources")]
    public AudioSource songSourceSub = null;

    [Header("Target AudioClips")]
    public AudioClip feverClip = null;

    [Header("Swap duration")]
    public float swapDuration = 0.5f;

    [Header("Playing State")]
    public int state = 0;

    // =============== UI ================
    [Header("Hide Items on Fever")]
    public SpriteRenderer hide_track;
    public GameObject hide_frameUI;
    public SpriteRenderer hide_TimingUI;
    public GameObject hide_tap_left_bg;
    public GameObject hide_tap_right_bg;
    public GameObject shape_Fever;

    [Header("UI Items on Fever")]
    public Text feverQuit_countDown;
    public GameObject text_Fever;

    // =============== Resource ================
    public BoxCollider[] noteAreas;
    public Transform[] fever_monster_SpawnPoints;
    public GameObject prefab_FeverMon_skeleton;
    public GameObject prefab_FeverMon_golem;
    public PlayerInput inputHandler;
    private Camera mainCamera;

    // =============== Scripts ================
    private CameraFollower cameraScript;
    private SongManager songManager;
    private PlayerCombo player;
    private ScoreManager scoreSystem;
    private HPManager hpManager;

    // =============== Variable for Fever ================
    public int maxFeverGauge;
    private GameObject feverMonstersParent;
    private Transform[] notesParent;
    private int curFeverGauge;
    private int monCount_skel;
    private int monCount_golem;
    private float spawn_breakTime;
    private float nonBlindModeTime;
    private float stopSpawnTime;
    private bool canStartFeverMode;
    private bool isStopSpawn;
    private bool isStartSpawn;
    private bool isStartFever;
    private bool isEndFever;
    private bool isFeverBlind;
    private bool isFinishing = false;
    public GameObject[] tracks;

    // ================ Fever Monster =================
    public float size_skel = 0.6f;

    public float GetFeverPercentage() => (float)curFeverGauge / maxFeverGauge;


    void Start()
    {
        notesParent = new Transform[tracks.Length];

        for (int i = 0; i < tracks.Length; i++)
        {
            notesParent[i] = tracks[i].transform.Find("Notes").transform;
        }

        cameraScript = GameObject.FindObjectOfType<CameraFollower>();
        songManager = GameObject.FindObjectOfType<SongManager>();
        player = GameObject.FindObjectOfType<PlayerCombo>();
        hpManager = GameObject.FindObjectOfType<HPManager>();
        scoreSystem = GameObject.FindObjectOfType<ScoreManager>();
        mainCamera = GameObject.FindObjectOfType<Camera>();

        text_Fever.SetActive(false);
        shape_Fever.SetActive(false);
        feverQuit_countDown.enabled = false;

        monCount_skel = 200;
        monCount_golem = 150;
        spawn_breakTime = 0.2f;
        nonBlindModeTime = 6.0f;
        stopSpawnTime = 7.5f;
        
        isStartSpawn = false;
        isStartFever = false;
        isEndFever = false;
        isFeverBlind = false;
        isStopSpawn = false;

        canStartFeverMode = false;

    }


    private void Update()
    {
        // 피버모드가 시작되면 노트와 몬스터들을 숨김
        if (isFeverBlind)
        {
            NoteAndMonsterActive(false);
        }
    }


    private IEnumerator Co_FadeBGM(AudioSource source, float targetVolume, float duration, Action callback = null)
    {
        float start = source.volume;
        float end = targetVolume;
        float time = 0.0f;

        while (Mathf.Abs(end - source.volume) >= 0.03)
        {
            time += Time.deltaTime / duration;
            source.volume = Mathf.Lerp(start, end, time);
            yield return null;
        }

        source.volume = targetVolume;
        callback?.Invoke();
    }

    public void ControlFeverStart()
    {
        if (IsStartFever())
        {
            Invoke(nameof(StartFeverMode), 0.5f);
        }
    }


    public void StartFeverMode()
    {
        SetInputHandler(false); // 플레이어 노트 입력을 막음 
        InitCurGauge(); // 피버 게이지 초기화

        player.SetFeverMode(true); // 플레이어 코드의 피버 모드를 활성화
        text_Fever.SetActive(true); // Fever UI 활성화
        scoreSystem.SetComboBeforeFever(); // 피버 전 점수 시스템에 대한 전처리
        // hide_frameUI.SetActive(false); // frame UI 비활성화 (temp - 피버 바 불꽃 보여줘야 해서 활성화 상태로 유지할 예정)
        cameraScript.SetFeverCamera(new Vector3(4.3f, 2.2f, 16), 6, 1.5f); // 피버모드 카메라 연출
        
        isEndFever = false;    
        isFeverBlind = true; // 노트 숨김 처리
        SomeObjectsActive(false); // 일부 오브젝트 숨김 처리

        feverMonstersParent = new GameObject("FeverMonstersParent"); // 피버 몬스터 집합의 부모 오브젝트 생성 

        StartCoroutine(ProgressFeverMode()); // 시간에 따라 피버에 필요한 프로세스 진행

        if (IsStartMonSpawn()) // 아직 몬스터를 스폰하지 않았다면 
        {
            StartCoroutine(SpawnFeverMonster()); // 몬스터 스폰 시작
        }
    }


    private IEnumerator SpawnFeverMonster()
    {
        isStartSpawn = true;
        for (int i = 0; i < monCount_skel; i++)
        {
            int ran;
            if (!isStopSpawn)
            {
                ran = UnityEngine.Random.Range(0, fever_monster_SpawnPoints.Length);
                GameObject mon_skel = (GameObject)Instantiate(prefab_FeverMon_skeleton);
                mon_skel.name = "Fever_Monster_Skeleton";
                mon_skel.transform.position = fever_monster_SpawnPoints[ran].position;
                mon_skel.transform.SetParent(feverMonstersParent.transform);
                mon_skel.transform.localScale = new Vector3(size_skel, size_skel, size_skel); 

                if (i < monCount_golem)
                {
                    ran = UnityEngine.Random.Range(0, fever_monster_SpawnPoints.Length);
                    GameObject mon_golem = (GameObject)Instantiate(prefab_FeverMon_golem);
                    mon_golem.name = "Fever_Monster_Golem";
                    mon_golem.transform.position = fever_monster_SpawnPoints[ran].position;
                    mon_golem.transform.SetParent(feverMonstersParent.transform);
                }
                yield return new WaitForSecondsRealtime(spawn_breakTime);
            }
        }
    }


    public void SomeObjectsActive(bool active)
    {

        hide_track.enabled = active;
        shape_Fever.SetActive(!active);
        //hide_frameUI.SetActive(active);
        hide_TimingUI.enabled = active;
        hide_tap_left_bg.SetActive(false);
        hide_tap_right_bg.SetActive(false);
    }

    void NoteAndMonsterActive(bool active)
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            foreach (Note note in tracks[i].transform.Find("Notes").GetComponentsInChildren<Note>())
            {
                note.SetNoteSprite(active);

                Transform mob = note.transform.Find("Monster");
                if(note.gameObject.layer >= NOTE_COMBO_01)
                {
                    for(int j=0; j<note.transform.childCount; j++)
                    {
                        if(note.transform.GetChild(j).tag == "FlyMonSet")
                        {
                            note.transform.GetChild(j).gameObject.SetActive(active);
                        }
                    }
                }
                
                    // 노트에 있는 몬스터도 숨김
                if (mob != null)
                {
                    if (mob.gameObject.layer == 9)
                    {
                        if (!active)
                        {
                            mob.GetComponent<Mob_Golem>().Disable_SkinnedMeshRenderers();
                        }
                        else
                        {
                            mob.GetComponent<Mob_Golem>().Enable_SkinnedMeshRenderers();
                        }

                    }
                    else
                    {
                        if (!active)
                        {
                            mob.GetComponent<Mob_Skeleton>().Disable_SkinnedMeshRenderers();
                        }
                        else
                        {
                            mob.GetComponent<Mob_Skeleton>().Enable_SkinnedMeshRenderers();
                        }

                    }
                }
            }
        }
    }


    private IEnumerator ProgressFeverMode()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        text_Fever.SetActive(false);
        hide_frameUI.SetActive(true);
        yield return new WaitForSecondsRealtime(nonBlindModeTime);
        isFeverBlind = false;
        yield return new WaitForSecondsRealtime(stopSpawnTime-nonBlindModeTime);
        isStopSpawn = true;
        player.SetFeverComboOn();

        // ======== COUNT DOWN ========
        feverQuit_countDown.text = "3";
        feverQuit_countDown.enabled = true;
        
        yield return new WaitForSecondsRealtime(1.0f);
        feverQuit_countDown.text = "2";
        yield return new WaitForSecondsRealtime(1.0f);
        feverQuit_countDown.text = "1";
        yield return new WaitForSecondsRealtime(1.0f);

        if (IsEndFever())
            EndFeverMode();
    }

    public void EndFeverMode()
    {
        StartCoroutine(PrepareGameResume());
    }

    private IEnumerator PrepareGameResume()
    {    
        ClearFeverMob();
        NoteAndMonsterActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        
        StartCoroutine(Co_FadeBGM(feverSource, 0.0f, 0.5f, () => { feverSource.time = 0.0f; }));
        songSource.volume = 0.0f;
        StartCoroutine(Co_FadeBGM(songSource, 0.5f, 0.5f, () => { }));

        isStartSpawn = false;
        canStartFeverMode = false;
        scoreSystem.SuperTimeAfterFever(); // 점수 무적 적용
        hpManager.SetSuperAfterFeverMode(); // HP 무적 적용
        player.SetFeverMode(false);
        SetInputHandler(true);

        if (GameObject.Find("FeverMonstersParent") != null)
        {
            Destroy(GameObject.Find("FeverMonstersParent"));
        }
        
        isStartFever = false;
        isFinishing = false;

        feverQuit_countDown.enabled = false;
        
        SomeObjectsActive(true); // 오브젝트 활성화

    }

    private void SetInputHandler(bool enabled)
    {
        inputHandler.enabled = enabled;
    }


    void ClearFeverMob()
    {
        foreach(Mob_Skeleton skel in GameObject.FindObjectsOfType<Mob_Skeleton>())
        {
            if(skel.IsFeverMode())
            {
                skel.FeverDead();
            }
        }
        foreach (Mob_Golem golem in GameObject.FindObjectsOfType<Mob_Golem>())
        {
            if (golem.IsFeverMode())
            {
                golem.FeverDead();
            }
        }
        isStopSpawn = false;
    }

    // 노트를 칠 때마다 1씩 오름
    public void AddCurGauge(int val)
    {
        curFeverGauge += val;
        //feverSlider.value = curGauge;
        if (GetCurGauge() >= maxFeverGauge)
        {         
            if (!canStartFeverMode)
            {
                canStartFeverMode = true;
            }
        }
    }


    void InitCurGauge()
    {
        curFeverGauge = 0;
    }

    public int GetCurGauge()
    {
        return curFeverGauge;
    }

    public bool CanEnterFeverMode()
    {
        if (canStartFeverMode)
        {
            return true;
        }
        else
            return false;

    }

    private bool IsStartFever()
    {
        if (!isStartFever)
        {
            isStartFever = true;
            return true;
        }
        else return false;
    }

    private bool IsStartMonSpawn()
    {
        if (!isStartSpawn)
        {
            isStartSpawn = true;
            return true;
        }
        else
            return false;
    }

    private bool IsEndFever()
    {
        if (!isEndFever)
        {
            isEndFever = true;
            return true;
        }
        else
            return false;
    }
}
