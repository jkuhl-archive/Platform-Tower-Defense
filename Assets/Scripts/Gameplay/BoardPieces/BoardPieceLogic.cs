using Gameplay.BoardPieces.Creeps;
using Gameplay.BoardPieces.Towers;
using UnityEngine;
using Utilities;

namespace Gameplay.BoardPieces
{
    public class BoardPieceLogic : MonoBehaviour
    {
        [Header("Starting Stats")]
        [SerializeField] private int startingHealth;
        [SerializeField] private int startingAttackDamage;
        [SerializeField] private float startingAttackSpeed;
        [SerializeField] private float startingMovementSpeed;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip damageSoundEffect;
        [SerializeField] private AudioClip deathSoundEffect;

        // Board piece gameplay state variables
        private bool isAlive;
        private int currentAttackDamage;
        private float currentAttackSpeed;
        private int currentHealth;
        private float currentMovementSpeed;

        // Start is called before the first frame update
        private void Start()
        {
            isAlive = true;
            currentHealth = startingHealth;
            currentAttackDamage = startingAttackDamage;
            currentAttackSpeed = startingAttackSpeed;
            currentMovementSpeed = startingMovementSpeed;
        }

        /// <summary>
        ///     Logic for when this GameObject dies
        /// </summary>
        /// <param name="attacker"> GameObject that killed this GameObject </param>
        private void Death(GameObject attacker)
        {
            isAlive = false;

            if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(deathSoundEffect);

            attacker.GetComponent<BoardPieceLogic>().KillTarget(gameObject);
            
            // Trigger creep death logic if this is a creep
            if (TryGetComponent(typeof(BaseCreepLogic), out var creepComponent))
            {
                BaseCreepLogic creepLogic = (BaseCreepLogic)creepComponent;
                creepLogic.CreepDeath(attacker);
                return;
            }
            
            // Trigger tower death logic if this is a tower
            if (TryGetComponent(typeof(BaseTowerLogic), out var towerComponent))
            {
                BaseTowerLogic towerLogic = (BaseTowerLogic)towerComponent;
                towerLogic.TowerDeath(attacker);
                return;
            }
            
            // If we haven't run specific death logic yet, just destroy the object
            Destroy(gameObject);
        }

        /// <summary>
        ///     Gets the current attack damage of this board piece
        /// </summary>
        /// <returns> Current attack damage as an int </returns>
        public int GetAttackDamage()
        {
            return currentAttackDamage;
        }

        /// <summary>
        ///     Gets the current attack speed of this board piece
        /// </summary>
        /// <returns> Attack speed as a float </returns>
        public float GetAttackSpeed()
        {
            return currentAttackSpeed;
        }

        /// <summary>
        ///     Gets the current health of this board piece
        /// </summary>
        /// <returns> Current health as an int </returns>
        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        /// <summary>
        ///     Gets the current movement speed of this board piece
        /// </summary>
        /// <returns> Movement speed as a float </returns>
        public float GetMovementSpeed()
        {
            return currentMovementSpeed;
        }

        /// <summary>
        ///     Gets the starting health of this board piece
        /// </summary>
        /// <returns> Starting health as an int </returns>
        public int GetStartingHealth()
        {
            return startingHealth;
        }

        /// <summary>
        ///     Checks if this board piece is alive
        /// </summary>
        /// <returns> True if alive, false if not </returns>
        public bool IsAlive()
        {
            return isAlive;
        }
        
        /// <summary>
        ///     Logic triggered when this GameObject kills something
        /// </summary>
        /// <param name="victim"> GameObject object for the thing that was killed </param>
        public void KillTarget(GameObject victim)
        {
        }

        /// <summary>
        ///     Deals the given amount of damage to this GameObject
        /// </summary>
        /// <param name="damageAmount"> Amount of damage that should be taken </param>
        /// <param name="attacker"> GameObject that dealt the damage to this GameObject </param>
        public void TakeDamage(int damageAmount, GameObject attacker)
        {
            currentHealth -= damageAmount;

            if (currentHealth <= 0 && isAlive)
            {
                Death(attacker);
            }
            else
            {
                if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(damageSoundEffect);
            }
        }
    }
}