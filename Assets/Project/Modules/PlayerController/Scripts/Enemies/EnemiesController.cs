using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    [SerializeField] private bool _initEnemies = true;
    [SerializeField] private bool _enemiesRespawn = true;

    [Header("PARENTS")]
    [SerializeField] private GameObject BigEnemies;
    [SerializeField] private GameObject MediumEnemies;
    [SerializeField] private GameObject SmallEnemies;

    [Header("REFERENCES")]
    [SerializeField] private Transform _enemyAttackTarget;
    private Enemy[] _enemies;



    private void Awake()
    {
        if (!_initEnemies) return;

        int numberOfBigEnemies = BigEnemies.transform.childCount;
        int numberOfMediumEnemies = MediumEnemies.transform.childCount;
        int numberOfSmallEnemies = SmallEnemies.transform.childCount;

        int numberOfEnemies = numberOfBigEnemies + numberOfMediumEnemies + numberOfSmallEnemies;
        _enemies = new Enemy[numberOfEnemies];

        int enemyI = 0;
        for (int i = 0; i < numberOfBigEnemies; ++i, ++enemyI)
        {
            Enemy enemy = BigEnemies.transform.GetChild(i).GetComponent<Enemy>();            
            enemy.AwakeInit(_enemyAttackTarget, _enemiesRespawn);
            enemy.SetRespawnPosition(enemy.Position);

            _enemies[enemyI] = enemy;
        }

        for (int i = 0; i < numberOfMediumEnemies; ++i, ++enemyI)
        {
            Enemy enemy = MediumEnemies.transform.GetChild(i).GetComponent<Enemy>();
            enemy.AwakeInit(_enemyAttackTarget, _enemiesRespawn);
            enemy.SetRespawnPosition(enemy.Position);

            _enemies[enemyI] = enemy;
        }
        
        for (int i = 0; i < numberOfSmallEnemies; ++i, ++enemyI)
        {
            Enemy enemy = SmallEnemies.transform.GetChild(i).GetComponent<Enemy>();
            enemy.AwakeInit(_enemyAttackTarget, _enemiesRespawn);
            enemy.SetRespawnPosition(enemy.Position);

            _enemies[enemyI] = enemy;
        }

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BigEnemies.SetActive(!BigEnemies.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MediumEnemies.SetActive(!MediumEnemies.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SmallEnemies.SetActive(!SmallEnemies.activeInHierarchy);
        }
    }
}
