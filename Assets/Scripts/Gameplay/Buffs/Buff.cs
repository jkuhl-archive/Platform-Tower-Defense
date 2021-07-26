using Gameplay;
using Gameplay.BoardPieces;
using UnityEngine;

namespace Buffs
{
    public abstract class Buff
    {
        [Header("Buff Parameters")]
        public float duration; //How long the buff should be applied
        public bool toggle; //Does this buff happen just once, or does it effect an attribute repetitively
        public float frequency; //How long between effect repetitions
        protected BoardPiece target;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target"></param> The object the buff is being applied to
        public Buff(BoardPiece target)
        {
            this.target = target;
            if (toggle == true)
            {
                frequency = duration;
            }
        }
        /// <summary>
        /// Define what the buff does
        /// </summary>
        /// <param name="target">What the buff is being applied to</param>
        protected virtual void Effect(BoardPiece target)
        {
        }
    }
}
