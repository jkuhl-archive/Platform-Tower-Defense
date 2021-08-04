using System.Collections.Generic;
using Gameplay.BoardPieces;
using UnityEngine;

namespace Gameplay.Buffs
{
    public class BuffController : MonoBehaviour
    {
        private readonly List<Buff> activeBuffs = new List<Buff>();

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
            
            foreach (var i in activeBuffs.ToArray())
            {
                var isBuffActive = i.ProcessBuff();
                if (!isBuffActive) activeBuffs.Remove(i);
            }
        }
        
        /// <summary>
        ///     Adds a buff to the active buffs list
        /// </summary>
        /// <param name="buff"> Buff object that we want to add </param>
        public void AddBuff(Buff buff)
        {
            activeBuffs.Add(buff);
        }

        /// <summary>
        ///     Looks up a buff by name and then applies it to the given BoardPiece 
        /// </summary>
        /// <param name="target"> BoardPiece we want to apply the buff to </param>
        /// <param name="buffName"> String containing the name of the buff </param>
        public void ApplyBuffByName(BoardPiece target, string buffName)
        {
            switch (buffName)
            {
                case "bleed":
                    AddBuff(new BleedDebuff(target));
                    break;
            }
        }
    }
}