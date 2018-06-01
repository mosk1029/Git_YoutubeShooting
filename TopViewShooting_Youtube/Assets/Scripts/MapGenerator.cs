using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;            // 타일의 프리팹
    public Transform obstaclePrefab;
    public Vector2 mapSize;                 // 맵사이즈

    [Range(0,1)]        // Range로 범위를 한정
    public float outlinePercent;            // 테두리 퍼센트
    [Range(0,1)]
    public float obstaclePercent;           // 장애물 비중(전체 타일에 대한)

    List<Coord> allTileCoords;              // 모든 타일의 xy를 담아내는 리스트
    Queue<Coord> shuffledTileCoords;        // 셔플된 타일의xy를 담아내는 큐

    public int seed = 10;
    Coord mapCenter;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                allTileCoords.Add(new Coord(i, j));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCenter = new Coord((int)(mapSize.x * 0.5), (int)(mapSize.y * 0.5));

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
                Vector3 tilePosition = CoordToPosition(i, j);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90f));       // quad이므로 x축으로 90도 회전시켜줌
                newTile.localScale = Vector3.one * (1 - outlinePercent);                                                // 타일의 크기를 줄여 테두리 생성
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)((mapSize.x * mapSize.y * obstaclePercent));
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if(randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
                newObstacle.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
    }

    bool MapIsFullyAccessible(bool[,] _obstacleMap, int _currentObstacleCount)
    {
        bool[,] mapFlags = new bool[_obstacleMap.GetLength(0), _obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;

                    if (x == 0 || y == 0)
                    {
                        if (neighborX >= 0 && neighborX < _obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < _obstacleMap.GetLength(1))
                        {
                            if(!mapFlags[neighborX, neighborY] && !_obstacleMap[neighborX,neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTIleCount = (int)(mapSize.x * mapSize.y - _currentObstacleCount);

        return targetAccessibleTIleCount == accessibleTileCount;                // true나 false로 반환한다
    }

    Vector3 CoordToPosition(int _x, int _y)
    {
        return new Vector3(-mapSize.x * 0.5f + 0.5f + _x, 0f, -mapSize.y * 0.5f + 0.5f + _y);
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    public struct Coord                 // 2차원 배열을 안쓰고 구조체로 만들어서 생성자를 통해 x, y좌표를 저장할 수 있다
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

}
