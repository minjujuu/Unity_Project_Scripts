using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 배경음악 관리자
public class SoundManager : MonoBehaviour {

    private static SoundManager _instance = null;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SoundManager)) as SoundManager;

                if (_instance == null)
                {
                    Debug.LogError("SoundManager 오브젝트 비활성화 상태");
                }
            }
            return _instance;
        }
    }

    AudioSource audioS;
    public AudioClip[] sounds;

    enum bgms
    {
        Default,
        Pastel,
        Mono
    }

    void Start()
    {
        audioS = GetComponent<AudioSource>();
        audioS.clip = sounds[(int)bgms.Default];
        audioS.Play();
    }

    public void PlayMainSound()
    {
        StartCoroutine(AudioFadeOut(sounds[(int)bgms.Default]));
        //audioS.clip = sounds[(int)bgms.Default];
        //audioS.Play();
    }

    public void PlayPastelSound()
    {
        StartCoroutine(AudioFadeOut(sounds[(int)bgms.Pastel]));
        //audioS.clip = sounds[(int)bgms.Pastel];
        //audioS.Play();
    }

    public void PlayMonoSound()
    {
        StartCoroutine(AudioFadeOut(sounds[(int)bgms.Mono]));
        //audioS.clip = sounds[(int)bgms.Mono];
        //audioS.Play();
    }




    // 볼륨이 점점 작아져야 한다. 
    // 볼륨이 0이 될 때까지 0.05씩 0.3초 마다 감소한다. 
    IEnumerator AudioFadeOut(AudioClip clip)
    {
        while (audioS.volume > 0)
        {
            audioS.volume -= 0.05f;
            yield return new WaitForSeconds(0.3f);
        }
        audioS.clip = clip;
        audioS.volume = 1;
        audioS.Play();
        yield return null;
    }

}
