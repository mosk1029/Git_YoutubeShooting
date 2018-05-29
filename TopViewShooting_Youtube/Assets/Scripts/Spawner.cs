using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;    // 인스펙터에서 지정해줌
    public Enemy enemy;     // 인스펙터에서 레퍼런스 할당

    Wave currentWave;       // 현재 웨이브에 대한 레퍼런스
    int currentWaveNumber;      // 현재 웨이브 넘버

    int enemiesRemainingToSpawn;    // 남아있는 스폰할 적
    int enemiesRemainingAlive;      // 살아있는 적의 숫자
    float nextSpawnTime;            // 다음 스폰타임

    void Start()
    {
        NextWave();
    }

    void Update()
    {
        if(enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)        // 스폰할 적이 남아있고 스폰타임이 지났으면 스폰함
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void NextWave()                 // 다음 웨이브를 불러오는 메소드
    {
        currentWaveNumber++;
        Debug.Log("Wave : " + currentWaveNumber);
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];     // 배열인덱스는 0부터 시작하므로

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
