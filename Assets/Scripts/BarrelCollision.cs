using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCollision : MonoBehaviour
{
    //폭발효과 와 스파크 파티클 연결변수.
    public GameObject expEffect;
    public GameObject sparkEffect;

    private Transform trans;
    //랜덤으로 생성할 텍스쳐 배열
    public Texture[] texture;

    //피격횟수 누적 변수.
    private int hitCount;

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();

        int idx = Random.Range(0, texture.Length);

        GetComponent<MeshRenderer>().material.mainTexture = texture[idx];
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision coll)
    {
        if(coll.collider.tag == "BULLET")
        {
            Destroy(coll.gameObject);
            GameObject spark = (GameObject)Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);

            //3회이상 Bullet과 Collision되면 폭발함수 호출.
            if(++hitCount >= 3)
            {
                ExpBarrel();
            }

            Destroy(spark, spark.GetComponent<ParticleSystem>().duration + 0.5f);
        }


    }

    void ExpBarrel()
    {
        //폭발이펙트 표시와 생성정보를 GameObject로 형변환하여 explosion에 담아줍니다.
        GameObject explosion = (GameObject)Instantiate(expEffect, trans.position, Quaternion.identity);

        //지정한 원점을 중심으로 10.0f 반경내에 들어와있는 Collider 객체 저장.
        Collider[] colls = Physics.OverlapSphere(trans.position, 10.0f);

        foreach(Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if(rbody != null)
            {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce(1000.0f, trans.position, 10.0f, 300.0f);
            }
        }

        Destroy(gameObject, 5.0f);
        //Barrel이 Destroy되고 Effect정보를 담은 explosion도 Destroy
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().duration + 5.0f);
    }

    void OnDamage(object[] _params)
    {
        //발사위치 가져옵니다.
        Vector3 firePos = (Vector3) _params[0];
        //피격위치 가져옵니다.
        Vector3 hitPos =  (Vector3) _params[1];

        //입사각 벡터(Ray의 각도를 구합니다) 벡터로 변경
        Vector3 inComeVector = hitPos - firePos;

        //입사각 벡터를 정규화 벡터로 변경
        inComeVector = inComeVector.normalized;

        //Ray의 hit좌표에 입사각 벡터의 각도로 힘을 생성합니다.
        GetComponent<Rigidbody>().AddForceAtPosition(inComeVector * 1000f, hitPos);

        if(++hitCount >= 3)
        {
            ExpBarrel();
        }
    }
}
