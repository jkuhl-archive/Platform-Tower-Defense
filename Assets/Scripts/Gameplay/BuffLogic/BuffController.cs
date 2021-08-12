using System.Collections.Generic;
using System.Linq;
using Gameplay.BuffLogic.Buffs;
using UnityEngine;

namespace Gameplay.BuffLogic
{
    public class BuffController : MonoBehaviour
    {
        private readonly Dictionary<GameObject, List<Buff>> activeBuffs = new Dictionary<GameObject, List<Buff>>();

        // Update is called once per frame
        private void Update()
        {
            ProcessBuffList();
        }

        /// <summary>
        ///     Process each buff in the ActiveBuffs list;
        /// </summary>
        private void ProcessBuffList()
        {
            if (activeBuffs.Count <= 0) return;

            foreach (GameObject target in activeBuffs.Keys.ToList())
            {
                foreach (Buff buff in activeBuffs[target].ToList())
                {
                    bool buffActive = buff.ProcessBuff(target);

                    if (!buffActive) activeBuffs[target].Remove(buff);
                    if (activeBuffs[target].Count <= 0) activeBuffs.Remove(target);
                }
            }
        }

        /// <summary>
        ///     Adds a buff to the active buffs list
        /// </summary>
        /// <param name="target"> GameObject that we want to apply the buff to</param>
        /// <param name="buff"> Buff object that we want to add </param>
        public void ApplyBuff(GameObject target, Buff buff)
        {
            // If the target already is being tracked, add the buff to it's list
            if (activeBuffs.ContainsKey(target))
            {
                List<Buff> existingBuffList = activeBuffs[target];

                // Check if a buff of this type has already been applied and return if it has
                foreach (Buff existingBuff in existingBuffList)
                {
                    if (existingBuff.GetType() == buff.GetType())
                    {
                        return;
                    }
                }
                
                existingBuffList.Add(buff);
                return;
            }

            // If this is the first time the target has received a buff create a new entry for it
            activeBuffs.Add(target, new List<Buff> { buff });
        }

        /// <summary>
        ///     Looks up a buff by name and then applies it to the given BoardPiece 
        /// </summary>
        /// <param name="target"> GameObject we want to apply the buff to </param>
        /// <param name="buffName"> String containing the name of the buff </param>
        public void ApplyBuffByName(GameObject target, string buffName)
        {
            switch (buffName)
            {
                case "bleed":
                    ApplyBuff(target, new BleedDebuff());
                    break;
            }
        }
    }
}