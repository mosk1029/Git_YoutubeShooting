using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]   // 이 스크립트를 더할 때 PlayerController 스크립트 또한 붙어있는 것을 강요
public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;    // 플레이어 이동속도
    PlayerController controller;    // PlayerController 스크립트 캐싱

	void Start ()
    {
        controller = GetComponent<PlayerController>();      // Player스크립트와 PlayerController스크립트가 같은 오브젝트에 붙어있음
	}
	
	void Update ()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertiacl"));    // 이동을 입력받음
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;                                        // 단위백터에 속도를 곱해줌
	}
}
