using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticBezierCurveBehaviour : MonoBehaviour
{
    private QuadraticBezierCurve _curve;

    [SerializeField] private LineRenderer _line;
    [SerializeField, Range(1, 50)] private int _numberOfSegments = 20;    

    [SerializeField] private Transform t0;
    [SerializeField] private Transform t1;
    [SerializeField] private Transform t2;
    [SerializeField] private Transform t3;
    
    public Transform T0 => t0;
    public Transform T1 => t1;
    public Transform T2 => t2;
    public Transform T3 => t3;


    private void Awake()
    {
        AwakeInit();
    }

    private void Update()
    {
        UpdatePositions();
        DrawLine();
    }


    private void AwakeInit()
    {
        _curve = new QuadraticBezierCurve(T0.position, T1.position, T2.position, T3.position);
    }

    private void UpdatePositions()
    {
        _curve.P0 = T0.position;
        _curve.P1 = T1.position;
        _curve.P2 = T2.position;
        _curve.P3 = T3.position;
    }

    private void DrawLine()
    {
        _line.positionCount = _numberOfSegments + 1;

        float step = 1.0f / _numberOfSegments;
        float t = 0.0f;

        for (int i = 0; i <= _numberOfSegments; ++i)
        {
            _line.SetPosition(i, _curve.GetPoint(t));
            t += step;
        }
    }

}
