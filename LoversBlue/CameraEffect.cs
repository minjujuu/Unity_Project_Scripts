using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour {
    public CameraFilterPack_TV_WideScreenCircle screenCircle;

    private IEnumerator SceneFadeIn()
    {
        screenCircle.Size = 0;
        screenCircle.Smooth = 0.4f;
        float delay = 3f, m_time = 0.0f;
        float increaseValue = delay * 0.01f;
        while (screenCircle.Size <= 0.8f)
        {
            screenCircle.Size += 0.008f;
            screenCircle.Smooth -= 0.004f;
            m_time += increaseValue;
            yield return new WaitForSeconds(increaseValue);
        }
        //screenCircle.enabled = false;

        yield return null;

    }

    private void Start()
    {
        StartCoroutine(this.SceneFadeIn());
    }
}
