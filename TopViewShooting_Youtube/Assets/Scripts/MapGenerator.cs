using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;            // 타일의 프리팹
    public Vector2 mapSize;                 // 맵사이즈

    [Range(0,1)]        // Range로 범위를 한정
    public float outlinePercent;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        string holderName = "Generated Map";        // 하이라키상에서 타일들을 가지고 있을(자식으로 가지고 있을) 부모
        if(transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Vector3 tilePosition = new Vector3(-mapSize.x * 0.5f + 0.5f + i, 0f, -mapSize.y * 0.5f + 0.5f + j);     // new Vector3 선언을 통해 정중앙에 오게함
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90f));       // quad이므로 x축으로 90도 회전시켜줌
                newTile.localScale = Vector3.one * (1 - outlinePercent);                                                // 타일의 크기를 줄여 테두리 생성
                newTile.parent = mapHolder;
            }
        }
    }

}
