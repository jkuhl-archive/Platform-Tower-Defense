using System;
using Gameplay.BoardPieces;
using Gameplay.Towers.Ammo;
using UnityEngine;
using Utilities;

namespace Gameplay.Towers
{
    // TODO: Buff system rework

        public class BleedTowerLogic : BaseTowerLogic
        {
            /// <summary>
            /// Define what the bullet does when it hits its target
            /// </summary>
            /// <param name="targetCreep">The object being hit</param> 
            protected override void Attack(BoardPiece targetCreep)
            {
                GameObject newAmmo = Instantiate(ammoPrefab, ammoSpawnPoint.transform.position, ammoPrefab.transform.rotation);
                newAmmo.GetComponent<BleedAmmoLogic>().InitializeAmmo(this, targetCreep);
            }
        }
}

