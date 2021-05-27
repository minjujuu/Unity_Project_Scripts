using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_EnemyHP : MonoBehaviour
{

    public static MJ_EnemyHP Instance;
    public ParticleSystem enemyExplosion;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    enum EnemyHpState
    {
        DamageMode,
        RecoverMode
    }

    EnemyHpState eState;
    public float maxHp = 1f;
    public float enemyDamageK = 0.005f;
    private float hp = 0;
    public float HP
    {
        get { return hp; }
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            // 게이지를 움직인다.
            hpMesh.material.SetFloat("_Guage", hp / maxHp);

            if(hp <= 0)
            {
                PlayerWin();
            }
        }
    }

    MeshRenderer hpMesh;
    float ranRed;
    void Start()
    {
        hpMesh = GetComponent<MeshRenderer>();
        HP = maxHp;
    }

    private void Update()
    {
        switch(eState)
        {
            case EnemyHpState.DamageMode:
                Damage();
                break;
            case EnemyHpState.RecoverMode:
                Recover();
                break;
        }
    }

    // 외부에서 호출하는 메서드
    public void EnemyDamaged()
    {
        eState = EnemyHpState.DamageMode;
        //Damage();
    }
    
    void Recover()
    {
        HP += enemyDamageK/2;
    }

    void Damage()
    {
        ranRed = Random.Range(0.5f, 1f);

        hpMesh.material.SetFloat("_Red", ranRed);
        HP -= enemyDamageK;

        eState = EnemyHpState.RecoverMode;
    }


    public void PlayerWin()
    {
        MJ_EndingManager.Instance.AppearWinText();
        //print("Player Win=====================");
        PlayDieEffect();
        GameObject.FindWithTag("Enemy").SetActive(false);
    }
    private void PlayDieEffect()
    {
        ParticleSystem par = Instantiate(enemyExplosion);
        par.transform.position = this.transform.position;
        par.Stop();
        par.Play();
    }
}
