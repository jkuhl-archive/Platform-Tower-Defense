using Gameplay.BoardPieces;
using UnityEngine;

namespace Gameplay.Buffs
{
    public class BleedDebuff : Buff
    {
        private readonly int bleedDamage = 1;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="target"></param>
        /// the object the buff is being applied to
        public BleedDebuff(BoardPiece target) : base(target)
        {
            toggle = false;
            duration = 3;
            timeLeft = duration;
            frequency = (float) .3;
            timeTilRepeat = frequency;
        }

        /// <summary>
        ///     Define what the buff does
        /// </summary>
        /// <param name="target"></param>
        /// The object being buffed
        protected override void Effect()
        {
            if (Target != null)
            {
                Target.TakeDamage(bleedDamage, Target);
                Debug.Log("Ouch I'm bleeding!");
            }
            else
            {
                active = false;
            }
        }

        /// <summary>
        ///     Define what the buff does when it reaches the end of its logic
        /// </summary>
        protected override void ExitEffect()
        {
            Debug.Log("I stopped bleeding");
        }
    }
}