using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class BleedAmmoLogic : AmmoLogic
{
    /// <summary>
    /// Define what happens when the bullet collides with the target
    /// </summary>

    new void Collide()
    {
        base.Collide();
        Debug.Log("IM WATCHING GAY PORNO!!!");
    }
}
