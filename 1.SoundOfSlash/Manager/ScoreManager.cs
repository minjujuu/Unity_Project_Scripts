using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using RhythmGameStarter;

using LitJson;

[Serializable] public class StringEvent : UnityEvent<string> { }
public class CollapsedEventAttribute : PropertyAttribute
{
    public bool visible;
    public string tooltip;

    public CollapsedEventAttribute(string tooltip = null)
    {
        this.tooltip = tooltip;
    }
}

public class ScoreManager : MonoBehaviour
{

    [CollapsedEvent]
    public StringEvent onScoreUpdate;

    StatsSystem statsSystem;
    public float score;
    GameManager gameManager;
    int totalNoteCount;

    public TextMeshProUGUI result_score;
    void Start()
    {
        score = 0;
        statsSystem = GameObject.FindObjectOfType<StatsSystem>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        SetDefaultScoreBeforeGame();

        totalNoteCount = gameManager.GetTotalNoteCount();
    }

    public float defaultScorePerNote = 100;

    void Update()
    {
        if (statsSystem.CanAddScore())
        {
            AddScore(defaultScorePerNote);
        }       
    }


    int subScoreValue = 100;
    // 점수 차감을 위한 함수
    public void SubScore()
    {
        if (score >= subScoreValue)
        {
            score -= subScoreValue;
            UpdateScoreDisplay();
        }
    }

    // 점수 증가를 위한 함수
    public void AddScore(float val)
    {
        SetDefaultScoreInGame();
        score += Mathf.RoundToInt(val);
        
        UpdateScoreDisplay();
    }

    float defaultOptionScore = 100;
    float activeOptionCount = 1;
    int difficultyMultiple = 2;
    float backUPdefaultScore;

    // 게임 시작 전 각 노트당 기본 점수 설정
    void SetDefaultScoreBeforeGame()
    {
        // 난이도에 따라 기본 점수에 곱해지는 수를 설정해줌
        switch (ModeData.difficulty)
        {
            case Difficulty.Key2:
                difficultyMultiple *= 1;
                break;
            case Difficulty.Key4:
                difficultyMultiple *= 2;
                break;
            case Difficulty.Key6:
                difficultyMultiple *= 4;
                break;
        }
        //(각 노트당)  -> (기본 + (기본옵션점수x옵션활성개수)) x difficulty)
        defaultScorePerNote = (defaultOptionScore + (defaultOptionScore * activeOptionCount)) * difficultyMultiple;
        backUPdefaultScore = defaultScorePerNote;
    }

    
  

    void SetDefaultScoreInGame()
    {
        if(statsSystem.GetCurCombo() < 10)
        {
            defaultScorePerNote = backUPdefaultScore;
        }
        switch(statsSystem.GetCurCombo())
        {
            case 10:
                defaultScorePerNote = backUPdefaultScore * 1.5f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 1.5배 했습니다  -> " + defaultScorePerNote);
                break;
            case 25:
                defaultScorePerNote = backUPdefaultScore * 2f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 2배 했습니다  -> " + defaultScorePerNote);
                break;
            case 50:
                defaultScorePerNote = backUPdefaultScore * 3f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 3배 했습니다  -> " + defaultScorePerNote);
                break;
            case 75:
                defaultScorePerNote = backUPdefaultScore * 4f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 4배 했습니다  -> " + defaultScorePerNote);
                break;
            case 100:
                defaultScorePerNote = backUPdefaultScore * 5f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 5배 했습니다  -> " + defaultScorePerNote);
                break;
            case 150:
                defaultScorePerNote = backUPdefaultScore * 8f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 8배 했습니다  -> " + defaultScorePerNote);
                break;
            case 200:
                defaultScorePerNote = backUPdefaultScore * 16f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 16배 했습니다  -> " + defaultScorePerNote);
                break;
            case 300:
                defaultScorePerNote = backUPdefaultScore * 32f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 32배 했습니다  -> " + defaultScorePerNote);
                break;
            case 400:
                defaultScorePerNote = backUPdefaultScore * 64f;
                Debug.Log("콤보 수 : " + statsSystem.GetCurCombo() + " | 64배 했습니다  -> " + defaultScorePerNote);
                break;
            case 500:
                defaultScorePerNote = backUPdefaultScore * 100f;
                break;
            case 750:
                defaultScorePerNote = backUPdefaultScore * 150f;
                break;
            case 1000:
                defaultScorePerNote = backUPdefaultScore * 200f;
                break;
        }


    }

    int perfectRatio = 0;

    public float SetTotalScoreOnScoreScene()
    {
        // 전체 노트 개수 중 perfect의 비율 계산
        perfectRatio = (statsSystem.levels.values[0].count / totalNoteCount) * 100;
        Debug.Log("콤보: perfectRatio = " + perfectRatio);
        Debug.Log("콤보: Perfect count = " + statsSystem.levels.values[0].count);
        if (perfectRatio >= 100)
            score *= 10;
        else if (perfectRatio >= 90)
            score *= 8;
        else if (perfectRatio >= 80)
            score *= 5;
        else if (perfectRatio >= 70)
            score *= 2;

        result_score.SetText(score.ToString());

        Temp();

        return score;
        
    }

    public void UpdateScoreDisplay()
    {
        onScoreUpdate.Invoke(score.ToString());
    }

    public void SetComboBeforeFever()
    {
        statsSystem.SetComboBeforeFever();
    }

    int superTime = 3;
    public void SuperTimeAfterFever() // 무적시간 존재
    {
        Invoke(nameof(SetComboAfterFever), superTime);
    }

    public void SetComboAfterFever()
    {
        statsSystem.SetComboAfterFever();
    }



    public GameObject leaderBoardPanel = null;
    public UnityEngine.UI.Text leaderBoardText = null;
    private void Temp()
    {
        leaderBoardText.text = "";
        leaderBoardPanel.SetActive(true);

        LoadingCanvas.Show();
        LeaderBoard.UpdateScore(gameManager.curSongitem.name, (int)score, (success) =>
        {
            LoadingCanvas.Hide();
            if (success)
            {
                Dictionary<string, string> getRankingData = new Dictionary<string, string>
                {
                    { "songName", gameManager.curSongitem.name }
                };
                UnityWebRequestor.GetRequest("getRanking", getRankingData, (success, result) =>
                {
                    if (success)
                    {
                        JsonData fullData = JsonMapper.ToObject(result);
                        JsonData data = fullData["data"];

                        string _result = "Public ranking : \n";
                        for (int i = 0; i < data.Count; i++)
                        {
                            string nickname = data[i]["nickname"].TryStringParse();
                            string score = data[i]["score"].TryStringParse();

                            _result += $"ranking: {i + 1}  |  nickname: {nickname}  |  score: {score}\n";
                        }

                        leaderBoardText.text += _result;
                    }
                    else
                    {
                        Debug.Log("getRanking fail");
                    }
                });



                Dictionary<string, string> getMyRankingData = new Dictionary<string, string>
                {
                    { "deviceId", SystemInfo.deviceUniqueIdentifier },
                    { "songName", gameManager.curSongitem.name }
                };
                UnityWebRequestor.GetRequest("getMyRanking", getMyRankingData, (success, result) =>
                {
                    if (success)
                    {
                        JsonData fullData = JsonMapper.ToObject(result);
                        int ranking = fullData["data"]["ranking"].TryIntParse();
                        string score = fullData["data"]["score"].TryStringParse();

                        leaderBoardText.text += $"ranking: {ranking}  |  score: {score}\n";
                    }
                    else
                    {
                        Debug.Log("getMyRanking fail");
                    }
                });
            }
            else
            {
                Debug.Log($"UpdateScore fail");
            }
        });
    }
}
