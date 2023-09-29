using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuadraticBezierCurve
{
    public Vector3 P0 { get; set; }
    public Vector3 P1 { get; set; }
    public Vector3 P2 { get; set; }
    public Vector3 P3 { get; set; }


    public QuadraticBezierCurve()
    {
        P0 = P1 = P2 = P3 = Vector3.zero;
    }
    public QuadraticBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        P0 = p0;
        P1 = p1;
        P2 = p2;
        P3 = p3;
    }


    public Vector3 GetPoint(float t)
    {
        float tPow2 = t * t;
        float tPow3 = tPow2 * t;

        Vector3 point = 
            P0 * (-tPow3 + (3*tPow2) - (3*t) + 1) +
            P1 * ((3 * tPow3) - (6 * tPow2) + (3*t)) +
            P2 * ((-3 * tPow3) + (3 * tPow2)) +
            P3 * tPow3;

        return point;
    }
    
    public Vector3 GetVelocity(float t)
    {
        float tPow2 = t * t;

        Vector3 velocity =
            P0 * ((-3 * tPow2) + (6 * t) - 3) +
            P1 * ((9 * tPow2) - (12 * t) + 3) +
            P2 * ((-9 * tPow2) + (6 * t)) +
            P3 * (3 * tPow2);

        return velocity;
    }
    
    public Vector3 GetAcceleration(float t)
    {
        Vector3 acceleration =
            P0 * ((-6 * t) + 6) +
            P1 * ((18 * t) - 12) +
            P2 * ((-18 * t) + 6) +
            P3 * (6 * t);

        return acceleration;
    }
    
    public Vector3 GetJerk()
    {
        Vector3 jerk =
            P0 * -6 +
            P1 * 18 +
            P2 * -18 +
            P3 * 6;

        return jerk;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

}
