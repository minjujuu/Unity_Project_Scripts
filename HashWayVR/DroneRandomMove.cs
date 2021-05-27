using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneRandomMove : MonoBehaviour {

    [Header("AttackZone")]
    public int minX = -3;
    public int maxX = 3;
    public int minY = -3;
    public int maxY = 3;
    public int minZ = -3;
    public int maxZ =3;

    public float playerViewAngle;    //p시야각
    public float playerViewDistance; //p시야거리 
    public LayerMask TargetMask;    //Enemy 레이어마스크 지정을 위한 변수
    Transform playerTr;

    [Header("RandomMove")]
    public float moveSpeed = 3f;
    public float rotSpeed = 100f; //몸 돌리는 속도
    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;
    // Use this for initialization
    void Start()
    {
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
    }
    private Vector3 PlayerDroneDis
    {
        get
        {
            return (this.transform.position - playerTr.position);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(playerTr);
        DrawView();      //Scene뷰에 시야범위 그리기
        DroneDetect();   //Drone 포착
    }
    public float droneToPlayerSpeed = 0;
    //Player가 뒤로 갈 상황(후진)은 절대 없다.

    //드론은, 플레이어 사잇각 안에 들어와야하고,
    //플레이어 코 앞보다는 좀더 멀리에 있어야 한다.(플레이어 기준 Z +5 정도)
    //고도는 항상 플레이어보다 높이 있게(그렇지 않으면 쫓아올 때 플레이어에 부딪힐 수 있음)
    //
    public void DroneDetect()
    {
        //Player로부터 타겟까지의 '단위벡터'
        Vector3 playerToDroneDir = (transform.position - playerTr.position).normalized;
        //transform.forward와 dirToTarget은 모두 '단위벡터'이므로 내적값은 두 벡터가 이루는 각의 Cos값과 같다.
        //내적값(Cos(a))이  Cos(시야각/2)값보다 크면 시야에 들어온 것이다.
        if (Vector3.Dot(playerTr.forward, playerToDroneDir) > Mathf.Cos((playerViewAngle / 2) * Mathf.Deg2Rad))
        {
            float playerToDroneDist = Vector3.Distance(playerTr.position, transform.position);
            if (!Physics.Raycast(transform.position, playerToDroneDir, playerToDroneDist))
            {
                Debug.DrawLine(playerTr.position, transform.position, Color.red);
            }
            //앞에는 있는데, 플레이어와 Z값이 너무 가까우면 어떻게 할지 고려
        }
        else //각 뒤에 있으면,
        {
            StartCoroutine(MoveFromTo(this.transform, playerTr, droneToPlayerSpeed));//여기에 랜덤위치 대입.
        }
    }
    IEnumerator MoveFromTo(Transform droneTr, Transform targetTr, float speed)
    {
        //speed 수치 인스펙터에서 조정 가능(0.1~1정도?) 테스트 해보면서 정하기
        //https://gamedev.stackexchange.com/questions/100535/coroutine-to-move-to-position-passing-the-movement-speed
        //핵심은 Lerp의 마지막 인수였음
        int ranX = Random.Range(minX, maxX);
        int ranY = Random.Range(minY, maxY);
        int ranZ = Random.Range(minZ, maxZ);

        float step = (speed / (droneTr.position - targetTr.TransformPoint(ranX,ranY,ranZ)).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            droneTr.position = Vector3.Lerp(droneTr.position, targetTr.TransformPoint(ranX, ranY, ranZ), t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();
        }
        transform.SetParent(playerTr);
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        //Player의 좌우 회전값 갱신
        angleInDegrees += playerTr.eulerAngles.y;
        //경계 벡터값 반환
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawView()
    {
        Vector3 leftBoundary = DirFromAngle(-playerViewAngle / 2);
        Vector3 rightBoundary = DirFromAngle(playerViewAngle / 2);
        Debug.DrawLine(playerTr.position, playerTr.position + leftBoundary * playerViewDistance, Color.blue);
        Debug.DrawLine(playerTr.position, playerTr.position + rightBoundary * playerViewDistance, Color.blue);
    }






    private void RandomMove()
    {
        //일단 그냥 움직이게 하자.(랜덤하게)
        if (isWandering == false)
            StartCoroutine(Wander());
        if (isRotatingRight == true)
            transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        if (isRotatingLeft == true)
            transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        if (isWalking == true)
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    IEnumerator Wander()
    {
        float rotTime = Random.Range(0f, 0.5f); //회전하는데 걸리는 시간
        float rotateWait = Random.Range(1f, 1.5f); //회전 사이에 대기 시간
        int rotateLeftOrRight = Random.Range(0, 3); //bool과 비슷한 기능, 왼쪽으로 도냐 오른족으로 도냐?
        int moveWait = Random.Range(1, 3);// move 사이에 대기 시간
        int moveTime = Random.Range(1, 4); //move 하는 시간(조금만 갈 수도, 멀리 갈 수도)

        isWandering = true;

        yield return new WaitForSeconds(moveWait); //1,2,3,4 초 랜덤하게 걷는사이 대기시간

        isWalking = true;
        yield return new WaitForSeconds(moveTime);
        isWalking = false;

        yield return new WaitForSeconds(rotateWait);
        if (rotateLeftOrRight == 1)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        if (rotateLeftOrRight == 2)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        isWandering = false;
    }
}
