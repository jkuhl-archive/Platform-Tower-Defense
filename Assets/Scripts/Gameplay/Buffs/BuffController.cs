using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Buffs
{
    public class BuffController : MonoBehaviour
    {
        public List<Buff> ActiveBuffs;

        // Update is called once per frame
        private void Update()
        {
            ProcessBuffList();
        }

        /// <summary>
        ///     Quick and easy way to add a buff
        /// </summary>
        /// <param name="buff">The class being added</param>
        public void AddBuff(Buff buff)
        {
            ActiveBuffs.Add(buff);
        }

        /// <summary>
        ///     Process each buff in the ActiveBuffs list;
        /// </summary>
        private void ProcessBuffList()
        {
            if (ActiveBuffs.Count <= 0)
                return;
            foreach (var i in ActiveBuffs.ToArray())
            {
                var isBuffActive = i.ProcessBuff(Time.deltaTime);
                if (!isBuffActive) ActiveBuffs.Remove(i);
            }
        }
    }
}