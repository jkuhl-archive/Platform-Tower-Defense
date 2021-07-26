using System.Collections.Generic;
using Gameplay.BoardPieces;
using UnityEngine;
using Utilities;

namespace Gameplay.Creeps
{
    public class BaseCreepLogic : BoardPiece
    {
        // TODO: Buff system rework

        [Header("Creep Starting Stats")]
        [SerializeField] private int rewardAmount;
        [SerializeField] private float despawnTime;

        // Creep behavior variables
        private Animator animator;

        // Movement status variables
        private bool isMoving;
        private int nextNode;
        private List<GameObject> nodeList;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();
            nodeList = GameUtils.GetMapLogic().GetNodeList();
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameUtils.IsGameInProgress() && !GameUtils.IsGamePaused()) MovementLogic();
        }

        /// <summary>
        ///     Moves the creep around the map by following nodes
        ///     Triggers the ReachEndPoint method if the creep safely reaches the end of the map
        /// </summary>
        private void MovementLogic()
        {
            // If creep is at the starting node, start moving towards the next
            if (nextNode == 0)
            {
                nextNode++;
                isMoving = true;
            }

            if (isMoving)
            {
                var nodeTransform = nodeList[nextNode].transform;
                var newPosition = new Vector3(nodeTransform.position.x, transform.position.y, nodeTransform.position.z);

                transform.position = Vector3.MoveTowards(transform.position,
                    newPosition, Time.deltaTime * movementSpeed);

                if (Vector3.Distance(transform.position, newPosition) < 0.1f)
                {
                    if (nextNode < nodeList.Count)
                    {
                        nextNode++;
                        transform.rotation = nodeTransform.rotation;
                    }

                    if (nextNode >= nodeList.Count) ReachEndPoint();
                }
            }
        }

        /// <summary>
        ///     Logic for when the creep reaches the end of the map and 'damages' the player
        /// </summary>
        private void ReachEndPoint()
        {
            isMoving = false;
            animator.SetBool("Victory", true);
            GameUtils.GetPlayerLogic().UpdatePlayerHealth(-attackDamage);
            GameUtils.GetWaveManager().creepList.Remove(gameObject);
            Destroy(gameObject, (float) (despawnTime * 0.1));
        }

        /// <summary>
        ///     Handles the creep death workflow
        /// </summary>
        /// <param name="attacker"> BoardPiece that killed this creep </param>
        public override void Death(BoardPiece attacker)
        {
            base.Death(attacker);
            isMoving = false;
            animator.SetBool("Death", true);
            GameUtils.GetPlayerLogic().UpdatePlayerMoney(rewardAmount);
            GameUtils.GetWaveManager().creepList.Remove(gameObject);
            Destroy(gameObject, despawnTime);
        }
    }
}