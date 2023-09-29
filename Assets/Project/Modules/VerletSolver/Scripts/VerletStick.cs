using UnityEngine;


namespace VerletSolver
{
    public class VerletStick
    {
        public VerletParticle FirstParticle { get; set; }
        public VerletParticle SecondParticle { get; set; }
        public float Length { get; set; }

        public VerletStick(VerletParticle firstParticle, VerletParticle secondParticle, float length)
        {
            FirstParticle = firstParticle;
            SecondParticle = secondParticle;
            Length = length;
        }


    }

}

