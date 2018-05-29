﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask; // 레이어마스크 캐싱
    float speed = 10f;               // 총알의 속도
    float damage = 1f;                  // 총알의 데미지

    public void SetSpeed(float _newSpeed)           // 총알에 속도를 할당해주는 메소드
    {
        speed = _newSpeed;
    }

	void Update ()
    {
        float moveDistance = speed * Time.deltaTime; // 프레임마다 이동할 거리
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);      // 총알 발사
	}

    void CheckCollisions(float _moveDistance)                     // 충돌을 감지하는 메소드(레이캐스트로 구현한다)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _moveDistance, collisionMask, QueryTriggerInteraction.Collide))   // QueryTriggerInteraction : 트리거랑 충돌했을때 충돌할것인가 말것인가
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit _hit)       // 충돌했을때 발생하는 메소드
    {
        IDamageable damageableObject = _hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null)        // 모든 오브젝트에 IDamageable 이 붙어있는 것은 아니므로
        {
            damageableObject.TakeHit(damage, _hit);         // 충돌했을때 그 콜라이더에 붙어있는 TakeHit메소드를 호출한다
        }
        Debug.Log(_hit.collider.name);
        Destroy(gameObject);
    }
}