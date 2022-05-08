using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Monster
{
    /* 스켈레톤에서만 쓰는 변수 */
    public bool is_Crown_Skeleton;    
    private float noteSpeed = 0;    
    private int speed_adjustVal = 250;
    private float noteAreaSpeed = 0.05f; // Note로부터 떨어져나왔을 때 이동속도    
    private float maxActiveTime = 7;
    private float elapsedTimeWithNoParent = 0;
    private bool isOnlyAttackState = false;
    private bool isBounce = false;

   
    public override void Start()
    {
        FEVER_MON_NAME = "Fever_Monster_Skeleton";

        base.Start();

        state = State.Move;
        anim.SetBool("IsMoving", true);
        anim.SetBool("IsAttack", false);
        if (!is_Crown_Skeleton)
        {
            defaultHp = 1;
            attackRange = 2;
        }
        else
        {
            defaultHp = 2;
            attackRange = 1;
        }
    }

    public override void Update()
    {
        if (!isFeverMon)
        {
            // 노트에 의해 컨트롤되지 않는 상황이면 일정 시간 후 Dead 상태로 만듦
            if (this.transform.parent == null)
            {
                elapsedTimeWithNoParent += Time.deltaTime;
                if (elapsedTimeWithNoParent > maxActiveTime)
                {
                    state = State.Dead;
                    elapsedTimeWithNoParent = 0;
                }
            }
        }
        else
        {            
            if (!IsDeadState())
            {
                if (isInAir)
                {
                    state = State.InAir;
                }
                else
                {
                    // TODO: 이 코드 피버모드 테스트할 때 없앨 수 있으면 없애도록
                    // Start에서 해당 몬스터 이름에 따라 Fever 상태로 해주므로
                    state = State.Fever;
                }
            }
        }

        base.Update();
    }

    // ============================================================
    // ========================= Move =============================
    // ============================================================
    public override void Move()
    {
        SetMobSpeed();
        SetMoveState();
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < attackRange) // 공격 범위 안이면 공격
        {
            state = State.Attack;
        }
    }

    private void SetMobSpeed()
    {
        // 이동 속도 설정
        // noteSpeed = InGameManager.Instance.GetNoteSpeed() / speed_adjustVal;
        // 콤보 몬스터가 아니면
        if (!isComboMon || !isFeverMon)
        {
            // 노트 자식으로 붙어있을 땐 moveSpeed 0
            if (transform.parent != null)
                moveSpeed = 0;
            else // 노트에 붙어있지 않은데 이동해야 하면 noteSpeed대로 이동
                moveSpeed = noteSpeed;
        }
        else // 콤보 몬스터이면
        {
            // 만약 ComboMonSet이 노트에 붙어있다면
            if (transform.parent.transform.parent != null)
                moveSpeed = 0;
            else
                moveSpeed = noteSpeed;
        }
    }

    public override void SetMoveState()
    {
        base.SetMoveState();

        // 플레이어를 보게 함 
        Vector3 dir = player.position - transform.position;
        if(dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    // ============================================================
    // ========================= Attack ===========================
    // ============================================================
    public override void Attack()
    {
        // 거리 체크
        float distance = Vector3.Distance(player.position, transform.position);

        moveSpeed = 0;
        SetDirection();
        // 만약 crown mob인 경우 
        // attack이 되었다가 다시 move가 되면 안됨 
        // 공격 범위를 벗어나면 다시 Move로 바꿈
        if (distance > attackRange) // 공격 범위 밖이면 이동
        {
            if (!isOnlyAttackState)
                state = State.Move;
        }
        SetAttackState();
    }

    public override void SetAttackState()
    {
        base.SetAttackState();
        // KeepState Trigger로 Attack1과 Attack2를 번갈아가면서 플레이 함
        anim.SetTrigger("KeepState");
    }

    private void SetDirection()
    {
        // 플레이어의 x좌표보다 몹의 x좌표가 작으면 왼쪽에 있는 것
        if (transform.position.x < player.transform.position.x)
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
    }

    // ========================= Combo ============================
    // 전부 부모 코드 사용

    // ========================= InAir ============================
    public override void InAir()
    {
        // ranHeight 보다 작으면 상승시켜줌
        if (transform.position.y <= ranHeight && isInAir)
        {
            transform.Translate(new Vector3(0, flySpeed, 0));
            if (curComboPatternNum == MonsterPatternGenerator.NOTE_COMBO_01) // 1연타이면 상승하자마자 바로 죽도록 함 
            {
                state = State.Dead;
            }
            else
            {
                anim.speed /= 1.2f;
            }
        }
        else
        {
            isInAir = false;
        }
    }


    // ========================= Fever ============================

    public override void Fever()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance < attackRange)
        {
            moveSpeed = 0;
            SetDirection();
            // 이동
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed);
            SetAttackState(); // 공격
        }
        else
        {
            moveSpeed = feverSpeed;
            SetMoveState();
        }
    }

    // ========================= Etc ============================

    
    public override void AddHpVal(int val)
    {
        base.AddHpVal(val);
        if (defaultHp >= 2)
        {
            if (hp == 1)
            {
                isOnlyAttackState = true;
            }
            state = State.Attack;
        }
    }
    public override void SetInitState()
    {
        base.SetInitState();
        isOnlyAttackState = false;
        state = State.Move;
    }

    void OnCollisionExit(Collision collision)
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
