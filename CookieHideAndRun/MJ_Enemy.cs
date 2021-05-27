using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 파티쉐 눈빛 공격
// 1. 적의 눈에서 눈빛(레이저) 발사
//   - 
// 2. 만약 레이저가 플레이어한테 닿으면 플레이어 HP--;

public class MJ_Enemy : MonoBehaviour
{
    private Transform tr;
    private LineRenderer line;
    public RaycastHit hit;
    //Enemy의 애니메이션
    Animator enemyAnim;

    public Transform playerTr;


    bool isWaitingTime = false;

    public AudioClip hohoSound;
    public float Volume;
    AudioSource audioS;
    public bool alreadyPlayed = false;

    void Start()
    {
        audioS = GetComponent<AudioSource>();
        enemyAnim = GetComponentInParent<Animator>();
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
    }

    bool IsWaiting()
    {
        if (isWaitingTime)
        {
            waitTime += Time.deltaTime;
            if (waitTime > isWaitingDelay)
            {
                waitTime = 0;
                isWaitingTime = false;
                return false;
            }

            return true;
        }

        return false;
    }

    float waitTime = 0;
    public float isWaitingDelay = 5;
    void Update()
    {
        
        Ray ray = new Ray(tr.position + (Vector3.down * 0.02f), tr.forward);

        line.SetPosition(0, tr.InverseTransformPoint(ray.origin));
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            line.SetPosition(1, tr.InverseTransformPoint(hit.point));

        }
        else
        {
            line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f)));
        }
        if (IsWaiting())
            return;
        //========== Ray (눈빛)에 플레이어가 맞으면,
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            // Ray에 부딪힌 물체를 판별하는 메소드 호출
            CheckCollTag();
        }
        else
        {
            enemyAnim.speed = 1;
            isChecking = false;
            isPlayerMoving = false;
        }
    }

    // Ray에 부딪힌 물체를 판별하는 메소드
    // hit 시작 포지션을 한 번만 저장하게 하기 위한 bool 값
    bool isChecking = false;
    // 플레이어가 움직였는지를 한 번만 판단하게 하기 위한 bool값
    bool isPlayerMoving = false;
    Vector3 playerTransform;
    float currentTime;
    float delayTime = 3.0f;
    void CheckCollTag()
    {
        // 플레이어를 쳐다보게 되면, 3초 동안 애니메이션을 멈춘다.
        // 멈춰있는 동안 플레이어가 움직이면 HP를 5 깎는다.
        // Ray와 부딪힌 물체가 Player이면 공격

        // ================ Hit Start Value ==================
        // 처음 플레이어가 Ray 걸렸을 때의 위치 값을 저장
        // 처음 위치랑 달라지면 플레이어가 움직였다는 거니까, 그럴 때 HP를 깎는다.
        if (isChecking == false)
        {
            playerTransform = playerTr.position;
            isChecking = true;
        }

        // ================3초동안 플레이어를 쳐다본다 / Enemy 애니메이션 제어==================
        // 플레이어를 쳐다보면
        // 일단 애니메이션을 멈춘다.
        enemyAnim.speed = 0;

        // 플레이어 보고있는 시간초
        currentTime += Time.deltaTime;
        // ============= 애니메이션을 멈춘 동안 플레이어가 움직이면 ==================
        // -----> 3초가 안지났거나 플레이어가 움직였으면 애니메이션을 멈춤!!!
        // [ 공격 ] 플레이어가 움직이면 HP를 깎는다
        // player 가 이동했을때 (플레이어의 처음 transform과 현재 transform 비교)
        bool isMoving = false;
        if (Vector3.Distance(playerTransform, playerTr.position) > 0.05f)
        {
            // 움직였어.
            isMoving = true;
            // 플레이어가 움직이면 Enemy가 계속 플레이어를 보고 있어야함.
            enemyAnim.speed = 0;

            // 그리고 HP를 깎는다.
            if (isPlayerMoving == false)
            {
                // Damage Blink Play
                DamageBlink.Instance.PlayDamageBlinkTrue();

                // Damage HoHo Sound Play
                PlayDamageSound();
                alreadyPlayed = false;

                currentTime = 0;
                MJ_PlayerHPManager.Instance.MinusPlayerHP();
                isPlayerMoving = true;
            }

        } // ============ 플레이어를 쳐다봤으나, 플레이어가 움직이지 않은채로 3초가 지나면
          // 다시 적 애니메이션 실행 
        else
        {
            if (currentTime > delayTime)
            {
                // 다시 애니메이션 실행
                enemyAnim.speed = 1;
                currentTime = 0;
                isPlayerMoving = false;
                // 몇초동안은 플레이어를 탐색 못하게
                isWaitingTime = true;
            }
        }

        if(isMoving)
        {
            // 3초지나면
            if (currentTime > delayTime)
            {
                // 다시 애니메이션 실행
                enemyAnim.speed = 1;
                currentTime = 0;
                isPlayerMoving = false;
                // 몇초동안은 플레이어를 탐색 못하게
                isWaitingTime = true;
            }
        }
    }

    void PlayDamageSound()
    {
        if (!alreadyPlayed)
        {
            audioS.PlayOneShot(hohoSound, Volume);
            alreadyPlayed = true;
        }
    }

}
