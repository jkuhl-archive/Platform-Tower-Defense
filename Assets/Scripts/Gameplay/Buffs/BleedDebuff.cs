using System;
using Gameplay;
using Gameplay.BoardPieces;
using Gameplay.Buffs;
using UnityEngine;

namespace Buffs
{
    public class BleedDebuff : Buff
    {
        
        private int bleedDamage = 1;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target"></param> the object the buff is being applied to
        public BleedDebuff(BoardPiece target) : base(target)
        {
            this.toggle = false;
            this.duration = 6;
            this.timeLeft = duration;
            this.frequency = 1;
            this.timeTilRepeat = this.frequency;
        }
        
        /// <summary>
        /// Define what the buff does
        /// </summary>
        /// <param name="target"></param> The object being buffed
        protected override void Effect()
        {
            Target.TakeDamage(bleedDamage, Target);
            Debug.Log("Ouch I'm bleeding!");
        }
        protected override void ExitEffect()
        {
            Debug.Log("I stopped bleeding");
        }
    }
}
