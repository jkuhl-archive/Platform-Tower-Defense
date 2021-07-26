using Gameplay;
using Gameplay.BoardPieces;

namespace Buffs
{
    public class BleedDebuff : Buff
    {
        public int bleedDamage;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target"></param> the object the buff is being applied to
        public BleedDebuff(BoardPiece target) : base(target)
        {
        
        }
        
        /// <summary>
        /// Define what the buff does
        /// </summary>
        /// <param name="target"></param> The object being buffed
        protected override void Effect(BoardPiece target)
        {
            target.TakeDamage(bleedDamage, null);
        }
    }
}
