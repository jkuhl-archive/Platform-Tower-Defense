using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class Buff
{
    public float duration = 1;
    public float frequency = 1;
    protected GameObject target;
    

    public Buff(GameObject target)
    {
        this.target = target;
    }

    // Define what the buff does
    public virtual void Effect(GameObject target)
    {
        
    }
}
