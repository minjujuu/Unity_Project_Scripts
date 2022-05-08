using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWin : MonoBehaviour {

    public CanvasRenderer successImage;

    public bool result = false;

    public static PlayerWin Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    void Start () {
        successImage.SetAlpha(0);
	}

    void Update()
    {
        // 다시 시작하려면 스페이스바를 누르세요.
        if(Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void Restart()
    {
        if (result == true)
        {
            SceneManager.LoadScene(0);
        }
       
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(MJ_ItemScript.Instance.itemList.Contains("Eyes") && MJ_ItemScript.Instance.itemList.Contains("Brow") 
                && MJ_ItemScript.Instance.itemList.Contains("Mouth") && MJ_ItemScript.Instance.itemList.Contains("Button")
                && MJ_ItemScript.Instance.itemList.Contains("Etc"))
            {
                successImage.SetAlpha(100);
                result = true;
                
            }
        }
    }
}
