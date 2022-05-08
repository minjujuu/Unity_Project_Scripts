using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public enum State
    {
        Loading, // 골렘만
        Move,
        Attack,
        Dead,
        InAir,
        Fever
    }

    public SkinnedMeshRenderer[] allMeshRenderers; // 0번째에 main body mesh 등록 필요
    public int defaultHp = 0;

    protected string FEVER_MON_NAME; // 상수

    public Material hit_mat;
    public Material default_mat;
  
    protected Animator anim;
    protected EffectManager effManager;

    protected int hp = 0;
    protected int curComboPatternNum = 0;
    protected float attackRange;
    protected float moveSpeed;
    protected float flySpeed;
    protected float ranHeight;
    protected float fallVal;
    protected float feverSpeed;

    protected bool isInAir;
    protected bool isComboMon;
    protected bool isGamePaused;
    protected bool isFeverMon;

    protected Transform player;
    protected Transform monsterPoolParent;
    protected State state;

 
    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindObjectOfType<Player>().transform;
        effManager = GameObject.FindObjectOfType<EffectManager>();

        if (GameObject.Find("MonsterPoolParent") != null)
            monsterPoolParent = GameObject.Find("MonsterPoolParent").transform;

        hp = defaultHp;

        if (gameObject.name == FEVER_MON_NAME)
        {
            SetFeverMode();
        }
    }

    public virtual void Update()
    {
        if (isGamePaused)
            Time.timeScale = 0;
        else
        {
            if (Time.timeScale == 0)
                Time.timeScale = 1;
        }

        switch (state)
        {
            case State.Loading:
                Loading(); // 골렘만
                break;
            case State.Move:
                Move();
                break;
            case State.Attack:
                Attack();
                break;
            case State.InAir:
                InAir();
                break;
            case State.Dead:
                Dead();
                break;
            case State.Fever:
                Fever();
                break;
        }
    }

    // ========================= Loading =============================
    public virtual void Loading()
    {
        // for golem
    }
    
    // ========================= Move =============================
    public abstract void Move();

    public virtual void SetMoveState()
    {
        transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);
        anim.SetBool("IsMoving", true);
        anim.SetBool("IsAttack", false);
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed);
    }

    // ========================= Attack ===========================
    public abstract void Attack();

    public virtual void SetAttackState()
    {
        anim.SetBool("IsAttack", true);
        anim.SetBool("IsMoving", false);

    }
    // ========================= Combo ============================

    public void SetComboMon() 
    {
        isComboMon = true;
    }

    public void StartMobInAirLogic(int patternNum) 
    {
        Debug.Log("Monster => StartMobInAirLogic()");
        isInAir = true;
        curComboPatternNum = patternNum;        
        moveSpeed = 0; // flyMon의 부모가 노트여서 자체 속도는 0으로 설정함
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        if (patternNum >= MonsterPatternGenerator.NOTE_COMBO_01)
        {
            ranHeight = 5.0f;
            Invoke(nameof(DelayInAirState), 0.3f); // 시그널을 받자마자 바로 떠오르면 애니메이션이랑 안맞음
        }
        else 
        {
            ranHeight = 6.0f;
            state = State.InAir;
        }
    }
    private void DelayInAirState() 
    {
        state = State.InAir;
    }

    public void StartMobAttackLogic()
    {
        anim.speed = 1;
        AddHpVal(-1);
    }

    public virtual void AddHpVal(int val)
    {
        // set body hit material
        allMeshRenderers[0].material = hit_mat;
        Invoke(nameof(InitMatrial), 0.3f);

        hp += val;

        if (hp <= 0)
        {
            state = State.Dead;
        }
        else
        {
            anim.speed = 1;
            anim.SetTrigger("Trig_Hit");
        }
        
    }

    private void InitMatrial()
    {
        allMeshRenderers[0].material = default_mat;
    }

    public void FallWithComboFinal()
    {
        transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z); // 떨어지게 하기
    }

    // ========================= InAir ============================
    public abstract void InAir();

    // ========================= Fever ============================
    public void SetFeverMode()
    {
        isFeverMon = true;
        state = State.Fever;
    }

    public abstract void Fever();

    public void FeverDead()
    {
        anim.speed = 1;
        anim.SetBool("IsDie", true);
        Invoke(nameof(FeverDestroySelf), 0.2f);
    }

    public void FeverDestroySelf()
    {
        Destroy(this.gameObject); // Fever몹은 오브젝트 풀을 사용하지 않으므로 
    }

    public bool IsFeverMode()
    {
        if (isFeverMon)
            return true;
        else
            return false;
    }
    // ============================================================
    // ========================= Dead =============================
    // ============================================================
    public void Dead()
    {
        anim.speed = 1;
        if (isComboMon)
        {
            while (this.transform.position.y > 0.3f)
            {
                this.transform.position -= new Vector3(0, fallVal, 0);
            }
        }

        anim.SetBool("IsDie", true);
        anim.SetBool("IsMoving", false);
        anim.SetBool("IsAttack", false);
        moveSpeed = 0; // 죽었을 때 움직이는 상황이 없도록 하기 위해 
        StartCoroutine(PushObjectPool());
    }

    private IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(5.0f);
        SetInitState();
    }

    public virtual void SetInitState()
    {
        gameObject.SetActive(false);
        Enable_SkinnedMeshRenderers();
        transform.SetParent(monsterPoolParent);
        hp = defaultHp;

        isInAir = false;
        isComboMon = false;
        curComboPatternNum = -1;        
    }

    public void GoDeadState() 
    {
        state = State.Dead;
    }

    public void DeadMissedMob() 
    {
        SetInitState();
    }

    public bool IsDeadState() 
    {
        if (state == State.Dead)
        {
            return true;
        }
        else
        {
            return false;
        }
    
    }
    // ============================================================
    // ========================= ETC ==============================
    // ============================================================
    // Fever 등 상황에서 기존 몬스터를 숨겨야 할 때 사용되는 함수
    // 오브젝트 자체를 Disable 시키면 기존 로직이 정상 동작하지 않기 때문에 Mesh를 숨기도록 처리함 
    public virtual void Disable_SkinnedMeshRenderers()
    {
        for (int i = 0; i < allMeshRenderers.Length; i++)
        {
            allMeshRenderers[i].enabled = false;
        }
    }
    // 숨긴 몬스터를 다시 활성화할 때 필요한 함수
    public virtual void Enable_SkinnedMeshRenderers()
    {
        for (int i = 0; i < allMeshRenderers.Length; i++)
        {
            allMeshRenderers[i].enabled = true;
        }
    }

    public void SetIsGamePaused(bool isPaused)
    {
        isGamePaused = isPaused;
    }

    public int GetHpVal()
    {
        return hp;
    }

}
