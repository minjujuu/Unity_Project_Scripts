using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBlink : MonoBehaviour {

    Animator anim;

    public static DamageBlink Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayDamageBlinkTrue()
    {
        anim.SetTrigger("DamageBlink");
    }

}
