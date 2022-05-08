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

    // 리플레이 셋팅을 위한 것들
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

        /* 필요한 변수에 대한 설정 */
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
        if(songManager.IsSongStopped() && !isPlayerDead && !isSettingFinish) // 플레이어가 죽어서 게임이 끝난게 아니라면 
        {
            isSettingFinish = true;
            scoreSystem.SetTotalScoreOnScoreScene();
            resultPanel.SetActive(true);
            KillAllMobInScene();
        }
    }
    // SetGameFinishedSignal()
    // - 게임 종료를 위해 씬의 모든 몹을 없애고, Death 연출을 위한 카메라 셋팅을 하며, Result panel을 설정함
    public void SetGameFinishedSignal()
    {
        isPlayerDead = true;
        songManager.StopSong(); // 노래를 멈춰야 노트가 멈춤 

        if (player.transform.rotation.y != -90) // Death 애니메이션 재생을 위해 회전값 셋팅
        {
            player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, -90, player.transform.rotation.z);
        }

        KillAllMobInScene();
        SetDeathCameraZoom();
        StartCoroutine(SetResult());
    }

    // KillAllMobInScene()
    // - 모든 씬의 몬스터를 Dead state로 만듦
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
    // - 플레이어 Death 연출을 위한 카메라 줌인 기능
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
    // - 카메라 연출을 위한 Lerp FOV 기능
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

    /* 게임이 끝난 후 result panel 에서 표시될 버튼 이벤트 */
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
            //Debug.Log("@@@@@@@@@@ 보스 스테이지가 없어서 이동 안함! @@@@@@@@@@");
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
            //Debug.Log("노트 평균 속도 = " + averageNoteSpeed); // TODO : LOG
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
