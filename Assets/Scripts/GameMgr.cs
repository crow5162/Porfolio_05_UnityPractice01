using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class GameMgr : MonoBehaviour
{
    //몬스터 생성 포인트를 담을 배열
    public Transform[] points;
    //몬스터 프리팹을 할당할 변수
    public GameObject monsterPrefabs;

    //몬스터를 발생 시킬 주기.
    public float monsterCreateTime = 2.0f;
    //몬스터의 최대 발생 갯수
    public int maxMonster = 10;
    //게임종료 여부변수.
    public bool isGameOver = false;

    //GameManager의 Singletone시키기위한 인스턴스 변수 선언.
    public static GameMgr instance = null;

    //Monster를 미리생성해 저장할 리스트 자료형.
    public List<GameObject> monsterPool = new List<GameObject>();

    //사운드 볼륨 설정변수.
    public float sfxVolume = 1.0f;
    //사운드 뮤트처리 설정 변수.
    public bool isSfxMute = false;


    void Awake()
    {
        //GameMgr을 Instance에 대입합니다.
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Point의 배열에 Hierachy에 있는 SpawnPoint의 하위에있는 Transform 컴포넌트를 찾아옵니다.
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //몬스터 생성 Coroutine 함수를 호출합니다.
        //포인트가 있을때만 코루틴 함수가 시작되도록 예외처리입니다
        if(points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
        }

        for(int i =0;i < maxMonster;i++)
        {
            GameObject monster = (GameObject)Instantiate(monsterPrefabs);

            monster.name = "Monster_" + i.ToString();

            monster.SetActive(false);

            monsterPool.Add(monster);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CreateMonster()
    {
        //오브젝트 풀링을 사용하기전까지 사용하였던 포인트로부터 몬스터를생하는 코루틴.
        //while(!isGameOver)
        //{
        //    //현재 Scene에 깔려있는 몬스터의 갯수 산출
        //    int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;
        //
        //    //Scene에 깔려있는 몬스터보다 maxMonster의 갯수가 적을때만 실행합니다.
        //    if(monsterCount < maxMonster)
        //    {
        //        //몬스터의 생성 간격
        //        yield return new WaitForSeconds(monsterCreateTime);
        //
        //        //불규칙적인 포인트위치 산출합니다.
        //        //1부터 포인트의 최대 갯수까지.
        //        int idx = Random.Range(1, points.Length);
        //
        //        Instantiate(monsterPrefabs, points[idx].position, points[idx].rotation);
        //    }
        //    else
        //    {
        //        yield return null;
        //    }
        //}

        while(!isGameOver)
        {
            yield return new WaitForSeconds(monsterCreateTime);



            if (isGameOver) yield break;

            //오브젝트풀의 처음부터 끝까지 진행합니다.
            foreach(GameObject monster in monsterPool)
            {
                //비활성화 여부로 사용가능한 몬스터를 판단합니다.
                if(!monster.activeSelf)
                {
                    //몬스터를 출현시킬 위치의 인덱스 값을 추출합니다.
                    int idx = Random.Range(1, points.Length);

                    //몬스터의 출현위치를 결정합니다.
                    monster.transform.position = points[idx].position;

                    //몬스터의 활성화를 시작합니다.
                    monster.SetActive(true);

                    break;
                }
            }
        }
    }

    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        if (isSfxMute) return;

        GameObject soundObj = new GameObject("sfx");

        soundObj.transform.position = pos;

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();

        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.volume = sfxVolume;
        audioSource.Play();

        Destroy(audioSource, sfx.length);
    }
}
