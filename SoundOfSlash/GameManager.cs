using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using RhythmGameStarter;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public SongItem curSongitem;
    public MonsterPattern monsterPatternInGm;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // SetSongItem(): Music selection Manager���� ȣ���Ͽ�, ���� ���ӿ� ���� songItem�� ����
    public void SetSongItem(SongItem si)
    {
        Debug.Log("GameManager - SongItem : " + si.name);
        curSongitem = si;
    }

    // SetMonsterPattern(): Music selection Manager���� ȣ���Ͽ�, ���� ���ӿ� ���� Monster pattern�� ����
    public void SetMonsterPattern(GameObject mp)
    {
        Debug.Log("GameManager - MonsterPattern : " + mp.name);
        monsterPatternInGm = mp.GetComponent<MonsterPattern>();
    }

    // GetTotalNoteCount(): Score System���� ȣ���Ͽ�, ��ü ��Ʈ ������ �ľ��ϰ� ������ ���
    public int GetTotalNoteCount()
    {
        return curSongitem.notes.Count;
    }

    // GetMonsterPattern(): Spawn Manager���� ȣ���Ͽ�, Monster pattern�� ���� ���͸� ����
    public GameObject GetMonsterPattern()
    {
        return monsterPatternInGm.gameObject;
    }

    public void OnClickMusicSelectionButton()
    {
        if (GameData.gameMode == GameMode.Free)
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName._03_MusicSelection);
        else // GameMode.Stage
        {
            int currentDifficuly = (int)GameData.difficulty + 1;
            if (currentDifficuly <= (int)MusicDifficulty.Boss)
            {
                SceneManager.LoadScene(SceneName._03_MusicSelection);
                GameData.difficulty = (MusicDifficulty)currentDifficuly;
            }
            else
            {
                Debug.Log("@@@@@@@@@@ ���� ���������� ��� �̵� ����! @@@@@@@@@@");
                SceneManager.LoadScene(SceneName._02_ModeSelect);
            }
        }
    }
}
