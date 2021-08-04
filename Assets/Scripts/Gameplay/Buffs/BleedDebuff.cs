using Gameplay.BoardPieces;
using UnityEngine;

namespace Gameplay.Buffs
{
    public class BleedDebuff : Buff
    {
        private const int BleedDamage = 1;
        private const bool IsBuffRepeating = true;
        private const float BuffDuration = 30;
        private const float BuffRepeatFrequency = .3f;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="target"> Object the buff is being applied to </param>
        public BleedDebuff(BoardPiece target) : base(target, IsBuffRepeating, BuffDuration, BuffRepeatFrequency)
        {
        }

        /// <summary>
        ///     Define what the buff does
        /// </summary>
        protected override void Effect()
        {
            if (target != null)
            {
                base.Effect();
                target.TakeDamage(BleedDamage, target);
                Debug.Log("Ouch I'm bleeding!");
            }
            else
            {
                buffActive = false;
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