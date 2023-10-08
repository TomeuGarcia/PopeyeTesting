using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitEffect : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private Transform _meshTransform;
    [SerializeField, Range(0.0f, 10.0f)] private float _duration = 0.5f;
    [SerializeField] private Color _color = Color.white;


    public float Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }

    public Vector3 LocalScale
    {
        get { return _meshTransform.localScale; }
        set {
            value.y = _meshTransform.localScale.y;
            _meshTransform.localScale = value; 
        }
    }


    private IEnumerator Start()
    {
        Material material = _mesh.material;

        material.SetFloat("_StartTime", Time.time);
        material.SetFloat("_WaveDuration", _duration);
        material.SetColor("_WaveColor", _color);

        yield return new WaitForSeconds(_duration);

        Destroy(gameObject);
    }




}
