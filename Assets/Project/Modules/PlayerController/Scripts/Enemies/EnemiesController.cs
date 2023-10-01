using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    [SerializeField] private GameObject BigEnemies;
    [SerializeField] private GameObject MediumEnemies;
    [SerializeField] private GameObject SmallEnemies;


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
