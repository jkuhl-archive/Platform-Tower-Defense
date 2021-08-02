using Buffs;
using UnityEngine;
using Utilities;


namespace Gameplay.Towers.Ammo
{
    // TODO: Buff system rework

       /// <summary>
       /// Define what happens when the bullet collides with the target
       /// </summary>
       public class BleedAmmoLogic : BaseAmmoLogic
       {
           protected override void Collide()
           {
               GameUtils.GetBuffController().AddBuff(new BleedDebuff(targetCreep));
               base.Collide();
           }
       }
}
