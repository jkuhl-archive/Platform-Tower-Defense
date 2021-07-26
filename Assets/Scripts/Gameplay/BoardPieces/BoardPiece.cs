using UnityEngine;
using Utilities;

namespace Gameplay.BoardPieces
{
    public abstract class BoardPiece : MonoBehaviour
    {
        // TODO: Buff system rework
        
        [Header("Starting Stats")]
        public int attackDamage;
        public int startingHealth;
        public float movementSpeed;
        public float attackSpeed;
        
        [Header("Sound Effects")]
        public AudioClip damageSoundEffect;
        public AudioClip deathSoundEffect;

        private float currentHealth;
        private bool isAlive;

        public virtual void Start()
        {
            currentHealth = startingHealth;
            isAlive = true;
        }

        /// <summary>
        /// Deals the given amount of damage to this BoardPiece
        /// </summary>
        /// <param name="damageAmount"> Amount of damage that should be taken </param>
        /// <param name="attacker"> BoardPiece that dealt the damage to this BoardPiece </param>
        public void TakeDamage(int damageAmount, BoardPiece attacker)
        {
            currentHealth -= damageAmount;
            
            if (currentHealth <= 0 && isAlive)
            {
                Death(attacker);
            }
            else
            {
                if (GameSettingsUtils.IsSoundEnabled())
                {
                    GetComponent<AudioSource>().PlayOneShot(damageSoundEffect);
                }
            }
        }

        /// <summary>
        /// Logic for when this BoardPiece dies
        /// </summary>
        /// <param name="attacker"> BoardPiece that killed this BoardPiece </param>
        public virtual void Death(BoardPiece attacker)
        {
            isAlive = false;
            
            if (GameSettingsUtils.IsSoundEnabled())
            {
                GetComponent<AudioSource>().PlayOneShot(deathSoundEffect);
            }
            
            attacker.Killed(this);
        }

        /// <summary>
        /// Logic triggered when this BoardPiece kills something
        /// </summary>
        /// <param name="victim"> BoardPiece object for the thing that was killed </param>
        public virtual void Killed(BoardPiece victim)
        {
            return;
        }
    }
}