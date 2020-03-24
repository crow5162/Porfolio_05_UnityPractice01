using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform targetTransform;          //추적할 대상의 Trnasform
    public float distance = 10.0f;             //카메라의 거리 (추적할 대상과의)
    public float height = 3.0f;                //카메라의 높이
    public float dampTrace = 20.0f;            //부드러운 카메라 추적을 위한 변수

    private Transform cameraTransform;         //카메라의 Transform

    void Start()
    {
        cameraTransform = GetComponent<Transform>();

        //게임이 활성화된 동안에는 마우스 커서 잠금, 마우스 커서 숨기기
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Update 함수 호출이후 한번만 호출되는 함수인 Lateupdate사용
    //추적할 Target의 이동이 종료된후에 카메라가 추적하기위해 LateUpdate 사용합니다.
    private void LateUpdate()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position,
            targetTransform.position -
            (targetTransform.forward * distance) +
            (Vector3.up * height),
            Time.deltaTime * dampTrace);

        cameraTransform.LookAt(targetTransform.position);
    }
}
