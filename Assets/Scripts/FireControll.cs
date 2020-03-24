using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//반드시필요한 컴포넌트를 명시하여 컴포넌트가 삭제되는것을 방지하는 Attribute
[RequireComponent(typeof(AudioSource))]

public class FireControll : MonoBehaviour
{
    //총알의 프리팹 가지고 오기
    public GameObject bullet;
    //총알의 발사좌표 가지고 오기
    public Transform firePos;
    //연발사격 딜레이
    public float fireDelay = 0.3f;
    //마우스 꾸욱누르고있을때 ++될 변수.
    private float _fire = 0.1f;

    //Bullet Fire 사운드
    public AudioClip firesfx;
    //AudioSource Component를 저장할 변수
    private AudioSource source = null;
    //MuzzleFalsh의 MeshRenderer Component 연결 변수
    public MeshRenderer muzzleFlash;

    //총알발사될 거리 (Ray쏠 거리)
    public float rayDistance = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        muzzleFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //왼쪽마우스 누르고있을때에눈 단발사격
        if(Input.GetMouseButtonDown(0))
        {
            Fire();

            //Ray에 적중한 게임오브젝트의 정보를담을 변수입니다.
            RaycastHit hit;

            //RayCast 함수로 Ray를 발사해 맞은 게임오브젝트가 있을 때 true 를 반환합니다.
            if(Physics.Raycast(firePos.position, firePos.forward, out hit, rayDistance))
            {
                //맞은 대상의 tag가 Monster 일때
                if(hit.collider.tag == "MONSTER")
                {
                    //SendMassage를 이용해 전달할 두가지 인자를 배열에 담습니다.
                    object[] _params = new object[2];
                    _params[0] = hit.point; //Ray에 맞은 정확 위치값(Vector3)
                    _params[1] = 20;        //몬스터에게 입힐 데미지 값.

                    hit.collider.gameObject.SendMessage("OnDamage",
                        _params,
                        SendMessageOptions.DontRequireReceiver);
                }

                if (hit.collider.tag == "BARREL")
                {
                    object[] _params = new object[2];
                    _params[0] = firePos.position;
                    _params[1] = hit.point;
                    hit.collider.gameObject.SendMessage("OnDamage",
                        _params,
                        SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        //오른쪽 마우스 꾸욱누르고있을때에는 연발사격.
        else if (Input.GetMouseButton(1))
        {
            FireAuto();
        }

        //레이캐스트 라인 표시하기.
        //Debug.DrawRay(firePos.position, firePos.forward * 30.0f, Color.red);
    }

    void Fire()
    {
        //RayCast방식으로 변경되므로 초앙ㄹ을 만드는 루틴은 주석으로 처리

        GameMgr.instance.PlaySfx(firePos.position, firesfx);

        //동적으로 총알을 생성하는 함수.
        //CreateBullet();
        //source.PlayOneShot(firesfx, 0.9f);
        StartCoroutine(this.MuzzleFlashShow());
    }

    //오른쪽 마우스 누르고있을때 자동으로 발사되는 함수.
    void FireAuto()
    {
        //누르고있을때마다 현재 프레임 시간을 누적합니다.
        //Time.DeltaTime을 1초간 누적하면 1이됩니다.
        _fire += Time.deltaTime;

        if (_fire >= fireDelay)
        {
            //누적된 시간보다 Delay보다 커지면 누적시간을 초기화하고 BulletFire.
            CreateBullet();
            source.PlayOneShot(firesfx, 0.9f);
            _fire = 0;
            StartCoroutine(this.MuzzleFlashShow());
        }
    }

    void CreateBullet()
    {
        //Bullet Prefabs를 동적으로 생성.
        //FirePos의 위치에 생성하겠습니다.
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    IEnumerator MuzzleFlashShow()
    {
        //scale변수에 랜덤한 값넣어주고.
        float scale = Random.Range(25.0f, 35.0f);
        //Child GameObject의 Transform 속성을 수정하면 localScale과 localRotation, localpositon을 사용해야합니다.
        //Vector3.one 의 의미는 Vector3(1, 1, 1) 이라는 의미이므로 , Vector3.one * scale은 
        //Vector3(scale, scale, scale) 과 같은 의미이다.
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //Quad를 Z축 기준으로 회전시킨다.
        //Transform Componenet의 localRotation 속성은 Quaternion 타입이기 때문에 Quaternion.Euler(x, y, z) 함수를 이용해 (회전)값을 추출합니다.
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        //이처럼 Child화된 GameObject의 MeshRenderer를 조작하는 방법도 있지만 다양한 총구화염 텍스쳐를 무작위로 출력하는 방식도 존재합니다.

        muzzleFlash.enabled = true;

        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

        muzzleFlash.enabled = false;
    }
}
