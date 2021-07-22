using Gameplay.Waves;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace Gameplay
{
    public class PlayerLogic : MonoBehaviour
    {
        [Header("Player Starting Stats")]
        public int playerStartingHealth;
        public int playerStartingMoney;
    
        [Header("Platform and Tower Costs")]
        public int buyPlatformCost;
        public int movePlatformCost;
        public int buyCannonTowerCost;

        [Header("UI Element GameObjects")]
        public GameObject buyPlatformButton;
        public GameObject movePlatformButton;
        public GameObject buyCannonTowerButton;
        public GameObject moneyCounterText;
        public GameObject healthCounterText;
        public GameObject platformCounterText;
    
        [Header("Gameplay Sound Effects")]
        public AudioClip selectionSoundEffect;
        public AudioClip spendMoneySoundEffect;

        // Game state variables
        private int currentPlayerMoney;
        private int currentPlayerHealth;
        private bool readyToStart;
        private bool gameStarted;
        private bool gameActive;

        // Gameplay GameObjects
        private MapLogic mapLogic;
        private GameObject selectedObject;
        private GameObject previousSelectedObject;

        // Start is called before the first frame update
        void Start()
        {
            currentPlayerHealth = playerStartingHealth;
            currentPlayerMoney = playerStartingMoney;
            
            // Set saved player preferences
            GameUtils.GetRootGameObjectByName("BackgroundMusic").SetActive(PlayerDataUtils.GetBool(PlayerDataUtils.key_soundEnabled, PlayerDataUtils.default_soundEnabled));
        }

        // Update is called once per frame
        void Update()
        {
            // Start game if we are ready
            if (IsReady() && GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<WaveManager>().IsReady() && !gameStarted)
            {
                gameStarted = true;
                gameActive = true;
            }
            
            // If we aren't ready load the game map
            if (!IsReady())
            {
                LoadMap();
                return;
            }
            
            // Core gameplay loop
            if (gameActive)
            {
                UpdateSelectedObject();
                HighlightSelectedObjects();
                ActivateBuyPlatformButton();
                ActivateMovePlatformButton();
                ActivateBuyTowerButtons();
                UpdateTextObjects();
            }
        }
    
        /// <summary>
        /// Enables or disables the buy platform button
        /// </summary>
        void ActivateBuyPlatformButton()
        {
            if (CanBuyPlatform())
            {
                if (!buyPlatformButton.GetComponent<Button>().interactable)
                {
                    buyPlatformButton.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                if (buyPlatformButton.GetComponent<Button>().interactable)
                {
                    buyPlatformButton.GetComponent<Button>().interactable = false;
                }
            }
        }

        /// <summary>
        /// Enables or disables the buy tower buttons
        /// </summary>
        void ActivateBuyTowerButtons()
        {
            // Cannon Tower
            if (CanBuyTower(buyCannonTowerCost))
            {
                if (!buyCannonTowerButton.GetComponent<Button>().interactable)
                {
                    buyCannonTowerButton.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                if (buyCannonTowerButton.GetComponent<Button>().interactable)
                {
                    buyCannonTowerButton.GetComponent<Button>().interactable = false;
                }
            }

        }

        /// <summary>
        /// Enables or disables the move platform button
        /// </summary>
        void ActivateMovePlatformButton()
        {
            if (CanMovePlatform())
            {
                if (!movePlatformButton.GetComponent<Button>().interactable)
                {
                    movePlatformButton.GetComponent<Button>().interactable = true;
                }
            }
            else
            {
                {
                    if (movePlatformButton.GetComponent<Button>().interactable)
                    {
                        movePlatformButton.GetComponent<Button>().interactable = false;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true when:
        /// - The player clicks on an open platform space
        /// - The maximum number of platforms has not been reached
        /// - The player has the funds to purchase a platform
        /// </summary>
        /// <returns> True or False if criteria above are met or not</returns>
        bool CanBuyPlatform()
        {
            if (mapLogic.platformSpaceList.Contains(selectedObject) &&
                mapLogic.platformList.Count < mapLogic.maxPlatformCount &&
                currentPlayerMoney >= buyPlatformCost)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true when:
        /// - The player clicks on an empty socket on a platform
        /// - The player has the funds to purchase the tower
        /// </summary>
        /// <param name="towerCost"> Cost of the tower we are checking for </param>
        /// <returns> True or False if criteria above are met or not</returns>
        bool CanBuyTower(int towerCost)
        {
            // Sub-method for checking if the selected object is a platform socket or not
            bool SelectedObjectIsSocket()
            {
                if (selectedObject != null &&
                    selectedObject.transform.parent != null &&
                    mapLogic.platformList.Contains(selectedObject.transform.parent.gameObject))
                {
                    return true;
                }
            
                return false; 
            }

            if (SelectedObjectIsSocket() &&
                selectedObject.transform.childCount == 0 &&
                currentPlayerMoney >= towerCost)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true when:
        /// - The player clicks on a platform, then clicks on an open platform space (can do this in reverse order as well)
        /// - The player has the funds to move a platform
        /// </summary>
        /// <returns> True or False if criteria above are met or not</returns>
        bool CanMovePlatform()
        {
            if ((mapLogic.platformList.Contains(selectedObject) &&
                 mapLogic.platformSpaceList.Contains(previousSelectedObject) ||
            
                 (mapLogic.platformList.Contains(previousSelectedObject) &&
                  mapLogic.platformSpaceList.Contains(selectedObject)) &&
            
                 currentPlayerMoney >= movePlatformCost))
            {
                return true;
            }

            return false;
        }
    

        /// <summary>
        /// Highlights certain types of GameObjects when selected via the mouse
        /// TODO: [PERFORMANCE] This can probably be refactored so that we don't need to iterate over the lists every run
        /// </summary>
        void HighlightSelectedObjects()
        {
            // Highlights a platform space GameObject
            void HighlightPlatformSpace(GameObject platformSpace, bool toggle)
            {
                if (platformSpace.GetComponent<MeshRenderer>().enabled != toggle)
                {
                    platformSpace.GetComponent<MeshRenderer>().enabled = toggle;

                    if (toggle)
                    {
                        if (PlayerDataUtils.IsSoundEnabled())
                        {
                            GetComponent<AudioSource>().PlayOneShot(selectionSoundEffect);
                        }
                    }
                }
            }

            // Attempt to highlight platform space objects
            foreach (GameObject obj in mapLogic.platformSpaceList)
            {
                bool toggle = obj == selectedObject || obj == previousSelectedObject && mapLogic.platformList.Contains(selectedObject);

                HighlightPlatformSpace(obj, toggle);
            }
        
            // Attempt to highlight platform objects or child objects of a platform such as a socket or tower
            foreach (GameObject obj in mapLogic.platformList)
            {
                // Attempt to highlight platform objects
                bool platformToggle = obj == selectedObject || obj == previousSelectedObject && mapLogic.platformSpaceList.Contains(selectedObject);
                obj.GetComponent<SwapMaterial>().Swap(platformToggle, GetComponent<AudioSource>());
            
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    // Attempt to highlight socket objects
                    GameObject socket = obj.transform.GetChild(i).gameObject;
                    bool socketToggle = socket == selectedObject && CanBuyTower(0);
                    socket.GetComponent<SwapMaterial>().Swap(socketToggle, GetComponent<AudioSource>());
                
                    // Attempt to highlight tower objects
                    if (socket.transform.childCount > 0)
                    {
                        GameObject tower = socket.transform.GetChild(0).gameObject;
                        bool towerToggle = socket == selectedObject || tower == selectedObject;
                        tower.GetComponent<TowerLogic>().HighlightTowerRange(towerToggle);
                    }
                }
            }
        }

        /// <summary>
        /// Captures a game object clicked on by the player
        /// </summary>
        void UpdateSelectedObject()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit))
                {
                    previousSelectedObject = selectedObject;
                    selectedObject = hit.transform.gameObject;
                    Debug.Log($"Selected GameObject '{selectedObject}'");

                    return;
                }
            
                previousSelectedObject = null;
                selectedObject = null;
                Debug.Log("Selected 'null'");
            }
        }
    
        /// <summary>
        /// Updates the text of all UI elements
        /// </summary>
        void UpdateTextObjects()
        {
            // Attempt to update buy platform button text
            string buyPlatformString = $"Buy Platform: ${buyPlatformCost}";
            if (buyPlatformButton.GetComponentInChildren<Text>().text != buyPlatformString)
            {
                buyPlatformButton.GetComponentInChildren<Text>().text = buyPlatformString;
            }
        
            // Attempt to update move platform text
            string movePlatformString = $"Move Platform: ${movePlatformCost}";
            if (movePlatformButton.GetComponentInChildren<Text>().text != movePlatformString)
            {
                movePlatformButton.GetComponentInChildren<Text>().text = movePlatformString;
            }
        
            // Attempt to update buy cannon tower text
            string buyCannonTowerString = $"Buy Cannon Tower: ${buyCannonTowerCost}";
            if (buyCannonTowerButton.GetComponentInChildren<Text>().text != buyCannonTowerString)
            {
                buyCannonTowerButton.GetComponentInChildren<Text>().text = buyCannonTowerString;
            }

            // Attempt to update platform count text
            string platformsString = $"Platforms: {mapLogic.platformList.Count} / {mapLogic.maxPlatformCount}";
            if (platformCounterText.GetComponent<Text>().text != platformsString)
            {
                platformCounterText.GetComponent<Text>().text = platformsString;
            }
        }
    
        /// <summary>
        /// Purchases a platform and adds it to the map in place of the currently selected platform space
        /// </summary>
        public void BuyPlatform()
        {
            if (CanBuyPlatform())
            {
                UpdatePlayerMoney(-buyPlatformCost);
            
                GameObject newPlatform = Instantiate(mapLogic.platformPrefab, selectedObject.transform.position,
                    mapLogic.platformPrefab.transform.rotation);
                mapLogic.platformList.Add(newPlatform);

                mapLogic.platformSpaceList.Remove(selectedObject);
                Destroy(selectedObject);
                selectedObject = null;
            
                Debug.Log("Purchased platform");
                buyPlatformButton.GetComponent<Button>().interactable = false;
            }
        }

        /// <summary>
        /// Purchases a tower and places it in the currently selected socket
        /// </summary>
        /// <param name="towerPrefab"></param>
        public void BuyTower(GameObject towerPrefab)
        {
            int towerCost = -1;
        
            switch (towerPrefab.name)
            {
                default:
                    towerCost = -1;
                    break;
                
                case "CannonTower":
                    towerCost = buyCannonTowerCost;
                    break;
                
                case "BleedTower":
                    towerCost = buyCannonTowerCost;
                    break;
            }

            if (towerCost == -1)
            {
                return;
            }
        
            if (CanBuyTower(towerCost))
            {
                UpdatePlayerMoney(-towerCost);

                GameObject newTower = Instantiate(towerPrefab, selectedObject.transform.position, towerPrefab.transform.rotation);
                newTower.transform.parent = selectedObject.transform;
            
                Debug.Log($"Purchased tower {towerPrefab.name}");
                movePlatformButton.GetComponent<Button>().interactable = false;
            }
        }

        /// <summary>
        /// Moves the selected platform to an open platform space
        /// </summary>
        public void MovePlatform()
        {
            if (CanMovePlatform())
            {
                GameObject platform = null;
                GameObject platformSpace = null;

                if (mapLogic.platformList.Contains(selectedObject))
                {
                    platform = selectedObject;
                }
                else if (mapLogic.platformSpaceList.Contains(selectedObject))
                {
                    platformSpace = selectedObject;
                }
            
                if (mapLogic.platformList.Contains(previousSelectedObject))
                {
                    platform = previousSelectedObject;
                }
                else if (mapLogic.platformSpaceList.Contains(previousSelectedObject))
                {
                    platformSpace = previousSelectedObject;
                }

                if (platform != null && platformSpace != null)
                {
                    UpdatePlayerMoney(-movePlatformCost);

                    Vector3 newPlatformPosition = platformSpace.transform.position;
                    Vector3 newPlatformSpacePosition = platform.transform.position;

                    platform.transform.position = newPlatformPosition;
                    platformSpace.transform.position = newPlatformSpacePosition;
                
                    Debug.Log("Moved platform");
                    movePlatformButton.GetComponent<Button>().interactable = false;
                }
            }
        }

        /// <summary>
        /// Returns true if the game is currently active, or false if not
        /// </summary>
        /// <returns> Returns true if game is active, or false if not </returns>
        public bool IsGameActive()
        {
            return gameActive;
        }
        
        /// <summary>
        /// Checks if the game is ready to begin
        /// </summary>
        /// <returns> Returns true if game is ready to start, false if not </returns>
        public bool IsReady()
        {
            return readyToStart;
        }

        /// <summary>
        /// Exits the current game and returns to the main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            if (PlayerDataUtils.IsSoundEnabled())
            {
                GetComponent<AudioSource>().PlayOneShot(selectionSoundEffect);
            }
            
            SceneManager.LoadScene(GameUtils.scene_mainMenu, LoadSceneMode.Single);
        }
        
        /// <summary>
        /// Attempts to load the game map prefab stored in GameUtils
        /// </summary>
        void LoadMap()
        {
            GameObject gameMap = GameUtils.GetRootGameObjectByName("Map");

            if (gameMap == null)
            {
                GameObject mapPrefab = GameUtils.GetSelectedMap();
                
                if (mapPrefab != null)
                {
                    gameMap = Instantiate(mapPrefab, mapPrefab.transform.position, mapPrefab.transform.rotation);
                    gameMap.name = "Map";
                }
            }
            
            mapLogic = gameMap.GetComponent<MapLogic>();
            readyToStart = true;
        }

        /// <summary>
        /// Updates the player's current money value
        /// </summary>
        /// <param name="amount"> Amount to add (or subtract if the given int is negative) </param>
        public void UpdatePlayerMoney(int amount)
        {
            if (gameActive)
            {
                currentPlayerMoney += amount;

                if (amount < 0)
                {
                    if (PlayerDataUtils.IsSoundEnabled())
                    {
                        GetComponent<AudioSource>().PlayOneShot(spendMoneySoundEffect);
                    }
                }

                if (currentPlayerMoney < 0)
                {
                    Debug.Log("You're bankrupt?");
                }
                
                // Attempt to update money count text
                string moneyString = $"Money: ${currentPlayerMoney}";
                if (moneyCounterText.GetComponent<Text>().text != moneyString)
                {
                    moneyCounterText.GetComponent<Text>().text = moneyString;
                }
            }
        }
    
        /// <summary>
        /// Subtracts the given amount from the player's total health and handles 'death' / game over 
        /// </summary>
        /// <param name="amount">Number of health points to subtract</param>
        public void UpdatePlayerHealth(int amount)
        {
            if (gameActive)
            {
                currentPlayerHealth += amount;

                if (currentPlayerHealth <= 0)
                {
                    currentPlayerHealth = 0;
                    Debug.Log("Game Over!");
                    gameActive = false;
                }
                
                // Attempt to update health count text, also displays game over message when game is not active
                string healthString = $"Health: {currentPlayerHealth} / {playerStartingHealth}";
                if (currentPlayerHealth == 0)
                {
                    healthString = "Game Over!";
                }
                if (healthCounterText.GetComponent<Text>().text != healthString)
                {
                    healthCounterText.GetComponent<Text>().text = healthString;
                }
            }
        }
    }
}
