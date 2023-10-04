using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class EnemySpawner : MonoBehaviour
{

    [System.Serializable]
    class EnemyWave
    {
        [SerializeField, Range(0.0f, 10.0f)] private float _delayBeforeWaveSpawning = 1.0f;
        [SerializeField] private SpawnSequenceBeat[] _spawnSequence;

        public float DelayBeforeWaveSpawning => _delayBeforeWaveSpawning;
        public SpawnSequenceBeat[] SpawnSequence => _spawnSequence;
        public int NumberOfEnemies => _spawnSequence.Length;


        [System.Serializable]
        public class SpawnSequenceBeat
        {
            [SerializeField, Range(0.0f, 10.0f)] private float _delayBeforeSpawn = 0.5f;
            [SerializeField] private Enemy _enemyPrefab;
            [SerializeField] private Transform _spawnSpot;

            public float DelayBeforeSpawn => _delayBeforeSpawn;
            public Enemy EnemyPrefab => _enemyPrefab;
            public Vector3 SpawnPosition => _spawnSpot.position;
        }

    }



    [SerializeField] private Transform _enemyAttackTarget;
    [SerializeField] private EnemyWave[] _enemyWaves;
    private int _activeEnemiesCount;
    private bool AllCurrentWaveEnemiesAreDead => _activeEnemiesCount == 0;

    public delegate void EnemySpawnerEvent();
    public EnemySpawnerEvent OnFirstWaveStarted;
    public EnemySpawnerEvent OnAllWavesFinished;



    public void StartWaves()
    {
        DoStartWaves().Forget();
    }


    private async UniTaskVoid DoStartWaves()
    {
        OnFirstWaveStarted?.Invoke();

        for (int waveI = 0; waveI < _enemyWaves.Length; ++waveI)
        {
            await SpawnEnemyWave(_enemyWaves[waveI]);
            await UniTask.WaitUntil(() => AllCurrentWaveEnemiesAreDead);
        }

        OnAllWavesFinished?.Invoke();
    }

    private async UniTask SpawnEnemyWave(EnemyWave enemyWave)
    {
        await UniTask.Delay((int)(enemyWave.DelayBeforeWaveSpawning * 1000));


        _activeEnemiesCount = enemyWave.NumberOfEnemies;

        for (int i = 0; i < enemyWave.SpawnSequence.Length; ++i)
        {
            EnemyWave.SpawnSequenceBeat spawnSequenceBeat = enemyWave.SpawnSequence[i];

            await UniTask.Delay((int)(spawnSequenceBeat.DelayBeforeSpawn * 1000));
            SpawnEnemy(spawnSequenceBeat.EnemyPrefab, spawnSequenceBeat.SpawnPosition);
        }
    }

    private void SpawnEnemy(Enemy enemyPrefab, Vector3 spawnPosition)
    {
        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.AwakeInit(_enemyAttackTarget, false);

        enemy.OnDeathAnimationFinished += DecrementActiveEnemiesCount;
    }

    

    private void DecrementActiveEnemiesCount(Enemy destroyedEnemy)
    {
        destroyedEnemy.OnDeathAnimationFinished -= DecrementActiveEnemiesCount;

        --_activeEnemiesCount;
    }

}
