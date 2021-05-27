using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Portal Manager
// 1. 플레이어가 각 행성에 있는 Portal 에 다가가면 그 행성 색깔에 맞는 Scene으로 이동한다.
// - Vivid 행성 Portal : NewVivid
// - Pastel 행성 Portal : NewPastel
// - Mono 행성 Portal : NewMono
public class PortalManager : MonoBehaviour {

    public GameObject ColorPalette;
    private void Awake()
    {
        DontDestroyOnLoad(ColorPalette);
        DontDestroyOnLoad(this);
    }

    // 플레이어가 들어간 Trigger의 태그에 따라서 어디로 이동할지가 정해진다.
    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            // 만약 태그가 VIVIDPORTAl 이면
            case "VIVIDPORTAL":
                // NewVivid 씬으로 이동
                SceneManager.LoadScene("NewVivid");
                break;
            // 만약 태그가 PASTELPORTAL 이면
            case "PASTELPORTAL":
                // NewPastel 씬으로 이동
                SceneManager.LoadScene("NewPastel");
                break;
            // 만약 태그가 MONOPORTAL 이면
            case "MONOPORTAL":
                // NewMono 씬으로 이동
                SceneManager.LoadScene("NewMono");
                break;
        }
    }
    
}
