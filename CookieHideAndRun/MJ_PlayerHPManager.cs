using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 두 명의 Enemy로부터 PlayerHP를 깎는 정보를 받아온다.
// - 깎을 때마다 HP Text UI를 변경한다.
// - 0이면 GameOver Text를 띄운다.
public class MJ_PlayerHPManager : MonoBehaviour {

    public static MJ_PlayerHPManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public int playerHP = 100;
    public int minusHpValue = 5;

    public CanvasRenderer gameOverImage;


    void Start()
    {
        gameOverImage.SetAlpha(0);
        playerHP = 100;
    }

    public void MinusPlayerHP()
    {
        playerHP -= minusHpValue;
        CheckZeroHP();
    }

    void CheckZeroHP()
    {
        if (playerHP == 0)
        {
            gameOverImage.SetAlpha(100);
            PlayerWin.Instance.result = true;
        }
    }
    
}
