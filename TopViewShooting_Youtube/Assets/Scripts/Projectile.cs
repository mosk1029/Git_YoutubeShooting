using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 10f;          // 총알의 속도

    public void SetSpeed

	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);      // 총알 발사
	}   
}
