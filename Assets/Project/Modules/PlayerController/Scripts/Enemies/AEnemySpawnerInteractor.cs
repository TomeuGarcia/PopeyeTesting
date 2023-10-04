using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemySpawnerInteractor : MonoBehaviour
{
    [Header("ENEMY SPAWNER")]
    [SerializeField] private EnemySpawner _enemySpawner;

    private void OnEnable()
    {
        _enemySpawner.OnFirstWaveStarted += OnOnFirstEnemyWaveStartedEvent;
        _enemySpawner.OnAllWavesFinished += OnAllEnemyWavesFinishedEvent;
    }
    private void OnDisable()
    {
        _enemySpawner.OnFirstWaveStarted -= OnOnFirstEnemyWaveStartedEvent;
        _enemySpawner.OnAllWavesFinished -= OnAllEnemyWavesFinishedEvent;
    }


    protected void StartEnemySpawnerWaves()
    {
        _enemySpawner.StartWaves();
    }

    protected abstract void OnOnFirstEnemyWaveStartedEvent();
    protected abstract void OnAllEnemyWavesFinishedEvent();


}
