using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerInteractor_Trigger : AEnemySpawnerInteractor
{
    [SerializeField] private Collider[] _triggers;
    [SerializeField] private AWorldInteractor[] _worldInteractors;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagUtilities.PLAYER_TAG))
        {
            StartEnemySpawnerWaves();
        }
    }

    protected override void OnAllEnemyWavesFinishedEvent()
    {
        foreach (AWorldInteractor worldInteractor in _worldInteractors)
        {
            worldInteractor.AddDeactivationInput();
        }
    }

    protected override void OnOnFirstEnemyWaveStartedEvent()
    {
        foreach (Collider trigger in _triggers)
        {
            trigger.enabled = false;
        }

        foreach (AWorldInteractor worldInteractor in _worldInteractors)
        {
            worldInteractor.AddActivationInput();
        }
    }
}
