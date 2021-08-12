using Gameplay.BoardPieces.Creeps;
using UnityEngine;
using Utilities;

namespace Gameplay.BoardPieces.Towers.Ammo
{
    public class BaseAmmoLogic : MonoBehaviour
    {
        [Header("Ammo Variables")]
        [SerializeField] private float ammoMovementSpeed;
        [SerializeField] private float ammoTriggerRange;
        [SerializeField] private GameObject ammoHitPrefab;

        // Ammo behavior variables
        private bool isInitialized;
        protected GameObject attackingTower;
        protected GameObject targetCreep;
        private int ammoDamage;

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
        ///     Triggers the ammo hit and then destroys the ammo GameObject
        /// </summary>
        private void Explode()
        {
            var ammoHit = Instantiate(ammoHitPrefab, transform.position, ammoHitPrefab.transform.rotation);
            Destroy(ammoHit, 3);
        }
        
        /// <summary>
        ///     Define what happens when the bullet collides with the target
        /// </summary>
        protected virtual void Collide()
        {
            targetCreep.GetComponent<BoardPieceLogic>().TakeDamage(ammoDamage, attackingTower);

            if (ammoHitPrefab != null) Explode();
            Destroy(gameObject);
        }

        /// <summary>
        ///     Initializes required ammo variables
        /// </summary>
        /// <param name="attackingTower"> Tower GameObject that generated the ammo GameObject </param>
        /// <param name="targetCreep"> Creep GameObject that the ammo being shot towards </param>
        public void InitializeAmmo(GameObject attackingTower, GameObject targetCreep)
        {
            this.attackingTower = attackingTower;
            this.targetCreep = targetCreep;
            ammoDamage = attackingTower.GetComponent<BoardPieceLogic>().GetAttackDamage();
            isInitialized = true;
        }
    }
}