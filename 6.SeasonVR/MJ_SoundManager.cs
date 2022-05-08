using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJ_SoundManager : MonoBehaviour {
    AudioSource[] audios;

    public static MJ_SoundManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public enum audioClips
    {
        siren,
        watchTowerMove,
        watchTowerSearch,
        whale,
        healing
    }

    void Start()
    {
        audios = GetComponents<AudioSource>();
    }

    public void PlayFadeOut()
    {
        audios[(int)audioClips.healing].Play();
        StartCoroutine(AudioFadeOut());
    }

    // 플레이어가 FarTrigger 를 지나갈 때,
    // 3가지 ( 사이렌소리 / 감시탑 움직이는 소리 / 감시탑 수색하는 소리 )
    // 사운드의 volume이 0 보다 크다면 (즉, 모두 0이 될 때 까지)
    // 점점 작아지도록 한다. 
    public IEnumerator AudioFadeOut()
    {
        while (audios[(int)audioClips.siren].volume > 0 || audios[(int)audioClips.watchTowerMove].volume > 0
            || audios[(int)audioClips.watchTowerSearch].volume > 0 || audios[(int)audioClips.healing].volume < 0.5)
        {
            for (int i = 0; i < audios.Length - 1; i++)
            {

                audios[i].volume -= 0.03f;
                audios[(int)audioClips.healing].volume += 0.01f;
                yield return new WaitForSeconds(0.7f);

            }
        }
        yield return null;
    }

    public void ReadyEnding()
    {
        StartCoroutine("PlayEnding");
    }

    public IEnumerator PlayEnding()
    {
        while (audios[(int)audioClips.healing].volume > 0)
        {
            audios[(int)audioClips.healing].volume -= 0.03f;
            yield return new WaitForSeconds(0.7f);
        }
        yield return null;
    }


    // 외부스크립트에서 접근해서 원하는 오디오 클립의 볼륨을 제어할 수 있다.
    public void VolumeControl(int clipIndex, float volumeValue)
    {
        audios[clipIndex].volume = volumeValue;
    }
}
