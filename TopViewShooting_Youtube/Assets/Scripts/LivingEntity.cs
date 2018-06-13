using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;     // 체력
    protected bool dead;        // 죽었는지 아닌지 확인해주는 파라미터

    public event System.Action OnDeath;     // 죽었을때 발생하는 이벤트

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float _damage, RaycastHit _hit)
    {
        // Do some stuff with hit variable
        TakeDamage(_damage);
    }

    public void TakeDamage(float _damage)
    {
        health -= _damage;

        if (health <= 0f && !dead)       // 체력이 0 이하로 떨어지면 Die 메소드 호출
        {
            Die();
        }
    }
    [ContextMenu("Self Destruct")]      // 스크립트 선택창에서 클릭하여 아래 메소드 강제 실행
    protected void Die()
    {
        dead = true;

        if(OnDeath != null)
        {
            OnDeath();
        }
        Destroy(gameObject);
    }
}
