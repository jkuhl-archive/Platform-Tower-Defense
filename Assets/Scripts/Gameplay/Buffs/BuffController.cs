using System.Collections.Generic;
using Gameplay.BoardPieces;
using Gameplay.Buffs;
using UnityEngine;
namespace Buffs
{
    public class BuffController : MonoBehaviour
    {
        public List<Buff> ActiveBuffs;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            ProcessBuffList(); 
        }

        // Process the buffs every frame
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
                        //
                    }
                }
            }
        }
    
        public void AddBuff(GameObject item, Buff buff)
        {
        
        }
    }
}
