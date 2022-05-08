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
    private float superTime = 1f; // ���� �ð�
    private float defaultSuperTime = 1f; // �⺻ ���� �ð�
    private float superTimeAfterFever = 3f; // Fever Mode ���� ���� �ð�
    private bool isOP = false;

    /* Main Frame�� Heart shape VFX ������ ���� ������ */
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

        /* HeartShape�� �������� �ʱ�ȭ */
        /* HeartShape�� Child component�� ������ */
        heartShape_wave = heartShape.transform.GetChild(0).GetComponent<RectTransform>();
        heartShape_alphaBlend = heartShape.transform.GetChild(1).GetComponent<RectTransform>();
        heartShape_line = heartShape.transform.GetChild(2).GetComponent<RectTransform>();
        heartShape_Particle4 = heartShape.transform.GetChild(3).GetComponent<RectTransform>();
        /* HeartShape�� component���� position�� ���� */
        heartShape_wave.anchoredPosition = new Vector2(heartShape_wave.anchoredPosition.x, wave_max);
        heartShape_alphaBlend.anchoredPosition = new Vector2(heartShape_alphaBlend.anchoredPosition.x, alphaBlend_max);
        heartShape_line.anchoredPosition = new Vector2(heartShape_line.anchoredPosition.x, line_max);
        heartShape_Particle4.anchoredPosition = new Vector2(heartShape_Particle4.anchoredPosition.x, particle4_max);

    }

    void Update()
    {
        if(curHP < 0) // hp�� 0���� ������
        {
            inGameManager.SetGameFinishedSignal();
            player.PlayerDead();
        }

        /* 10 Combo ���� Hp�� ���ݾ� �÷��ִ� �ý��� */
        if (statsSystem.CanAddHeartPerTenCombo())
        {
            AddPlayerHP(1);
        }
    }

    // GetHpPercentage()
    // - Main Frame Manager���� ȣ���Ͽ�, ������� Hp �ۼ�Ʈ�� ���
    public float GetHpPercentage() => (float)curHP / maxHP;

    // SubPlayerHP()
    // - PlayerCombo���� ���� ��Ȳ�� �� ȣ���Ͽ�, Hp�� ��� �޺� ���� ó����
    //  1) ��Ʈ�� ��ģ ���
    //  2) ĥ �� �ִ� Ÿ�̹��� �ƴѵ� ģ ���
    public void SubPlayerHP()
    {
        if (isOP) return;
        else StartCoroutine(SetPlayeSuperTime()); /* ���� �ð� ������ �ǰ� �� */

        if (curHP >= 0)
        {
            curHP-= hpSubval;

            /* Hp�� �پ�꿡 ���� heart shape�� ������ �絵 �پ��� �� */
            heartShape_wave.anchoredPosition -= new Vector2(0, (wave_diff / (maxHP / hpSubval)));
            heartShape_alphaBlend.anchoredPosition -= new Vector2(0, (alphaBlend_diff / (maxHP / hpSubval)));
            heartShape_line.anchoredPosition -= new Vector2(0, (line_diff / (maxHP / hpSubval)));
            heartShape_Particle4.anchoredPosition -= new Vector2(0, (particle4_diff / (maxHP / hpSubval)));
             
            /* �� ��� �޺��� ���µ� */
            statsSystem.ResetCombo();
        }
    }

    // AddPlayerHP()
    // - �÷��̾��� Hp�� �÷��ֱ� ���� ȣ���
    // - ����� ���� �޺����� �߰� Hp�� �������ִ� �ý��ۿ� ����
    private void AddPlayerHP(int hpVal)
    {
        curHP += hpVal;
    }


    // SetSuperAfterFeverMode()
    // - Fever Manager���� ȣ���Ͽ�, Fever mode ���Ŀ��� �����ð��� ���� �� ��� �������� ������
    public void SetSuperAfterFeverMode()
    {
        superTime = superTimeAfterFever;
    }

    // SetPlayerOverPower()
    // - Hp�� ���� ����, �ǹ���� ���Ŀ��� ���� �ð� ������ �ǰ� ��
    private IEnumerator SetPlayeSuperTime()
    {
        isOP = true;
        yield return new WaitForSeconds(superTime);
        if (superTime == superTimeAfterFever)
        {
            superTime = defaultSuperTime; // �ǹ��� ���� �þ ����Ÿ�� ���󺹱�
        }
        isOP = false;    
    }

}
