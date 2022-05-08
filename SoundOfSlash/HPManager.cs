using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RhythmGameStarter;

public class HPManager : MonoBehaviour
{
    public GameObject heartShape;

    private InGameManager inGameManager;
    private PlayerCombo player;
    private StatsSystem statsSystem;

    public int maxHP = 100;

    private int curHP;
    private int hpSubval = 5;
    private float superTime = 1f; // 무적 시간
    private float defaultSuperTime = 1f; // 기본 무적 시간
    private float superTimeAfterFever = 3f; // Fever Mode 직후 무적 시간
    private bool isOP = false;

    /* Main Frame의 Heart shape VFX 연출을 위한 변수들 */
    private RectTransform heartShape_wave;
    private float wave_max = 1f, wave_min = -10, wave_diff = 9;
    private RectTransform heartShape_alphaBlend;
    private float alphaBlend_max = 0, alphaBlend_min = -30, alphaBlend_diff = 30;
    private RectTransform heartShape_line;
    private float line_max = 0.56f, line_min = -25, line_diff = 25.5f;
    private RectTransform heartShape_Particle4;
    private float particle4_max = 0, particle4_min = -30, particle4_diff = 30;

    void Start()
    {
        inGameManager = Transform.FindObjectOfType<InGameManager>();
        player = Transform.FindObjectOfType<PlayerCombo>();
        statsSystem = Transform.FindObjectOfType<StatsSystem>();
        curHP = maxHP;

        /* HeartShape의 포지션을 초기화 */
        /* HeartShape의 Child component를 가져옴 */
        heartShape_wave = heartShape.transform.GetChild(0).GetComponent<RectTransform>();
        heartShape_alphaBlend = heartShape.transform.GetChild(1).GetComponent<RectTransform>();
        heartShape_line = heartShape.transform.GetChild(2).GetComponent<RectTransform>();
        heartShape_Particle4 = heartShape.transform.GetChild(3).GetComponent<RectTransform>();
        /* HeartShape과 component들의 position을 설정 */
        heartShape_wave.anchoredPosition = new Vector2(heartShape_wave.anchoredPosition.x, wave_max);
        heartShape_alphaBlend.anchoredPosition = new Vector2(heartShape_alphaBlend.anchoredPosition.x, alphaBlend_max);
        heartShape_line.anchoredPosition = new Vector2(heartShape_line.anchoredPosition.x, line_max);
        heartShape_Particle4.anchoredPosition = new Vector2(heartShape_Particle4.anchoredPosition.x, particle4_max);

    }

    void Update()
    {
        if(curHP < 0) // hp가 0보다 작으면
        {
            inGameManager.SetGameFinishedSignal();
            player.PlayerDead();
        }

        /* 10 Combo 마다 Hp를 조금씩 올려주는 시스템 */
        if (statsSystem.CanAddHeartPerTenCombo())
        {
            AddPlayerHP(1);
        }
    }

    // GetHpPercentage()
    // - Main Frame Manager에서 호출하여, 생명바의 Hp 퍼센트를 계산
    public float GetHpPercentage() => (float)curHP / maxHP;

    // SubPlayerHP()
    // - PlayerCombo에서 다음 상황일 때 호출하여, Hp를 깎고 콤보 리셋 처리함
    //  1) 노트를 놓친 경우
    //  2) 칠 수 있는 타이밍이 아닌데 친 경우
    public void SubPlayerHP()
    {
        if (isOP) return;
        else StartCoroutine(SetPlayeSuperTime()); /* 일정 시간 무적이 되게 함 */

        if (curHP >= 0)
        {
            curHP-= hpSubval;

            /* Hp가 줄어듦에 따라 heart shape의 물약의 양도 줄어들게 함 */
            heartShape_wave.anchoredPosition -= new Vector2(0, (wave_diff / (maxHP / hpSubval)));
            heartShape_alphaBlend.anchoredPosition -= new Vector2(0, (alphaBlend_diff / (maxHP / hpSubval)));
            heartShape_line.anchoredPosition -= new Vector2(0, (line_diff / (maxHP / hpSubval)));
            heartShape_Particle4.anchoredPosition -= new Vector2(0, (particle4_diff / (maxHP / hpSubval)));
             
            /* 이 경우 콤보가 리셋됨 */
            statsSystem.ResetCombo();
        }
    }

    // AddPlayerHP()
    // - 플레이어의 Hp를 올려주기 위해 호출됨
    // - 현재는 일정 콤보마다 추가 Hp를 누적해주는 시스템에 사용됨
    private void AddPlayerHP(int hpVal)
    {
        curHP += hpVal;
    }


    // SetSuperAfterFeverMode()
    // - Fever Manager에서 호출하여, Fever mode 직후에는 무적시간을 조금 더 길게 가지도록 설정함
    public void SetSuperAfterFeverMode()
    {
        superTime = superTimeAfterFever;
    }

    // SetPlayerOverPower()
    // - Hp가 깎인 직후, 피버모드 직후에는 일정 시간 무적이 되게 함
    private IEnumerator SetPlayeSuperTime()
    {
        isOP = true;
        yield return new WaitForSeconds(superTime);
        if (superTime == superTimeAfterFever)
        {
            superTime = defaultSuperTime; // 피버로 인해 늘어난 무적타임 원상복귀
        }
        isOP = false;    
    }

}
