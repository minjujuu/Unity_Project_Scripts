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
    // ���� ������ ���� �Լ�
    public void SubScore()
    {
        if (score >= subScoreValue)
        {
            score -= subScoreValue;
            UpdateScoreDisplay();
        }
    }

    // ���� ������ ���� �Լ�
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

    // ���� ���� �� �� ��Ʈ�� �⺻ ���� ����
    void SetDefaultScoreBeforeGame()
    {
        // ���̵��� ���� �⺻ ������ �������� ���� ��������
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
        //(�� ��Ʈ��)  -> (�⺻ + (�⺻�ɼ�����x�ɼ�Ȱ������)) x difficulty)
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
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 1.5�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 25:
                defaultScorePerNote = backUPdefaultScore * 2f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 2�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 50:
                defaultScorePerNote = backUPdefaultScore * 3f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 3�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 75:
                defaultScorePerNote = backUPdefaultScore * 4f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 4�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 100:
                defaultScorePerNote = backUPdefaultScore * 5f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 5�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 150:
                defaultScorePerNote = backUPdefaultScore * 8f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 8�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 200:
                defaultScorePerNote = backUPdefaultScore * 16f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 16�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 300:
                defaultScorePerNote = backUPdefaultScore * 32f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 32�� �߽��ϴ�  -> " + defaultScorePerNote);
                break;
            case 400:
                defaultScorePerNote = backUPdefaultScore * 64f;
                Debug.Log("�޺� �� : " + statsSystem.GetCurCombo() + " | 64�� �߽��ϴ�  -> " + defaultScorePerNote);
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
        // ��ü ��Ʈ ���� �� perfect�� ���� ���
        perfectRatio = (statsSystem.levels.values[0].count / totalNoteCount) * 100;
        Debug.Log("�޺�: perfectRatio = " + perfectRatio);
        Debug.Log("�޺�: Perfect count = " + statsSystem.levels.values[0].count);
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
    public void SuperTimeAfterFever() // �����ð� ����
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
