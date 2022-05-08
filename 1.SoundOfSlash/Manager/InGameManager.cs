using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RhythmGameStarter;
using TMPro;

using LitJson;

public class InGameManager : MonoBehaviour
{
    private MonsterPatternGenerator mpGenerator;
    private GameManager gameManager;
    private SongManager songManager;
    private SpawnManager spawnManager;
    private ScoreManager scoreSystem;
    private PlayerCombo player;
    private Camera mainCamera;
    private Button pauseBtn;
    private Button stopBtn;
    public GameObject startMenu;
    public GameObject resultPanel;

    // ���÷��� ������ ���� �͵�
    public TextMeshProUGUI result_score;
    public Text ingame_score;

    private int callCount;
    private float mainCamera_FOV_death;
    private float lerpFOVval;
    private float deathAnimPlayTime;
    private float noteSpeed;
    private float averageNoteSpeed;
    private bool isGamePaused;
    private bool isPlayerDead;
    private bool isCompleted;
    private bool isSettingFinish;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        mpGenerator = FindObjectOfType<MonsterPatternGenerator>();
        //mpGenerator.targetSongItem = gameManager.curSongitem;
        //mpGenerator.toCopy = gameManager.monsterPatternInGm;
        //mpGenerator.Generate();
        Fade.In(0.5f);

        songManager = GameObject.FindObjectOfType<SongManager>();
        songManager.defaultSong = gameManager.curSongitem;

        spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        player = GameObject.FindObjectOfType<PlayerCombo>();
        scoreSystem = GameObject.FindObjectOfType<ScoreManager>();

        //pauseBtn = GameObject.Find("Pause").GetComponent<Button>();
        //stopBtn = GameObject.Find("Stop").GetComponent<Button>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        /* �ʿ��� ������ ���� ���� */
        mainCamera_FOV_death = 31;
        lerpFOVval = 0.4f;
        deathAnimPlayTime = 2.0f;
        noteSpeed = 4.0f;

        isPlayerDead = false;
        isGamePaused = false;
        isCompleted = false;
        isSettingFinish = false;
        //pauseBtn.interactable = false;
        //stopBtn.interactable = true;
        startMenu.SetActive(false);
        resultPanel.SetActive(false);

        if (songManager == null)
        {
            Debug.Log("songmanger null");
        }
        else
        {
            songManager.PlaySong();
        }
    }
 
    private void Update()
    {
        if(songManager.IsSongStopped() && !isPlayerDead && !isSettingFinish) // �÷��̾ �׾ ������ ������ �ƴ϶�� 
        {
            isSettingFinish = true;
            scoreSystem.SetTotalScoreOnScoreScene();
            resultPanel.SetActive(true);
            KillAllMobInScene();
        }
    }
    // SetGameFinishedSignal()
    // - ���� ���Ḧ ���� ���� ��� ���� ���ְ�, Death ������ ���� ī�޶� ������ �ϸ�, Result panel�� ������
    public void SetGameFinishedSignal()
    {
        isPlayerDead = true;
        songManager.StopSong(); // �뷡�� ����� ��Ʈ�� ���� 

        if (player.transform.rotation.y != -90) // Death �ִϸ��̼� ����� ���� ȸ���� ����
        {
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, -90, player.transform.rotation.z);
        }

        KillAllMobInScene();
        SetDeathCameraZoom();
        StartCoroutine(SetResult());
    }

    // KillAllMobInScene()
    // - ��� ���� ���͸� Dead state�� ����
    private void KillAllMobInScene()
    {
        foreach (Mob_Skeleton obj in FindObjectsOfType<Mob_Skeleton>())
        {
            obj.GoDeadState();
        }
        foreach (Mob_Golem obj in FindObjectsOfType<Mob_Golem>())
        {
            obj.GoDeadState();
        }
    }

    // SetDeathCameraZoom()
    // - �÷��̾� Death ������ ���� ī�޶� ���� ���
    private void SetDeathCameraZoom()
    {
        if (isCompleted)
        {
            return;
        }
        StartCoroutine(LerpFieldOfView());
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(14, 180, 0));
    }

    // LerpFieldOfView()
    // - ī�޶� ������ ���� Lerp FOV ���
    IEnumerator LerpFieldOfView()
    {
        isCompleted = true;
        while (mainCamera.fieldOfView >= mainCamera_FOV_death)
        {
            mainCamera.fieldOfView -= lerpFOVval;
            yield return null;
        }
    }

    IEnumerator SetResult()
    {
        yield return new WaitForSeconds(deathAnimPlayTime);

        resultPanel.SetActive(true);
        scoreSystem.SetTotalScoreOnScoreScene();
    }

    /* ������ ���� �� result panel ���� ǥ�õ� ��ư �̺�Ʈ */
    public void OnClickReplayButton()
    {
        Time.timeScale = 1;
        Fade.Out(0.5f, () => { LoadingManager.LoadScene(SceneName._04_GameScene); });

        //UnityEngine.SceneManagement.SceneManager.LoadScene(currentInGameSceneName);
    }

    public void OnClickMusicSelectionButton()
    {

        if (GameData.gameMode == GameMode.Free)
            Fade.Out(0.5f, () => { UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName._03_MusicSelection); });
        else // GameMode.Stage
        {
            int currentDifficuly = (int)GameData.difficulty + 1;
            if (currentDifficuly <= (int)MusicDifficulty.Boss)
            {
                Fade.Out(0.5f, () => { UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName._03_MusicSelection); });
                GameData.difficulty = (MusicDifficulty)currentDifficuly;
            }
            else
                Fade.Out(0.5f, () => { UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName._02_ModeSelect); });
                //Fade.Out(0.5f, () => { UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName._05_GameSceneBoss); });
            //Debug.Log("@@@@@@@@@@ ���� ���������� ��� �̵� ����! @@@@@@@@@@");
        }
    }

   
    /* On Click */
    public void OnClickPauseButton()
    {
        isGamePaused = true;
        IsPausedMonstersLife(true);
    }

    public void OnClickResumeButton()
    {
        IsPausedMonstersLife(false);
        isGamePaused = false;
    }

    public void OnClickStopButton()
    {
        isGamePaused = true;
        IsPausedMonstersLife(true);
        SetReplayState();
    }

    public void OnClickPlayOnStop()
    {
        SetReplayState();
        isGamePaused = false;

        //UnityEngine.SceneManagement.SceneManager.LoadScene(currentInGameSceneName);
        spawnManager.InitForReplay();
        player.Initialize();
    }

    public void SetReplayState()
    {
        //pauseBtn.gameObject.SetActive(true);
        //stopBtn.interactable = false;
        //pauseBtn.interactable = false;
        ingame_score.text = "0000";
        result_score.text = "0000";
    }


    public void CalNoteSpeed(float speed)
    {
        callCount++;
        if (callCount < 200)
        {
            noteSpeed += speed;
            averageNoteSpeed = Mathf.Round((noteSpeed / callCount) * 10) * 0.1f;
            //Debug.Log("��Ʈ ��� �ӵ� = " + averageNoteSpeed); // TODO : LOG
        }
    }

    public float GetNoteSpeed()
    {
        return averageNoteSpeed;
    }


    void IsPausedMonstersLife(bool tf)
    {
        foreach (Mob_Skeleton skel in GameObject.FindObjectsOfType<Mob_Skeleton>())
        {
            skel.SetIsGamePaused(tf);
        }

        foreach (Mob_Golem go in GameObject.FindObjectsOfType<Mob_Golem>())
        {
            go.SetIsGamePaused(tf);
        }
    }

  

}
