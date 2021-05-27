using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_LightHouse : MonoBehaviour
{
    AreaLight laserLight;

    public GameObject lightSpriteParent;
    MeshRenderer[] mrs;
    public GameObject bulletPrefab;
    public ParticleSystem dieParticle;
    Animator towerAnim;
    Transform playerTr;
    Transform centerEyes;
    float fogValue;

    private void Start()
    {
        mrs = lightSpriteParent.GetComponentsInChildren<MeshRenderer>();
        towerAnim = GetComponentInParent<Animator>();
        laserLight = GetComponentInChildren<AreaLight>();
        dieParticle = GameObject.Find("DieParticle").GetComponent<ParticleSystem>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        centerEyes = GameObject.Find("CenterEyeAnchor").GetComponent<Transform>();
    }

    private void Update()
    {
        //Debug.DrawLine(transform.position, playerTr.position, new Color(1, 0, 0, 1));
    }


    // 감시탑 레이저에 플레이어가 걸리면,
    // 1. 레이저를 빨강색으로 변경하고, 애니메이션을 멈추고, 수색 소리를 키운다.
    // 2. 3초동안 플레이어가 레이저 안에 있다면 GameOver 한다.
    // 3. 3초 후에 플레이어가 없다면, 레이저를 다시 원래 색으로 변경하고 애니메이션을 다시 실행한다.

    float currentTime;
    float laderTime = 1.5f;
    private void OnTriggerStay(Collider coll)
    {
        // 플레이어라면 플레이어는 죽는다.
        // 플레이어가 죽었다는 UI를 띄울 것이다.
        if (coll.tag == "Player")
        {

            for(int i = 0; i < mrs.Length; i++)
            {
                mrs[i].material.SetColor("_TintColor", Color.red);
            }

            //mrs[0].material.SetColor("_TintColor", Color.red);
            //mrs[1].material.SetColor("_TintColor", Color.red);

            towerAnim.speed = 0; // 수색 애니메이션을 stop
            MJ_SoundManager.Instance.VolumeControl((int)MJ_SoundManager.audioClips.watchTowerSearch, 1.0f); // 수색사운드 볼륨 up

            // 부딪힌 게 플레이어라면,
            // 시간초를 잰다.
            currentTime += Time.deltaTime;
            //print(currentTime);
            //RaycastHit hitInfo;
            //int layer = 1 << 11;

            // 시간이 지난 후에도
            if (currentTime > laderTime)
            {
                if (coll.tag == "Player")
                {
                    // 플레이어를 향해 총알을 발사한다. 
                    GameObject bullet = Instantiate(bulletPrefab);
                    bullet.transform.position = transform.position;
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.GetComponent<Rigidbody>().AddForce(centerEyes.position - transform.position * 1000 * Time.deltaTime);

                    // GameOver
                    PlayOver();
                }
                else
                {
                    // 없으면
                    // 색깔을 원래대로 돌린다.
                    for (int i = 0; i < mrs.Length; i++)
                    {
                        mrs[i].material.SetColor("_TintColor", Color.white);
                    }

                    //mrs[0].material.SetColor("_TintColor", Color.white);
                    //mrs[1].material.SetColor("_TintColor", Color.white);

                    currentTime = 0;
                    towerAnim.speed = 1;
                    MJ_SoundManager.Instance.VolumeControl((int)MJ_SoundManager.audioClips.watchTowerSearch, 0.3f);
                }

                // 플레이어가 레이저 안에 있으면, 
                //if (Physics.Linecast(transform.position, playerTr.position, out hitInfo, ~layer))
                //{
                   
                    //Debug.DrawLine(transform.position, playerTr.position, Color.green);
                    //Debug.Log("부딪힌 것" + hitInfo.collider.gameObject.name, hitInfo.collider.gameObject);

                    //if (hitInfo.collider.gameObject.name.Contains("Player"))
                    //{
                    //    // 플레이어를 향해 총알을 발사한다. 
                    //    GameObject bullet = Instantiate(bulletPrefab);
                    //    bullet.transform.position = transform.position;
                    //    bullet.transform.rotation = Quaternion.identity;
                    //    bullet.GetComponent<Rigidbody>().AddForce(centerEyes.position - transform.position * 1000 * Time.deltaTime);

                    //    // GameOver
                    //    StartCoroutine(FlashLight());
                    //}
                    //else
                    //{
                    //    // 없으면
                    //    // 색깔을 원래대로 돌린다.
                    //    laserLight.m_Color = new Color(68, 193, 255);
                    //    currentTime = 0;
                    //}
                //}
            }
        }
        else
        {
            // 만약 플레이어가 아닌 것이
            // 감시탑의 레이저에 닿으면 사라진다.
            // 사라질 때 파티클이 생성된다.
            if (coll.tag == "Fish" || coll.tag == "Other")
            {
                dieParticle.transform.position = coll.transform.position;
                dieParticle.Stop();
                dieParticle.Play();
                Destroy(coll.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            for (int i = 0; i < mrs.Length; i++)
            {
                mrs[i].material.SetColor("_TintColor", Color.white);
            }
            //mrs[0].material.SetColor("_TintColor", Color.white);
            //mrs[1].material.SetColor("_TintColor", Color.white);
            currentTime = 0;
            towerAnim.speed = 1;
            MJ_SoundManager.Instance.VolumeControl((int)MJ_SoundManager.audioClips.watchTowerSearch, 0.3f);
        }
    }

    void PlayOver()
    {
        GameManager.Instance.GameOver();
        MJ_SoundManager.Instance.VolumeControl((int)MJ_SoundManager.audioClips.siren, 0.5f); // 사이렌 사운드 볼륨 up
    }



}
