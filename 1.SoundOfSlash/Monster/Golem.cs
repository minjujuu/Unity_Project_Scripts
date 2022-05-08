using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Monster
{
    
    /* 골렘에서만 쓰는 변수 */        
    private Animator warningEffect;
    private Vector3 initalDir;    
    private int golemSpawnDir = -1;    
    private float stateDelayTime = 7;     
    private float timer = 0;
    private bool isDeadCompleted = false;
    private bool isSetDir = false;


    public override void Start() {
        FEVER_MON_NAME = "Fever_Monster_Golem";
        base.Start();
        warningEffect = transform.Find("WarningEffect").GetComponent<Animator>();
        state = State.Loading;
    }

    // ============================================================
    // ======================== Loading ===========================
    // ============================================================

    public override void Update()
    {
        base.Update();
    }

    public override void Loading()
    {
        // 스폰 위치에 따라 플레이어 쪽으로 방향 설정
        if (!isSetDir)
        {
            isSetDir = true;
            initalDir = player.position - transform.position;
            if (transform.position.x < player.transform.position.x)
            {
                golemSpawnDir = 0;
            }
            else
            {
                golemSpawnDir = 1;
            }

        }
        transform.position = new Vector3(transform.position.x, 0.3f, transform.position.z);

        //float distance = Vector3.Distance(player.position, transform.position);
        transform.rotation = Quaternion.LookRotation(player.position - transform.position);

        moveSpeed = 0;
        if (transform.parent == null || isComboMon) // ComboMon 이면 바로 Move로 감 
        {
            state = State.Move;
        }
    }

      // ============================================================
    // ========================= Move =============================
    // ============================================================
    public override void Move()
    {
        SetDirection();
        SetMoveState();

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < attackRange) // 공격 범위 안이면 공격
        {
            state = State.Attack;
        }
    }
    private void SetDirection()
    {
        if (golemSpawnDir == 0)
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
    }

    // ============================================================
    // ========================= Attack ===========================
    // ============================================================
    public override void Attack()
    {
        // 거리 체크
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance > attackRange) // 공격 범위 밖이면 이동
        {
            timer += Time.deltaTime;
            if (timer > stateDelayTime)
            {
                timer = 0;
                state = State.Move;
            }
        }
        SetAttackState();        
    }

    public override void SetAttackState()
    {
        base.SetAttackState();
        timer += Time.deltaTime;
        if (timer > stateDelayTime)
        {
            timer = 0;
            anim.SetTrigger("KeepState");
        }
    }
    // ============================================================
    // ========================= Combo ============================
    // 전부 부모 코드 사용 =========================================

    // ============================================================
    // ========================= InAir ============================
    // ============================================================
    public override void InAir()
    {
        anim.speed /= 1.2f;
        // ranHeight 보다 작으면 상승시켜줌
        if (transform.position.y <= ranHeight && isInAir)
        {
            //transform.Translate(new Vector3(0, flySpeed, 0)); // y값을 증가시키면 z값이 올라감 (?)
            transform.Translate(new Vector3(0, 0, flySpeed)); // 이렇게 해야 y값이 올라감
        }
        else
        {
            isInAir = false;
        }
    }

    // ============================================================
    // ========================= Fever ============================
    // ============================================================

    public override void Fever()
    {
        if (transform.position.x < player.transform.position.x)
        {
            golemSpawnDir = 0;
        }
        else
        {
            golemSpawnDir = 1;
        }

        moveSpeed = feverSpeed;
        attackRange = 4.0f;
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < attackRange)
        {
            SetAttackState();
        }
        else
        {
            SetDirection();
            SetMoveState();
        }
    }

    public override void SetInitState()
    {
        base.SetInitState();
        golemSpawnDir = -1;
        isDeadCompleted = false;
        isSetDir = false;
        state = State.Loading;
    }
    
    // ============================================================
    // =================== Call from Animator =====================
    // ============================================================
    /* Animation에서 호출되는 함수들 */
    // 애니메이션 속도 조절을 위함 
    public void SendAttackSignal()
    {
        warningEffect.SetTrigger("warning");
    }

    public void SendAttackOutSignal()
    {
        warningEffect.SetTrigger("warningOut");
    }

    public void SetAnimSpeed(float speedVal)
    {
        anim.speed = speedVal;
    }

    public void SetHitAnimSpeed(int startEnd)
    {
        float tmpSpeed = anim.speed;
        switch (startEnd)
        {
            case 0: // 애니메이션 시작점에서 호출한 거면
                anim.speed = 1;
                break;
            case 1:
                anim.speed = tmpSpeed;
                break;
        }
    }

    // ============================================================
    // ========================= ETC ==============================
    // ============================================================
    public override void Disable_SkinnedMeshRenderers()
    {
        warningEffect.gameObject.SetActive(false);
    }

    public override void Enable_SkinnedMeshRenderers()
    {
        warningEffect.gameObject.SetActive(true);
    }

    
    void OnCollisionEnter(Collision collision)
    {
        if (isFeverMon)
        {
            if (collision.transform.tag == "player_weapon")
            {
                effManager.ShowMobHitEffect(this.transform.position);
                AddHpVal(-1);
            }
        }
    }
}