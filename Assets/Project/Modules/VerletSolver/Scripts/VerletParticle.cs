using UnityEngine;


namespace VerletSolver
{
    public class VerletParticle
    {
        public Vector3 CurrentPosition { get; set; }
        public Vector3 PreviousPosition { get; set; }
        public float Mass { get; set; }


        public VerletParticle(Vector3 position, float mass = 1.0f)
        {
            PreviousPosition = CurrentPosition = position;
            Mass = mass;
        }


    }

}

