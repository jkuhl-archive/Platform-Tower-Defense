using Gameplay.BoardPieces;
using Gameplay.Creeps;
using UnityEngine;
using Utilities;

namespace Gameplay.Towers.Ammo
{
    public class BaseAmmoLogic : MonoBehaviour
    {
        [Header("Ammo Variables")]
        [SerializeField] private float ammoMovementSpeed;
        [SerializeField] private float ammoTriggerRange;
        [SerializeField] private GameObject ammoHitPrefab;
        private int ammoDamage;

        // Ammo behavior variables
        private bool isInitialized;
        private BoardPiece spawnTower;
        private GameObject targetCreep;

        // Update is called once per frame
        private void Update()
        {
            if (!isInitialized) Destroy(gameObject);

            if (targetCreep == null) Explode();

            if (GameUtils.IsGameInProgress() && !GameUtils.IsGamePaused())
            {
                // Move ammo towards target creep
                transform.position = Vector3.MoveTowards(transform.position,
                    targetCreep.transform.position, Time.deltaTime * ammoMovementSpeed);

                // When the ammo hits the target creep, process collision
                if (Vector3.Distance(transform.position, targetCreep.transform.position) < ammoTriggerRange) Collide();
            }
        }

        /// <summary>
        ///     Define what happens when the bullet collides with the target
        /// </summary>
        private void Collide()
        {
            targetCreep.GetComponent<BaseCreepLogic>().TakeDamage(ammoDamage, spawnTower);

            if (ammoHitPrefab != null) Explode();

            Destroy(gameObject);
        }

        /// <summary>
        ///     Triggers the ammo hit and then destroys the ammo GameObject
        /// </summary>
        private void Explode()
        {
            var ammoHit = Instantiate(ammoHitPrefab, transform.position, ammoHitPrefab.transform.rotation);
            Destroy(ammoHit, 3);
        }

        /// <summary>
        ///     Initializes required ammo variables
        /// </summary>
        /// <param name="spawnTower"> Tower GameObject that generated the ammo GameObject </param>
        /// <param name="targetCreep"> Creep GameObject that the ammo being shot towards </param>
        public void InitializeAmmo(BoardPiece spawnTower, GameObject targetCreep)
        {
            this.spawnTower = spawnTower;
            this.targetCreep = targetCreep;
            ammoDamage = spawnTower.attackDamage;
            isInitialized = true;
        }
    }
}