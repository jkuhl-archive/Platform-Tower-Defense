using System;
using Buffs;
using Gameplay.BoardPieces;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Buffs
{
    [Serializable]
    public class Buff

    {
        [Header("Buff Parameters")] 
        public bool active = true;
        public float duration; //How long the buff should be applied
        protected float timeLeft; //
        public bool toggle; //Does this buff happen just once, or does it effect an attribute repetitively
        public float frequency; //How long between effect repetitions
        public float timeTilRepeat;
        protected BoardPiece Target;
        
        
        /// <summary>
        /// Iterate through the list of buffs and process their respective logic
        /// </summary>
        /// <param name="time">Time between frames which we will pass to the buffs to process their logic</param>
        /// <returns></returns>
        public bool ProcessBuff(float time)
        {
            if (Target is null)
            {
                active = false;
                ExitEffect();
            }
            timeLeft -= time;
            if (timeLeft <= 0 && active)
            {
                ExitEffect();
                active = false;
                return active;
            }
            if (!toggle && active)
            {
                timeTilRepeat -= time;
                if (timeTilRepeat <= 0)
                {
                    Effect();
                    timeTilRepeat = frequency;
                }
            }
            return active;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target"></param> The object the buff is being applied to
        protected Buff(BoardPiece target)
        {
            Target = target;
            if (!toggle)
            {
                frequency = duration;
            }
            
            Effect();
        }
        /// <summary>
        /// Define what the buff does
        /// </summary>
        /// <param name="target">What the buff is being applied to</param>
        protected virtual void Effect()
        {
        }
        /// <summary>
        /// Define what, if anything, the buff does upon completion.
        /// </summary>
        protected virtual void ExitEffect()
        {
            
        }
    }
}
