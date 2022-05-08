using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    enum State
    {
        Loading, // 골렘만
        Move,
        Attack,
        Dead,
        InAir,
        Fever
    }

    public string FEVER_MON_NAME; // 상수


    public Material hit_mat;
    public Material default_mat;
    public SkinnedMeshRenderer skel_smr;
    public SkinnedMeshRenderer[] smrs;
    private Animator skelAnim;
    private EffectManager effManager;

    private int hp = 0;
    private int defaultHp = 0;

    private float attackRange;
    private float moveSpeed;
    private float flySpeed;
    private float ranHeight;
    private float fallVal;
    private float feverSpeed;

    private bool isFly;
    private bool isComboMon;
    private bool isGamePaused;
    private bool isFeverMon;

    private Transform player;
    private Transform monsterPoolParent;
    private State state;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void Initialize()
    {

    }
    
    private void Move()
    {

    }

    private void SetMoveState()
    {

    }

    private void Attack()
    {

    }

    private void SetAttackState()
    {

    }

    public void SetComboMon() // 100% 같음 
    {

    }
    public void StartMobInAirLogic(int patternNum)
    {

    }
    private void DelayInAirState() // 100% 같음
    {

    }

    public void StartMobAttackLogic() // 100% 같음
    {

    }
    public void SetHPVal(int val)
    {

    }

    private void SetHitMatrial()
    {

    }
    private void InitMatrial()
    {

    }
    public void FallWithComboFinal()
    {

    }

    private void InAir()
    {

    }
    public void SetFeverMode()
    {

    }
    private void Fever()
    {

    }

    public void FeverDead()
    {

    }

    public bool IsFeverMode()
    {
        return false;
    }

    private void Dead()
    {

    }
    private IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(2.0f);
    }

    private void SetInitState()
    {

    }
    public void GoDeadState() // 100% 같음
    {

    }
    public void DeadMissedMob() // 100% 같음
    {

    }
    public bool IsDeadState() // 100% 같음
    {
        return false;
    }

    public void Disable_SkinnedMeshRenderers()
    {

    }
    public void Enable_SkinnedMeshRenderers()
    {

    }

    public void SetIsGamePaused(bool isPaused)
    {

    }
}
