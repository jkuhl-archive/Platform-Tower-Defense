using Gameplay.BoardPieces.Towers.Ammo;
using UnityEngine;
using Utilities;

namespace Gameplay.BoardPieces.Towers.TowerAttacks
{
    public class BaseTowerAttackLogic : MonoBehaviour
    {
        [Header("Attack Ammo Prefab")]
        [SerializeField] protected GameObject ammoPrefab;
        
        [Header("Attack Sound Effect")]
        [SerializeField] protected AudioClip attackSoundEffect;
        
        /// <summary>
        ///     Basic tower single shot attack logic
        /// </summary>
        /// <param name="ammoSpawnPoint"> Position where ammo prefabs should be spawned </param>
        /// <param name="attackingTower"> Tower that is triggering this attack </param>
        /// <param name="targetCreep"> Creep that this attack is targeting </param>
        public virtual void Attack(Vector3 ammoSpawnPoint, BoardPiece attackingTower, BoardPiece targetCreep)
        {
            var newAmmo = Instantiate(ammoPrefab, ammoSpawnPoint, ammoPrefab.transform.rotation);
            newAmmo.GetComponent<BaseAmmoLogic>().InitializeAmmo(attackingTower, targetCreep);
            if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(attackSoundEffect);
        }
    }
}
