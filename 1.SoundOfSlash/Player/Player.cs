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
        // �ǹ������� �Է��� anykey�̹Ƿ�, �� �޺� �Է� ���� �� ũ�� ������
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
    /* Swipe note�� ���� Ű�� */
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
        // �޺��� ���� ��Ʈ��
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
    // TODO : temp �Ϲ� �÷��� ���
    private void ControlPlayer()
    {
        CheckNoteMissing();

        /* Handle Combo Progress */

        SetComboSignal();
        if (isComboSignal)
        {
            CheckWaitingComboTerm();
            CheckBreakCombo(); // �޺��� Break �Ǿ����� üũ
        }
  
        GetPlayerInput();
    }

    public void CheckNoteMissing()
    {
        for (int i = 0; i < noteAreas.Length; i++)
        {
            if (noteAreas[i].IsMissed())
            {
                if (!isFeverMode) // �ǹ� ��忡�� ��Ʈ ���ĵ� miss ó�� ����
                {
                    hpManager.SubPlayerHP();
                    scoreSystem.SubScore(); // ��������
                }

                /* �޺� ��Ʈ�� ��ģ���� �Ǵ� */
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

                // hp 2 �̻��� ���(�հ��ذ���� ��)�� ������ ���� ģ �ɷ� �Ǿ�� ��
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
            // comboSignal�̴��� �̹��� �޺��� break�Ǹ� �ٲ�� �ȵ�
            // ���å
            // 1. breakCombo�� �̹� �� ���¿����� �Է��� ��ȿ�� �Ѵ٤�
            // - ���� �޺����ͼ��� �Ҵ���� �� ����
            // - comboInput ���� ��ȭ�� �� �� ����

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
            // ���� ��ǲó���ϴ� ��
        }


        // ��Ʈ�� ���� �Ÿ� �ȿ� ������ ��, �Է� ������ ��Ȳ�� UI�� ǥ������
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
        // �ٸ� Ű�� ������ ��츦 ����ؼ� ���� false�� �ʱ�ȭ����
        isPressed_Track1 = false;
        isPressed_Track2 = false;
        isPressed_Track3 = false;
        isPressed_Track4 = false;
        isPressed_Track5 = false;
        isPressed_Track6 = false;
    }

    public void GetNoteTapSignal()
    {
        // Rhythm Game Starter�� NoteArea���� �� �ñ׳��� �޾ƿ�
        for (int i = 0; i < noteAreas.Length; i++)
        {
            if (noteAreas[i].GetNoteTapNumberSignal()) // KillNote �Ǿ��� �� (�÷��̾ ���� �Է����� ��)
            {
                Debug.Log($"{nameof(GetNoteTapSignal)}.i : {i}");
                if (i == Direction.TRACK_1 || i == Direction.TRACK_3 || i == Direction.TRACK_5) // ���� Ʈ��
                {
                    LeftAttack(i);
                }
                else if (i == Direction.TRACK_2 || i == Direction.TRACK_4 || i == Direction.TRACK_6) // ������ Ʈ��
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
            if (IsNearNotes()) // ���� ����� ���� ��Ʈ�� �ְ�
            {
                if (IsPerfectNoteTap(track)) // Ÿ�̹� �°� �����ٸ�
                {
                    canDashAndAttack = true;
                    Debug.Log("canDashAndAttack=true");
                }
                else
                {
                    hpManager.SubPlayerHP(); // Ÿ�̹��� �� �¾����� HP ����
                    // ����� ���� ��Ʈ�� �־� �뽬 �Ұ�
                    canDashAndAttack = false;
                }
            }
            else // ����� ���� ��Ʈ�� ���ٸ� �׳� �뽬 ����
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
            if (IsNearNotes()) // ���� ����� ���� ��Ʈ�� �ְ�
            {
                if (IsPerfectNoteTap(track)) // Ÿ�̹� �°� �����ٸ�
                {
                    canDashAndAttack = true;
                }
                else
                {
                    canDashAndAttack = false; // �߸����� �� 
                    hpManager.SubPlayerHP();
                }
            }
            else // ����� ���� ��Ʈ�� ���ٸ� 
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

                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // �޺� ��Ʈ�� �ƴ� ��쿡�� 
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

                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // �޺� ��Ʈ�� �ƴ� ��쿡�� 
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
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // �޺� ��Ʈ�� �ƴ� ��쿡�� 
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
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // �޺� ��Ʈ�� �ƴ� ��쿡�� 
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
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // �޺� ��Ʈ�� �ƴ� ��쿡�� 
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
                    if (spawnManager.GetNextPatternNumber() < NOTE_COMBO_01 && spawnManager.GetNextPatternNumber() != NO_NOTE_COMBO) // �޺� ��Ʈ�� �ƴ� ��쿡�� 
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
                FindNearestMob(dir, false); // Track �ε���(dir)�� �־��־�� �� 
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
                        // ������ �� �뽬�� ���� ���� 
                        mon_right = target_golemMob;
                        mon_left = null;
                    }
                }
            }

            transform.rotation = Quaternion.Euler(0, 90, 0);

            // �Ʒ� �����ִ� ������ Ʈ�� ������� ������ LEFT
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
                        // ������ �� �뽬�� ���� ���� 
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

        // spawn manager���� ������� ���� ��ť���� ���� �����´�
        // - target_golem_mob�� target_crownskelMob�� null �� ���� �� �Լ��� ȣ��Ǿ�� �Ѵ� 
        // - �񷽰� �հ��ذ�� �����̸� �׸�ŭ �� ��Ʈ�� ������� �Ŷ� ���� �� �� ���� ȣ��Ǿ�� �Ѵ� 
        if (isMissed)
        {
            GameObject missedMob = noteAreas[dir].GetTargetMob();
            if (missedMob != null)
            {
                int peek_layer = missedMob.layer;
                // ��� ��ġ�� input �� 0�̶� ���ǿ� �� �� �ְ� �� ?
                // ��� ���� ���� �޺��� �ƴϾ�� ��
                switch (peek_layer)
                {
                    case Layer.SKELETON:
                        break;
                    case Layer.GOLEM:
                        if (inputCount_golem == 0)
                        {
                            //Debug.Log("FindNearestMob(missed) : Golem �Ҵ�");
                            target_golemMob = missedMob;
                            targetMob = target_golemMob;
                        }
                        break;
                    case Layer.CROWN_SKELETON:
                        if (inputCount_crownSkel == 0)
                        {
                            target_crownSkelMob = missedMob;
                            //Debug.Log("FindNearestMob(missed) : Crown Skel �Ҵ�");
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
                    //Debug.Log("FindNearestMob( NOT ) : Crown Skel �Ҵ�");
                    target_crownSkelMob = targetMob;
                    inputCount_crownSkel++;
                }
                else if (targetMob.layer == Layer.GOLEM)
                {
                    //Debug.Log("FindNearestMob( NOT ) : Golem �Ҵ�");
                    target_golemMob = targetMob;
                    inputCount_golem++;
                }
                else
                    //Debug.Log("FindNearestMob( NOT ) : Skel �Ҵ�");

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

    // dash �� attack�ϴ� �Լ�
    private void DashAndAttack(int dir, GameObject target)
    {
        // Ȥ�� ��� ���� Attack Animation�� �ִٸ� ��� Off
        OffAttackAnimForDash(0);
        OffAttackAnimForDash(1);
        OffAttackAnimForDash(2);
        playerAnim.ResetTrigger("IsBackdash");
        PlayDashEffect(dir);

        Vector3 targetPos = target.transform.position;

        // Track�̶� ������� ���� �ƴϸ� �������� 
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

        // �� ���� ������ �����ư��鼭 ���� 
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


        // ���� �� null�� ���־�� ��
        // - ���� ���Ͱ� �Ҵ�Ǿ����� ���� �����Ƿ�
        //Debug.Log("���� Ÿ�� �ʱ�ȭ");


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
            transform.rotation = Quaternion.Euler(0, 90, 0); // ������ ���� ��
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, -90, 0); // �������� ���� ��

        }
        if (comboInputNum == 0)
        {
            //Debug.Log("----------------- Front Dash ------------------");
            PlayDashEffect(dir);

            // Ȥ�� ��� ���� Attack Animation�� �ִٸ� ��� Off
            OffAttackAnimForDash(0);
            OffAttackAnimForDash(1);
            OffAttackAnimForDash(2);

            playerAnim.SetTrigger("IsFrontdash");

            //��ư�� ������ �ٶ󺸴� �������� ���� �̵��Ѵ�
            transform.Translate(Vector3.forward * dashValue_front);
        }
    }

    private void BackDash(int dir)
    {
        if (comboInputNum == 0)
        {
            //Debug.Log("--------------------Back dash---------------");

            PlayDashEffect(dir);

            // Ȥ�� ��� ���� Attack Animation�� �ִٸ� ��� Off
            OffAttackAnimForDash(0);
            OffAttackAnimForDash(1);
            OffAttackAnimForDash(2);

            playerAnim.SetTrigger("IsBackdash");

            if (dir == 0) // �����̸�
            {
                transform.rotation = Quaternion.Euler(0, -90, 0); // �������� ���� ��
            }
            else // �������̸�
            {
                transform.rotation = Quaternion.Euler(0, 90, 0); // ������ ���� ��
            }

            //��ư�� ������ �ٶ󺸴� ����� �ݴ�������� ���� �̵��Ѵ�
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

    // �޺� ��Ʈ �Է� ���� ������ �� isComboSignal�� true�� ����
    public void SetComboSignal()
    {
        // �޺� ��Ʈ�� �Է� ���� ���� �ȿ� ������
        if (detectComboNoteArea2.IsSetComboSignal() || detectComboNoteArea4.IsSetComboSignal())
        {
            // ���� �޺� break�� �̹� �޺��� ������ ��ġ�� �ʰ� �ϱ� ���� 
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
        // �Ʒ� ���ǵ� ��ü�� �޺��� ���� �����ϹǷ� isComboSignal�� �����־��� 
        // 1. ���߿� ��� ���� �� breakCombo�� �Ǹ� �÷��̾ ���󺹱� �����ֱ� ���� �ڵ�
        if (isComboWaiting || playerAnim.speed == 0) // �̰� isComboWaiting�̶� isComboSignal�̶� Ÿ�̹��� �� �ȸ��� 
        {
            if (detectComboNoteArea2.IsBreakCombo() || detectComboNoteArea4.IsBreakCombo())
            {
                Debug.Log("### ���߿� �����ִٰ� breakcombo��");
                playerAnim.speed = 1;
                if (!isFeverMode)
                    playerAnim.SetTrigger("ComboEnd");

                isBreakCombo = true;
                breakComboMonSet = comboMonSet;
                ComboReset();
            }
        }
        // 2. �޺� ��Ʈ�� �ƿ� ��ġ�ų� ���� �� �÷��̾ ���󺹱� �����ֱ� ���� 
        else if (!detectComboNoteArea2.IsSetComboSignal())
        {
            // IsSetComboSignal ���� comboSignal�� NoteArea�� �Ϲ� ��Ʈ�� ������ false
            // NoteArea�� �޺� ��Ʈ�� ������ ��Ʈ�� TriggerExit�ϸ� false�� �� 
            Debug.Log("### �Ϲ� ��Ʈ�� ���԰ų� ������ �޺� ��Ʈ�� TriggerExit��");
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
        //    if (isComboWaiting && playerAnim.speed == 0) // �̰� isComboWaiting�̶� isComboSignal�̶� Ÿ�̹��� �� �ȸ��� 
        //    {
        //        if (detectComboNoteArea2.IsBreakCombo())
        //        {
        //            Debug.Log("### ���߿� �����ִٰ� breakcombo��");
        //            playerAnim.speed = 1;
        //            if (!isFeverMode)
        //                playerAnim.SetTrigger("ComboEnd");

        //            isBreakCombo = true;
        //            ComboReset();
        //        }
        //    }
        //    // 2. �޺� ��Ʈ�� �ƿ� ��ġ�ų� ���� �� �÷��̾ ���󺹱� �����ֱ� ���� 
        //    else if (!detectComboNoteArea2.IsSetComboSignal())
        //    {
        //        // IsSetComboSignal ���� comboSignal�� NoteArea�� �Ϲ� ��Ʈ�� ������ false
        //        // NoteArea�� �޺� ��Ʈ�� ������ ��Ʈ�� TriggerExit�ϸ� false�� �� 
        //        Debug.Log("### �Ϲ� ��Ʈ�� ���԰ų� ������ �޺� ��Ʈ�� TriggerExit��");
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
        // ������ ��Ʈ�� ���� ������ ��
        // - ����� �������� ���� �������� �ʱ�ȭ����
        // - ����� ������ �ʾҴٸ� �޺��� �ʱ�ȭ���� 
        if (detectComboNoteArea2.IsEnterFinalComboNote() || detectComboNoteArea4.IsEnterFinalComboNote()) // ��Ÿ�� ������ ��Ʈ�� Ʈ���ſ� �������� 
        {

            if (curComboPattern - comboInputNum == (NOTE_COMBO_01 - 1)) // ������ ��Ʈ ĥ ���ʶ�� 
            {
                Debug.Log("CheckTriggerFinalNote(): ������ ��Ʈ ĥ ����");
                
                //SendFallSignal();
            }
            else if ((curComboPattern - NOTE_COMBO_01) > comboInputNum)  // ������ ��Ʈ�� ĥ ���ʰ� �ƴ� �� �ƴµ�, ���� �ľ��ϴ� Ƚ���� �ȸ�����
            {
                Debug.Log("CheckTriggerFinalNote(): ������ ��Ʈ ĥ ���ʾƴ�");
                // ������ ��Ʈ �ƴµ� �ȸ��� ///// ���� ����
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

                Debug.Log($"1�� ��({comboTimes}");

                cameraFollower.SetYPositionRelative(3f);

                playerAnim.SetBool("ComboStart", true);
                if(!isSetComboMons)
                {
                    //ChangeComboMonSet();
                    isSetComboMons = true;
                }
                //comboMonSet = spawnManager.GetComboMonSet();
                // �÷��̾� ��ġ ����
                if (comboMonSet.transform.childCount != 0)
                {
                    tempComboMon = comboMonSet.transform.Find("Monster").gameObject;
                    if(tempComboMon != null)
                    {
                        // Player rotation ����
                        if (transform.position.x - tempComboMon.transform.position.x > 0)
                        {
                            transform.rotation = Quaternion.Euler(0, -90, 0);
                        }
                        else
                            transform.rotation = Quaternion.Euler(0, 90, 0);
                        // Player position ����
                        transform.position = tempComboMon.transform.position; // �÷��̾� ��ġ�� �޺� ���͵� ��ġ�� ����
                    }

                    //transform.LookAt(tempComboMon.transform);                    
                    comboMon_xPos = transform.position.x;
                }


                SendFlySignal(comboMonSet, curComboPattern);
                CheckTriggerFinalComboNote();

                if (curComboPattern <= NOTE_COMBO_01) // 1��Ÿ�̸� ���⿡�� �� 
                {
                    /* 1��Ÿ �޺������� Fall Signal�� �ִϸ��̼ǿ��� ���� => SendFallSignalFromAnim() */
                    /* ComboReset()�� �������� */
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
                Debug.Log("2�� ��");
                cameraQuake.StartQuake();

                if (isComboWaiting) // ����ϴ� �߿� tap�� �� �� �� �����Ŷ�� animspeed�� ���󺹱� 
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
                Debug.Log("3�� ��");
                cameraQuake.StartQuake();

                if (isComboWaiting) // ����ϴ� �߿� tap�� �� �� �� �����Ŷ�� animspeed�� ���󺹱� 
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

                Debug.Log("4�� ��");
                if (isComboWaiting) // ����ϴ� �߿� tap�� �� �� �� �����Ŷ�� animspeed�� ���󺹱� 
                {
                    playerAnim.speed = 1;
                }

                PlayComboEffect(4);
                SendAttackSignal(comboMonSet, curComboPattern);


                CheckTriggerFinalComboNote();
                if (curComboPattern <= NOTE_COMBO_04) // 4��Ÿ�̸� ���⿡�� �� 
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

                Debug.Log("5�� ��");
                if (isComboWaiting) // ����ϴ� �߿� tap�� �� �� �� �����Ŷ�� animspeed�� ���󺹱� 
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

    // SpawnManager���� �޺����������� ������� �� ȣ���
    public void SetComboOptionSignal(int optionNum)
    {
        // �ϴ� ������ ���ڴ� ������� �迭�� ���� ��� �̹� �޺��� ���� �� ���� ������ 
        // comboSignalTimesFromSpawnManager : spawnManager���� �޺����͸� ���� Ƚ�� 
        optionNums[totalComboTimes] = optionNum;
        flags[totalComboTimes] = optionNum;

        totalComboTimes++;
    }

    
    private bool isFirstAddComboTimes = true;
    public void AddComboTimes()
    {
        // ����� �޺� Ƚ�� ����
        // - ������ �� ���ϴ� �� ���� �����ؾ� ��
        // => �ش� ������ ������ �޺� ��Ʈ�� KillNote�ǰų� TriggerExit�� ��쿡�� ���������� (Track2�� NoteArea����)
        if (isFirstAddComboTimes) // �ʱⰪ ����
        {             
            comboTimes = detectComboNoteArea2.GetPlayerComboTimes();
            
            ChangeComboMonSet();
            isFirstAddComboTimes = false;
        }           
        
        if (comboTimes <= totalComboTimes)
        {
            // ������ �޺� ������ �Ҵ�� ���Ŀ��� comboTimes�� ����Ǿ�� ��
            // �ȱ׷��� �޺� ������ �ϳ� �پ�Ѵ� ���� �߻�
            curComboPattern = optionNums[comboTimes];
            comboTimes = detectComboNoteArea2.GetPlayerComboTimes();
            ChangeComboMonSet();
            
        }
    }

    bool isFirstSetComboMon = true;
    private void ChangeComboMonSet()
    {
        // �޺� ���� ���� �ٲ� �� ���� ��ȣ�� �޺� ���� ���� �˻��ؼ� �ڽ��� ������ �� �׿�����
        // ���� ��ġ�� �� comboMonSet �ٲٱ� ���̴ϱ� ���⼭ ���ָ� �ɵ�
        //Debug.Log($"ChangeComboMonSet => comboTimes : ${comboTimes}, peekNumber : ${spawnManager.GetComboMonSetPeekNumber()}");
        if (spawnManager.GetComboMonSetPeekNumber() == comboTimes + 1)
        {
            
            // �ι�° => comboTimes = 1, comboMonSet2
            // ����° => comboTimes = 2, comboMonSet3 
            // ... (comboTimes�� �޺��� �Ϸ�Ǿ� �÷����Ǵ� ������)
            if (comboMonSet != null)
            {
                if (comboMonSet.transform.childCount != 0)
                {
                    //Debug.Log($"ChangeComboMonSet(): ���� ���ͼ�(${comboMonSet.name}�� �ڽĵ��� [ ${comboMonSet.transform.childCount} ] ���� ����");
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

    // �޺� ��Ÿ �Է� ���̿� ���� �� ��� ���߿��� �����ְ� �� 
    private void CheckWaitingComboTerm()
    {
        // ���� n��Ÿ ���� ���̶��
        // - break combo�̸� combo end
        // - not break combo�̸� ���߿� ���缭 ���� �Է� ���
        if (curComboPattern > NOTE_COMBO_01 && comboInputNum == 1) // 1��Ÿ �Է� �Ϸ� �� 2��Ÿ �Է� ��� ��
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo01") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 2��Ÿ - Break Combo. Idle�� ���ư�
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);
                }
                else
                {
                    // �޺��� ������ �ʾ����� ���߿��� ��� ���
                    isComboWaiting = true;
                    playerAnim.speed = 0;

                }
            }
        }

        if (curComboPattern > NOTE_COMBO_02 && comboInputNum == 2) // 2��Ÿ �Է� �Ϸ� �� 3��Ÿ �Է� ��� ��
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo02") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 3��Ÿ - Break Combo. Idle�� ���ư�
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);
                }
                else
                {
                    // �޺��� ������ �ʾ��� ���߿��� ��� ���
                    isComboWaiting = true;
                    playerAnim.speed = 0;
                }
            }
        }

        if (curComboPattern > NOTE_COMBO_03 && comboInputNum == 3) // 3��Ÿ �Է� �Ϸ� �� 4��Ÿ �Է� ��� ��
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo03") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 4��Ÿ - Break Combo. Idle�� ���ư�
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);
                }
                else
                {
                    // �޺��� ������ �ʾ��� ���߿��� ��� ���
                    isComboWaiting = true;
                    playerAnim.speed = 0;
                }
            }
        }

        if (curComboPattern > NOTE_COMBO_04 && comboInputNum == 4) // 4��Ÿ �Է� �Ϸ� �� 5��Ÿ �Է� ��� ��
        {
            if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("combo04") &&
                playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f)
            {
                if (comboSystem.IsBreakCombo() == true) // 5��Ÿ - Break Combo. Idle�� ���ư�
                {
                    Debug.Log("Check_WaitingComboTerm(): Animation ComboEnd");
                    playerAnim.SetTrigger("ComboEnd");
                    playerAnim.SetBool("ComboStart", false);

                }
                else
                {
                    // �޺��� ������ �ʾ��� ���߿��� ��� ���
                    isComboWaiting = true;
                    playerAnim.speed = 0;
                }
            }
        }
    }

    public void ComboReset() // ��� �޺��� �����ưų�, �߰��� ����������
    {
        // ��� �� ������ ��� �� ���� comboTimes �� �� ���� ȣ��Ǿ�� ��
        Debug.Log("------------------------ COMBO RESET -----------------------");
        if (!isFeverMode)
        {
            // �÷��̾��� ���̸� ���󺹱� ������ 
            transform.position = new Vector3(this.transform.position.x, 0.3f, this.transform.position.z);            
            // �޺� �ִϸ��̼� �ʱ�ȭ
            if (curComboPattern != MonsterPatternGenerator.NOTE_COMBO_01) // 1��Ÿ�� �ƴ� ��� combo animation �ʱ�ȭ
            {
                playerAnim.SetBool("ComboStart", false);
            }
            // ī�޶��� ���̸� ���󺹱� ������
            cameraFollower.SetYPositionRelative();
        }

        // 2. �ܿ� ���� ó��
        // �޺��� �߰��� ���еǾ� �� ��� ComboMonSet�� �������� 

        if (breakComboMonSet != null && isBreakCombo)
        {
            Debug.Log($" {breakComboMonSet.name}�� �ܿ� ���� ó�� | ComboMonSet.transform.childCount = " + breakComboMonSet.transform.childCount);
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

        // 3. ���� ���� �ʱ�ȭ
        comboInputNum = 0;
        if (isComboWaiting)
        {
            isComboWaiting = false;
        }

        // �̰� �ν� �� �ȵǰ� ���� 
        // - comboreset ���Ŀ� final note�ν��� �� ���
        if (detectComboNoteArea2.IsExitFinalComboNote()) // ������ �޺� ��Ʈ�� �������ٸ� 
        {
            Debug.Log("������ �޺� ��Ʈ�� ������ ");
            detectComboNoteArea2.SetIsExitFinalComboNoteFalse();
        }

        // �޺��� ġ�� �ʰ� �Ѿ�� �� ComboMonSetQueue���� Dequeue�� ���־�� ��
        if (!isSetComboMons) // 1�� �ǿ��� ������ �̷��� ���ٸ�
        {
            //ChangeComboMonSet();
        }
        else
        {
            // 1��Ÿ���� �̹� comboMonSet�� �Ҵ������� ������ �ٽ� �Ҵ��� �� �ֵ��� bool���� �ʱ�ȭ����
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

            // �޺� ���� �� Ȥ�� �ǹ��� ��ٸ��� ���̾��ٸ� �����ϵ��� �� 
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
            // �÷��̾� �������� �������� ��������Ʈ�� ��ġ�� �������� 
            transform.position = new Vector3(this.transform.position.x, 0.3f, this.transform.position.z);
            spawnPointsParent.position = transform.position; /* ���� ������ Spawn Manager���� �߾��� */

        if (playerAnim.speed == 0)
            {
                playerAnim.speed = 1;
            }
        }
    }

    private void ControlPlayerOnFever()
    {
        // �ǹ���� ���� ��Ʈ�� �׳� �����ϱ� ������ ���� ó������ ���־�� ��
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
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); // ó�� ���� �� ���� ���� �� 
            //StartCoroutine(StartFeverCombo());
        }
        else
        {
            if (cameraFollower.IsFeverTimeCameraEnd())
            {

                // �ǹ���忡���� ����
                // 1. �ƹ� Ű�� ������ ������ �����ϴ�
                // 2. ���� ������ ���Ͱ� �ִ� �������� 
                // - �� �� �̻� ���� �������� �� ���� �Ѵ� 
                // 3. ��ҿ� �ٸ��� �ݶ��̴� �浹�� �����Ѵ�. (�� ���� ���� ���Ͱ� �״� ������ ����)
                spawnPointsParent.position = transform.position; /* ���� ������ Spawn Manager���� �߾��� */
                if (Input.anyKeyDown) // �ƹ�Ű�� ������ �� 
                {
                    SetFeverWeaponColliders(true);
                    fever_mobParent = GameObject.Find("FeverMonstersParent").transform;
                    if (fever_mobParent.childCount > 0)
                    {
                        // ���� �������� 2�̻� ���� ���� ������ ���� ã��
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
        // Ȥ�� ��� ���� Attack Animation�� �ִٸ� ��� Off
        OffAttackAnimForDash(0);
        OffAttackAnimForDash(1);
        OffAttackAnimForDash(2);
        // ��ư�� ������ �ٶ󺸴� �������� ���� �̵��Ѵ� 
        transform.Translate(Vector3.forward * dashValue_front);
        FeverAttack(dir); // �׸��� �ٷ� �����Ѵ�
    }

    public void FeverAttack(int dir)
    {

        scoreSystem.AddScore(500);
        // �� ���� ������ �����ư��鼭 ���� 
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
        // combowaiting �Ǵ� ������
        // - �ִϸ��̼��� 60 % �̻� ����Ǿ��µ� �߰����� �Է��� ���� ��

        // breakcombo�Ǵ� ������
        // - �ǹ���尡 ������ ��
        // - combowaiting ���¿��� �߰����� �Է��� ���� ��

        // ���� 1��Ÿ �Է� �� 1��Ÿ �ִϸ��̼��� �������µ� 2��Ÿ �Է��� ������ 
        // 1��Ÿ �ִϸ��̼��� ������ ���� �ִϸ��̼��� ����
        // �׸��� �Է� �� TAP�� Ŀ���� �ٽ� �ִϸ��̼��� ����� 

        // ���� ���� �Է� �� ���� 1combo ������ �۰ų� ������,
        // - combo1 �ִϸ��̼��� 60% �̻� ����ƴµ��� �Է� ���� ��ȭ�� ������ anim.speed = 0;
        // - combo1 �ִϸ��̼��� 60% �̻� ����ƴµ� �Է� ���� ��ȭ�� ����� anim.speed = 1;

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

        // �ǹ� �߿� ������ ���� �� breakCombo �� �Ϳ� ���� �ʱ�ȭ�� ���־�� �� 
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

            // �޺� ���� �ƴϰų�, ��, �հ��ذ�� ���� ������ �ƴ� ��쿡�� �ǹ� ���� 
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
                // �ٷ� ������ �� ���� ��Ȳ�� �� ���� ���¸� �����.
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
                && nextPattern < NOTE_COMBO_01 // ���� ���ϵ� �޺� ������ �ƴϰ� 
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
        // ũ��� ���� ���� �ʱ�ȭ �� Ȥ�� �ǹ��� ��ٸ��� ���̾��ٸ� �����ϵ��� �� 
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

        // �� ���� �ʱ�ȭ �� Ȥ�� �ǹ��� ��ٸ��� ���̾��ٸ� �����ϵ��� �� 
        CheckFeverModeAvailablity();
    }


    // ============================ Animation ============================ 

    // Attack Animation���� ȣ��Ǵ� Event
    public void OffAttackAnim(int animSig)
    {
        // attack01, 02, 03 ����� ������ Idle �ִϸ��̼����� ���ư��� �ϱ� ���� 
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

    // combo01-05 Animation���� ȣ��Ǵ� Event
    public void PlayNextAnimation(int idx)
    {
        // �޺������� ���� �Է� �������� ���� �ִϸ��̼� ���̺��� �� ��츦 �����,
        // �޺� �ִϸ��̼��� 60% �̻� ����� ��� ���߿� �������� (anim.speed = 0)
        // => �ִϸ��̼��� ������ ����ƴٴ� �� breakCombo�� �ƴٴ� ���̹Ƿ� �ִϸ��̼� ��ó���� ����
        playerAnim.SetTrigger("ComboEnd");
        playerAnim.SetBool("ComboStart", false);

        playerAnim.SetBool("AttA", false);
        playerAnim.SetBool("AttB", false);
        playerAnim.SetBool("AttC", false);


        if (!isFeverMode) // �ǹ������ ������ ���� ������ ����
        {
            // ������ combo5���� ȣ���ߴٸ� ���ʿ��� �ִϸ��̼� ����� ���� ���� ResetTrigger�� ����
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

    // Idle �ִϸ��̼��� ����� �� ȣ��Ǵ� �Լ�
    // - trigger���� �����ؼ� ���� ���࿡�� ������� �ʵ��� �Ѵ�.
    public void IdleSet()
    {
  
        if (curComboPattern == NOTE_COMBO_01)
        {
            playerAnim.SetBool("ComboStart", false);
        }
        // Idel�� ���ƿ��� ���� �ٽ� Trigger�� �������� �ʵ��� �� 
        playerAnim.ResetTrigger("GoCombo02");
        playerAnim.ResetTrigger("GoCombo03");
        playerAnim.ResetTrigger("GoCombo04");
        playerAnim.ResetTrigger("GoCombo05");
        playerAnim.ResetTrigger("ComboEnd");
        playerAnim.ResetTrigger("IsFrontdash");
        playerAnim.ResetTrigger("IsBackdash");

        isComboSignal = false;
    }

    // dash�� ���� attack �߰��� �ִϸ��̼� ��ȯ�� �ʿ��� �� ȣ���
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

    // Combo01 Animation ���� ȣ��Ǵ� �Լ�
    // - ������ FallSignal�� �ִϸ��̼� Ÿ�ְ̹� �°� �������� ��
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