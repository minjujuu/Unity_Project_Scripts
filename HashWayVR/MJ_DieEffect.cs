using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_DieEffect : MonoBehaviour {

    private void OnEnable()
    {
        Destroy(this.gameObject, 2.0f);
    }
}
