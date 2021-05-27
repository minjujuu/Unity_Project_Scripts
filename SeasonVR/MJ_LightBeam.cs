using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_LightBeam : MonoBehaviour {

    Transform lightBeam;
    Vector3 originSize;
    public float kSizeAdjust = 2.0f;

	void Start () {
        lightBeam = GetComponent<Transform>();
        // lightbeam의 원래 사이즈 기억
        originSize = lightBeam.localScale;

	}
	
	void Update () {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        // 플레이어는 빼고
        int layer = LayerMask.NameToLayer("Player");
        layer = 1 << layer;

        if(Physics.Raycast(ray, out hitInfo, 100, ~layer))
        {
            //Vector3 temp = new Vector3(originSize.x, originSize.y, originSize.z * hitInfo.distance * kSizeAdjust);
            lightBeam.localScale = originSize * hitInfo.distance * kSizeAdjust;
        }
	}
}
