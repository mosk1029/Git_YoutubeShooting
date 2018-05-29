﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
    }
    State currentState;

    NavMeshAgent pathfinder;        // 에너미 NavMeshAgent 캐싱
    Transform target;               // 타겟 캐싱
    Material skinMaterial;          // 적의 머테리얼

    Color originalColor;

    float attackDistanceThreshold = 0.5f;      // 공격거리
    float timeBetweenAttacks = 1f;             // 공격쿨타임

    float nextAttackTime;
    float myCollisionRadius;            // 자신(적)의 반지름
    float targetCollisionRadius;        // 타겟의 반지름

	protected override void Start ()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();  // NavMeshAgent 레퍼런스 가져옴
        skinMaterial = GetComponent<Renderer>().material;       // 에너미에 머테리얼이 바로 달려있는게 아니라 렌더러에서 가져와야함
        originalColor = skinMaterial.color;

        currentState = State.Chasing;
        target = GameObject.FindGameObjectWithTag("Player").transform;      // 타겟을 플레이어 태그로 찾음

        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

        StartCoroutine(UpdatePath());
	}
	
	void Update ()
    {
        if(Time.time > nextAttackTime)
        {
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;       // 목표까지의 제곱거리(Vector3.Distance는 처리비용이 비싸기 때문)
            if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2f))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }

	}

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;     // 공격중 경로계산 하지않음

        Vector3 originalPosition = transform.position;      // 원래 위치
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * myCollisionRadius;  // 공격할 위치     
        
        float attackSpeed = 3f;
        float percent = 0f;

        skinMaterial.color = Color.red;

        while(percent <= 1f)
        {
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2f) + percent) * 4f;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;      // 공격이 끝나면 다시 경로계산 켜줌
    }
    
    IEnumerator UpdatePath()            // 코루틴 사용하여 경로 업데이트
    {
        float refreshRate = 0.25f;

        while (target != null)
        {
            if(currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold * 0.5f);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
