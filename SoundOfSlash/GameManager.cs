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

    // SetSongItem(): Music selection Manager에서 호출하여, 현재 게임에 쓰일 songItem을 설정
    public void SetSongItem(SongItem si)
    {
        Debug.Log("GameManager - SongItem : " + si.name);
        curSongitem = si;
    }

    // SetMonsterPattern(): Music selection Manager에서 호출하여, 현재 게임에 쓰일 Monster pattern을 설정
    public void SetMonsterPattern(GameObject mp)
    {
        Debug.Log("GameManager - MonsterPattern : " + mp.name);
        monsterPatternInGm = mp.GetComponent<MonsterPattern>();
    }

    // GetTotalNoteCount(): Score System에서 호출하여, 전체 노트 개수를 파악하고 점수를 계산
    public int GetTotalNoteCount()
    {
        return curSongitem.notes.Count;
    }

    // GetMonsterPattern(): Spawn Manager에서 호출하여, Monster pattern에 따라 몬스터를 스폰
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
                Debug.Log("@@@@@@@@@@ 보스 스테이지가 없어서 이동 안함! @@@@@@@@@@");
                SceneManager.LoadScene(SceneName._02_ModeSelect);
            }
        }
    }
}
