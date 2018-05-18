using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;    // 총을 들고 있는 위치
    public Gun startingGun;         // 시작할 때 총
    Gun equippedGun;                // 현재 장착중인 총

    void Start()
    {
        // 시작총이 null이 아니면 시작총 장착
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }

    public void EquipGun(Gun _gunToEquip)           // 총을 장착하는 메소드. _gunToEquip은 장착할 총
    {
        // 이미 장착중인 총이 있다면 그 총의 게임오브젝트를 파괴함
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(_gunToEquip, weaponHold.position, weaponHold.rotation);     // 장착중인 총을 장착할 총으로 바꿔줌
                                                                                              // as Gun으로 형변환해줘야하는데 버전업되면서 템플릿 적용되서 오류 안나는듯
        equippedGun.transform.parent = weaponHold;                                            // weaponHold를 장착중인 총의 부모로 설정
    }

}
