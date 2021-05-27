﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPlayer : MonoBehaviour {

    void Update()
    {
        Vector3 dir = transform.position - Camera.main.transform.position;
        transform.forward = dir.normalized;
    }
}
