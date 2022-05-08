using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move / Hacked / Change / Attack / Die
// 1. Move
// - 랜덤으로 움직인다(Perlin Noise).
// - 수색하고 있다. (Shot Light)

// 2. Hacked
// - Enemy에게 해킹당한다. 
// - 해킹당하는 걸 표현하는 게 중요 (이펙트, 해킹 아이콘) // 솜브라 참고
// - 해킹 당해서 상태와 겉모습이 변화한다.
// - Drone's StateRing Material 

// 3. Attack
// - 플레이어를 공격한다. 
// - 플레이어에게 레이저 발사 
// - Ray, Line Renderer

// 4. Damage
// - 플레이어에게 공격당한다.
// - 부들부들댄다.

// 5. Die
// - 죽는다.
public class MJ_CityDrone : MonoBehaviour {

    //----------------- Drone's State Ring ----------------
    public GameObject stateRing;
    Material stateMt;
    Color32 hackedColor = new Color32(80, 00, 255, 255); // 보라색
    

    // ------------- Hacked ------------
    public ParticleSystem hackingParticle;
    public ParticleSystem cubeHackParticle;
    public GameObject hackingIcon;
    Animator iconAnim;

    // ------------- Attack ------------------
    public float attackDistance = 7.0f;
    public Transform laserPos;
    LineRenderer[] laserLines;
    public GameObject laserMuzzle;
    public ParticleSystem laserSparkParticle;
    public ParticleSystem collectEnergyParticle;

    // ------------- Die ------------------

    public ParticleSystem dieEffect;
    // -------------- Player ----------------
    Transform playerTr;


    public enum CityDroneState
    {
        Move,
        Hacked,
        Attack,
        Damage,
        Die,
        Delay
    }

    public CityDroneState cState;

    enum DroneSound
    {
        Move, // 움직일 때
        Shot, // 쏘기 시작할 때 (레이저 피융)
        Attack, // 레이저 지지는 소리
        Hacked // 해킹당하는 소리
    }

    AudioSource[] cAudios;

    void Start () {
        cState = CityDroneState.Move;

        // << 플레이어 >>
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        // << 상태 링 >>
        stateMt = stateRing.GetComponent<MeshRenderer>().material;

        // << 해킹 이펙트 >>
        iconAnim = hackingIcon.GetComponent<Animator>();

        // << 어택 >>
        laserLines = new LineRenderer[2];
        // 레이저
        laserLines[0] = laserPos.GetComponentsInChildren<LineRenderer>()[0];
        laserLines[1] = laserPos.GetComponentsInChildren<LineRenderer>()[1];
        // 처음에는 레이저가 보이지 않도록 한다.
        laserLines[0].enabled = false;
        laserLines[1].enabled = false;
        // 레이저 발사할 때 나오는 muzzle도 보이지 않게 한다.
        laserMuzzle.SetActive(false);

        // << 사운드 >>
        cAudios = new AudioSource[4];
        cAudios = GetComponents<AudioSource>();

    }

    void FixedUpdate ()
    {
        DrawView();             //Scene뷰에 시야범위 그리기
        FindVisibleTargets();   //Enemy인지 Obstacle인지 판별
        switch (cState)
        {
            case CityDroneState.Move:
                // 1. 플레이어의 앞에 있는지를 판단하고
                // 플레이어의 앞에 있다면 공격
                Move();
                PlaySound(cAudios[(int)DroneSound.Move]);
                break;
            case CityDroneState.Hacked:
                // 1. Hacking Effect
                // - Particle, Hacked Icon
                Hacked();
                
                break;
            case CityDroneState.Attack:
                // 1. 플레이어를 쫓아감. (항상 플레이어의 시야에 있어야 함.)
                // 2. 플레이어의 정면에 있을 때에만 레이저 발사. 
                Attack();
                PlaySound(cAudios[(int)DroneSound.Attack]);
                break;
            case CityDroneState.Damage:
                // 1. 플레이어의 총에 맞았을 경우 Damage를 입는다.
                // 2. Damage를 입었으나 Hp가 남았을 경우 다시 Attack으로 변화한다. 
                Damage();
                cAudios[(int)DroneSound.Attack].Stop();
                break;
            case CityDroneState.Die:
                Die();
                break;
            case CityDroneState.Delay:
                collectEnergyParticle.Play();
                Delay();
                break;
        }

        if (MJ_EndingManager.Instance.isEnding)
        {
            cState = CityDroneState.Die;
        }

    }
    
    bool isFirst = true;
    Vector3 randomAttackPoint;
    private void Move()
    {
        // 플레이어의 앞에 있으면,
        if(IsPlayerForward() && isHacked)
        {
            cState = CityDroneState.Delay;
        }
    }

    public bool isHacked = false;
    // 에너미에서 호출할 메서드
    public void BeHacked()
    {
        // 드론의 상태를 Hacked로 변경
        cState = CityDroneState.Hacked;
        isHacked = true;
        // 해킹 사운드 재생
        PlaySound(cAudios[(int)DroneSound.Hacked]);
    }

  
    private void Hacked()
    {
        // 1. 해킹된 파티클 플레이
        // 2. 해킹된 아이콘 띄우기
        // 3. 상태 링을 해킹색으로 변경 
        hackingParticle.Stop();
        hackingParticle.Play();
        stateMt.SetColor("_RimColor", hackedColor);
        iconAnim.SetBool("IsAppear", true);
        cubeHackParticle.Play();
        cState = CityDroneState.Delay;
    }

    // 드론이 플레이어 뒤에 있을 때 공격을 하면 플레이어가 "띠용" 하니까
    // 드론이 플레이어의 앞에 있을 때만 공격을 하고 싶다. 
    // - 내적을 이용하여 드론이 플레이어의 앞에 있는지 뒤에 있는지 알 수 있다. 
    float attackDelayTime = 3.0f;
    float currentTIme = 0;

    private void Attack()
    {
        if (IsPlayerForward())
        {
            FireLaser();
        }
        else
        {
            StopFire();
        }
    }

    bool isFirstAttack = true;

    void FireLaser()
    {
        if (isFirstAttack)
        {
            cAudios[(int)DroneSound.Shot].Play();
            isFirstAttack = false;
        }

        currentTIme += Time.deltaTime;
        if (currentTIme > attackDelayTime)
        {
            cState = CityDroneState.Delay;
            currentTIme = 0;
        }

        // 1. 레이저가 있어야 한다. 
        for (int i = 0; i < laserLines.Length; i++)
        {
            laserLines[i].enabled = true;
            laserLines[i].SetPosition(0, laserPos.position);
            laserLines[i].SetPosition(1, playerTr.position);
        }
        // - Laser Muzzle도 보이게 한다.
        laserMuzzle.SetActive(true);
        // - Laser Spark Particle을 플레이 한다.
        laserSparkParticle.Play();

        // 2. 레이저의 방향 
        Vector3 rayDir = playerTr.position - transform.position;

        // 3. 플레이어를 공격할 레이저 Ray 생성
        Ray ray = new Ray(laserPos.position, rayDir);

        // 4. Ray를 쐈을 때 부딪힌 정보를 저장하기 위한 변수
        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(transform.position, rayDir, out hitInfo))
        {
            if (hitInfo.transform.tag == "Player")
            {
                J_PlayerHP.Instance.PlayerDamaged();
            }
        }

    }

    void StopFire()
    {
        // 1. 레이저가 있어야 한다. 
        for (int i = 0; i < laserLines.Length; i++)
        {
            laserLines[i].enabled = false;
        }
        // - Laser Muzzle도 안 보이게 한다.
        laserMuzzle.SetActive(false);
        // - Laser Spark Particle을 멈춘다.
        laserSparkParticle.Stop();
        isFirstAttack = true;
        isFirstAttack = true;
    }

    private void Damage()
    {
        transform.position = Vector3.Lerp(transform.position, damagePos, damageEffectTime * Time.deltaTime);

        if (hp <= 0)
        {
            // 0이라면 Die
            cState = CityDroneState.Die;
        }
        else if (hp > 0)
        {
            // 아직 hp가 남아있으면 Delay으로 전환
            cState = CityDroneState.Delay;
        }
    }

    // 맞을때 뒤로 밀리는 거리
    public float damageDistance = 0.5f;
    // 맞은 방향
    Vector3 damagePos;
    public int hp = 2;
    public float damageEffectTime = 10;
    public void MinusHP(Vector3 dir)
    {
        // 맞으면 부들부들
        Vector3 randDir = UnityEngine.Random.insideUnitSphere;
        float dis = UnityEngine.Random.Range(damageDistance, damageDistance + 15);
        damagePos = transform.position + (dir.normalized + randDir) * damageDistance;
        // 만약 플레이어가 쏘는 총(Ray)에 맞으면
        // heart의 수가 줄어든다.
        hp--;
        // Drone의 상태를 Damage로 바꾼다.
        cState = CityDroneState.Damage;

        laserLines[0].enabled = false;
        laserLines[1].enabled = false;
    }

    private void Die()
    {
        // 드론이 죽으면
        // 1. 파티클 시스템이 생긴다.
        // 2. Die Sound가 재생된다.
        // - 드론은 사라지거나 비활성화 될 거니까 파티클시스템에 사운드가 있어야 할 거 같다.
        PlayDieEffect();
        Destroy(this.gameObject);
    }


    float dCurrentTime;
    float dDelayTime = 3.0f;
    void Delay()
    {
        
        dCurrentTime += Time.deltaTime;
        // 1. Attack 중 딜레이를 하는 상태
        // 2. 공격을 멈춘다.
        StopFire();
        // 3. 레이저 공격을 위한 에너지 모으는 행동을 한다. 
        // - 에너지 모으는 파티클

        // 일정시간 딜레이 후에 Attack 상태로 전환한다.
        if(dCurrentTime > dDelayTime)
        {
            cState = CityDroneState.Attack;
            dCurrentTime = 0;
        }

    }

    // 1. Die Particle 재생
    // 2. Die Particle에 있는 오디오 재생
    void PlayDieEffect()
    {
        ParticleSystem par = Instantiate(dieEffect);
        par.transform.position = this.transform.position;
        par.Play();
        par.GetComponent<AudioSource>().Play();
    }


    void PlaySound(AudioSource audioc)
    {
        // 오디오가 여러번 플레이되는 것을 막기 위해
        // 해당 오디오소스가 플레이중이 아닐 때만
        // 플레이를 진행한다.
        if (!audioc.isPlaying)
        {
            audioc.Play();
        }
    }

    // 드론이 플레이어의 앞에 있는지와
    // 드론이 attackDistance 안에 있는지를 분별하는 메서드.
    #region
    public bool IsPlayerForward()
    {
        // Player로부터 나(Drone)까지의 '단위벡터'
        Vector3 playerTodroneDir = (transform.position - playerTr.position).normalized;
        //transform.forward와 dirToTarget은 모두 '단위벡터'이므로 내적값은 두 벡터가 이루는 각의 Cos값과 같다.
        //내적값(Cos(a))이  Cos(시야각/2)값보다 크면 시야에 들어온 것이다.
        // 만약 플레이어의 시야에 드론이 들어왔다면,
        if (Vector3.Dot(playerTr.forward, playerTodroneDir) > Mathf.Cos((droneViewAngle / 2) * Mathf.Deg2Rad)
            && Vector3.Distance(playerTr.position, transform.position) < attackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 드론이 시야 범위에 있는지 아닌지를 판별하는 코드들.
  
    public float droneViewAngle;    //시야각
    public float droneViewDistance; //시야거리 
    public LayerMask TargetMask;    //Player 레이어마스크 지정을 위한 변수

    public float moveSpeed = 10f, rotSpeed = 10f;

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        //Drone의 좌우 회전값 갱신
        angleInDegrees += playerTr.eulerAngles.y;
        //경계 벡터값 반환
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawView()
    {
        Vector3 leftBoundary = DirFromAngle(-droneViewAngle / 2);
        Vector3 rightBoundary = DirFromAngle(droneViewAngle / 2);
        Debug.DrawLine(playerTr.position, playerTr.position + leftBoundary * droneViewDistance, Color.blue);
        Debug.DrawLine(playerTr.position, playerTr.position + rightBoundary * droneViewDistance, Color.blue);
    }

    public void FindVisibleTargets()
    {
        // Player로부터 나(Drone)까지의 '단위벡터'
        Vector3 playerTodroneDir = (transform.position - playerTr.position).normalized;
        //transform.forward와 dirToTarget은 모두 '단위벡터'이므로 내적값은 두 벡터가 이루는 각의 Cos값과 같다.
        //내적값(Cos(a))이  Cos(시야각/2)값보다 크면 시야에 들어온 것이다.
        // 만약 플레이어의 시야에 드론이 들어왔다면,
        if (Vector3.Dot(playerTr.forward, playerTodroneDir) > Mathf.Cos((droneViewAngle / 2) * Mathf.Deg2Rad))
        {
            //Player와 드론 사이의 거리
            float distToPlayer = Vector3.Distance(transform.position, playerTr.position);
            if (!Physics.Raycast(transform.position, playerTodroneDir, distToPlayer))
            {
                Debug.DrawLine(transform.position, playerTr.position, Color.red);
            }
        }
    }

    // 드론이 플레이어의 앞쪽의 랜덤 position으로 날아가야 한다.
    // playerTr.position.x는 양 옆으로 -2, 2 정도의 범위
    // playerTr.position.y는 위 아래로 -1, 3 정도의 범위
    // playerTr.position.z는 앞 뒤로 0, 5 정도의 범위

    public float minX = -2;
    public float maxX = 2;
    public float minY = -1;
    public float maxY = 3;
    public float minZ = 0;
    public float maxZ = 5;


    float ranX;
    float ranY;
    float ranZ;
    // 플레이어의 시야 범위 중에서 랜덤의 좌표값을 받아온다.
    // 드론은 여기서 정해진 좌표에서 플레이어를 공격한다.
    public Vector3 SetRandomDestPosition()
    {
        ranX = UnityEngine.Random.Range(minX, maxX);
        ranY = UnityEngine.Random.Range(minY, maxY);
        ranZ = UnityEngine.Random.Range(minZ, maxZ);
        return new Vector3(playerTr.position.x+ranX, playerTr.position.y + ranY, playerTr.position.z + ranZ);
        
    }
    #endregion


    // trash
    #region

    // 차이값은 지정되었을 때 한 번만 받아온다.

    //public float followSpeed = 9;
    //private void LateUpdate()
    //{
    //    Vector3 spherePos = testSphere.position;
    //    spherePos.x = playerTr.position.x + ranX;
    //    spherePos.y = playerTr.position.y + ranY;
    //    spherePos.z = playerTr.position.z + ranZ;
    //    testSphere.position = Vector3.Lerp(testSphere.position, spherePos, followSpeed * Time.deltaTime);

    //    // -------------------------------------------------------
    //    //Vector3 dronePos = transform.position;
    //    //dronePos.x = spherePos.x;
    //    //dronePos.y = spherePos.y;
    //    //dronePos.z = spherePos.z;

    //    //transform.position = Vector3.Lerp(transform.position, dronePos, moveSpeed * Time.deltaTime);

    //}
    #endregion
}
