using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]              // PlayerController 스크립트가 있는 오브젝트에는 Rigidbody 컴포넌트가 강제로 붙어있어야함
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;       // RIgidbody 캐싱

	void Start ()
    {
        myRigidbody = GetComponent<Rigidbody>();
	}
    
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    
    public void LookAt(Vector3 _lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(_lookPoint.x, transform.position.y, _lookPoint.z);       // y좌표를 고정시킨 마우스포인트 좌표
        transform.LookAt(heightCorrectedPoint);                                                             // heightCorrectedPoint를 바라보게 함
    }

    public void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);        // 실제 이동값을 받아서 이동시키는 부분
    }
}
