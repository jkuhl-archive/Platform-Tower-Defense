using System.Collections.Generic;
using Gameplay.Waves;
using UnityEngine;
using Utilities;

namespace Gameplay
{
    public class CreepLogic : BoardPiece
    {
        [Header("Creep Starting Stats")]
        public int rewardAmount;
        public float despawnTime;

        // Creep behavior variables
        private Animator animator;

        // Variables for map interaction logic
        private List<GameObject> nodeList;
        private int nextNode;
        private bool isMoving;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();
        
            nodeList = GameUtils.GetRootGameObjectByName("Map").GetComponent<MapLogic>().nodeList;
        }

        // Update is called once per frame
        void Update()
        {
            MovementLogic();
        }

        /// <summary>
        /// Moves the creep around the map by following nodes
        /// Triggers the ReachEndPoint method if the creep safely reaches the end of the map
        /// </summary>
        void MovementLogic()
        {
            if (nextNode == 0)
            {
                nextNode++;
                isMoving = true;
            }
        
            if (isMoving)
            {
                Transform nodeTransform = nodeList[nextNode].transform;
                Vector3 newPosition = new Vector3(nodeTransform.position.x, transform.position.y, nodeTransform.position.z);
            
                transform.position = Vector3.MoveTowards(transform.position,
                    newPosition, Time.deltaTime * movementSpeed);

                if (Vector3.Distance(transform.position, newPosition) < 0.1f)
                {
                    if (nextNode < nodeList.Count)
                    {
                        nextNode++;
                        transform.rotation = nodeTransform.rotation;
                    }
                
                    if (nextNode >= nodeList.Count)
                    {
                        ReachEndPoint();
                    }
                }
            }
        }

        /// <summary>
        /// Logic for when the creep reaches the end of the map and 'damages' the player
        /// </summary>
        void ReachEndPoint()
        {
            isMoving = false;
            animator.SetBool("Victory", true);
            GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().UpdatePlayerHealth(-attackDamage);
            GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<WaveManager>().creepList.Remove(gameObject);
            Destroy(gameObject, (float)(despawnTime * 0.1));
        }
        
        /// <summary>
        /// Handles the creep death workflow
        /// </summary>
        /// <param name="attacker"> BoardPiece that killed this creep </param>
        public override void Death(BoardPiece attacker)
        {
            base.Death(attacker);
            isMoving = false;
            animator.SetBool("Death", true);
            GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().UpdatePlayerMoney(rewardAmount);
            GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<WaveManager>().creepList.Remove(gameObject);
            Destroy(gameObject, despawnTime);
        }
    }
}
