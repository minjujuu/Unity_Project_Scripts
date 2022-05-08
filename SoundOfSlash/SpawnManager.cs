using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RhythmGameStarter;

using static MonsterPatternGenerator;

public class SpawnManager : MonoBehaviour
{
    public const int MOB_SKEL = 0;
    public const int MOB_GOLEM = 1;
    private const int COMBOMONSET_NAME_LENGTH = 11;
    
    public Transform spawnPointsParent;
    public GameObject prefab_skel_hp1;
    public GameObject prefab_skel_hp2;
    public GameObject prefab_crownSkel_hp2;
    public GameObject prefab_golem_hp3;
    public GameObject prefab_golem_hp4;
    public GameObject prefab_golem_hp5;
    private GameObject[] monsterPool_skel_hp1;
    private GameObject[] monsterPool_skel_hp2;
    private GameObject[] monsterPool_crownSkel_hp2;
    private GameObject[] monsterPool_golem_hp3;
    private GameObject[] monsterPool_golem_hp4;
    private GameObject[] monsterPool_golem_hp5;
    private TrackManager trackManager; // ������� ������ Ʈ�� �Ŵ��� 
    private GameManager gameManager;
    private Transform[] spawnPoints;
    private GameObject mon;
    private GameObject monsterPoolParent;
    private Transform player;
    private Queue<int> patternQueue;   
    private Note curNote;
    private Note comboNote;
    private Vector3 spawnPosition;
    private Vector3 comboMonSetLocalPos;
    private Vector3 comboMonDefaultLocalPos;
    public int[] monsterPatternArray;
    private int[] comboControlArray;
    private int ccIdx = 0;
    private int prev_PatternNumber = 0;
    private int num_Monster1;
    private int num_Monster2;
    private int num_crownSkel;
    private int num_Monster3;
    private int num_Monster4;
    private int num_Monster5;
    private int flag;
    private int mpIdx;
    private int tmpComboCount = 0;
    private int monstersNum;
    private int count_comboNote = 0;
    private float size_skeleton = 1.2f;
    private float size_crownSkeleton = 1.4f;
    private float size_golem = 2.0f;
    private float monLocalPosWithNote = 2f; // ���Ϳ� ��Ʈ�� Ÿ�̹��� ��ġ�ϰ� ����� �� 
    private float shiftValueFromNote; // ���Ϳ� ��Ʈ Ÿ�̹��� ��߳��� ����� ��(Ŭ ���� �� ��߳�)
    private float shiftValueFromComboNote;
    private float mobSizeAdjustVal_Skel = 1.5f;
    private float mobSizeAdjustVal_Golem = 2f;  
    private bool isComboPlaying = false;

    // Test
    public List<GameObject> comboMonSetList;
    public Queue<GameObject> comboMonSetQueue;
    private void Awake()
    {
        comboMonSetList = new List<GameObject>();
        comboMonSetQueue = new Queue<GameObject>();
        monsterPoolParent = new GameObject("MonsterPoolParent");
        trackManager = FindObjectOfType<TrackManager>();
        gameManager = FindObjectOfType<GameManager>();

        player = GameObject.Find("Player").transform;
        
    }

    void Start()
    {
        monsterPatternArray = gameManager.GetMonsterPattern().GetComponent<MonsterPattern>().monsterPatternArray;
        patternQueue = new Queue<int>();
        for (int i = 0; i < monsterPatternArray.Length; i++)
        {
            patternQueue.Enqueue(monsterPatternArray[i]);
        }
        flag = 1;

        size_skeleton = 1.2f;
        size_crownSkeleton = 1.4f;
        size_golem = 2.0f;

        num_Monster1 = 150;
        num_Monster2 = 150;
        num_crownSkel = 150;
        num_Monster3 = 100;
        num_Monster4 = 50;
        num_Monster5 = 50;

        monLocalPosWithNote = 2;
        shiftValueFromNote = 7;
        shiftValueFromComboNote = 10;

        spawnPosition = new Vector3(0, monLocalPosWithNote, -7.0f);
        comboMonSetLocalPos = new Vector3(-1.35f, shiftValueFromComboNote, -7.0f);
        comboMonDefaultLocalPos = new Vector3(0, 0, 0);

        spawnPoints = new Transform[2];
        spawnPoints[0] = spawnPointsParent.GetChild(0).transform;
        spawnPoints[1] = spawnPointsParent.GetChild(1).transform;

        monsterPool_skel_hp1 = new GameObject[num_Monster1];
        monsterPool_skel_hp2 = new GameObject[num_Monster2];
        monsterPool_crownSkel_hp2 = new GameObject[num_crownSkel];
        monsterPool_golem_hp3 = new GameObject[num_Monster3];
        monsterPool_golem_hp4 = new GameObject[num_Monster4];
        monsterPool_golem_hp5 = new GameObject[num_Monster5];

        SetMonsterPool(monsterPool_skel_hp1, num_Monster1, prefab_skel_hp1);
        SetMonsterPool(monsterPool_skel_hp2, num_Monster2, prefab_skel_hp2);
        SetMonsterPool(monsterPool_crownSkel_hp2, num_crownSkel, prefab_crownSkel_hp2);
        SetMonsterPool(monsterPool_golem_hp3, num_Monster3, prefab_golem_hp3);
        SetMonsterPool(monsterPool_golem_hp4, num_Monster4, prefab_golem_hp4);
        SetMonsterPool(monsterPool_golem_hp5, num_Monster5, prefab_golem_hp5);

        comboControlArray = new int[50];
        for (int i = 0; i < 50; i++)
        {
            comboControlArray[i] = -1;
        }

    }

    void SetMonsterPool(GameObject[] monsterPool, int num_Monster, GameObject prefab_Monster)
    {
        for (int i = 0; i < num_Monster; i++)
        {
            monsterPool[i] = (GameObject)Instantiate(prefab_Monster);
            monsterPool[i].name = "Monster";
            monsterPool[i].SetActive(false);
            monsterPool[i].transform.SetParent(monsterPoolParent.transform);
        }
    }

    // ��Ʈ�� Ȱ��ȭ�� �� ȣ��� 
    // ��Ʈ �ϳ��� ���ε� Pattern�� ������ ������ ���̱� ������ 
    // - �޺��� ���� ������ (�� ���� Ʈ������ ������ ��Ʈ �� �ֿ� �ϳ��� pattern�� ���ε�)
    public void SpawnMonster(string trackName)
    {
        int pattern = monsterPatternArray[mpIdx];
        if (pattern >= NOTE_COMBO_01 || pattern == NO_NOTE_COMBO) 
        {
            count_comboNote++;
            if (count_comboNote == 2) 
            {
                count_comboNote = 0;
                mpIdx++;
            }
            comboNote = trackManager.GetComboNote(pattern);
            // ���� ������ �޺� ������ ��� 2�� Ʈ�������� �޺� ���͸� ������ 
            if (trackName == "1") 
                return;
        }
        else 
            mpIdx++;

        // �������Ͽ� ������ ���ڿ� ���� ��Ʈ�� ���͸� ������ �� 
        switch (pattern)
        {
            case NO_NOTE_GOLEM: // �� ���Ͽ��� EMPTY NOTE�� ��� 
                trackManager.GetCurNote(pattern).gameObject.layer = 0;
                break;
            case NO_NOTE_CROWN_SKELETON: // �հ��ذ�� ���Ͽ��� EMPTY NOTE�� ���
                trackManager.GetCurNote(pattern).gameObject.layer = 0;
                break;
            case NO_NOTE_COMBO: // �޺� ���Ͽ��� EMPTY NOTE�� ���
                if (isComboPlaying && comboControlArray[ccIdx] > tmpComboCount) // ���� ��Ÿ��Ʈ�� ������ �ʾ�����
                {
                    if (comboControlArray[ccIdx] - 1 == tmpComboCount) // ������ ��Ʈ�� layer ����
                    {
                        comboNote.gameObject.layer = 12; // �̹� �޺��� �������� �˸��� layer 12: FinalComboNote �Ҵ�
                        tmpComboCount = 0;
                        isComboPlaying = false; // �̹� �޺��� �������� �˸�. ���� �޺��� �ִٸ� ���� �����ϰ� �ִ� ccArray ���� ��ü

                        if (comboControlArray[ccIdx + 1] != -1)
                        {
                            ccIdx++;
                        }
                    }
                    else // ��Ÿ ��Ʈ�� layer ����
                    {   
                        comboNote.gameObject.layer = 11;
                        tmpComboCount++;
                    }
                }
                else
                {
                    trackManager.GetCurNote(pattern).gameObject.layer = 0;
                }
                break;
            case NOTE_SKELETON_L: // 0
            case NOTE_SKELETON_R: // 1
            case NOTE_SKELETON_U: // 2
            case NOTE_SKELETON_D: // 3
                curNote = trackManager.GetCurNote(pattern);
                curNote.gameObject.layer = 0;
                if (curNote.transform.Find("Monster") == null)
                {
                    foreach (GameObject obj in monsterPool_skel_hp1)
                    {
                        if (obj.activeSelf == false)
                        {
                            mon = obj;
                            break;
                        }
                    }

                    spawnPosition = new Vector3(0, shiftValueFromNote, -7.0f);
                    mon.transform.SetParent(curNote.transform);
                    mon.transform.localPosition = spawnPosition;
                    mon.transform.localScale = new Vector3(size_skeleton, size_skeleton, size_skeleton);
                    mon.SetActive(true);
                }
                break;

            case NOTE_CROWN_SKELETON:
                curNote = trackManager.GetCurNote(pattern);
                curNote.gameObject.layer = 0;
                if (curNote.transform.Find("Monster") == null)
                {
                    foreach (GameObject obj in monsterPool_crownSkel_hp2)
                    {
                        if (obj.activeSelf == false)
                        {
                            mon = obj;
                            break;
                        }
                    }

                    spawnPosition = new Vector3(0, shiftValueFromNote, -7.0f);
                    mon.transform.SetParent(curNote.transform);
                    mon.transform.localPosition = spawnPosition;
                    mon.transform.localScale = new Vector3(size_crownSkeleton, size_crownSkeleton, size_crownSkeleton);
                    mon.SetActive(true);
                }
                break;
            case NOTE_GOLEM: // 2 => 3���� ���� (��)
                curNote = trackManager.GetCurNote(pattern);
                curNote.gameObject.layer = 0;
                if (curNote.transform.Find("Monster") == null)
                {
                    foreach (GameObject obj in monsterPool_golem_hp3)
                    {
                        if (obj.activeSelf == false)
                        {
                            mon = obj;
                            break;
                        }
                    }
                    spawnPosition = new Vector3(0, shiftValueFromNote, -7.0f);
                    mon.transform.SetParent(curNote.transform);
                    mon.transform.localPosition = spawnPosition;
                    mon.transform.localScale = new Vector3(size_golem, size_golem, size_golem);
                    mon.SetActive(true);
                }

                break;
            case NOTE_COMBO_01: // �޺��� 1���� �ݺ��� ��
                //Debug.Log("�޺� 1��Ÿ");
                if (!isComboPlaying)
                {
                    isComboPlaying = true;
                    if (comboControlArray[ccIdx] == -1)
                    {
                        comboControlArray[ccIdx] = 1;
                    }
                    else
                    {
                        comboControlArray[ccIdx + 1] = 1;
                        ccIdx++;
                    }
                }
                else
                {
                    // ���� �޺��� �������̸�
                    comboControlArray[ccIdx + 1] = 1;
                }
                comboNote.gameObject.layer = 13;
    
                isComboPlaying = false;
                MakeComboMonsterSet(NOTE_COMBO_01, comboNote);

                break;
            case NOTE_COMBO_02: // �޺��� 2���� �ݺ��� ��

                if (!isComboPlaying)
                {
                    isComboPlaying = true;
                    if (comboControlArray[ccIdx] == -1)
                    {
                        comboControlArray[ccIdx] = 2;
                    }
                    else
                    {
                        comboControlArray[ccIdx + 1] = 2;
                        ccIdx++;
                    }
                }
                else
                {
                    // ���� �޺��� �������̸�
                    comboControlArray[ccIdx + 1] = 2;
                }

                comboNote.gameObject.layer = 11;
                tmpComboCount++;
                MakeComboMonsterSet(NOTE_COMBO_02, comboNote);
                break;
            case NOTE_COMBO_03: // �޺��� 3���� �ݺ��� �� 

                if (!isComboPlaying)
                {
                    isComboPlaying = true;
                    if (comboControlArray[ccIdx] == -1)
                    {
                        comboControlArray[ccIdx] = 3;
                    }
                    else // �̹� ĭ�� ������� ������ ���� ĭ�� ��
                    {
                        comboControlArray[ccIdx + 1] = 3;
                        ccIdx++;
                    }
                }
                else
                {
                    // ���� �޺��� �������̸�
                    comboControlArray[ccIdx + 1] = 3;
                }
                //Debug.Log("layer = 11");
                comboNote.gameObject.layer = 11;
                tmpComboCount++;
                MakeComboMonsterSet(NOTE_COMBO_03, comboNote);
                break;
            case NOTE_COMBO_04: // �޺��� 4���� �ݺ��� ��
                //Debug.Log("�޺� 4��Ÿ");
                if (!isComboPlaying)
                {
                    isComboPlaying = true;
                    if (comboControlArray[ccIdx] == -1)
                    {
                        comboControlArray[ccIdx] = 4;
                    }
                    else // �̹� ĭ�� ������� ������ ���� ĭ�� ��
                    {
                        comboControlArray[ccIdx + 1] = 4;
                        ccIdx++;
                    }
                }
                else
                {
                    // ���� �޺��� �������̸�
                    comboControlArray[ccIdx + 1] = 4;
                }
                //Debug.Log("layer = 11");
                comboNote.gameObject.layer = 11;
                tmpComboCount++;
                MakeComboMonsterSet(NOTE_COMBO_04, comboNote);
                break;
            case NOTE_COMBO_05: // �޺��� 5���� �ݺ��� ��
                //Debug.Log("�޺� 5��Ÿ");
                if (!isComboPlaying)
                {
                    isComboPlaying = true;
                    if (comboControlArray[ccIdx] == -1)
                    {
                        comboControlArray[ccIdx] = 5;
                    }
                    else // �̹� ĭ�� ������� ������ ���� ĭ�� ��
                    {
                        comboControlArray[ccIdx + 1] = 5;
                        ccIdx++;
                    }
                }
                else
                {
                    // ���� �޺��� �������̸�
                    comboControlArray[ccIdx + 1] = 5;
                }
                //Debug.Log("layer = 11");
                comboNote.gameObject.layer = 11;
                tmpComboCount++;
                MakeComboMonsterSet(NOTE_COMBO_05, comboNote);
                break;
        }    
    }


    void MakeComboMonsterSet(int opNum, Note comboNote)
    {
        monstersNum = Random.Range(6, 12); // ���� ����

        FindObjectOfType<PlayerCombo>().SetComboOptionSignal(opNum); // �÷��̾�� �޺� �ɼ��� ������ �˸�

        switch (opNum)
        {
            case NOTE_COMBO_01: // �ɼ��� 3(1��Ÿ)�̸� �� ���� Ÿ������ ���� �� �ֵ��� hp1�� ���� ����
                SetComboMonDependOnHP(comboNote, monstersNum, 0, 0, 0, 0);
                break;
            case NOTE_COMBO_02: // �ɼ��� 4(2��Ÿ)�̸� hp�� 1-2�� ���ͷ� ���� 
                SetComboMonDependOnHP(comboNote, monstersNum / 3, monstersNum / 3, 0, 0, 0);
                break;
            case NOTE_COMBO_03: // �ɼ��� 5(3��Ÿ)�̸� hp�� 1-3�� ���ͷ� ����
                SetComboMonDependOnHP(comboNote, monstersNum / 3, monstersNum / 3, monstersNum / 3, 0, 0);
                break;
            case NOTE_COMBO_04: // �ɼ��� 6(4��Ÿ)�̸� hp�� 1-4�� ���ͷ� ����
                SetComboMonDependOnHP(comboNote, monstersNum / 4, monstersNum / 4, monstersNum / 4, monstersNum / 4, 0);
                break;
            case NOTE_COMBO_05: // �ɼ��� 7(5��Ÿ)�̸�, hp�� 1-5�� ���ͷ� ����
                SetComboMonDependOnHP(comboNote, monstersNum / 5, monstersNum / 5, monstersNum / 5, monstersNum / 5, monstersNum / 5);
                break;
        }
        flag++;
    }

    void SetComboMonDependOnHP(Note comboNote, int skeleton1Num, int skeleton2Num, int golem1Num, int golem2Num, int golem3Num)
    {
        // �÷��̾� �������� �������� ��������Ʈ�� ��ġ�� �������� 
        spawnPointsParent.position = player.position;

        if (GameObject.Find("ComboMonSet" + flag.ToString()) == null)
        {
            GameObject monsterSet = new GameObject("ComboMonSet" + flag.ToString());
            monsterSet.tag = "ComboMonSet";

            monsterSet.transform.SetParent(comboNote.transform); /* T */
            monsterSet.transform.localPosition = comboMonSetLocalPos; /* T */

            for (int i = 0; i < skeleton1Num; i++)
            {
                foreach (GameObject obj in monsterPool_skel_hp1)
                {
                    if (obj.activeSelf == false)
                    {
                        mon = obj;
                        break;
                    }
                }
                SetComboMonsterProperty(mon, monsterSet, size_skeleton / mobSizeAdjustVal_Skel, MOB_SKEL);
            }

            for (int i = 0; i < skeleton2Num; i++)
            {
                foreach (GameObject obj in monsterPool_skel_hp2)
                {
                    if (obj.activeSelf == false)
                    {
                        mon = obj;
                        break;
                    }
                }
                SetComboMonsterProperty(mon, monsterSet, size_skeleton / mobSizeAdjustVal_Skel, MOB_SKEL);                                                              
            }

            for (int i = 0; i < golem1Num; i++)
            {
                foreach (GameObject obj in monsterPool_golem_hp3)
                {
                    if (obj.activeSelf == false)
                    {
                        mon = obj;
                        break;
                    }
                }
                SetComboMonsterProperty(mon, monsterSet, size_golem / mobSizeAdjustVal_Golem, MOB_GOLEM);                                                               
            }

            for (int i = 0; i < golem2Num; i++)
            {
                foreach (GameObject obj in monsterPool_golem_hp4)
                {
                    if (obj.activeSelf == false)
                    {
                        mon = obj;
                        break;
                    }
                }
                SetComboMonsterProperty(mon, monsterSet, size_golem / mobSizeAdjustVal_Golem, MOB_GOLEM);
            }

            for (int i = 0; i < golem3Num; i++)
            {
                foreach (GameObject obj in monsterPool_golem_hp5)
                {
                    if (obj.activeSelf == false)
                    {
                        mon = obj;
                        break;
                    }
                }
                SetComboMonsterProperty(mon, monsterSet, size_golem / mobSizeAdjustVal_Golem, MOB_GOLEM);
            }
            comboMonSetQueue.Enqueue(monsterSet);
            comboMonSetList.Add(monsterSet);
        }
    }

 

    void SetComboMonsterProperty(GameObject mon, GameObject monsterSet, float mobSize, int mobKinds)
    {
        mon.transform.localScale = new Vector3(mobSize, mobSize, mobSize); // ���� ���� �ذ���̶� ũ�Ⱑ �� �ٸ� 
        if(mobKinds == MOB_SKEL)
            mon.GetComponent<Mob_Skeleton>().SetComboMon();
        else
            mon.GetComponent<Mob_Golem>().SetComboMon();

        mon.SetActive(true);
        mon.transform.SetParent(monsterSet.transform);
        mon.transform.localPosition = comboMonDefaultLocalPos; /* TEST */   
        mon.transform.Translate(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))); // �ణ �����̷� ���;� ��
        //mon.transform.localPosition = spawnPoints[spawnPNum].position; // ���� ��ġ�� spawn point
        //mon.GetComponent<Mob_Golem>().Disable_SkinnedMeshRenderers()
    }

    public void InitForReplay()
    {
        mpIdx = 0;
        flag = 1; //comboTimes�� ���ڸ� ���߱� ���� 1�� �ʱ�ȭ
    }


    public void DequeuePatternQueue()
    {  
        if(patternQueue.Count > 0)
        {
            prev_PatternNumber = patternQueue.Peek();
            patternQueue.Dequeue();
        }       
    }

    public int GetPreviousPatternNumber()
    {
        return prev_PatternNumber;
    }

    public int GetNextPatternNumber()
    {
        if (patternQueue.Count > 0)

            return patternQueue.Peek();
        else
            return 0;
    }

    
    public GameObject GetComboMonSet()
    {
        if(comboMonSetQueue.Count > 0)
        {
            return comboMonSetQueue.Dequeue();
        }
        //if(idx < comboMonSetList.Count)
        //    return comboMonSetList[idx];
        return null;
    }


    public int GetComboMonSetPeekNumber()
    {
        int num = -1;
        if(comboMonSetQueue.Count > 0)
        {
            string tmp = comboMonSetQueue.Peek().name;
            num = int.Parse(tmp.Substring(COMBOMONSET_NAME_LENGTH, tmp.Length - COMBOMONSET_NAME_LENGTH));
        }

        return num;
    }
}

