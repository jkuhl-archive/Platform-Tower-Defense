using System.Collections.Generic;
using System.Linq;
using Gameplay.BoardPieces;
using Gameplay.Buffs;
using UnityEngine;
namespace Buffs
{
    public class BuffController : MonoBehaviour
    {
        public List<Buff> ActiveBuffs;
        public List<Buff> InactiveBuffs;

        // Update is called once per frame
        void Update()
        {
            ProcessBuffList(); 
        }

        /// <summary>
        /// Process each buff in the ActiveBuffs list;
        /// </summary>
        private void ProcessBuffList()
        {
            if (ActiveBuffs.Count <= 0)
            {
                return;
            }
            else
            {
                foreach (var i in ActiveBuffs.ToArray())
                {
                    var alive = i.ProcessBuff(Time.deltaTime);
                    if (!alive)
                    {
                        InactiveBuffs.Add(i);
                    }

                    foreach (var e in InactiveBuffs)
                    {
                        if (ActiveBuffs.Contains(e))
                        {
                            ActiveBuffs.Remove(e);
                        }
                    }
                    InactiveBuffs.Clear();
                }
            }
        }
    
        public void AddBuff(Buff buff)
        {
            ActiveBuffs.Add(buff);
        }
    }
}
