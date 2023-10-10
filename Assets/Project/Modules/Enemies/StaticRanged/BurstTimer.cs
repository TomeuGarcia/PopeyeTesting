using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class BurstTimer : MonoBehaviour
{
    [SerializeField] private UnityEvent Shoot = new UnityEvent();
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float timeBetweenBursts;
    [SerializeField] private float bulletsPerBurst;
    private bool active = false;

    private float timer=0;
    private float resetTimer=0;
    private float bulletsShot=0;

    private void Update()
    {
        if (active)
        {
            if (bulletsShot < bulletsPerBurst)
            {
                if (timer >= timeBetweenShots)
                {
                    Shoot.Invoke();
                    timer = 0;
                    bulletsShot += 1;
                }

                timer += Time.deltaTime;
            }
            else
            {
                if (resetTimer >= timeBetweenBursts)
                {
                    bulletsShot = 0;
                    timer = 0;
                    resetTimer = 0;
                }

                resetTimer += Time.deltaTime;
            }
        }
    }

    public void Activate()
    {
        timer = 0;
        resetTimer = 0;
        bulletsShot = 0;
        active = true;
    }
    public void DeActivate()
    {
        active = false;
    }
}
