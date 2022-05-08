// System
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public GameObject go_nickname_input = null;
    public InputField input_new_nickname = null;
    public Button btn_submit_nickname = null;

    private void Awake()
    {
        btn_submit_nickname.onClick.AddListener(() =>
        {
            LoadingCanvas.Show();
            LeaderBoard.Register(input_new_nickname.text.Trim(), (success) =>
            {
                LoadingCanvas.Hide();
                if (success)
                {
                    go_nickname_input.SetActive(false);
                }
                else
                {
                    Debug.Log($"Nickname already exists");
                }
            });
        });
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        LoadingCanvas.Show();
        LeaderBoard.IsUserAuthorized((success, nickname) =>
        {
            LoadingCanvas.Hide();

            if (success)
            {
                Debug.Log($"success. nickname : {nickname}");
            }
            else
            {
                go_nickname_input.gameObject.SetActive(true);
            }
        });

        Fade.In(0.5f);
    }


    public void OnClickSelectModeBtn()
    {
        Fade.Out(0.5f, () => { UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName._02_ModeSelect); });
    }
     
    public void Btn_Quit()
    {
        ExitGame();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
