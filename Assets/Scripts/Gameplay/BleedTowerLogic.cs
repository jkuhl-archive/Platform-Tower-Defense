using UnityEngine;
using Utilities;


namespace Gameplay
{
    public class BleedTowerLogic : TowerLogic
    {
        /// <summary>
        /// Define what the bullet does when it hits its target
        /// </summary>
        /// <param name="targetCreep">The object being hit</param> 
        protected override void Attack(GameObject targetCreep)
        {
            GameObject newAmmo = Instantiate(ammoPrefab, ammoSpawnPoint.transform.position, ammoPrefab.transform.rotation);
            newAmmo.GetComponent<BleedAmmoLogic>().InitializeAmmo(this, targetCreep);
            if (PlayerDataUtils.IsSoundEnabled())
            {
                GetComponent<AudioSource>().PlayOneShot(towerFireSoundEffect);
            }
        }
    }
}
