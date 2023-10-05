using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSnapTarget : MonoBehaviour
{

    [SerializeField] private Transform _snapSpot;
    public Vector3 SnapPosition => _snapSpot.position;
}
