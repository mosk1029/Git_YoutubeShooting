using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]   // 이 스크립트를 더할 때 PlayerController 스크립트 또한 붙어있는 것을 강요
[RequireComponent(typeof(GunController))]       // GunController 또한 붙어있음
public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;    // 플레이어 이동속도

    Camera viewCamera;              // 카메라 캐싱
    PlayerController controller;    // PlayerController 스크립트 캐싱
    GunController gunController;

	void Start ()
    {
        controller = GetComponent<PlayerController>();      // Player스크립트와 PlayerController스크립트가 같은 오브젝트에 붙어있음
        gunController = GetComponent<GunController>();      // GunController 가져옴
        viewCamera = Camera.main;                           // 메인카메라 할당
	}
	
	void Update ()
    {
        // Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));      // 이동을 입력받음 (GetAxisRaw는 기본 스무딩 적용X)
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;                                                // 단위백터에 속도를 곱해줌
        controller.Move(moveVelocity);

        // Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);     // 카메라부터 마우스포지션까지의 Ray 생성
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);        // inNormal은 법선벡터, inPoint는 원점에서의 거리
                                                                        // 기존 바닥이 아니라 new Plane 을 만들어주는 이유는 마우스로 기존바닥 밖을 가리키면 false 반환
        float rayDistance;                                              // 카메라에서 플레인에 부딪힌 거리

        if(groundPlane.Raycast(ray, out rayDistance))               // rayDistance가 Raycast 메소드 안에서 할당받은 값으로 변함
        {
            Vector3 point = ray.GetPoint(rayDistance);              // GetPoint는 Ray가 부딪힌 좌표를 반환함
            //Debug.DrawLine(ray.origin, point, Color.green);
            controller.LookAt(point);
        }

        // Weapon Input
        if(Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }

	}
}
