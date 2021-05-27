using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCamera : MonoBehaviour {

    public Camera sketchCamera;
    public Camera mainCamera;
    void ChoiceSketchCamera()
    {
        sketchCamera.depth = 1;
        mainCamera.depth = 0;
    }

    void ChoiceMainCamera()
    {
        mainScreenCircle.enabled = false;
        sketchCamera.depth = 0;
        mainCamera.depth = 1;
    }

    // Use this for initialization
    void Start () {
        ChoiceMainCamera();
    }
    

    public GameObject GoalText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GoalSketchCollider")
        {
            ChoiceSketchCamera();
            StartCoroutine(this.SceneFadeIn());
            PlayUiManager.Instance.HideUI((int)PlayUiManager.textUI.monoclear);
            GoalText.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "GoalSketchCollider")
        {

            ChoiceSketchCamera();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GoalSketchCollider")
        {
            ChoiceMainCamera();
            mainScreenCircle.enabled = true;
            StartCoroutine(this.SceneFadeOut());
        }
    }


    // ============= 카메라 페이드인 페이드아웃 ==============
    public CameraFilterPack_TV_WideScreenCircle sketchScreenCircle;

    // 피아노카메라로 바뀔 때 실행되는 코루틴함수
    // 화면이 점점 밝아진다.
    // 화면 테두리는 어두운 상태이다.
    private IEnumerator SceneFadeIn()
    {
        sketchScreenCircle.Size = 0.8f;
        sketchScreenCircle.Smooth = 0.4f;
        float delay = 3f, m_time = 0.0f;
        float increaseValue = delay * 0.01f;
        while (sketchScreenCircle.Size >= 0.5f)
        {
            sketchScreenCircle.Size -= 0.008f;
            sketchScreenCircle.Smooth -= 0.006f;
            m_time += increaseValue;
            yield return new WaitForSeconds(increaseValue);
        }
        // screenCircle.enabled = false;

        yield return null;

    }

    float currentTime = 0;

    // 피아노카메라에서 메인카메라로 전환될 때 실행된다.
    public CameraFilterPack_TV_WideScreenCircle mainScreenCircle;
    private IEnumerator SceneFadeOut()
    {
        mainScreenCircle.Size = 0.7f;
        sketchScreenCircle.Smooth = 0.4f;
        float delay = 2f, m_time = 0.0f;
        float increaseValue = delay * 0.01f;
        while (mainScreenCircle.Size <= 0.8f && mainScreenCircle.Smooth > 0.1)
        {
            mainScreenCircle.Size += 0.005f;
            mainScreenCircle.Smooth -= 0.005f;
            m_time += increaseValue;
            yield return new WaitForSeconds(increaseValue);

        }
        yield return null;

    }
}
