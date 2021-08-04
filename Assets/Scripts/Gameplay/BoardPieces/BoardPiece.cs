using UnityEngine;
using Utilities;

namespace Gameplay.BoardPieces
{
    public abstract class BoardPiece : MonoBehaviour
    {
        [Header("Starting Stats")]
        [SerializeField] protected int startingHealth;
        [SerializeField] protected int startingAttackDamage;
        [SerializeField] protected float startingAttackSpeed;
        [SerializeField] protected float startingMovementSpeed;

        [Header("Sound Effects")]
        [SerializeField] protected AudioClip damageSoundEffect;
        [SerializeField] protected AudioClip deathSoundEffect;

        // Board piece gameplay state variables
        protected bool isAlive;
        protected int currentAttackDamage;
        protected float currentAttackSpeed;
        protected int currentHealth;
        protected float currentMovementSpeed;

        // Start is called before the first frame update
        public virtual void Start()
        {
            isAlive = true;
            currentHealth = startingHealth;
            currentAttackDamage = startingAttackDamage;
            currentAttackSpeed = startingAttackSpeed;
            currentMovementSpeed = startingMovementSpeed;
        }

        /// <summary>
        ///     Logic for when this BoardPiece dies
        /// </summary>
        /// <param name="attacker"> BoardPiece that killed this BoardPiece </param>
        protected virtual void Death(BoardPiece attacker)
        {
            isAlive = false;

            if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(deathSoundEffect);

            attacker.KillTarget(this);
        }

        /// <summary>
        ///     Logic triggered when this BoardPiece kills something
        /// </summary>
        /// <param name="victim"> BoardPiece object for the thing that was killed </param>
        protected virtual void KillTarget(BoardPiece victim)
        {
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
        ///     Deals the given amount of damage to this BoardPiece
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
                if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(damageSoundEffect);
            }
        }
    }
}