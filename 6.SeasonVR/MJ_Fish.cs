using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_Fish : MonoBehaviour {

    // 1. 물고기의 기본은 Idle 
    // - 속도에 따라 Move 혹은 move2
    // 2. 플레이어의 손 콜라이더에 닿았을 때 No / Yes 중에 하나를 랜덤으로 
    // 3. 이동도 랜덤으로 x, y, z 

    private Animator anim;

    Transform playerTr;
    Transform seaCityTr;
    public float moveSpeed;
    Vector3 dir;
    bool isPunch = false;

    void Start () {
        
        anim = GetComponent<Animator>();
        playerTr = Camera.main.transform;

        // 랜덤 스피드 부여
        moveSpeed = Random.Range(moveSpeed, moveSpeed + 2.0f);
        //1. 랜덤의 방향으로 수영한다.
        dir = new Vector3(Random.Range(transform.position.x - 10, transform.position.x + 10),
            Random.Range(transform.position.y - 5, transform.position.y + 5),
            Random.Range(transform.position.z - 10, transform.position.z + 10));


        Invoke("DestroyFish", 30.0f);
    }

    // 물고기가 충격먹었니?
    bool isShock = false;
	void Update () {

        // 2. 이동
        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
        // - 바라보는 방향
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 3 * Time.deltaTime);
        
        // ====== 속도에 따른 animation clip ======
        // 스피드가 느리다면 move, 빠르다면 move2
        if (moveSpeed < (moveSpeed + 1.0f))
        {
            anim.SetTrigger("Move");
        }
        else
        {
            anim.SetTrigger("Move2");
        }

        if(!isPunch)
        {
            // 물고기의 주변에 플레이어가 있으면,
            // 물고기가 플레이어 주변에 한 번도 안갔으면, 충격을 먹지 않았으면, 플레이어 쪽으로 이동.
            if(Vector3.Distance(transform.position, playerTr.position) < 10.0f && !isShock)
            {
                // 플레이어의 방향으로 향한다.
                dir = playerTr.position - transform.position;
                // 2. 이동
                // - 플레이어의 방향 혹은 반대 방향.
                transform.position += dir.normalized * moveSpeed * Time.deltaTime;
                // - 바라보는 방향
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 3 * Time.deltaTime);

                
                
            }

            if(Vector3.Distance(transform.position, playerTr.position) > 10)
            {
                isShock = false;
            }

            // 플레이어와 더 가까워지면 
            // 1. 물고기가 플레이어에 반응하는 듯한 애니메이션을 실행한다. 
            // 2. 물고기가 플레이어를 피하는 듯이 방향을 반대방향으로 전환한다.
            if (Vector3.Distance(transform.position, playerTr.position) < 2.0f)
            {
                // 사람을 만난 물고기가 충격 먹었다.
                // 도망가자.
                isShock = true;
                int ran = Random.Range(0, 20);
                // 가까워지면 반대방향으로.
                dir = dir * -1;


                if (ran < 10)
                {
                    // No
                    anim.SetBool("No", true);
                }
                else if (ran < 20)
                {
                    // Yes
                    anim.SetBool("Yes", true);
                }
                // 도망가야 하니까 speed 가 일시적으로 늘어나도록.
                moveSpeed += 1.0f;
                Invoke("FalseAnim", 2.0f);
            }
            
        }
        // 플레이어의 주먹에 맞으면 반대방향으로 간다.
        else if (isPunch)
        {
            // 주먹에 맞은 후에는 No나 Yes로 상태가 넘어가지 않도록 false 처리
            anim.SetBool("No", false);
            anim.SetBool("Yes", false);

            // 맞으면 반대방향으로 .
            dir = playerTr.position - transform.position;
            dir *= -1;
            // 2. 이동
            transform.position += dir.normalized * moveSpeed * Time.deltaTime;
            // - 바라보는 방향
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 3 * Time.deltaTime);
            isPunch = false;
        }
    }

    private void FalseAnim()
    {
        anim.SetBool("No", false);
        anim.SetBool("Yes", false);
        moveSpeed -= 1.0f;
    }

    private void OnCollisionEnter(Collision coll)
    {
        // 만약 플레이어의 손에 닿았으면
        // No / Yes 중에 하나를 랜덤으로 재생한다.
        if(coll.gameObject.tag == "PlayerHands")
        {
            //print("coll=================");
            isPunch = true;
            int ran = Random.Range(0, 20);
            if(ran < 10)
            {
                // No
                anim.SetBool("No", true);
            }
            else if(ran < 20)
            {
                // Yes
                anim.SetBool("Yes", true);
            }
        }
    }

    void DestroyFish()
    {
        Destroy(gameObject);
    }
}
