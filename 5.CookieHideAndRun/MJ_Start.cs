using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MJ_Start : MonoBehaviour {

	
    // 버튼 클릭 메서드
    public void ClickPlayBtn()
    {
        SceneManager.LoadScene(1);
    }
}
