using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private Coroutine destroyBullet;
    private void OnCollisionEnter(Collision other)
    {
        StopCoroutine(destroyBullet);
        Destroy(gameObject);
    }

    private void Start()
    {
        destroyBullet = StartCoroutine(DestroyBullet());
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
