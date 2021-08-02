using System.Collections.Generic;
using Gameplay.BoardPieces;
using Gameplay.Buffs;
using UnityEngine;
namespace Buffs
{
    public class BuffController : MonoBehaviour
    {
        public List<Buff> ActiveBuffs;

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
                foreach (var i in ActiveBuffs)
                {
                    var kill = i.ProcessBuff(Time.deltaTime);
                    if (kill == true)
                    {
                        //Figure out how to handle buffs that are done
                    }
                }
            }
        }
    
        public void AddBuff(GameObject item, Buff buff)
        {
        
        }
    }
}
