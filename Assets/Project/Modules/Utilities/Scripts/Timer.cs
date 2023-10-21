using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float _duration;
    private float _counter;

    
    public Timer(float duration)
    {
        _duration = duration;
        _counter = 0;
    }


    public void Update(float deltaTime)
    {
        _counter += deltaTime;        
    }

    public bool HasFinished()
    {
        return _counter >= _duration;
    }

    public float GetCounterRatio01()
    {
        return Mathf.Min(1.0f, _counter / _duration);
    }

}
