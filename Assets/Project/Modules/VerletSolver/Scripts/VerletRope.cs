using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VerletSolver
{

    public class VerletRope : MonoBehaviour
    {
        private VerletSolver verletSolver;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private int _numberOfSegments = 20;
        [SerializeField] private float _ropeWidth = 0.2f;
        [SerializeField] private float _segmentLength= 0.25f;

        private VerletParticle[] _particles;
        private Vector3[] _linePositions;



        private void Awake()
        {
            verletSolver = new VerletSolver(Vector3.down * 9.8f);

            _particles = new VerletParticle[_numberOfSegments];
            for (int i = 0; i < _numberOfSegments; ++i)
            {
                _particles[i] = new VerletParticle(transform.position + Vector3.right * _segmentLength * i);
            }

            verletSolver.AddParticles(_particles);
            verletSolver.ConnectParticlesWithSticks();

            _linePositions = new Vector3[_particles.Length];
        }


        private void Update()
        {            
            DrawRope();
        }

        private void FixedUpdate()
        {
            verletSolver.Step(Time.fixedDeltaTime);
            _particles[0].CurrentPosition = transform.position;
        }


        private void DrawRope()
        {
            _lineRenderer.startWidth = _ropeWidth;
            _lineRenderer.endWidth = _ropeWidth;

            for (int i = 0; i < _particles.Length; ++i)
            {
                _linePositions[i] = _particles[i].CurrentPosition;
            }

            _lineRenderer.positionCount = _linePositions.Length;
            _lineRenderer.SetPositions(_linePositions);
        }


    }


}


