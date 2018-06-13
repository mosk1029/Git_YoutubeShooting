using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;    // 인스펙터에서 지정해줌
    public Enemy enemy;     // 인스펙터에서 레퍼런스 할당

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;       // 현재 웨이브에 대한 레퍼런스
    int currentWaveNumber;      // 현재 웨이브 넘버

    int enemiesRemainingToSpawn;    // 남아있는 스폰할 적
    int enemiesRemainingAlive;      // 살아있는 적의 숫자
    float nextSpawnTime;            // 다음 스폰타임

    MapGenerator map;

    float timeBetweenCampingChecks = 2f;     // 캠핑 체크 시간 간격
    float campThresholdDistance = 1.5f;      // 캠핑한걸로 간주되지 않기 위해 필요한 최소 이동거리
    float nextCampCheckTime;
    Vector3 campPositionOld;                // 가장 최근 체크시 플레이어 위치
    bool isCamping;                         // 캠핑여부

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update()
    {
        if(!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)        // 스폰할 적이 남아있고 스폰타임이 지났으면 스폰함
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1f;              // 적이 스폰되기 전에 얼마나 깜빡일지
        float tileFlashSpeed = 4f;          // 초당 몇번 깜빡일지(깜빡임 속도)

        Transform spawnTile = map.GetRandomOpenTIle();
        
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0f;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1f));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3f;
    }

    void NextWave()                 // 다음 웨이브를 불러오는 메소드
    {
        currentWaveNumber++;

        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];     // 배열인덱스는 0부터 시작하므로

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }

            ResetPlayerPosition();
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
