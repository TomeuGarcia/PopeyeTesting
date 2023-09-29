using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VerletSolver
{
    public class VerletSolver
    {
        public Vector3 GravityForce { get; set; }

        private List<VerletParticle> _particles;
        private List<VerletStick> _sticks;


        public VerletSolver(Vector3 gravityForce, int particlesIntention = 10)
        {
            GravityForce = gravityForce;
            _particles = new List<VerletParticle>(particlesIntention);
            _sticks = new List<VerletStick>(particlesIntention-1);
        }


        public void AddParticle(VerletParticle particle)
        {
            _particles.Add(particle);
        }
        
        public void AddParticles(VerletParticle[] particles)
        {
            _particles.AddRange(particles);
        }

        public void RemoveAllParticles()
        {
            _particles.Clear();
        }
        

        public void AddStick(VerletStick stick)
        {
            _sticks.Add(stick);
        }
        public void AddSticks(VerletStick[] sticks)
        {
            _sticks.AddRange(sticks);
        }
        public void RemoveAllSticks()
        {
            _sticks.Clear();
        }

        public void ConnectParticlesWithSticks()
        {
            VerletStick[] sticks = new VerletStick[_particles.Count - 1];
            for (int i = 0; i < _particles.Count - 1; ++i)
            {
                VerletParticle firstParticle = _particles[i];
                VerletParticle secondParticle = _particles[i+1];
                sticks[i] = new VerletStick(firstParticle, secondParticle, Vector3.Distance(firstParticle.CurrentPosition, secondParticle.CurrentPosition));
            }

            AddSticks(sticks);
        }


        public void Step(float deltaTime)
        {
            StepParticles(deltaTime);
            ApplyConstraints();
        }

        private void StepParticles(float deltaTime)
        {
            foreach (VerletParticle particle in _particles)
            {
                Vector3 newPosition = (particle.CurrentPosition * 2) - particle.PreviousPosition + (GravityForce / particle.Mass * deltaTime * deltaTime);

                particle.PreviousPosition = particle.CurrentPosition;
                particle.CurrentPosition = newPosition;
            }                
        }



        private void ApplyConstraints()
        {
            StepSticks();
        }
  

        private void StepSticks()
        {
            for (int i = 0; i < _sticks.Count; ++i)
            {
                VerletStick stick = _sticks[i];

                Vector3 diff = stick.FirstParticle.CurrentPosition - stick.SecondParticle.CurrentPosition;
                float diffMagnitude = diff.magnitude;
                float diffFactor = (stick.Length - diffMagnitude) / diffMagnitude * 0.5f;
                Vector3 offset = diff * diffFactor;

                stick.FirstParticle.CurrentPosition += offset;
                stick.SecondParticle.CurrentPosition -= offset;
            }
        
        }
    

    }

}


