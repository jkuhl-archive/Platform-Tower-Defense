using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedDebuff : Buff
{
    // Start is called before the first frame update
    public BleedDebuff(GameObject target) : base(target)
    {
        
    }

    public void Effect()
    {
        //target.TakeDamage(1);
        //base.Effect();
    }
}
