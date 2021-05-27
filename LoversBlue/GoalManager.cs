using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 플레이어의 컴포넌트로 동작합니다.
// 플레이어가 현재 어떤 행성에 도착했는지 관리하는 매니저스크립트
// 1. 플레이어가 모노행성에 도착하면 다른 모든 행성들과 길이 사라진다.
//  1) 모노행성 도착지점에 있는 콜라이더 (Is Trigger)
//  2) 만약, 플레이어가 GoalMonoCollider 를 통과하면
//      모노행성을 제외한 모든 행성들이 비활성화 된다.
//  3) 가만히 있던 그림자괴물이 애니메이션을 실행한다. (공격 시작)
     
public class GoalManager : MonoBehaviour {

    public GameObject VividParentObject;
    public GameObject PastelParentObject;

    private static GoalManager _instance = null;
    public static GoalManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GoalManager)) as GoalManager;

                if (_instance == null)
                {
                    Debug.LogError("GoalManager 오브젝트 비활성화 상태");
                }
            }
            return _instance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "GoalMonoCollider")
        {
            VividParentObject.SetActive(false);
            PastelParentObject.SetActive(false);
            CloudAttraction.Instance.ChoiceMainCamera();
            PlayUiManager.Instance.HideUI((int)PlayUiManager.textUI.mono);
        }
    }

    public void ShowAllPlanet()
    {
        VividParentObject.SetActive(true);
        PastelParentObject.SetActive(true);
    }

}
