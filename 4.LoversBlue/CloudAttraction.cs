using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudAttraction : MonoBehaviour {

    public static CloudAttraction Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // 플레이어가 구름기구를 타면
    // - 플레이어에 붙은 메인카메라 대신에
    // - Cloud 카메라가 활성화 되었으면 좋겠다.
    public Camera CloudCamera;
    public Camera mainCamera;
    void ChoiceCloudCamera()
    {
        mainCamera.GetComponent<AudioSource>().enabled = false;
        CloudCamera.depth = 1;
        mainCamera.depth = 0;
    }

    // 플레이어가 구름기구를 타고 도착하면 다시 메인카메라로 변경
    public void ChoiceMainCamera()
    {
        mainCamera.GetComponent<AudioSource>().enabled = true;
        CloudCamera.depth = 0;
        mainCamera.depth = 1;
        CloudCamera.enabled = false;
        Destroy(gameObject, 2.0f);
    }

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            // 플레이어의 포지션을 저장한다.
            Vector3 Playerpos = other.transform.position;
            // 플레이어의 자식으로 구름 기구를 넣는다.
            gameObject.transform.SetParent(other.gameObject.transform);
            // 구름기구를 플레이어의 자식이므로 플레이어의 위치에 있도록 zero 로 설정하고
            gameObject.transform.localPosition = new Vector3(0, -2, 0);
            // 그렇게 되면 이미 구름기구가 자식이 된 후 위치를 조정하니까 
            // 플레이어까지 0,0,0 이 되므로
            // 다시 플레이어 포지션에 아까 저장했던 포지션 값을 준다.
            //other.transform.position = Playerpos;
            ChoiceCloudCamera();
        }
    }

}
