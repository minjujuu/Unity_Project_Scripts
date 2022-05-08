using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_LaserMuzzle : MonoBehaviour {

    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        StartCoroutine(this.ShowMuzzleFlash());
    }

    //MuzzleFlash 활성/비활성화를 짧은 시간동안 반복
    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash 스케일을 불규칙하게 변경
        float scale = Random.Range(0.03f, 0.05f);
        transform.localScale = Vector3.one * scale;

        //MuzzleFlash를 Z축을 기준으로 불규칙하게 회전시킴
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        sr.transform.localRotation = rot;

        //활성화해서 보이게 함
        //sr.enabled = true;

        //불규칙적인 시간 동안 Delay 한 다음 MeshRenderer를 비활성화
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        //비활성화해서 보이지 않게 함
        //sr.enabled = false;
    }
}
