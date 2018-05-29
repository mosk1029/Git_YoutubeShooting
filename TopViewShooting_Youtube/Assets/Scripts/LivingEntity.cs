using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;     // 체력
    protected bool dead;        // 죽었는지 아닌지 확인해주는 파라미터

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public void TakeHit(float _damage, RaycastHit _hit)
    {
        health -= _damage;

        if (health <= 0f && !dead)       // 체력이 0 이하로 떨어지면 Die 메소드 호출
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        Destroy(gameObject);
    }
}
