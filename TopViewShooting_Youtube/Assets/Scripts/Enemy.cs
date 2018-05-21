using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    NavMeshAgent pathfinder;        // 에너미 NavMeshAgent 캐싱
    Transform target;               // 타겟 캐싱

	void Start ()
    {
        pathfinder = GetComponent<NavMeshAgent>();  // NavMeshAgent 레퍼런스 가져옴
        target = GameObject.FindGameObjectWithTag("Player").transform;      // 타겟을 플레이어 태그로 찾음

        StartCoroutine(UpdatePath());
	}
	
	void Update ()
    {
	}

    IEnumerator UpdatePath()            // 코루틴 사용하여 경로 업데이트
    {
        float refreshRate = 0.25f;

        while (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0f, target.position.z);
            pathfinder.SetDestination(targetPosition);

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
