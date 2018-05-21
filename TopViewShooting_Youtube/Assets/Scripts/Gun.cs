using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;                      // 총구의 위치(인스팩터 상에서 정해줌)
    public Projectile projectile;                 // 총알 스크립트 캐싱(인스팩터 상에서 정해줌)
    public float msBetweenShots = 100f;           // 총알의 연사력(밀리세컨드)
    public float muzzleVelocity = 35f;            // 총알 발사시 속력

    float nextShotTime;

    public void Shoot()                             // 실제로 총알발사 구현부분(호출은 GunController에서 함)
    {
        if(Time.time > nextShotTime)                // 연사시간이 지났을 때만 총 발사
        {
            nextShotTime = Time.time + msBetweenShots * 0.001f;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);     // 총알 속력 할당
        }
    }
}
