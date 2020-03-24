using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private Transform tr;
    private LineRenderer line;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();

        //Line의 월드좌표 사용 비활성
        line.useWorldSpace = false;
        //Line은 시작할때 비활성화해줍니다.
        line.enabled = false;
        //Line의 시작폭과 종료폭 설정합니다.
        line.SetWidth(2.5f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);

        if(Input.GetMouseButtonDown(0))
        {
            line.SetPosition(0, tr.InverseTransformPoint(ray.origin));

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                line.SetPosition(1, tr.InverseTransformPoint(hit.point));
            }
            else
            {
                line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f)));
            }

            StartCoroutine(this.ShowLaserBeam());
        }
    }

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}
