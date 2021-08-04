using Gameplay.BoardPieces;
using UnityEngine;

namespace Gameplay.Buffs
{
    public class Buff
    {
        // Buff effect variables
        private readonly bool isBuffRepeating;
        private readonly float buffDuration;
        private readonly float buffRepeatFrequency;

        // Object the buff is being applied to 
        protected readonly BoardPiece target;

        // Buff state parameters
        protected bool buffActive = true;
        private float timeUntilBuffExpires;
        private float timeUntilRepeat;

        /// <summary>
        ///     Base buff constructor
        /// </summary>
        /// <param name="target"> The object the buff is being applied to </param>
        /// <param name="isBuffRepeating"> Should the buff effect be applied multiple times on a time interval </param>
        /// <param name="buffDuration"> Amount of time the buff effect should be active </param>
        /// <param name="buffRepeatFrequency"> Amount of time between buff effect application for repeating buffs </param>
        protected Buff(BoardPiece target, bool isBuffRepeating, float buffDuration, float buffRepeatFrequency)
        {
            this.target = target;
            this.isBuffRepeating = isBuffRepeating;
            this.buffDuration = buffDuration;
            this.buffRepeatFrequency = buffRepeatFrequency;

            // Set buff timers based on input duration and repeat frequency
            timeUntilBuffExpires = buffDuration;
            if (isBuffRepeating) timeUntilRepeat = buffRepeatFrequency;

            Effect();
        }

        /// <summary>
        ///     Defines buff logic
        /// </summary>
        protected virtual void Effect()
        {
        }

        /// <summary>
        ///     Define what, if anything, the buff does upon completion.
        /// </summary>
        protected virtual void ExitEffect()
        {
        }

        /// <summary>
        ///     Handles applying the buff's effect if it is time to do so
        /// </summary>
        /// <returns> Returns true if the buff is active, false if not </returns>
        public bool ProcessBuff()
        {
            if (target is null)
            {
                buffActive = false;
                ExitEffect();
            }

            // What to do when time runs out
            timeUntilBuffExpires -= Time.deltaTime;
            if (timeUntilBuffExpires <= 0 && buffActive)
            {
                ExitEffect();
                buffActive = false;
            }

            // If this isn't a toggle effect, process the frequency
            if (isBuffRepeating && buffActive)
            {
                timeUntilRepeat -= Time.deltaTime;
                if (timeUntilRepeat <= 0)
                {
                    Effect();
                    timeUntilRepeat = buffRepeatFrequency;
                }
            }

            return buffActive;
        }
    }
}