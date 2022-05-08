using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RhythmGameStarter;

using static MonsterPatternGenerator;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    // ============== Enum ============== 
    class Direction
    {
        public const int TRACK_1 = 0;
        public const int TRACK_2 = 1;
        public const int TRACK_3 = 2;
        public const int TRACK_4 = 3;
        public const int TRACK_5 = 4;
        public const int TRACK_6 = 5;
    }

    class Layer
    {
        public const int GOLEM = 9;
        public const int SKELETON = 10;
        public const int CROWN_SKELETON = 16;
    }

    class FeverTapCount
    {
        // 피버에서의 입력은 anykey이므로, 각 콤보 입력 수를 더 크게 설정함
        public const int COMBO_1 = 1;
        public const int COMBO_2 = 4;
        public const int COMBO_3 = 6;
        public const int COMBO_4 = 8;
        public const int COMBO_5 = 10;
    }


    // ============== Player ============== 
    public Transform spawnPointsParent;
    private Animator playerAnim;
    private Rigidbody rig;
    private CapsuleCollider selfCollider;
    private bool isDead = false;
    private bool isAutoPlayer = false;
    private bool isBreakCombo = false;

    // ============== Resource ============== 
    public Image img_comboSignal;
    public SpriteRenderer spriteRenderer_timingZone;
    public Sprite sprite_timingZone_default;
    public Sprite sprite_timingZone_bling;

    // ============== Attack & Dash  ============== 
    public float dashValue_front = 4f;
    public float dashValue_back = 2.5f;
    public float val_canDashAndAttack = 1;
    private float playerShiftValue = 2.5f;
    private GameObject mon_left;
    private GameObject mon_right;
    private GameObject targetMob;
    private GameObject noteChildMob;
    private GameObject target_golemMob;
    private GameObject target_crownSkelMob;
    private int inputCount_golem = 0;
    private int inputCount_crownSkel = 0;
    private int attackAnimSwitchCount = 0;
    private bool canDashAndAttack = false;

    // ============== Combo ============== 
    private ComboSystem comboSystem;
    private int comboInputNum = 0;
    private int curComboPattern = 0;
    private int comboTimes = 0;
    private int totalComboTimes = 0;
    private float comboMon_xPos = 0;
    private bool isComboSignal = false;
    private bool isComboWaiting = false;
    private bool isComboNoteMissed = false;
    private int[] optionNums = new int[30];
    private int[] flags = new int[30];
    private GameObject comboMonSet;
    private GameObject tempComboMon;

    public NoteArea detectComboNoteArea2; // Track 2 - Note Area
    public NoteArea detectComboNoteArea4;
    public NoteArea[] noteAreas;

    // ============== Fever ============== 
    public SphereCollider[] feverWeaponColliders;
    private Transform fever_mobParent;
    private int fever_comboTapNum = 0;
    private int fever_dirLeft = 0;
    private int fever_dirRight = 0;
    private int fever_curDir = 0;
    private bool isFeverMode = false;
    private bool canStartFeverCombo = false;
    private bool isExecutingFeverCombo = false;
    private bool canStartFever = false;
    private bool isFeverComboWaiting = false;
    private float fever_playerYPosOnCombo = 0.5f;
    private float fever_startComboTime = 9f;


    // ============== Manager ============== 
    private SpawnManager spawnManager;
    private EffectManager effManager;
    private FeverManager feverManager;
    private HPManager hpManager;
    private ScoreManager scoreSystem;


    // ============== Etc ============== 
    public CameraFollower cameraFollower = null;
    public CameraQuake cameraQuake = null;

    private InputAction inputAction_track1;
    private InputAction inputAction_track2;
    private InputAction inputAction_track3;
    private InputAction inputAction_track4;
    private InputAction inputAction_track5;
    private InputAction inputAction_track6;
    /* Swipe note를 위한 키들 */
    private InputAction inputAction_upRef; 
    private InputAction inputAction_downRef;
    private InputAction inputAction_leftRef;
    private InputAction inputAction_rightRef;

    public InputSystemHandler inputHandlerInTrack;

    private bool isPressed_Track1 = false;
    private bool isPressed_Track2 = false;
    private bool isPressed_Track3 = false;
    private bool isPressed_Track4 = false;
    private bool isPressed_Track5 = false;
    private bool isPressed_Track6 = false;


    // ======================== Unity Event ============================

    private void Awake()
    {
        comboSystem = GameObject.FindObjectOfType<ComboSystem>();
        feverManager = GameObject.FindObjectOfType<FeverManager>();
        effManager = GameObject.FindObjectOfType<EffectManager>();
        scoreSystem = GameObject.FindObjectOfType<ScoreManager>();
        hpManager = GameObject.FindObjectOfType<HPManager>();
        spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        hpManager = GameObject.FindObjectOfType<HPManager>();

        Initialize();

        inputAction_track1 = inputHandlerInTrack.keyMapping[0].action;
        inputAction_track2 = inputHandlerInTrack.keyMapping[1].action;
        inputAction_track3 = inputHandlerInTrack.keyMapping[2].action;
        inputAction_track4 = inputHandlerInTrack.keyMapping[3].action;
        inputAction_track5 = inputHandlerInTrack.keyMapping[4].action;
        inputAction_track6 = inputHandlerInTrack.keyMapping[5].action;

        inputAction_upRef = inputHandlerInTrack.upRef;
        inputAction_downRef = inputHandlerInTrack.downRef;
        inputAction_leftRef = inputHandlerInTrack.leftRef;
        inputAction_rightRef = inputHandlerInTrack.rightRef;
       

    }
    public void Initialize()
    {
        playerAnim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        selfCollider = GetComponent<CapsuleCollider>();

        /* InitComponentValue */
        img_comboSignal.enabled = false;
        spriteRenderer_timingZone.sprite = sprite_timingZone_default;

        rig.mass = 1;
        rig.drag = 0;
        rig.angularDrag = 0.05f;
        rig.useGravity = true;
        rig.isKinematic = true;

        selfCollider.center = new Vector3(0, 0.8f, 0);
        selfCollider.height = 1.85f;
        selfCollider.direction = 1;

        for (int i = 0; i < flags.Length; i++)
        {
            flags[i] = -1;
        }
    }

    private void OnEnable()
    {    
        inputAction_track1.started += InputCallbackTrack1;
        inputAction_track1.Enable();

        inputAction_track2.started += InputCallbackTrack2;
        inputAction_track2.Enable();

        inputAction_track3.started += InputCallbackTrack3;
        inputAction_track3.Enable();

        inputAction_track4.started += InputCallbackTrack4;
        inputAction_track4.Enable();

        inputAction_track5.started += InputCallbackTrack5;
        inputAction_track5.Enable();

        inputAction_track6.started += InputCallbackTrack6;
        inputAction_track6.Enable();
    }
    

    private void InputCallbackTrack1(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            if (!isComboSignal)
                HandleLeftInput(Direction.TRACK_1);
            else
                isPressed_Track1 = true;
        }
    }

    private void InputCallbackTrack2(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            if (!isComboSignal)
                HandleRightInput(Direction.TRACK_2);
            else
                isPressed_Track2 = true;
        }
    }

    private void InputCallbackTrack3(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            if (!isComboSignal)
                HandleLeftInput(Direction.TRACK_3);
            else
                isPressed_Track3 = true;
        }
    }

    private void InputCallbackTrack4(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            if (!isComboSignal)
                HandleRightInput(Direction.TRACK_4);
            else
                isPressed_Track4 = true;
        }
    }

    private void InputCallbackTrack5(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            if (!isComboSignal)
                HandleLeftInput(Direction.TRACK_5);
            else
                isPressed_Track5 = true;
        }
    }

    private void InputCallbackTrack6(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Started)
        {
            if (!isComboSignal)
                HandleRightInput(Direction.TRACK_6);
            else
                isPressed_Track6 = true;
        }
    }

  

    void Start()
    {      
        if (GameObject.Find("AutoPlayer") != null)
            isAutoPlayer = true;
        else
            isAutoPlayer = false;
    }

 

    void Update()
    {

        //Debug.Log($"Z =>> Player Position Z : {this.transform.position.z}");
        SetTestUI();
        // 콤보에 대한 컨트롤
        AddComboTimes();
        if(TestUIManager.Instance != null)
        {
            TestUIManager.Instance.SetComboTimes(comboTimes);
            TestUIManager.Instance.SetCurComboPattern(curComboPattern);
        }
        
        //if(comboMonSet != null)
        //Debug.Log($"<color=blue> comboMonSet : ${comboMonSet.name}</color>");
        if (!isDead && !isFeverMode)
        {
            ControlPlayer();
        }
        else if (isFeverMode)
        {
            ControlPlayerOnFever();
        }
    }

    private void OnDisable()
    {
        inputAction_track1.started -= InputCallbackTrack1;
        inputAction_track1.Disable();

        inputAction_track2.started -= InputCallbackTrack2;
        inputAction_track2.Disable();

        inputAction_track3.started -= InputCallbackTrack3;
        inputAction_track3.Disable();

        inputAction_track4.started -= InputCallbackTrack4;
        inputAction_track4.Disable();

        inputAction_track5.started -= InputCallbackTrack5;
        inputAction_track5.Disable();

        inputAction_track6.started -= InputCallbackTrack6;
        inputAction_track6.Disable();

    }

    private void SetTestUI()
    {
        if(comboMonSet != null) {
            if(TestUIManager.Instance != null)
                TestUIManager.Instance.SetCurComboMonSet(comboMonSet.name);
        }
            
    }


    // ============================ Player Control ===================================
    // TODO : temp 일반 플레이 모드
    private void ControlPlayer()
    {
        CheckNoteMissing();

        /* Handle Combo Progress */

        SetComboSignal();
        if (isComboSignal)
        {
            CheckWaitingComboTerm();
            CheckBreakCombo(); // 콤보가 Break 되었는지 체크
        }
  
        GetPlayerInput();
    }

    public void CheckNoteMissing()
    {
        for (int i = 0; i < noteAreas.Length; i++)
        {
            if (noteAreas[i].IsMissed())
            {
                if (!isFeverMode) // 피버 모드에서 노트 놓쳐도 miss 처리 안함
                {
                    hpManager.SubPlayerHP();
                    scoreSystem.SubScore(); // 점수차감
                }

                /* 콤보 노트를 놓친건지 판단 */
                int missedNotePattern = noteAreas[i].CurMissedNote().GetMonsterPattern();
                if (missedNotePattern >= NOTE_COMBO_01 || missedNotePattern == NO_NOTE_COMBO)
                {
                    isComboNoteMissed = true;
                }

                if (spawnManager.GetNextPatternNumber() >= NOTE_COMBO_01 || spawnManager.GetNextPatternNumber() == NO_NOTE_COMBO)
                {
                    if (i == 1)
                    {
                        spawnManager.DequeuePatternQueue();
                    }
                }
                else
                {
                    spawnManager.DequeuePatternQueue();
                }

                if (!isComboNoteMissed && target_golemMob == null && target_crownSkelMob == null)
                {
                    FindNearestMob(i, true);
                }

                // hp 2 이상인 경우(왕관해골몹과 골렘)은 놓쳤을 때도 친 걸로 되어야 함
                if (target_golemMob != null)
                {
                    inputCount_golem++;
                    if (inputCount_golem >= 4)
                        InitGolemSetting(true);
                }
                else if (target_crownSkelMob != null)
                {
                    inputCount_crownSkel++;
                    if (inputCount_crownSkel >= 2)
                        InitCrownSkelSetting(true);
                }
            }
            isComboNoteMissed = false;
        }

    }


    private void GetPlayerInput()
    {

        //else if (inputAction_track2.IsPressed() && isStarted_Track1)
        //{
        //    isStarted_Track1 = false;
        //    isStarted_Track2 = false;
        //    Debug.Log("2 >> Track 2 & Track 1");
        //}

        if (isComboSignal)
        {
            // comboSignal이더라도 이번에 콤보가 break되면 바뀌면 안됨
            // 방어책
            // 1. breakCombo가 이미 된 상태에서는 입력을 무효로 한다ㅇ
            // - 다음 콤보몬스터셋을 할당받을 수 없다
            // - comboInput 수에 변화를 줄 수 없다

            // Track 1 & 2
            if (isPressed_Track1 && isPressed_Track2)
            {
                Debug.Log("Track 1 & 2");
                InitPressedValue();
                HandleComboInput(Direction.TRACK_1, Direction.TRACK_2);
            }

            if (isPressed_Track1 && isPressed_Track4)
            {
                Debug.Log("Track 1 & 4");
                InitPressedValue();
                HandleComboInput(Direction.TRACK_1, Direction.TRACK_4);
            }

            // Track 3 & 4
            // if (isPressed_Track3 && isPressed_Track4)
            // {
            //     isPressed_Track3 = false;
            //     isPressed_Track4 = false;
            //     HandleComboInput();
            // }

            // // Track 5 & 6
            // if (isPressed_Track5 && isPressed_Track6)
            // {
            //     isPressed_Track5 = false;
            //     isPressed_Track6 = false;
            //     HandleComboInput();
            // }

        }
        else
        {
            GetNoteTapSignal(); 
            // 원래 인풋처리하던 곳
        }


        // 노트가 일정 거리 안에 들어왔을 때, 입력 가능한 상황을 UI로 표시해줌
        if (IsNearNotes())
        {
            spriteRenderer_timingZone.sprite = sprite_timingZone_bling;
        }
        else
        {
            spriteRenderer_timingZone.sprite = sprite_timingZone_default;
        }
    }


    private void InitPressedValue() {
        // 다른 키가 눌렸을 경우를 대비해서 전부 false로 초기화해줌
        isPressed_Track1 = false;
        isPressed_Track2 = false;
        isPressed_Track3 = false;
        isPressed_Track4 = false;
        isPressed_Track5 = false;
        isPressed_Track6 = false;
    }

    public void GetNoteTapSignal()
    {
        // Rhythm Game Starter의 NoteArea에서 탭 시그널을 받아옴
        for (int i = 0; i < noteAreas.Length; i++)
        {
            if (noteAreas[i].GetNoteTapNumberSignal()) // KillNote 되었을 때 (플레이어가 정상 입력했을 때)
            {
                Debug.Log($"{nameof(GetNoteTapSignal)}.i : {i}");
                if (i == Direction.TRACK_1 || i == Direction.TRACK_3 || i == Direction.TRACK_5) // 왼쪽 트랙
                {
                    LeftAttack(i);
                }
                else if (i == Direction.TRACK_2 || i == Direction.TRACK_4 || i == Direction.TRACK_6) // 오른쪽 트랙
                {
                    RightAttack(i);
                }

                return;
            }
        }
    }


    private bool IsNearNotes()
    {
        Note[] notes = Transform.FindObjectsOfType<Note>();
        if(notes.Length > 0)
        {
            foreach (Note no in notes)
            {
                if(no != null)
                {
                    if (Mathf.Abs(no.transform.position.x) - Mathf.Abs(this.transform.position.x) < val_canDashAndAttack)
                    {
                        return true;
                    }
                }

            }
        }


        return false;
    }

    public void HandleLeftInput(int track)
    {
        if (isAutoPlayer)
        {
            IsPerfectNoteTap(track);
            canDashAndAttack = true;
           
            ControlFeverMode();
        }
        else
        {
            if (IsNearNotes()) // 만약 가까운 곳에 노트가 있고
            {
                if (IsPerfectNoteTap(track)) // 타이밍 맞게 눌렀다면
                {
                    canDashAndAttack = true;
                    Debug.Log("canDashAndAttack=true");
                }
                else
                {
                    hpManager.SubPlayerHP(); // 타이밍이 안 맞았으면 HP 차감
                    // 가까운 곳에 노트가 있어 대쉬 불가
                    canDashAndAttack = false;
                }
            }
            else // 가까운 곳에 노트가 없다면 그냥 대쉬 가능
            {
                if (target_golemMob == null && target_crownSkelMob == null)
                {
                    FrontDash(Direction.TRACK_1);
                }
            }
        }
    }

    public void HandleRightInput(int track)
    {
        if (isAutoPlayer)
        {
            IsPerfectNoteTap(track);
            canDashAndAttack = true;
            ControlFeverMode();
        }
        else
        {
            if (IsNearNotes()) // 만약 가까운 곳에 노트가 있고
            {
                if (IsPerfectNoteTap(track)) // 타이밍 맞게 눌렀다면
                {
                    canDashAndAttack = true;
                }
                else
                {
                    canDashAndAttack = false; // 잘못쳤을 때 
                    hpManager.SubPlayerHP();
                }
            }
            else // 가까운 곳에 노트가 없다면 
            {
                if (target_golemMob == null && target_crownSkelMob == null)
                {
                    FrontDash(Direction.TRACK_2);
                }
            }
        }

    }

    public void HandleComboInput(int trackA, int trackB)
    {
        if (isAutoPlayer)
        {
            IsPerfectNoteTap(Direction.TRACK_1);
            IsPerfectNoteTap(Direction.TRACK_2);
            ControlFeverMode();
            comboInputNum++;
            HandleComboAttack();
        }
        else
        {
            if (IsNearNotes())
            {
                if (IsPerfectNoteTap(trackA) || IsPerfectNoteTap(trackB))
                {
                    ControlFeverMode();
                    comboInputNum++;
                    HandleComboAttack();
                }
            }
        }
    }

    private bool IsPerfectNoteTap(int trackNum)
    {
        switch (trackNum)
        {
            case Direction.TRACK_1:
                if (noteAreas[Direction.TRACK_1].GetNoteTapSignal())
                {

                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // 콤보 노트가 아닌 경우에만 
                    {
                        spawnManager.DequeuePatternQueue();
                    }
                    if (!isDead && !isFeverMode)
                        ControlFeverMode();

                    return true;
                }
                break;
            case Direction.TRACK_2:
                if (noteAreas[Direction.TRACK_2].GetNoteTapSignal())
                {

                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // 콤보 노트가 아닌 경우에만 
                    {
                        spawnManager.DequeuePatternQueue();
                    }
                    if (!isDead && !isFeverMode)
                        ControlFeverMode();

                    return true;
                }
                break;
            case Direction.TRACK_3:
 
                if (noteAreas[Direction.TRACK_3].GetNoteTapSignal())
                {
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // 콤보 노트가 아닌 경우에만 
                    {
                        spawnManager.DequeuePatternQueue();
                    }
                    if (!isDead && !isFeverMode)
                        ControlFeverMode();

                    return true;
                }
                break;
            case Direction.TRACK_4:
    
                if (noteAreas[Direction.TRACK_4].GetNoteTapSignal())
                {
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // 콤보 노트가 아닌 경우에만 
                    {
                        spawnManager.DequeuePatternQueue();
                    }
                    if (!isDead && !isFeverMode)
                        ControlFeverMode();

                    return true;
                }
                break;
            case Direction.TRACK_5:

                if (noteAreas[Direction.TRACK_5].GetNoteTapSignal())
                {
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // 콤보 노트가 아닌 경우에만 
                    {
                        spawnManager.DequeuePatternQueue();
                    }
                    if (!isDead && !isFeverMode)
                        ControlFeverMode();

                    return true;
                }
                break;
            case Direction.TRACK_6:

                if (noteAreas[Direction.TRACK_6].GetNoteTapSignal())
                {
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // 콤보 노트가 아닌 경우에만 
                    {
                        spawnManager.DequeuePatternQueue();
                    }
                    if (!isDead && !isFeverMode)
                        ControlFeverMode();

                    return true;
                }
                break;
        }
        return false;
    }


    private void LeftAttack(int dir)
    {
        Debug.Log("LeftAttack => canDashAndAttack? :" + canDashAndAttack);
        if (!isComboSignal && canDashAndAttack)
        {
            if (target_crownSkelMob == null && target_golemMob == null)
            {
                FindNearestMob(dir, false); // Track 인덱스(dir)를 넣어주어야 함 
                canDashAndAttack = false;
            }
            else
            {
                if (target_crownSkelMob != null)
                    inputCount_crownSkel++;
                if (target_golemMob != null)
                {
                    inputCount_golem++;
                    if (inputCount_golem == 3)
                    {
                        // 오른쪽 백 대쉬를 위한 셋팅 
                        mon_right = target_golemMob;
                        mon_left = null;
                    }
                }
            }

            transform.rotation = Quaternion.Euler(0, 90, 0);

            // 아래 적어주는 방향은 트랙 상관없이 무조건 LEFT
            if (mon_left != null)
            {
                DashAndAttack(Direction.TRACK_1, mon_left);
            }
            else
            {
                if (mon_right != null && mon_right.layer == Layer.GOLEM)
                {
                    BackDash(Direction.TRACK_1);
                }
                else
                {
                    FrontDash(Direction.TRACK_1);
                }
            }
        }

        if (target_crownSkelMob != null)
        {
            if (inputCount_crownSkel >= 2)
            {
                InitCrownSkelSetting(false);
            }
        }

        if (target_golemMob != null)
        {
            if (inputCount_golem >= 4)
            {
                InitGolemSetting(false);
            }
        }
    }

    private void RightAttack(int dir)
    {
        if (!isComboSignal && canDashAndAttack)
        {
            if (target_crownSkelMob == null && target_golemMob == null)
            {
                FindNearestMob(dir, false);
                canDashAndAttack = false;
            }
            else
            {
                if (target_crownSkelMob != null)
                    inputCount_crownSkel++;
                if (target_golemMob != null)
                {
                    inputCount_golem++;
                    if (inputCount_golem == 3)
                    {
                        // 오른쪽 백 대쉬를 위한 셋팅 
                        mon_left = target_golemMob;
                        mon_right = null;
                    }
                }
            }

            transform.rotation = Quaternion.Euler(0, -90, 0);

            if (mon_right != null)
            {
                DashAndAttack(Direction.TRACK_2, mon_right);
            }
            else
            {
                if (mon_left != null && mon_left.layer == Layer.GOLEM)
                {
                    BackDash(Direction.TRACK_2);
                }
                else
                {
                    FrontDash(Direction.TRACK_2);
                }
            }
        }
        if (inputCount_crownSkel >= 2)
        {
            InitCrownSkelSetting(false);
        }
        if (inputCount_golem >= 4)
        {
            InitGolemSetting(false);
        }
    }

    public void FindNearestMob(int dir, bool isMissed)
    {
        canDashAndAttack = false;

        // spawn manager에서 순서대로 넣은 몹큐에서 몹을 가져온다
        // - target_golem_mob과 target_crownskelMob이 null 일 때만 이 함수가 호출되어야 한다 
        // - 골렘과 왕관해골몹 패턴이면 그만큼 빈 노트도 들어있을 거라서 패턴 당 한 번만 호출되어야 한다 
        if (isMissed)
        {
            GameObject missedMob = noteAreas[dir].GetTargetMob();
            if (missedMob != null)
            {
                int peek_layer = missedMob.layer;
                // 계속 놓치면 input 도 0이라서 조건에 들어갈 수 있게 됨 ?
                // 대신 지금 꺼가 콤보가 아니어야 함
                switch (peek_layer)
                {
                    case Layer.SKELETON:
                        break;
                    case Layer.GOLEM:
                        if (inputCount_golem == 0)
                        {
                            //Debug.Log("FindNearestMob(missed) : Golem 할당");
                            target_golemMob = missedMob;
                            targetMob = target_golemMob;
                        }
                        break;
                    case Layer.CROWN_SKELETON:
                        if (inputCount_crownSkel == 0)
                        {
                            target_crownSkelMob = missedMob;
                            //Debug.Log("FindNearestMob(missed) : Crown Skel 할당");
                            targetMob = target_crownSkelMob;
                        }
                        break;
                }
                missedMob = null;
            }
        }
        else
        {
            noteChildMob = noteAreas[dir].GetTargetMob();
            if (noteChildMob != null)
            {
                targetMob = noteChildMob;

                if (targetMob.layer == Layer.CROWN_SKELETON)
                {
                    //Debug.Log("FindNearestMob( NOT ) : Crown Skel 할당");
                    target_crownSkelMob = targetMob;
                    inputCount_crownSkel++;
                }
                else if (targetMob.layer == Layer.GOLEM)
                {
                    //Debug.Log("FindNearestMob( NOT ) : Golem 할당");
                    target_golemMob = targetMob;
                    inputCount_golem++;
                }
                else
                    //Debug.Log("FindNearestMob( NOT ) : Skel 할당");

                    noteChildMob = null;
            }
        }

        if (targetMob != null)
        {
            if (dir == Direction.TRACK_1 || dir == Direction.TRACK_3 || dir == Direction.TRACK_5)
                mon_left = targetMob;
            else if (dir == Direction.TRACK_2 || dir == Direction.TRACK_4 || dir == Direction.TRACK_6)
                mon_right = targetMob;
        }
        else
        {

            if (dir == Direction.TRACK_1 || dir == Direction.TRACK_3 || dir == Direction.TRACK_5)
                mon_left = null;
            else if (dir == Direction.TRACK_2 || dir == Direction.TRACK_4 || dir == Direction.TRACK_6)
                mon_right = null;
        }
    }

    // dash 후 attack하는 함수
    private void DashAndAttack(int dir, GameObject target)
    {
        // 혹시 재생 중인 Attack Animation이 있다면 모두 Off
        OffAttackAnimForDash(0);
        OffAttackAnimForDash(1);
        OffAttackAnimForDash(2);
        playerAnim.ResetTrigger("IsBackdash");
        PlayDashEffect(dir);

        Vector3 targetPos = target.transform.position;

        // Track이랑 상관없이 왼쪽 아니면 오른쪽임 
        if (dir == Direction.TRACK_1)
        {
            transform.position = new Vector3(targetPos.x - playerShiftValue, targetPos.y, targetPos.z);
        }
        else if (dir == Direction.TRACK_2)
        {
            transform.position = new Vector3(targetPos.x + playerShiftValue, targetPos.y, targetPos.z);
        }

        Attack(dir, target);
        effManager.ShowMobHitEffect(targetPos);
    }

    public void Attack(int dir, GameObject mob)
    {

        //Debug.Log("Attack :: ========================");
        if (mob.layer == Layer.GOLEM)
        {
            mob.GetComponent<Mob_Golem>().SetHPVal(-1);
        }
        else if (mob.layer == Layer.SKELETON)
        {
            mob.GetComponent<Mob_Skeleton>().SetHPVal(-1);
        }
        else if (mob.layer == Layer.CROWN_SKELETON)
        {
            mob.GetComponent<Mob_Skeleton>().SetHPVal(-1);
        }

        // 세 가지 어택을 번갈아가면서 실행 
        switch (attackAnimSwitchCount)
        {
            case 0:
                playerAnim.SetBool("AttC", false);
                playerAnim.SetBool("AttA", true);
                PlayAttackEffect(1, dir);
                attackAnimSwitchCount += 1;
                break;
            case 1:
                playerAnim.SetBool("AttA", false);
                playerAnim.SetBool("AttB", true);
                PlayAttackEffect(2, dir);
                attackAnimSwitchCount += 1;
                break;
            case 2:
                playerAnim.SetBool("AttB", false);
                playerAnim.SetBool("AttC", true);
                PlayAttackEffect(3, dir);
                attackAnimSwitchCount = 0;
                break;
        }


        // 어택 후 null을 해주어야 함
        // - 죽은 몬스터가 할당되어있을 수도 있으므로
        //Debug.Log("몬스터 타겟 초기화");


        if (target_golemMob == null && target_crownSkelMob == null)
        {
            mon_left = null;
            mon_right = null;
        }
    }


    private void FrontDash(int dir)
    {

        if (dir == Direction.TRACK_1 || dir == Direction.TRACK_3 || dir == Direction.TRACK_5)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0); // 왼쪽을 보게 함
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -90, 0); // 오른쪽을 보게 함

        }
        if (comboInputNum == 0)
        {
            //Debug.Log("----------------- Front Dash ------------------");
            PlayDashEffect(dir);

            // 혹시 재생 중인 Attack Animation이 있다면 모두 Off
            OffAttackAnimForDash(0);
            OffAttackAnimForDash(1);
            OffAttackAnimForDash(2);

            playerAnim.SetTrigger("IsFrontdash");

            //버튼을 누르면 바라보는 방향으로 조금 이동한다
            transform.Translate(Vector3.forward * dashValue_front);
        }
    }

    private void BackDash(int dir)
    {
        if (comboInputNum == 0)
        {
            //Debug.Log("--------------------Back dash---------------");

            PlayDashEffect(dir);

            // 혹시 재생 중인 Attack Animation이 있다면 모두 Off
            OffAttackAnimForDash(0);
            OffAttackAnimForDash(1);
            OffAttackAnimForDash(2);

            playerAnim.SetTrigger("IsBackdash");

            if (dir == 0) // 왼쪽이면
            {
                transform.rotation = Quaternion.Euler(0, -90, 0); // 오른쪽을 보게 함
            }
            else // 오른쪽이면
            {
                transform.rotation = Quaternion.Euler(0, 90, 0); // 왼쪽을 보게 함
            }

            //버튼을 누르면 바라보는 방향과 반대방향으로 조금 이동한다
            transform.Translate(-Vector3.forward * dashValue_back);
        }
    }


    public void PlayerDead()
    {
        transform.position = new Vector3(this.transform.position.x, 0.3f, this.transform.position.z);
        isDead = true;
        playerAnim.SetTrigger("trig_Death");
    }

    // ============================ Combo ===================================

    // 콤보 노트 입력 가능 상태일 때 isComboSignal을 true로 만듦
    public void SetComboSignal()
    {
        // 콤보 노트가 입력 가능 범위 안에 들어오면
        if (detectComboNoteArea2.IsSetComboSignal() || detectComboNoteArea4.IsSetComboSignal())
        {
            // 이전 콤보 break가 이번 콤보에 영향을 미치지 않게 하기 위해 
            if(comboInputNum == 0)
            {
                detectComboNoteArea2.InitBreakCombo();
                // detectComboNoteArea4.InitBreakCombo();
            }
            isComboSignal = true;

            if (!isFeverMode)
                img_comboSignal.enabled = true;
        }
        else
        {
            isComboSignal = false;
            if (!isFeverMode)
                img_comboSignal.enabled = false;
        }

    }

    private GameObject breakComboMonSet = null;
    private void CheckBreakCombo()
    {
        // 아래 조건들 자체가 콤보일 때만 가능하므로 isComboSignal을 없애주었음 
        // 1. 공중에 대기 중일 때 breakCombo가 되면 플레이어를 원상복귀 시켜주기 위한 코드
        if (isComboWaiting || playerAnim.speed == 0) // 이게 isComboWaiting이랑 isComboSignal이랑 타이밍이 잘 안맞음 
        {
            if (detectComboNoteArea2.IsBreakCombo() || detectComboNoteArea4.IsBreakCombo())
            {
                Debug.Log("### 공중에 멈춰있다가 breakcombo됨");
                playerAnim.speed = 1;
                if (!isFeverMode)
                    playerAnim.SetTrigger("ComboEnd");

                isBreakCombo = true;
                breakComboMonSet = comboMonSet;
                ComboReset();
            }
        }
        // 2. 콤보 노트를 아예 놓치거나 했을 때 플레이어를 원상복구 시켜주기 위함 
        else if (!detectComboNoteArea2.IsSetComboSignal())
        {
            // IsSetComboSignal 에서 comboSignal은 NoteArea에 일반 노트가 들어오면 false
            // NoteArea에 콤보 노트의 마지막 노트가 TriggerExit하면 false가 됨 
            Debug.Log("### 일반 노트가 들어왔거나 마지막 콤보 노트가 TriggerExit함");
            if (!isFeverMode)
            {
                playerAnim.speed = 1;
                playerAnim.SetTrigger("ComboEnd");
            }

            isBreakCombo = true;
            breakComboMonSet = comboMonSet;
            ComboReset();
        }

        //if (isComboSignal)
        //{
        //    if (isComboWaiting && playerAnim.speed == 0) // 이게 isComboWaiting이랑 isComboSignal이랑 타이밍이 잘 안맞음 
        //    {
        //        if (detectComboNoteArea2.IsBreakCombo())
        //        {
        //            Debug.Log("### 공중에 멈춰있다가 breakcombo됨");
        //            playerAnim.speed = 1;
        //            if (!isFeverMode)
        //                playerAnim.SetTrigger("ComboEnd");

        //            isBreakCombo = true;
        //            ComboReset();
        //        }
        //    }
        //    // 2. 콤보 노트를 아예 놓치거나 했을 때 플레이어를 원상복구 시켜주기 위함 
        //    else if (!detectComboNoteArea2.IsSetComboSignal())
        //    {
        //        // IsSetComboSignal 에서 comboSignal은 NoteArea에 일반 노트가 들어오면 false
        //        // NoteArea에 콤보 노트의 마지막 노트가 TriggerExit하면 false가 됨 
        //        Debug.Log("### 일반 노트가 들어왔거나 마지막 콤보 노트가 TriggerExit함");
        //        if (!isFeverMode)
        //        {
        //            playerAnim.speed = 1;
        //            playerAnim.SetTrigger("ComboEnd");
        //        }

        //        isBreakCombo = true;
        //        ComboReset();
        //    }
        //}
        
    }

    private void CheckTriggerFinalComboNote()
    {
        // CheckTriggerFinalNote()
        // 마지막 노트를 누를 차례일 때
        // - 제대로 눌렀으면 각종 변수들을 초기화해줌
        // - 제대로 누르지 않았다면 콤보를 초기화해줌 
        if (detectComboNoteArea2.IsEnterFinalComboNote() || detectComboNoteArea4.IsEnterFinalComboNote()) // 연타의 마지막 노트가 트리거에 들어왔으면 
        {

            if (curComboPattern - comboInputNum == (NOTE_COMBO_01 - 1)) // 마지막 노트 칠 차례라면 
            {
                Debug.Log("CheckTriggerFinalNote(): 마지막 노트 칠 차례");
                
                //SendFallSignal();
            }
            else if ((curComboPattern - NOTE_COMBO_01) > comboInputNum)  // 마지막 노트를 칠 차례가 아닐 때 쳤는데, 원래 쳐야하는 횟수랑 안맞으면
            {
                Debug.Log("CheckTriggerFinalNote(): 마지막 노트 칠 차례아님");
                // 마지막 노트 쳤는데 안맞음 ///// 강제 리셋
                ComboReset();
            }
        }
    }

    private bool isSetComboMons = false;
    private GameObject combo01MonSet = null;
    private void HandleComboAttack()
    {
        switch (comboInputNum)
        {
            case 1:

                Debug.Log($"1번 탭({comboTimes}");

                cameraFollower.SetYPositionRelative(3f);

                playerAnim.SetBool("ComboStart", true);
                if(!isSetComboMons)
                {
                    //ChangeComboMonSet();
                    isSetComboMons = true;
                }
                //comboMonSet = spawnManager.GetComboMonSet();
                // 플레이어 위치 설정
                if (comboMonSet.transform.childCount != 0)
                {
                    tempComboMon = comboMonSet.transform.Find("Monster").gameObject;
                    if(tempComboMon != null)
                    {
                        // Player rotation 설정
                        if (transform.position.x - tempComboMon.transform.position.x > 0)
                        {
                            transform.rotation = Quaternion.Euler(0, -90, 0);
                        }
                        else
                            transform.rotation = Quaternion.Euler(0, 90, 0);
                        // Player position 설정
                        transform.position = tempComboMon.transform.position; // 플레이어 위치를 콤보 몬스터들 위치로 변경
                    }

                    //transform.LookAt(tempComboMon.transform);                    
                    comboMon_xPos = transform.position.x;
                }


                SendFlySignal(comboMonSet, curComboPattern);
                CheckTriggerFinalComboNote();

                if (curComboPattern <= NOTE_COMBO_01) // 1연타이면 여기에서 끝 
                {
                    /* 1연타 콤보에서의 Fall Signal은 애니메이션에서 보냄 => SendFallSignalFromAnim() */
                    /* ComboReset()도 마찬가지 */
                    isCombo01 = true;
                    combo01MonSet = comboMonSet;
                    ComboReset();
                }
                else
                    SendAttackSignal(comboMonSet, curComboPattern);
                PlayComboEffect(1);
                this.transform.position = new Vector3(comboMon_xPos, this.transform.position.y, this.transform.position.z);
                break;
            case 2:
                Debug.Log("2번 탭");
                cameraQuake.StartQuake();

                if (isComboWaiting) // 대기하는 중에 tap이 한 번 더 눌린거라면 animspeed를 원상복귀 
                {
                    playerAnim.speed = 1;
                }

                this.transform.position = new Vector3(comboMon_xPos, this.transform.position.y, this.transform.position.z);

                SendAttackSignal(comboMonSet, curComboPattern);

                CheckTriggerFinalComboNote();
                if (curComboPattern <= NOTE_COMBO_02)
                {
                    SendFallSignal(comboMonSet);
                    playerAnim.SetTrigger("GoCombo05");
                    PlayComboEffect(5);
                    ComboReset();
                }
                else
                {
                    if (comboMonSet.transform.childCount != 0)
                    {
                        transform.position = comboMonSet.transform.Find("Monster").transform.position;
                        transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
                    }

                    playerAnim.SetTrigger("GoCombo02");
                    PlayComboEffect(2);
                }
                this.transform.position = new Vector3(comboMon_xPos, this.transform.position.y, this.transform.position.z);
                break;
            case 3:
                Debug.Log("3번 탭");
                cameraQuake.StartQuake();

                if (isComboWaiting) // 대기하는 중에 tap이 한 번 더 눌린거라면 animspeed를 원상복귀 
                {
                    playerAnim.speed = 1;
                }

                PlayComboEffect(3);
                SendAttackSignal(comboMonSet, curComboPattern);

                CheckTriggerFinalComboNote();
                if (curComboPattern <= NOTE_COMBO_03)
                {
                    SendFallSignal(comboMonSet);
                    playerAnim.SetTrigger("GoCombo05");
                    PlayComboEffect(5);
                    ComboReset();
                }
                else
                {
                    if (comboMonSet.transform.childCount != 0)
                    {
                        transform.position = comboMonSet.transform.Find("Monster").transform.position;
                        transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
                    }

                    playerAnim.SetTrigger("GoCombo03");
                    PlayComboEffect(3);
                }
                this.transform.position = new Vector3(comboMon_xPos, this.transform.position.y, this.transform.position.z);

                break;
            case 4:

                cameraQuake.StartQuake();

                Debug.Log("4번 탭");
                if (isComboWaiting) // 대기하는 중에 tap이 한 번 더 눌린거라면 animspeed를 원상복귀 
                {
                    playerAnim.speed = 1;
                }

                PlayComboEffect(4);
                SendAttackSignal(comboMonSet, curComboPattern);


                CheckTriggerFinalComboNote();
                if (curComboPattern <= NOTE_COMBO_04) // 4연타이면 여기에서 끝 
                {
                    SendFallSignal(comboMonSet);
                    playerAnim.SetTrigger("GoCombo05");
                    PlayComboEffect(5);
                    ComboReset();
                }
                else
                {
                    if (comboMonSet.transform.childCount != 0)
                    {
                        transform.position = comboMonSet.transform.Find("Monster").transform.position;
                        transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
                    }

                    playerAnim.SetTrigger("GoCombo04");
                    PlayComboEffect(4);
                }
                this.transform.position = new Vector3(comboMon_xPos, this.transform.position.y, this.transform.position.z);
                break;
            case 5:
                cameraQuake.StartQuake();

                Debug.Log("5번 탭");
                if (isComboWaiting) // 대기하는 중에 tap이 한 번 더 눌린거라면 animspeed를 원상복귀 
                {
                    playerAnim.speed = 1;
                }
                SendAttackSignal(comboMonSet, curComboPattern);
                SendFallSignal(comboMonSet);
                playerAnim.SetTrigger("GoCombo05");
                PlayComboEffect(5);
                CheckTriggerFinalComboNote();
                ComboReset();
                this.transform.position = new Vector3(comboMon_xPos, this.transform.position.y, this.transform.position.z);
                break;
        }
    }

    public void SendFlySignal(GameObject curComboMonSet, int patternNum)
    {
        if (!isFeverMode)
        {
            if (curComboMonSet != null && curComboMonSet.transform.childCount > 0)
            {
                foreach (Mob_Skeleton obj in curComboMonSet.GetComponentsInChildren<Mob_Skeleton>())
                {
                    obj.StartMobInAirLogic(patternNum);
                }
                foreach (Mob_Golem obj in curComboMonSet.GetComponentsInChildren<Mob_Golem>())
                {
                    obj.StartMobInAirLogic(patternNum);
                }
            }
        }
    }

    public void SendAttackSignal(GameObject curComboMonSet, int patternNum)
    {
        Debug.Log($"SendAttackSignal | ComboMonSet => {curComboMonSet}, ComboPattern => {patternNum} ");
        if (!isFeverMode)
        {
            if (curComboMonSet != null && curComboMonSet.transform.childCount > 0)
            {
                foreach (Mob_Skeleton obj in curComboMonSet.GetComponentsInChildren<Mob_Skeleton>())
                {
                    obj.StartMobAttackLogic();
                }
                foreach (Mob_Golem obj in curComboMonSet.GetComponentsInChildren<Mob_Golem>())
                {
                    obj.StartMobAttackLogic();
                }
            }
        }
    }

    
    private bool isFalling = false;
    public void SendFallSignal(GameObject tmpMonSet)
    {         
        if (!isFalling)
        {
            isFalling = true;
            Debug.Log($">>> <color=green>SendFallSignal: ComboMonSet = {tmpMonSet.name}, child count = {tmpMonSet.transform.childCount}</color>");
            cameraFollower.SetYPositionRelative();

            if (tmpMonSet != null && tmpMonSet.transform.childCount > 0)
            {
                foreach (Mob_Skeleton obj in tmpMonSet.GetComponentsInChildren<Mob_Skeleton>())
                {
                    obj.FallWithComboFinal();
                }
                foreach (Mob_Golem obj in tmpMonSet.GetComponentsInChildren<Mob_Golem>())
                {
                    obj.FallWithComboFinal();
                }
            }
            isFalling = false;
        }
    }

    // SpawnManager에서 콤보몬스터집합이 만들어질 때 호출됨
    public void SetComboOptionSignal(int optionNum)
    {
        // 일단 들어오는 숫자는 순서대로 배열에 전부 담고 이번 콤보가 끝날 때 적용 시켜줌 
        // comboSignalTimesFromSpawnManager : spawnManager에서 콤보몬스터를 만든 횟수 
        optionNums[totalComboTimes] = optionNum;
        flags[totalComboTimes] = optionNum;

        totalComboTimes++;
    }

    
    private bool isFirstAddComboTimes = true;
    public void AddComboTimes()
    {
        // 진행된 콤보 횟수 증가
        // - 무조건 한 패턴당 한 번만 증가해야 함
        // => 해당 패턴의 마지막 콤보 노트가 KillNote되거나 TriggerExit된 경우에만 증가시켜줌 (Track2의 NoteArea에서)
        if (isFirstAddComboTimes) // 초기값 설정
        {             
            comboTimes = detectComboNoteArea2.GetPlayerComboTimes();
            
            ChangeComboMonSet();
            isFirstAddComboTimes = false;
        }           
        
        if (comboTimes <= totalComboTimes)
        {
            // 현재의 콤보 패턴이 할당된 이후에만 comboTimes가 변경되어야 함
            // 안그러면 콤보 패턴을 하나 뛰어넘는 문제 발생
            curComboPattern = optionNums[comboTimes];
            comboTimes = detectComboNoteArea2.GetPlayerComboTimes();
            ChangeComboMonSet();
            
        }
    }

    bool isFirstSetComboMon = true;
    private void ChangeComboMonSet()
    {
        // 콤보 몬스터 셋을 바꿀 때 이전 번호의 콤보 몬스터 셋을 검색해서 자식이 있으면 다 죽여주쟈
        // 여기 위치가 딱 comboMonSet 바꾸기 전이니까 여기서 해주면 될듯
        //Debug.Log($"ChangeComboMonSet => comboTimes : ${comboTimes}, peekNumber : ${spawnManager.GetComboMonSetPeekNumber()}");
        if (spawnManager.GetComboMonSetPeekNumber() == comboTimes + 1)
        {
            
            // 두번째 => comboTimes = 1, comboMonSet2
            // 세번째 => comboTimes = 2, comboMonSet3 
            // ... (comboTimes가 콤보가 완료되야 플러스되는 구조라서)
            if (comboMonSet != null)
            {
                if (comboMonSet.transform.childCount != 0)
                {
                    //Debug.Log($"ChangeComboMonSet(): 이전 몬스터셋(${comboMonSet.name}의 자식들이 [ ${comboMonSet.transform.childCount} ] 마리 남음");
                    for(int i = comboMonSet.transform.childCount - 1; i>=0; i--)
                    {
                        GameObject mob = comboMonSet.transform.GetChild(i).gameObject;
                        if (mob.layer == Layer.GOLEM)
                        {
                            mob.GetComponent<Mob_Golem>().GoDeadState();
                        }
                        else if (mob.layer == Layer.SKELETON || mob.layer == Layer.CROWN_SKELETON)
                        {
                            mob.GetComponent<Mob_Skeleton>().GoDeadState();
                        }
                    }

                }
            }
            comboMonSet = spawnManager.GetComboMonSet();

        }

    }

    // 콤보 연타 입력 사이에 텀이 길 경우 공중에서 멈춰있게 함 
    private void CheckWaitingComboTerm()
    {
        // 만약 n연타 진행 중이라면
        // - break combo이면 combo end
        // - not break combo이면 공중에 멈춰서 다음 입력 대기
        if (curComboPattern > NOTE_COMBO_01 && comboInputNum == 1) // 1연타 입력 완료 후 2연타 입력 대기 중
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo01") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 2연타 - Break Combo. Idle로 돌아감
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);
                }
                else
                {
                    // 콤보가 끝나지 않았으면 공중에서 잠깐 대기
                    isComboWaiting = true;
                    playerAnim.speed = 0;

                }
            }
        }

        if (curComboPattern > NOTE_COMBO_02 && comboInputNum == 2) // 2연타 입력 완료 후 3연타 입력 대기 중
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo02") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 3연타 - Break Combo. Idle로 돌아감
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);
                }
                else
                {
                    // 콤보가 끝나지 않았음 공중에서 잠깐 대기
                    isComboWaiting = true;
                    playerAnim.speed = 0;
                }
            }
        }

        if (curComboPattern > NOTE_COMBO_03 && comboInputNum == 3) // 3연타 입력 완료 후 4연타 입력 대기 중
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo03") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 4연타 - Break Combo. Idle로 돌아감
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);
                }
                else
                {
                    // 콤보가 끝나지 않았음 공중에서 잠깐 대기
                    isComboWaiting = true;
                    playerAnim.speed = 0;
                }
            }
        }

        if (curComboPattern > NOTE_COMBO_04 && comboInputNum == 4) // 4연타 입력 완료 후 5연타 입력 대기 중
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo04") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 5연타 - Break Combo. Idle로 돌아감
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);

                }
                else
                {
                    // 콤보가 끝나지 않았음 공중에서 잠깐 대기
                    isComboWaiting = true;
                    playerAnim.speed = 0;
                }
            }
        }
    }

    public void ComboReset() // 모든 콤보를 끝마쳤거나, 중간에 실패했을떄
    {
        // 모든 게 정상인 경우 한 번의 comboTimes 당 한 번만 호출되어야 함
        Debug.Log("------------------------ COMBO RESET -----------------------");
        if (!isFeverMode)
        {
            // 플레이어의 높이를 원상복구 시켜줌 
            transform.position = new Vector3(this.transform.position.x, 0.3f, this.transform.position.z);            
            // 콤보 애니메이션 초기화
            if (curComboPattern != MonsterPatternGenerator.NOTE_COMBO_01) // 1연타가 아닌 경우 combo animation 초기화
            {
                playerAnim.SetBool("ComboStart", false);
            }
            // 카메라의 높이를 원상복구 시켜줌
            cameraFollower.SetYPositionRelative();
        }

        // 2. 잔여 몬스터 처리
        // 콤보가 중간에 실패되어 온 경우 ComboMonSet이 남아있음 

        if (breakComboMonSet != null && isBreakCombo)
        {
            Debug.Log($" {breakComboMonSet.name}의 잔여 몬스터 처리 | ComboMonSet.transform.childCount = " + breakComboMonSet.transform.childCount);
            if (breakComboMonSet.transform.childCount != 0)
            {
                for (int i = 0; i < breakComboMonSet.transform.childCount; i++)
                {
                    GameObject mob = breakComboMonSet.transform.GetChild(0).gameObject;
                    if (mob.layer == Layer.GOLEM)
                    {
                        mob.GetComponent<Mob_Golem>().GoDeadState();
                    }
                    else if (mob.layer == Layer.SKELETON || mob.layer == Layer.CROWN_SKELETON)
                    {
                        mob.GetComponent<Mob_Skeleton>().GoDeadState();
                    }
                }
            }
            isBreakCombo = false;
            breakComboMonSet = null;
        }

        // 3. 관련 변수 초기화
        comboInputNum = 0;
        if (isComboWaiting)
        {
            isComboWaiting = false;
        }

        // 이거 인식 잘 안되고 있음 
        // - comboreset 이후에 final note인식이 된 경우
        if (detectComboNoteArea2.IsExitFinalComboNote()) // 마지막 콤보 노트가 지나갔다면 
        {
            Debug.Log("마지막 콤보 노트가 지나감 ");
            detectComboNoteArea2.SetIsExitFinalComboNoteFalse();
        }

        // 콤보를 치지 않고 넘어갔을 때 ComboMonSetQueue에서 Dequeue는 해주어야 함
        if (!isSetComboMons) // 1번 탭에서 설정된 이력이 없다면
        {
            //ChangeComboMonSet();
        }
        else
        {
            // 1연타에서 이미 comboMonSet을 할당했으면 다음에 다시 할당할 수 있도록 bool값만 초기화해줌
            isSetComboMons = false;
        }

        isComboSignal = false;
        detectComboNoteArea2.SetComboSignalFalse();
        detectComboNoteArea4.SetComboSignalFalse();

        playerAnim.SetBool("AttA", false);
        playerAnim.SetBool("AttB", false);
        playerAnim.SetBool("AttC", false);

        if (!isFeverMode)
        {
            if (target_golemMob != null)
            {
                InitGolemSetting(true);
            }
            if (target_crownSkelMob != null)
            {
                InitCrownSkelSetting(true);
            }

            // 콤보 리셋 후 혹시 피버를 기다리는 중이었다면 시작하도록 함 
            CheckFeverModeAvailablity();
        }
    }


    // ============================ Fever ===================================
    public void SetFeverMode(bool onoff)
    {
        isFeverMode = onoff;
        if (!onoff)
        {
            InitFeverSetting();
            if (target_golemMob != null)
            {
                target_golemMob.transform.position = this.transform.position;
            }
            if (target_crownSkelMob != null)
            {
                target_crownSkelMob.transform.position = this.transform.position;
            }
        }
        else
        {
            img_comboSignal.enabled = false;
            canStartFever = false;
            // 플레이어 포지션을 기준으로 스폰포인트들 위치를 조정해줌 
            transform.position = new Vector3(this.transform.position.x, 0.3f, this.transform.position.z);
            spawnPointsParent.position = transform.position; /* 원래 조정은 Spawn Manager에서 했었음 */

        if (playerAnim.speed == 0)
            {
                playerAnim.speed = 1;
            }
        }
    }

    private void ControlPlayerOnFever()
    {
        // 피버모드 때도 노트가 그냥 동작하기 때문에 각종 처리들을 해주어야 함
        CheckNoteMissing();
        CheckBreakCombo();
        if(isComboSignal)
        {
            CheckWaitingComboTerm();
            if (comboTimes <= totalComboTimes)
            {
                curComboPattern = optionNums[comboTimes];
                Debug.Log($"curComboPattern : {curComboPattern}");
                TestUIManager.Instance.SetCurComboPattern(curComboPattern);
            }
        }
        SetComboSignal();

        if (isExecutingFeverCombo)
        {
            HandleFeverComboWaiting();
        }

        if (CanStartFeverCombo())
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); // 처음 시작 시 앞을 보게 함 
            //StartCoroutine(StartFeverCombo());
        }
        else
        {
            if (cameraFollower.IsFeverTimeCameraEnd())
            {

                // 피버모드에서의 공격
                // 1. 아무 키나 누르면 공격이 가능하다
                // 2. 공격 방향은 몬스터가 있는 방향으로 
                // - 세 번 이상 같은 방향으로 못 가게 한다 
                // 3. 평소와 다르게 콜라이더 충돌로 공격한다. (한 번에 여러 몬스터가 죽는 연출을 위해)
                spawnPointsParent.position = transform.position; /* 원래 조정은 Spawn Manager에서 했었음 */
                if (Input.anyKeyDown) // 아무키를 눌렀을 때 
                {
                    SetFeverWeaponColliders(true);
                    fever_mobParent = GameObject.Find("FeverMonstersParent").transform;
                    if (fever_mobParent.childCount > 0)
                    {
                        // 한쪽 방향으로 2이상 갔을 때만 방향을 새로 찾음
                        if (fever_dirLeft > 2 || fever_dirRight > 2)
                        {
                            if (fever_mobParent.GetChild(0).transform.position.x < transform.position.x)
                            {
                                if (fever_dirRight <= 4)
                                {

                                    fever_curDir = Direction.TRACK_2;
                                }
                                else
                                {
                                    fever_curDir = Direction.TRACK_1;
                                }
                            }
                            else
                            {
                                if (fever_dirLeft <= 4)
                                {
                                    fever_curDir = Direction.TRACK_1;
                                }
                                else
                                {
                                    fever_curDir = Direction.TRACK_2;
                                }
                            }
                        }
                    }
                    else
                    {
                        fever_curDir = UnityEngine.Random.Range(0, 2);
                    }

                    if (isExecutingFeverCombo)
                    {
                        SetFeverWeaponColliders(false);
                        fever_comboTapNum++;
                        FeverCombo();
                    }
                    else
                    {
                        if (fever_curDir == Direction.TRACK_1)
                        {
                            fever_dirLeft++;

                            fever_dirRight = 0;
                            transform.rotation = Quaternion.Euler(0, 90, 0);
                            PlayDashEffect(Direction.TRACK_1);
                            FeverDashAndAttack(Direction.TRACK_1);
                        }
                        else
                        {
                            fever_dirRight++;
                            fever_dirLeft = 0;
                            transform.rotation = Quaternion.Euler(0, -90, 0);
                            PlayDashEffect(Direction.TRACK_2);
                            FeverDashAndAttack(Direction.TRACK_2);
                        }

                    }
                }
            }

        }

    }

    private void FeverDashAndAttack(int dir)
    {
        // 혹시 재생 중인 Attack Animation이 있다면 모두 Off
        OffAttackAnimForDash(0);
        OffAttackAnimForDash(1);
        OffAttackAnimForDash(2);
        // 버튼을 누르면 바라보는 방향으로 조금 이동한다 
        transform.Translate(Vector3.forward * dashValue_front);
        FeverAttack(dir); // 그리고 바로 공격한다
    }

    public void FeverAttack(int dir)
    {

        scoreSystem.AddScore(500);
        // 세 가지 어택을 번갈아가면서 실행 
        switch (attackAnimSwitchCount)
        {
            case 0:
                playerAnim.SetBool("AttC", false);
                playerAnim.SetBool("AttA", true);
                PlayAttackEffect(1, dir);
                attackAnimSwitchCount += 1;
                break;
            case 1:
                playerAnim.SetBool("AttA", false);
                playerAnim.SetBool("AttB", true);
                PlayAttackEffect(2, dir);
                attackAnimSwitchCount += 1;
                break;
            case 2:
                playerAnim.SetBool("AttB", false);
                playerAnim.SetBool("AttC", true);
                PlayAttackEffect(3, dir);
                attackAnimSwitchCount = 0;
                break;
        }
    }

    public void SetFeverComboOn()
    {
        isExecutingFeverCombo = true;
    }

    public void FeverCombo()
    {
        Transform feverMobs = GameObject.Find("FeverMonstersParent").transform;


        switch (fever_comboTapNum)
        {
            case FeverTapCount.COMBO_1:
                cameraFollower.SetYPositionRelative(2f);
                playerAnim.SetBool("ComboStart", true);

                transform.position = new Vector3(this.transform.position.x, fever_playerYPosOnCombo, this.transform.position.z);
                SendFlySignalOnFever(feverMobs);
                PlayComboEffect(1);
                break;
            case FeverTapCount.COMBO_2:
                if (isFeverComboWaiting)
                {
                    playerAnim.speed = 1;
                }
                cameraQuake.StartQuake();

                SendFlySignalOnFever(feverMobs);
                playerAnim.SetTrigger("GoCombo02");
                PlayComboEffect(2);
                break;
            case FeverTapCount.COMBO_3:
                if (isFeverComboWaiting)
                {
                    playerAnim.speed = 1;
                }
                cameraQuake.StartQuake();

                playerAnim.SetTrigger("GoCombo03");
                SendFlySignalOnFever(feverMobs);
                SendAttackSignalOnFever(feverMobs);
                PlayComboEffect(3);
                break;
            case FeverTapCount.COMBO_4:
                if (isFeverComboWaiting)
                {
                    playerAnim.speed = 1;
                }
                cameraQuake.StartQuake();
                playerAnim.SetTrigger("GoCombo04");
                SendFlySignalOnFever(feverMobs);
                SendAttackSignalOnFever(feverMobs);
                PlayComboEffect(4);
                break;
            case FeverTapCount.COMBO_5:
                if (isFeverComboWaiting)
                {
                    playerAnim.speed = 1;
                }
                cameraQuake.StartQuake();
                playerAnim.SetTrigger("GoCombo05");

                PlayComboEffect(5);
                SendFlySignalOnFever(feverMobs);
                SendAttackSignalOnFever(feverMobs);

                foreach (Transform obj in feverMobs.GetComponentsInChildren<Transform>())
                {
                    obj.transform.position = new Vector3(this.transform.position.x, 0.3f, obj.transform.position.z);
                }
                this.transform.position = new Vector3(this.transform.position.x, 0.3f, this.transform.position.z);
                break;
        }
    }

    private void SendFlySignalOnFever(Transform feverMobTr)
    {
        foreach (Transform obj in feverMobTr.GetComponentsInChildren<Transform>())
        {
            obj.transform.position = new Vector3(this.transform.position.x, 2.5f, obj.transform.position.z);
        }
    }

    private void SendAttackSignalOnFever(Transform feverMobTr)
    {
        foreach (Transform obj in feverMobTr.GetComponentsInChildren<Transform>())
        {
            if (obj.gameObject.layer == Layer.GOLEM)
            {
                Mob_Golem gol = obj.GetComponent<Mob_Golem>();
                if (gol != null)
                {
                    if (!gol.IsDeadState())
                    {
                        gol.StartMobAttackLogic();
                    }
                }
            }
            else
            {
                Mob_Skeleton skel = obj.GetComponent<Mob_Skeleton>();
                if (skel != null)
                {
                    if (!skel.IsDeadState())
                    {
                        skel.StartMobAttackLogic();
                    }
                }
            }
        }
    }

    private void HandleFeverComboWaiting()
    {
        // combowaiting 되는 기준은
        // - 애니메이션이 60 % 이상 재생되었는데 추가적인 입력이 없을 때

        // breakcombo되는 기준은
        // - 피버모드가 끝났을 때
        // - combowaiting 상태에서 추가적인 입력이 있을 때

        // 만약 1연타 입력 후 1연타 애니메이션이 끝나가는데 2연타 입력이 없으면 
        // 1연타 애니메이션이 끝나기 전에 애니메이션을 멈춤
        // 그리고 입력 수 TAP이 커지면 다시 애니메이션을 재생함 

        // 만약 현재 입력 탭 수가 1combo 수보다 작거나 같으면,
        // - combo1 애니메이션이 60% 이상 재생됐는데도 입력 수에 변화가 없으면 anim.speed = 0;
        // - combo1 애니메이션이 60% 이상 재생됐는데 입력 수에 변화가 생기면 anim.speed = 1;

        if (fever_comboTapNum == FeverTapCount.COMBO_1 || fever_comboTapNum < FeverTapCount.COMBO_2)
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo01") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                isFeverComboWaiting = true;
                playerAnim.speed = 0;
            }
        }
        else if (fever_comboTapNum == FeverTapCount.COMBO_2 || fever_comboTapNum < FeverTapCount.COMBO_3)
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo02") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                isFeverComboWaiting = true;
                playerAnim.speed = 0;
            }
        }
        else if (fever_comboTapNum == FeverTapCount.COMBO_3 || fever_comboTapNum < FeverTapCount.COMBO_4)
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo03") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                isFeverComboWaiting = true;
                playerAnim.speed = 0;
            }
        }
        else if (fever_comboTapNum == FeverTapCount.COMBO_4 || fever_comboTapNum < FeverTapCount.COMBO_5)
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo04") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                isFeverComboWaiting = true;
                playerAnim.speed = 0;
            }
        }

    }

    private void InitFeverSetting()
    {
        img_comboSignal.enabled = true;
        SetFeverWeaponColliders(true); /* TEST */
        cameraFollower.SetYPositionRelative(0f);
        isFeverComboWaiting = false;

        playerAnim.speed = 1;
        playerAnim.SetTrigger("ComboEnd");

        isExecutingFeverCombo = false;
        canStartFeverCombo = false;
        fever_comboTapNum = 0;
        fever_dirLeft = 0;
        fever_dirRight = 0;

        // 피버 중에 가려져 있을 때 breakCombo 된 것에 대해 초기화를 해주어야 함 
        noteAreas[0].InitBreakCombo();
        noteAreas[1].InitBreakCombo();
    }

    public bool IsFeverMode()
    {
        return isFeverMode;
    }

    private void ControlFeverMode()
    {
        if (!isFeverMode)
            feverManager.AddCurGauge(1);

        if (feverManager.CanEnterFeverMode())
        {
            int nextPatternNumber = spawnManager.GetNextPatternNumber();

            // 콤보 중이 아니거나, 골렘, 왕관해골몹 등의 패턴이 아닌 경우에만 피버 시작 
            if (!isComboSignal && comboInputNum == 0
                && inputCount_golem == 0 && inputCount_crownSkel == 0
                && spawnManager.GetPreviousPatternNumber() < NOTE_COMBO_01
                && nextPatternNumber < NOTE_COMBO_01
                && nextPatternNumber != NO_NOTE_COMBO
                && Mathf.Abs(nextPatternNumber) != NOTE_CROWN_SKELETON
                && Mathf.Abs(nextPatternNumber) != NOTE_GOLEM)
            {
                feverManager.ControlFeverStart();
            }
            else
            {
                // 바로 시작할 수 없는 상황일 땐 보류 상태를 만든다.
                canStartFever = true;
            }
        }
    }

    private void CheckFeverModeAvailablity()
    {
        if (canStartFever)
        {
            int nextPattern = spawnManager.GetNextPatternNumber();
            if (!isComboSignal && comboInputNum == 0 && inputCount_golem == 0 && inputCount_crownSkel == 0 && target_crownSkelMob == null
                && spawnManager.GetPreviousPatternNumber() < NOTE_COMBO_01
                && nextPattern < NOTE_COMBO_01 // 다음 패턴도 콤보 패턴이 아니고 
                && nextPattern != NO_NOTE_COMBO
                && nextPattern != NO_NOTE_CROWN_SKELETON
                && nextPattern != NOTE_CROWN_SKELETON)
            {
                canStartFever = false;
                feverManager.ControlFeverStart();
            }
        }
    }

    private bool CanStartFeverCombo()
    {
        if (!canStartFeverCombo)
        {
            canStartFeverCombo = true;
            return true;
        }
        else
            return false;
    }


    private void SetFeverWeaponColliders(bool enable)
    {
        for (int i = 0; i < feverWeaponColliders.Length; i++)
        {
            feverWeaponColliders[i].enabled = enable;
        }
    }

    // ============================ Effect ============================ 
    private void PlayAttackEffect(int effNum, int dir)
    {
        effManager.ShowAttackEffect(effNum, transform.position, dir);
    }

    private void PlayComboEffect(int effNum)
    {
        effManager.ShowComboEffect(effNum, transform.position);
    }

    private void PlayDashEffect(int dir)
    {
        if (playerAnim.speed != 0)
        {
            //Debug.Log(">> Front DAsh--------------");
            if (dir == 0)
            {
                effManager.ShowDashEffect(1, transform.position);
            }
            else
            {
                effManager.ShowDashEffect(2, transform.position);
            }
        }
    }


    // ============================ Monster ============================ 

    private void InitCrownSkelSetting(bool isMissed)
    {
        Mob_Skeleton missedSkel = target_crownSkelMob.GetComponent<Mob_Skeleton>();

        if (target_crownSkelMob != null)
        {
            if (isMissed)
                missedSkel.DeadMissedMob();
            else
                missedSkel.GoDeadState();
        }

        inputCount_crownSkel = 0;
        target_crownSkelMob = null;

        mon_left = null;
        mon_right = null;
        canDashAndAttack = false;
        // 크라운 스컬 셋팅 초기화 후 혹시 피버를 기다리는 중이었다면 시작하도록 함 
        CheckFeverModeAvailablity();
    }
    public void InitGolemSetting(bool isMissed)
    {
        Mob_Golem curGolem = target_golemMob.GetComponent<Mob_Golem>();
        if (target_golemMob != null && curGolem.GetHpVal() < 4)
        {
            if (isMissed)
                curGolem.DeadMissedMob();
            else
                curGolem.GoDeadState();
        }

        inputCount_golem = 0;
        target_golemMob = null;

        mon_left = null;
        mon_right = null;
        canDashAndAttack = false;

        // 골렘 셋팅 초기화 후 혹시 피버를 기다리는 중이었다면 시작하도록 함 
        CheckFeverModeAvailablity();
    }


    // ============================ Animation ============================ 

    // Attack Animation에서 호출되는 Event
    public void OffAttackAnim(int animSig)
    {
        // attack01, 02, 03 재생이 끝나고 Idle 애니메이션으로 돌아가게 하기 위해 
        playerAnim.SetTrigger("IsIdle");
        switch (animSig)
        {
            case 0:
                playerAnim.SetBool("AttA", false);
                break;
            case 1:
                playerAnim.SetBool("AttB", false);
                break;
            case 2:
                playerAnim.SetBool("AttC", false);
                break;
        }
    }

    // combo01-05 Animation에서 호출되는 Event
    public void PlayNextAnimation(int idx)
    {
        // 콤보에서는 다음 입력 전까지의 텀이 애니메이션 길이보다 길 경우를 대비해,
        // 콤보 애니메이션이 60% 이상 재생된 경우 공중에 멈춰있음 (anim.speed = 0)
        // => 애니메이션이 끝까지 재생됐다는 건 breakCombo가 됐다는 것이므로 애니메이션 끝처리를 해줌
        playerAnim.SetTrigger("ComboEnd");
        playerAnim.SetBool("ComboStart", false);

        playerAnim.SetBool("AttA", false);
        playerAnim.SetBool("AttB", false);
        playerAnim.SetBool("AttC", false);


        if (!isFeverMode) // 피버모드일 때에는 현재 패턴을 무시
        {
            // 마지막 combo5에서 호출했다면 불필요한 애니메이션 재생을 막기 위해 ResetTrigger를 해줌
            if (idx == 0 && curComboPattern == NOTE_COMBO_01)
            {
                SendFallSignal(comboMonSet);
            }
            if (idx == 4)
            {
                playerAnim.ResetTrigger("IsFrontdash");
                playerAnim.ResetTrigger("IsBackdash");
            }
        }
    }

    // Idle 애니메이션이 재생될 때 호출되는 함수
    // - trigger들을 리셋해서 다음 실행에서 영향받지 않도록 한다.
    public void IdleSet()
    {
  
        if (curComboPattern == NOTE_COMBO_01)
        {
            playerAnim.SetBool("ComboStart", false);
        }
        // Idel로 돌아오고 나서 다시 Trigger에 반응하지 않도록 함 
        playerAnim.ResetTrigger("GoCombo02");
        playerAnim.ResetTrigger("GoCombo03");
        playerAnim.ResetTrigger("GoCombo04");
        playerAnim.ResetTrigger("GoCombo05");
        playerAnim.ResetTrigger("ComboEnd");
        playerAnim.ResetTrigger("IsFrontdash");
        playerAnim.ResetTrigger("IsBackdash");

        isComboSignal = false;
    }

    // dash를 위해 attack 중간에 애니메이션 전환이 필요할 때 호출됨
    private void OffAttackAnimForDash(int animNum)
    {
        switch (animNum)
        {
            case 0:
                playerAnim.SetBool("AttA", false);
                break;
            case 1:
                playerAnim.SetBool("AttB", false);
                break;
            case 2:
                playerAnim.SetBool("AttC", false);
                break;
        }
    }

    // Combo01 Animation 에서 호출되는 함수
    // - 몬스터의 FallSignal이 애니메이션 타이밍과 맞게 떨어져야 함
    private bool isCombo01 = false;
    public void SendFallSignalFromAnim()
    {
        if(combo01MonSet != null)
        {
            if(isCombo01)
            {
                Debug.Log("SendFallSignalFromAnim => ComboMonSet.name = " + combo01MonSet.name);
                SendAttackSignal(combo01MonSet, NOTE_COMBO_01);
                SendFallSignal(combo01MonSet);
                combo01MonSet = null;
                isCombo01 = false;
            }
        }            
    }
}