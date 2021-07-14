using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveLogic : MonoBehaviour
{
    // Wave and creep related GameObject variables
    public GameObject enemyPrefab;
    public GameObject waveCounterText;
    public GameObject waveInfoText;

    // Number of seconds between waves
    public float timeBetweenWaves;
    
    // List of currently active creeps
    public List<GameObject> creepList;

    // Wave data used for spawning creeps
    private Dictionary<int, Dictionary<string, float>> waveData = new Dictionary<int, Dictionary<string, float>>();
    private string creepCountString = "creep_count";
    private string spawnDelayString = "spawn_delay";
    
    private GameObject startPoint;

    // Gameplay related variables
    private int waveCounter;
    private int spawnCounter;
    private float nextWaveTime;
    private float nextSpawnTime;
    private bool waveActive;

    // Start is called before the first frame update
    void Start()
    {
        PopulateWaveData();
        startPoint = GameUtils.GetRootGameObjectByName("Map").GetComponent<MapLogic>().nodeList[0];
        nextWaveTime = Time.time + timeBetweenWaves;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().IsGameActive())
        {
            Execute();
            UpdateTextObjects();
        }
    }

    /// <summary>
    /// Populates the wave data dictionary with wave variables
    /// </summary>
    void PopulateWaveData()
    {
        // Method for generating a populated waveDictionary
        Dictionary<string, float> GenerateWaveDict(float creepCount, float spawnDelay)
        {
            Dictionary<string, float> waveDict = new Dictionary<string, float>();
            waveDict.Add(creepCountString, creepCount);
            waveDict.Add(spawnDelayString, spawnDelay);
            return waveDict;
        }
        
        // Populate waves 0-10
        for (int i = 0; i <= 10; i++)
        {
            waveData.Add(i, GenerateWaveDict(10*i, 1.5f));
        }
        
        waveData.Add(11, GenerateWaveDict(110, .5f));
        waveData.Add(12, GenerateWaveDict(120, .5f));
    }
    
    /// <summary>
    /// Handles starting and stopping of waves and spawning creeps
    /// </summary>
    void Execute()
    {
        // If the game is not active, return immediately
        if (!GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().IsGameActive())
        {
            return;
        }

        // If we are between waves, check and see if it's time to start a new wave or not
        if (!waveActive)
        {
            // If it's not time to start a new wave, return
            if (!(Time.time > nextWaveTime))
            {
                return;
            }
            
            // If it is time to start a new wave, kick it off
            if (Time.time > nextWaveTime)
            {
                if (waveCounter < waveData.Count - 1)
                {
                    spawnCounter = 0;
                    waveCounter++;
                    waveActive = true;
                }
            }
        }
        
        // Spawn creep
        if (Time.time > nextSpawnTime)
        {
            nextSpawnTime = Time.time + waveData[waveCounter][spawnDelayString];
            spawnCounter++;
            
            GameObject newCreep = Instantiate(enemyPrefab, startPoint.transform.position, startPoint.transform.rotation);
            creepList.Add(newCreep);
            
            // When all creeps in a wave have been spawned, end the wave and wait for the next
            if (spawnCounter >= waveData[waveCounter][creepCountString])
            {
                nextWaveTime = Time.time + timeBetweenWaves;
                waveActive = false;
            }
        }
    }
    
    /// <summary>
    /// Updates the text of all UI elements
    /// </summary>
    void UpdateTextObjects()
    {
        // Attempt to update wave count text
        string waveCounterString = $"Wave: {waveCounter}";
        if (waveCounterText.GetComponent<Text>().text != waveCounterString)
        {
            waveCounterText.GetComponent<Text>().text = waveCounterString;
        }
        
        // Attempt to update wave info text
        string waveInfoString = $"Spawned: {spawnCounter} / {waveData[waveCounter][creepCountString]}";
        if (!waveActive)
        {
            if (waveCounter < waveData.Count - 1)
            {
                waveInfoString = $"Next wave in {timeBetweenWaves} seconds...";
            }
            else
            {
                waveInfoString = "All waves completed! You win!";
            }
        }
        
        if (waveInfoText.GetComponent<Text>().text != waveInfoString)
        {
            waveInfoText.GetComponent<Text>().text = waveInfoString;
        }
    }
}
