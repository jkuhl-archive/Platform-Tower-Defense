using UnityEngine;
using Utilities;

namespace Gameplay
{
    public abstract class BoardPiece : MonoBehaviour
    {
        [Header("Starting Stats")]
        public int attackDamage;
        public int startingHealth;
        public float movementSpeed;
        public float attackSpeed;
        
        [Header("Sound Effects")]
        public AudioClip damageSoundEffect;
        public AudioClip deathSoundEffect;

        private float currentHealth;

        public virtual void Start()
        {
            currentHealth = startingHealth;
        }

        /// <summary>
        /// Deals the given amount of damage to this BoardPiece
        /// </summary>
        /// <param name="damageAmount"> Amount of damage that should be taken </param>
        /// <param name="attacker"> BoardPiece that dealt the damage to this BoardPiece </param>
        public void TakeDamage(int damageAmount, BoardPiece attacker)
        {
            currentHealth -= damageAmount;
            
            if (currentHealth <= 0)
            {
                Death(attacker);
            }
            else
            {
                if (PlayerDataUtils.IsSoundEnabled())
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
            if (PlayerDataUtils.IsSoundEnabled())
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