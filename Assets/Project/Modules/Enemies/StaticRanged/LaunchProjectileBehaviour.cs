using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private float relativeSpeed;


    public void Launch()
    {
        var projectileInstance = Instantiate(projectilePrefab, spawnPoint.position,
            Quaternion.LookRotation(spawnPoint.transform.forward));
        

        if (projectileInstance.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = rb.transform.TransformDirection(spawnPoint.transform.forward * relativeSpeed);
        }
    }
}
