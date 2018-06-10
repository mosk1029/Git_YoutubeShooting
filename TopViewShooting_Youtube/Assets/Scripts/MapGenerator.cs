using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;            // 타일의 프리팹
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 maxMapSize;              // 최대 맵 크기

    [Range(0,1)]        // Range로 범위를 한정
    public float outlinePercent;            // 테두리 퍼센트

    public float tileSize;

    List<Coord> allTileCoords;              // 모든 타일의 xy를 담아내는 리스트
    Queue<Coord> shuffledTileCoords;        // 셔플된 타일의xy를 담아내는 큐
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tileMap;                   // 2차원 배열의 Transform(Enemy 스폰할때 사용)

    Map currentMap;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);

        // LivingEntity가 바닥에서 돌아다닐 수 있게 콜라이더 만들어줌
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, tileSize * 0.05f, currentMap.mapSize.y * tileSize);

        // Generating Coords
        allTileCoords = new List<Coord>();
        for (int i = 0; i < currentMap.mapSize.x; i++)
        {
            for (int j = 0; j < currentMap.mapSize.y; j++)
            {
                allTileCoords.Add(new Coord(i, j));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // Creating map holder object
        string holderName = "Generated Map";        // 하이라키상에서 타일들을 가지고 있을(자식으로 가지고 있을) 부모
        if(transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90f));       // quad이므로 x축으로 90도 회전시켜줌
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;                                     // 타일의 크기를 줄여 테두리 생성
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        // spawning obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)((currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent));
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if(randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight * 0.5f, Quaternion.identity);
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);        //obstacleRenderer.material가 아니라 공유된머테리얼 할당
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }

            shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));
        }

        // Creating navmesh mask
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) * 0.25f * tileSize, Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) * 0.5f, 1f, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) * 0.25f * tileSize, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) * 0.5f, 1f, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) * 0.25f * tileSize, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1f, (maxMapSize.y - currentMap.mapSize.y) * 0.5f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) * 0.25f * tileSize, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1f, (maxMapSize.y - currentMap.mapSize.y) * 0.5f) * tileSize;



        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] _obstacleMap, int _currentObstacleCount)
    {
        bool[,] mapFlags = new bool[_obstacleMap.GetLength(0), _obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

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

        int targetAccessibleTIleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - _currentObstacleCount);

        return targetAccessibleTIleCount == accessibleTileCount;                // true나 false로 반환한다
    }

    Vector3 CoordToPosition(int _x, int _y)
    {
        return new Vector3(-currentMap.mapSize.x * 0.5f + 0.5f + _x, 0f, -currentMap.mapSize.y * 0.5f + 0.5f + _y) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 _position)
    {
        int x = Mathf.RoundToInt(_position.x / tileSize + (currentMap.mapSize.x - 1) * 0.5f);   // Mathf.RoundToInt float을 int로 변환(반올림) 그냥 (int)로 형변환하면 무조건 내림
        int y = Mathf.RoundToInt(_position.z / tileSize + (currentMap.mapSize.y - 1) * 0.5f);

        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);

        return tileMap[x, y];
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    public Transform GetRandomOpenTIle()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);

        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
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

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;               // 맵의 크기를 정수로 나타내기 위해 Coord 구조체 사용
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter
        {
            get
            {
                return new Coord((int)(mapSize.x * 0.5f), (int)(mapSize.y * 0.5f));
            }
        }
    }
}
