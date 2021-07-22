using Gameplay;
using UnityEngine;

public class BleedDebuff : Buff
{
    public int bleedDamage;
    public BleedDebuff(BoardPiece target) : base(target)
    {
        
    }

    protected override void Effect(BoardPiece target)
    {
        target.TakeDamage(bleedDamage, null);
    }
}
