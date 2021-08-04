using UnityEngine;
using Utilities;

namespace Gameplay.BoardPieces.Towers.Ammo
{
    public class BuffApplicatorAmmoLogic : BaseAmmoLogic
    {
        [Header("Buff Name")]
        [SerializeField] private string buffName;
        
        /// <summary>
        ///     Runs the base class ammo logic and then applies the given buff
        /// </summary>
        protected override void Collide()
        {
            base.Collide();
            GameUtils.GetBuffController().ApplyBuffByName(targetCreep, buffName);
        }
    }
}