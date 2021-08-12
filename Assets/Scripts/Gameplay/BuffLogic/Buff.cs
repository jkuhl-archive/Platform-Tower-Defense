using UnityEngine;
using Utilities;

namespace Gameplay.BuffLogic
{
    public class Buff
    {
        // Buff effect variables
        private readonly bool isBuffRepeating;
        private readonly float buffDuration;
        private readonly float buffRepeatFrequency;
        private readonly Material buffEffectMaterial;

        // Buff state parameters
        protected bool buffActive = true;
        private float timeUntilBuffExpires;
        private float timeUntilRepeat;

        /// <summary>
        ///     Base buff constructor
        /// </summary>
        /// <param name="isBuffRepeating"> Should the buff effect be applied multiple times on a time interval </param>
        /// <param name="buffDuration"> Amount of time the buff effect should be active </param>
        /// <param name="buffRepeatFrequency"> Amount of time between buff effect application for repeating buffs </param>
        /// <param name="effectMaterialName"> Name of the effect material this buff should apply to the target </param>
        protected Buff(bool isBuffRepeating, float buffDuration, float buffRepeatFrequency, string effectMaterialName)
        {
            this.isBuffRepeating = isBuffRepeating;
            this.buffDuration = buffDuration;
            this.buffRepeatFrequency = buffRepeatFrequency;
            buffEffectMaterial = GameUtils.GetEffectMaterialByName(effectMaterialName);

            // Set buff timers based on input duration and repeat frequency
            timeUntilBuffExpires = buffDuration;
        }

        /// <summary>
        ///     Defines buff logic
        /// </summary>
        protected virtual void Effect(GameObject target)
        {
            if (buffEffectMaterial != null && target.GetComponent<EffectMaterial>() == null)
            {
                EffectMaterial effectMaterial = target.AddComponent<EffectMaterial>();
                effectMaterial.ApplyEffect(buffEffectMaterial);
            }
        }

        /// <summary>
        ///     Define what, if anything, the buff does upon completion.
        /// </summary>
        protected virtual void ExitEffect(GameObject target)
        {
            if (target.GetComponent<EffectMaterial>() != null)
            {
                target.GetComponent<EffectMaterial>().DisableEffect();
            }
        }

        /// <summary>
        ///     Handles applying the buff's effect if it is time to do so
        /// </summary>
        /// <returns> Returns true if the buff is active, false if not </returns>
        public bool ProcessBuff(GameObject target)
        {
            if (target is null)
            {
                buffActive = false;
                ExitEffect(target);
            }

            // What to do when time runs out
            timeUntilBuffExpires -= Time.deltaTime;
            if (timeUntilBuffExpires <= 0 && buffActive)
            {
                ExitEffect(target);
                buffActive = false;
            }

            // If this isn't a toggle effect, process the frequency
            if (isBuffRepeating && buffActive)
            {
                timeUntilRepeat -= Time.deltaTime;
                if (timeUntilRepeat <= 0)
                {
                    Effect(target);
                    timeUntilRepeat = buffRepeatFrequency;
                }
            }

            return buffActive;
        }
    }
}