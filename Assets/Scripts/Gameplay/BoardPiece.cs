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

        private float health;

        public virtual void Start()
        {
            health = startingHealth;
        }

        public void TakeDamage(int damageAmount, BoardPiece attacker)
        {
            health -= damageAmount;
            print(health);
            if (health <= 0)
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

        public virtual void Death(BoardPiece attacker)
        {
            if (PlayerDataUtils.IsSoundEnabled())
            {
                GetComponent<AudioSource>().PlayOneShot(deathSoundEffect);
            }
            
            attacker.Killed(this);
        }

        public virtual void Killed(BoardPiece victim)
        {
            return;
        }
    }
}