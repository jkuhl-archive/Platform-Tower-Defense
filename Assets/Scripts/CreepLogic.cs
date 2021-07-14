using System.Collections.Generic;
using UnityEngine;

public class CreepLogic : MonoBehaviour
{
    // Public creep behavior variables
    public int creepStartingHealth;
    public int creepDamageDealt;
    public int rewardAmount;
    public float movementSpeed;
    public float despawnTime;
    public AudioClip creepDamageSoundEffect;
    public AudioClip creepDeathSoundEffect;

    // Creep behavior variables
    private int currentCreepHealth;
    private Animator animator;

    // Variables for map interaction logic
    private List<GameObject> nodeList;
    private int nextNode;
    private bool isMoving;


    // Start is called before the first frame update
    void Start()
    {
        currentCreepHealth = creepStartingHealth;
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
        GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().UpdatePlayerHealth(-creepDamageDealt);
        GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<WaveLogic>().creepList.Remove(gameObject);
        Destroy(gameObject, (float)(despawnTime * 0.1));
    }

    /// <summary>
    /// Deals damage to the creep and handles creep death
    /// </summary>
    /// <param name="damageAmount"> Amount of points of damage the creep is receiving </param>
    /// <param name="damagingTower"> Tower GameObject that dealt the damage to this creep </param>
    public void TakeDamage(int damageAmount, GameObject damagingTower)
    {
        currentCreepHealth -= damageAmount;

        if (currentCreepHealth <= 0)
        {
            isMoving = false;
            animator.SetBool("Death", true);
            GetComponent<AudioSource>().PlayOneShot(creepDeathSoundEffect);
            GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().UpdatePlayerMoney(rewardAmount);
            damagingTower.GetComponent<TowerLogic>().creepKillCounter += 1;
            GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<WaveLogic>().creepList.Remove(gameObject);
            Destroy(gameObject, despawnTime);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(creepDamageSoundEffect);

        }
    }
}
