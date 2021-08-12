using Gameplay.BoardPieces;
using UnityEngine;

namespace Gameplay.BuffLogic.Buffs
{
    public class BleedDebuff : Buff
    {
        private const int BleedDamage = 1;
        private const bool IsBuffRepeating = true;
        private const float BuffDuration = 3;
        private const float BuffRepeatFrequency = .3f;
        private const string EffectMaterialName = "Bleed";

        /// <summary>
        ///     Empty constructor
        /// </summary>
        public BleedDebuff() : base(IsBuffRepeating, BuffDuration, BuffRepeatFrequency, EffectMaterialName)
        {
        }

        /// <summary>
        ///     Define what the buff does
        /// </summary>
        protected override void Effect(GameObject target)
        {
            if (target == null)
            {
                buffActive = false;
                return;
            }
            
            base.Effect(target);
            
            BoardPieceLogic targetBoardPieceLogic = target.GetComponent<BoardPieceLogic>();
            if (targetBoardPieceLogic != null)
            {
                if (targetBoardPieceLogic.IsAlive())
                {
                    targetBoardPieceLogic.TakeDamage(BleedDamage, target);
                }
            }
        }

        /// <summary>
        ///     Define what the buff does when it reaches the end of its logic
        /// </summary>
        protected override void ExitEffect(GameObject target)
        {
            base.ExitEffect(target);
        }
    }
}