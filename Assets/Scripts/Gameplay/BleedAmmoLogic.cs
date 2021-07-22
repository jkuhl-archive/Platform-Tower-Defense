using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Define what happens when the bullet collides with the target
    /// </summary>
    public class BleedAmmoLogic : AmmoLogic
    {
        protected override void Collide()
        {
            base.Collide();
            Debug.Log("Ouch I'm bleeding!");
        }
    }
}
