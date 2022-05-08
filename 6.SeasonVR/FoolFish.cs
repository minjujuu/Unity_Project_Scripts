using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 바보물고기
// - 플레이어와 인터랙션한다.
// 1. Spawn 되자마자 플레이어를 찾아 다가간다.
// 2. 바보물고기는 플레이어에게 아주 가까이 다가가서 공격한다.
// 3. 바보물고기는 플레이어가 주먹으로 때리면 죽는다.


// - 물고기가 플레이어를 공격할 수 있는 거리
// - 바보물고기의 speed

public class FoolFish : MonoBehaviour {


    public ParticleSystem foolDieParticle;
    public float attackDis = 2.0f;
    public float moveSpeed = 2.0f;
    Transform playerTr;
    Animator foolAnim;

    void Start()
    {
        foolAnim = gameObject.GetComponent<Animator>();
        playerTr = Camera.main.transform;

    }

    void Update()
    {
        // 물고기를 이동하게 하자.
        // - 방향 구하기
        Vector3 dir = playerTr.position - transform.position;
        // - 이동하기
        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
        // - 바라보는 방향 
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 3 * Time.deltaTime);

        // 만약 플레이어와 물고기 사이의 거리가 attackDis보다 작으면 공격한다.
        if (Vector3.Distance(transform.position, playerTr.position) < attackDis)
        {
            foolAnim.SetTrigger("Attack");
            //현철추가
            GameManager.Instance.GameOver();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "PlayerHands")
        {
            ParticleSystem dieParticle = Instantiate(foolDieParticle);
            dieParticle.transform.position = collision.transform.position;
            dieParticle.Stop();
            dieParticle.Play();
            foolAnim.SetTrigger("Die");
            // 0702 죽으면 콜라이더 삭제
            // - Destroy 되기전에 플레이어와 닿아서 GameOver 되버리는 상황을 방지한다.
            GetComponent<CapsuleCollider>().enabled = false;
            Destroy(dieParticle, 4.0f);
            Destroy(gameObject, 5.0f);
        }
    }
}
