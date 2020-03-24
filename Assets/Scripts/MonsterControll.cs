using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterControll : MonoBehaviour
{
    //몬스터 상태를 담고있는 Enumerable  변수 선언.
    public enum MonsterState { idle, trace, attack, die};
    //몬스터의 상태정보를 저장할 Enum변수.
    public MonsterState monsterstate = MonsterState.idle;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;

    private bool isDie = false;

    //피격시 혈흔 효과 
    public GameObject bloodEffect;
    public GameObject bloodDecal;

    private int monsterHP = 100;

    //스코어에 접근하기위한 변수.
    private GameUI gameUI;

    // Start is called before the first frame update
    void Awake()
    {
        //Monster의 Transform 할당.
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적 대상인 PLayer의 Transform 할당
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //NavMeshAgent Component 할당.
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        nvAgent.destination = playerTr.position;



        //GameUI 게임 오브젝트의 GmaeUI 스크립트를 할당합니다.
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
    }

    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

    }

    void MonsterDie()
    {
        gameObject.tag = "Untagged";

        StopAllCoroutines();

        isDie = true;
        monsterstate = MonsterState.die;
        nvAgent.Stop();
        animator.SetTrigger("IsDie");

        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        gameUI.DispScore(130);

        StartCoroutine(this.PushObjectPool());
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        isDie = false;
        monsterHP = 100;
        gameObject.tag = "MONSTER";
        monsterstate = MonsterState.idle;

        //Monster가 죽었을때 호출되던함수에서 비활성화 시켜주었던 콜라이더를 다시 살려주는 작업입니다.
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        gameObject.SetActive(false);
    }

    //Bullet과 충돌
    private void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            Destroy(coll.gameObject);
            animator.SetTrigger("IsHit");
            CreateBloodEffect(coll.transform.position);

            monsterHP -= coll.gameObject.GetComponent<BulletControll>().bulletDamage;

            if(monsterHP <= 0)
            {
                MonsterDie();
            }
        }
    }

    //일정한 간격으로 몬스터의 행동상태를 체크하고 monsterstate 값 변경해줄 코루틴 함수.
    IEnumerator CheckMonsterState()
    {
        //몬스터다 Die 상태가 아닐때를 전제로 합니다.
        while(!isDie)
        {
            //0.2초의 간격을 두고 몬스터와 플레이어와의 거리를 체크한다.
            yield return new WaitForSeconds(0.2f);

            //몬스터와 플레이어의 거리를 저장할 변수.
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            //거리체크하고 몬스터의 상태를 바꿔준다.
            if(dist <= attackDist)
            {
                monsterstate = MonsterState.attack;
            }

            else if (dist <= traceDist)
            {
                monsterstate = MonsterState.trace;
            }
            else
            {
                monsterstate = MonsterState.idle;
            }
        }
    }

    //상태변화에 따른 행동을 결정하는 코루틴 함수입니다.
    IEnumerator MonsterAction()
    {
        //마찬가지, Die 상태가 아닐때를 전제로..
        while(!isDie)
        {
            switch (monsterstate)
            {
                //플레이어와의 거리가 멀어 인식하지 못하는 상태 입니다.
                case MonsterState.idle:
                    //Navigation Agent 를 멈춤상태로 둡니다.
                    nvAgent.Stop();
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsAttack", false);

                    break;
                    //플레이어를 인지한 상태라면 Nav Agent를 다시 실행하고 
                    //Nav Agent를 다시 활성화 합니다.
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);

                    break;
                case MonsterState.attack:
                    nvAgent.Stop();
                    animator.SetBool("IsAttack", true);

                    break;
            }

            yield return null;

        }
    }

    void CreateBloodEffect(Vector3 pos)
    {
        //Bloody Effect
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, blood1.GetComponent<ParticleSystem>().duration);

        //Blood Decal
        //데칼의 위치 생성 변수 (몬스터의 포지션의 바닥에서 조금 내린 위치 산출
        Vector3 decalPos = monsterTr.position + (Vector3.up * -0.5f);
        //데칼의 랜덤한 회전 변수 Z축으로 랜덤한 회전을 부여하고 변수에 저장합니다.
        //X 축을 90도 회전시켜야 수직으로 서있는 상태가 되지않은다.
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));
        //Blood Decal Prefabs Create
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //데칼의 크기를 랜덤한 크기로 생성합니다.
        float decalScale = Random.Range(55.0f, 65.0f);
        blood2.transform.localScale = Vector3.one * decalScale;

        Destroy(blood2, 5.0f);
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        nvAgent.Stop();
        animator.SetTrigger("IsPlayerDie");
    }

    void OnEnable()
    {
        PlayerControll.OnPlayerDie += this.OnPlayerDie;

        //Start에 코루틴 함수 시작해줍니다.
        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
    }

    void OnDisable()
    {
        PlayerControll.OnPlayerDie -= this.OnPlayerDie;
    }

    //Monster가 Player의 Ray에 맞았을때
    void OnDamage(object[] _params)
    {
        Debug.Log(string.Format("Hit ray {0} : {1}", _params[0], _params[1]));

        CreateBloodEffect((Vector3)_params[0]);

        monsterHP -= (int)_params[1];

        if(monsterHP <= 0)
        {
            MonsterDie();
        }

        animator.SetTrigger("IsHit");
    }
}
