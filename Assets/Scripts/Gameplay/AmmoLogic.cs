using UnityEngine;

namespace Gameplay
{
    public class AmmoLogic : MonoBehaviour
    {
        [Header("Ammo Variables")]
        public float ammoMovementSpeed;
        public float ammoTriggerRange;
        public GameObject ammoHitPrefab;

        // Ammo behavior variables
        private bool isInitialized = false;
        protected GameObject targetCreep;
        protected BoardPiece spawnTower;
        protected int ammoDamage;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        protected void Update()
        {
            if (!isInitialized)
            {
                Destroy(gameObject);
            }

            if (targetCreep == null)
            {
                Destroy(gameObject);
            }
        
            // Move ammo towards target creep
            transform.position = Vector3.MoveTowards(transform.position,
                targetCreep.transform.position, Time.deltaTime * ammoMovementSpeed);
        
            // When the ammo hits the target creep, process collision
            if (Vector3.Distance(transform.position, targetCreep.transform.position) < ammoTriggerRange)
            {
                Collide();
            }
        }
        
        /// <summary>
        /// Define what happens when the bullet collides with the target
        /// </summary>
        protected void Collide()
        {
            targetCreep.GetComponent<CreepLogic>().TakeDamage(ammoDamage, spawnTower);

            if (ammoHitPrefab != null)
            {
                GameObject ammoHit = Instantiate(ammoHitPrefab, transform.position, ammoHitPrefab.transform.rotation);
                Destroy(ammoHit, 3);
            }
            
            Destroy(gameObject);
        }

        /// <summary>
        /// Initializes required ammo variables
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
