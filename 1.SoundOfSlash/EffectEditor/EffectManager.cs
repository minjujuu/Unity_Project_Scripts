using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public int effectPoolSize = 5;

    // 여러 이펙트가 동시에 사용될 가능성이 빈번한 Attack, Dash만 pool을 만들어서 사용함
    // Combo 이펙트는 풀로 만들지 않음 

    /* Attack */
    public GameObject prefab_fX_attack_1;
    public GameObject prefab_fX_attack_2;
    public GameObject prefab_fX_attack_3;

    /* Dash */
    public GameObject prefab_fX_dash_1; // 왼쪽 대쉬
    public GameObject prefab_fX_dash_2; // 오른쪽 대쉬

    /* Combo */
    public GameObject prefab_fX_combo_1;
    public GameObject prefab_fX_combo_2;
    public GameObject prefab_fX_combo_3;
    public GameObject prefab_fX_combo_4;
    public GameObject prefab_fX_combo_5;

    /* Monster Hit */
    public GameObject prefab_fX_mob_hit;

    /* Effect pools */
    private GameObject[] pool_fX_attack_1;
    private GameObject[] pool_fX_attack_2;
    private GameObject[] pool_fX_attack_3;
    private GameObject[] pool_fX_dash_1;
    private GameObject[] pool_fX_dash_2;
    private GameObject[] pool_fX_mon_hit;
    private GameObject effectPoolParent;
    private GameObject fX_combo_1, fX_combo_2, fX_combo_3, fX_combo_4, fX_combo_5;
    

    private GameObject player;
    private GameObject vfx_attack;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        if(player == null)
        {
            player = GameObject.FindObjectOfType<PlayerCombo>().gameObject;
        }
        pool_fX_attack_1 = new GameObject[effectPoolSize];
        pool_fX_attack_2 = new GameObject[effectPoolSize];
        pool_fX_attack_3 = new GameObject[effectPoolSize];
        pool_fX_dash_1 = new GameObject[effectPoolSize]; 
        pool_fX_dash_2 = new GameObject[effectPoolSize];
        pool_fX_mon_hit = new GameObject[effectPoolSize];

        effectPoolParent = new GameObject("EffectPoolParent");

        SetEffectPool(pool_fX_attack_1, prefab_fX_attack_1);
        SetEffectPool(pool_fX_attack_2, prefab_fX_attack_2);
        SetEffectPool(pool_fX_attack_3, prefab_fX_attack_3);
        SetEffectPool(pool_fX_dash_1, prefab_fX_dash_1);
        SetEffectPool(pool_fX_dash_2, prefab_fX_dash_2);
        SetEffectPool(pool_fX_mon_hit, prefab_fX_mob_hit);
        SetComboEffect();
    }
    
    void SetComboEffect()
    {
        // 콤보 이펙트는 플레이어 위치에서 재생되도록 포지션 설정되어 있음 
        fX_combo_1 = Instantiate(prefab_fX_combo_1);
        fX_combo_1.transform.SetParent(player.transform);
        fX_combo_1.transform.localPosition = new Vector3(0, 0, 0);

        fX_combo_2 = Instantiate(prefab_fX_combo_2);
        fX_combo_2.transform.SetParent(player.transform);
        fX_combo_2.transform.localPosition = new Vector3(0, 0, 0);

        fX_combo_3 = Instantiate(prefab_fX_combo_3);
        fX_combo_3.transform.SetParent(player.transform);
        fX_combo_3.transform.localPosition = new Vector3(0, 0, 0);

        fX_combo_4 = Instantiate(prefab_fX_combo_4);
        fX_combo_4.transform.SetParent(player.transform);
        fX_combo_4.transform.localPosition = new Vector3(0, 0, 0);

        fX_combo_5 = Instantiate(prefab_fX_combo_5);
        fX_combo_5.transform.SetParent(player.transform);
        fX_combo_5.transform.localPosition = new Vector3(0, 0, 0);
    }

    void SetEffectPool(GameObject[] effectPool, GameObject prefab_effect)
    {
        for (int i = 0; i < effectPoolSize; i++)
        {
            effectPool[i] = (GameObject)Instantiate(prefab_effect);
            effectPool[i].SetActive(false);
            effectPool[i].transform.SetParent(effectPoolParent.transform);
        }
    }

    
    // vfxNum - 1: attack01, 2: attack02, 3: attack03
    public void ShowAttackEffect(int vfxNum, Vector3 playPos, int dir)
    {
        switch (vfxNum)
        {
            case 1:
                foreach (GameObject obj in pool_fX_attack_1)
                {
                    if (obj.activeSelf == false)
                    {
                        vfx_attack = obj;
                        break;
                    }
                }
                vfx_attack.transform.localPosition = playPos;
                vfx_attack.SetActive(true);
                break;
            case 2:
                foreach (GameObject obj in pool_fX_attack_2)
                {
                    if (obj.activeSelf == false)
                    {
                        vfx_attack = obj;
                        break;
                    }
                }
                vfx_attack.transform.localPosition = playPos;
                vfx_attack.SetActive(true);
                break;
            case 3:
                foreach (GameObject obj in pool_fX_attack_3)
                {
                    if (obj.activeSelf == false)
                    {
                        vfx_attack = obj;
                        break;
                    }
                }
                vfx_attack.transform.localPosition = playPos;
                if(dir == 0)
                {
                    vfx_attack.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                }
                else
                {
                    vfx_attack.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                }
                vfx_attack.SetActive(true);
                break;

        }
        vfx_attack.GetComponent<ParticleSystem>().Play();
        StartCoroutine(PushEffectPool(vfx_attack));
    }


    // vfxNum - 1: dash(left), 2: dash(right)
    GameObject vtx_dash;
    public void ShowDashEffect(int vfxNum, Vector3 playPos)
    {
        switch(vfxNum)
        {
            case 1:
                foreach (GameObject obj in pool_fX_dash_1) // 왼쪽, 오른쪽
                {
                    if (obj.activeSelf == false)
                    {
                        vtx_dash = obj;
                        break;
                    }
                }
                vtx_dash.transform.localPosition = playPos;
                vtx_dash.SetActive(true);
                break;
            case 2:
                foreach (GameObject obj in pool_fX_dash_2)
                {
                    if (obj.activeSelf == false)
                    {
                        vtx_dash = obj;
                        break;
                    }
                }
                vtx_dash.transform.localPosition = playPos;
                vtx_dash.SetActive(true);
                break;
        }
        vtx_dash.GetComponent<ParticleSystem>().Play();
        StartCoroutine(PushEffectPool(vtx_dash));
    }
        
    // effNum - 1: combo01, 2: combo02, 3: combo03, 4: combo04, 5: combo05
    public void ShowComboEffect(int effNum, Vector3 playPos)
    {

        switch(effNum)
        {
            case 1:
                //fX_combo_1.transform.position = playPos;
                fX_combo_1.GetComponent<ParticleSystem>().Play();
                break;
            case 2:
                //fX_combo_2.transform.position = playPos;
                fX_combo_2.GetComponent<ParticleSystem>().Play();
                break;
            case 3:
                //fX_combo_3.transform.position = playPos;
                fX_combo_3.GetComponent<ParticleSystem>().Play();
                break;
            case 4:
                //fX_combo_4.transform.position = playPos;
                fX_combo_4.GetComponent<ParticleSystem>().Play();
                break;
            case 5:
                //fX_combo_5.transform.position = playPos;
                fX_combo_5.GetComponent<ParticleSystem>().Play();
                break;
        }
    }

    GameObject vfx_m_hit;
    public void ShowMobHitEffect(Vector3 playPos)
    {
        foreach (GameObject obj in pool_fX_mon_hit)
        {
            if (obj.activeSelf == false)
            {
                vfx_m_hit = obj;
                break;
            }
        }
        playPos += new Vector3(0, 0.7f, 0);
        vfx_m_hit.transform.localPosition = playPos;
        vfx_m_hit.SetActive(true);

        vfx_m_hit.GetComponent<ParticleSystem>().Play();
        StartCoroutine(PushEffectPool(vfx_m_hit));
    }

    IEnumerator PushEffectPool(GameObject eff)
    {
        yield return new WaitForSeconds(1.5f);
        eff.SetActive(false);
    }
}
