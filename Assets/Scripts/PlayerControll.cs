using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runLeft;
    public AnimationClip runRight;
}

public class PlayerControll : MonoBehaviour
{
    private float v = 0.0f;
    private float h = 0.0f;

    //Player의 이동하는처리 본 스크립트에서 사용할 트랜스폼 컴포넌트.
    private Transform playerTransform;
    //Player의 점프에 필요한 RigidBody Component 가져오기.
    private Rigidbody _rigid;
    //Player의 이동속도 선언 (Public으로 선언하여 Inspector에서 조정 가능하게 했습니다.)
    public float speed = 30.0f;
    //Player의 회전하는 속도값
    public float rotateSpeed = 100.0f;
    //Player의 점프파워
    public float jumpPower = 10.0f;
    //Inpector View 에 표시할 애니메이션 변수
    public Anim anim;
    //Player하위에있는 Animation 컴포넌트에 접근하기 위한 변수입니다.
    public Animation _animation;
    //Player의 체력
    public int playerHP = 100;
    private int initHp;

    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    public Image imgHpbar;

    //GameManager에 접근하기 위한 변수
    private GameMgr gameMgr;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();

        _rigid = GetComponent<Rigidbody>();

        _animation = GetComponentInChildren<Animation>();

        _animation.clip = anim.idle;
        _animation.Play();

        initHp = playerHP;

        //GameMgr을 게임매지너 컴포넌트와 연결시켜줍니다.
        gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //Debug.Log("Horizontal : " + h.ToString());
        //Debug.Log("Vertical : " + v.ToString());

        //전후좌우 이동방향 벡터 계산.
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //이동부
        playerTransform.Translate(moveDir * Time.deltaTime * speed, Space.Self);
        //회전부
        playerTransform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed * Input.GetAxis("Mouse X"));

        //애니메이션 재생부
        if (v >= 0.1f)
        {
            //CrossFade함수의 두가지 인자(변경할 애니메이션 클립명칭, 페이드 아웃되는 시간.)
            //CrossFade 함수는 애니메이션이 변경될때 끊어지지않고 부드럽게 진행되는 효과를 얻을수있다.
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        {
            _animation.CrossFade(anim.idle.name, 0.3f);
        }

        Jump();
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PUNCH")
        {
            playerHP -= 10;

            Debug.Log(playerHP.ToString());

            if(playerHP <= 0)
            {
                PlayerDie();
            }

            imgHpbar.fillAmount = (float)playerHP / (float)initHp;
        }
    }

    void PlayerDie()
    {
        Debug.Log("Player Die !!");

        ////MONSTER라는 태그를 가진 모든 게임오브젝트를 찾아오고 배열에 담습니다.
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        ////배열에 담긴 모든 몬스터의 OnplayerDie 함수를 순차적으로 호출시킵니다.
        //foreach(GameObject monster in monsters)
        //{
        //    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        OnPlayerDie();
        //gameMgr.isGameOver = true;

        //GameManager를 Singletone으로 만들어서 이렇게 가져올수 도 있습니다.
        GameMgr.instance.isGameOver = true;
    }
}
