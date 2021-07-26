using Data;
using Gameplay.Towers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Gameplay
{
    public class PlayerLogic : MonoBehaviour
    {
        [Header("Gameplay Sound Effects")]
        [SerializeField] private AudioClip selectionSoundEffect;
        [SerializeField] private AudioClip spendMoneySoundEffect;
        
        // Game state variables
        private int currentPlayerHealth;
        private int currentPlayerMoney;

        // Gameplay GameObjects
        private MapLogic mapLogic;
        private GameObject previousSelectedObject;
        private GameObject selectedObject;

        // Start is called before the first frame update
        private void Start()
        {
            // Set player state variables to starting values 
            currentPlayerHealth = GameplayPlayerConstants.PlayerStartingHealth;
            currentPlayerMoney = GameplayPlayerConstants.PlayerStartingMoney;
            GameUtils.GetBaseGameUI().UpdateHealthCountText(currentPlayerHealth);
            GameUtils.GetBaseGameUI().UpdateMoneyCountText(currentPlayerMoney);

            // Store the MapLogic component for ease of access
            mapLogic = GameUtils.GetMapLogic();

            // Set saved player preferences
            GameUtils.GetRootGameObjectByName("BackgroundMusic").SetActive(
                PlayerPrefsUtils.GetBool(PlayerPrefsConstants.KeySoundEnabled, PlayerPrefsConstants.DefaultSoundEnabled));
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameUtils.IsGameInProgress() && !GameUtils.IsGamePaused())
            {
                UpdateSelectedObject();
                HighlightSelectedObjects();
            }
        }

        /// <summary>
        ///     Highlights certain types of GameObjects when selected via the mouse
        ///     TODO: [PERFORMANCE] This can probably be refactored so that we don't need to iterate over the lists every run
        /// </summary>
        private void HighlightSelectedObjects()
        {
            // Highlights a platform space GameObject
            void HighlightPlatformSpace(GameObject platformSpace, bool toggle)
            {
                if (platformSpace.GetComponent<MeshRenderer>().enabled != toggle)
                {
                    platformSpace.GetComponent<MeshRenderer>().enabled = toggle;

                    if (toggle)
                        if (GameSettingsUtils.IsSoundEnabled())
                            GetComponent<AudioSource>().PlayOneShot(selectionSoundEffect);
                }
            }

            // Attempt to highlight platform space objects
            foreach (var obj in mapLogic.platformSpaceList)
            {
                var toggle = obj == selectedObject ||
                             obj == previousSelectedObject && mapLogic.platformList.Contains(selectedObject);

                HighlightPlatformSpace(obj, toggle);
            }

            // Attempt to highlight platform objects or child objects of a platform such as a socket or tower
            foreach (var obj in mapLogic.platformList)
            {
                // Attempt to highlight platform objects
                var platformToggle = obj == selectedObject ||
                                     obj == previousSelectedObject &&
                                     mapLogic.platformSpaceList.Contains(selectedObject);
                obj.GetComponent<SwapMaterial>().Swap(platformToggle, GetComponent<AudioSource>());

                for (var i = 0; i < obj.transform.childCount; i++)
                {
                    // Attempt to highlight socket objects
                    var socket = obj.transform.GetChild(i).gameObject;
                    var socketToggle = socket == selectedObject && CanBuyTower(0);
                    socket.GetComponent<SwapMaterial>().Swap(socketToggle, GetComponent<AudioSource>());

                    // Attempt to highlight tower objects
                    if (socket.transform.childCount > 0)
                    {
                        var tower = socket.transform.GetChild(0).gameObject;
                        var towerToggle = socket == selectedObject || tower == selectedObject;
                        tower.GetComponent<BaseTowerLogic>().HighlightTowerRange(towerToggle);
                    }
                }
            }
        }

        /// <summary>
        ///     Captures a game object clicked on by the player
        /// </summary>
        private void UpdateSelectedObject()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
        ///     Purchases a platform and adds it to the map in place of the currently selected platform space
        /// </summary>
        public void BuyPlatform()
        {
            if (CanBuyPlatform())
            {
                UpdatePlayerMoney(-GameplayPriceConstants.BuyPlatformCost);

                var newPlatform = Instantiate(mapLogic.GetPlatformPrefab(), selectedObject.transform.position,
                    mapLogic.GetPlatformPrefab().transform.rotation);
                mapLogic.platformList.Add(newPlatform);

                mapLogic.platformSpaceList.Remove(selectedObject);
                Destroy(selectedObject);
                selectedObject = null;

                Debug.Log("Purchased platform");
                GameUtils.GetBaseGameUI().GetUiObjectByName("BuyPlatformButton").GetComponent<Button>().interactable =
                    false;
                GameUtils.GetBaseGameUI().UpdatePlatformCountText();
            }
        }

        /// <summary>
        ///     Purchases a tower and places it in the currently selected socket
        /// </summary>
        /// <param name="towerPrefab"> Prefab for the tower we want to buy </param>
        public void BuyTower(GameObject towerPrefab)
        {
            var towerCost = -1;

            switch (towerPrefab.name)
            {
                default:
                    towerCost = -1;
                    break;

                case "CannonTower":
                    towerCost = GameplayPriceConstants.BuyCannonTowerCost;
                    break;

                case "BleedTower":
                    towerCost = GameplayPriceConstants.BuyCannonTowerCost;
                    break;
            }

            if (towerCost == -1) return;

            if (CanBuyTower(towerCost))
            {
                UpdatePlayerMoney(-towerCost);

                var newTower = Instantiate(towerPrefab, selectedObject.transform.position,
                    towerPrefab.transform.rotation);
                newTower.transform.parent = selectedObject.transform;

                Debug.Log($"Purchased tower {towerPrefab.name}");
            }
        }

        /// <summary>
        ///     Returns true when:
        ///     - The player clicks on an open platform space
        ///     - The maximum number of platforms has not been reached
        ///     - The player has the funds to purchase a platform
        /// </summary>
        /// <returns> True or False if criteria above are met or not </returns>
        public bool CanBuyPlatform()
        {
            if (mapLogic.platformSpaceList.Contains(selectedObject) &&
                mapLogic.platformList.Count < mapLogic.GetMaxPlatformCount() &&
                currentPlayerMoney >= GameplayPriceConstants.BuyPlatformCost)
                return true;

            return false;
        }

        /// <summary>
        ///     Returns true when:
        ///     - The player clicks on an empty socket on a platform
        ///     - The player has the funds to purchase the tower
        /// </summary>
        /// <param name="towerCost"> Cost of the tower we are checking for </param>
        /// <returns> True or False if criteria above are met or not </returns>
        public bool CanBuyTower(int towerCost)
        {
            // Sub-method for checking if the selected object is a platform socket or not
            bool SelectedObjectIsSocket()
            {
                if (selectedObject != null &&
                    selectedObject.transform.parent != null &&
                    mapLogic.platformList.Contains(selectedObject.transform.parent.gameObject))
                    return true;

                return false;
            }

            if (SelectedObjectIsSocket() &&
                selectedObject.transform.childCount == 0 &&
                currentPlayerMoney >= towerCost)
                return true;

            return false;
        }

        /// <summary>
        ///     Returns true when:
        ///     - The player clicks on a platform, then clicks on an open platform space (can do this in reverse order as well)
        ///     - The player has the funds to move a platform
        /// </summary>
        /// <returns> True or False if criteria above are met or not </returns>
        public bool CanMovePlatform()
        {
            if ((mapLogic.platformList.Contains(selectedObject) &&
                 mapLogic.platformSpaceList.Contains(previousSelectedObject) ||
                 mapLogic.platformList.Contains(previousSelectedObject) &&
                 mapLogic.platformSpaceList.Contains(selectedObject)) &&
                currentPlayerMoney >= GameplayPriceConstants.MovePlatformCost)
                return true;

            return false;
        }

        /// <summary>
        ///     Moves the selected platform to an open platform space
        /// </summary>
        public void MovePlatform()
        {
            if (CanMovePlatform())
            {
                GameObject platform = null;
                GameObject platformSpace = null;

                if (mapLogic.platformList.Contains(selectedObject))
                    platform = selectedObject;
                else if (mapLogic.platformSpaceList.Contains(selectedObject)) platformSpace = selectedObject;

                if (mapLogic.platformList.Contains(previousSelectedObject))
                    platform = previousSelectedObject;
                else if (mapLogic.platformSpaceList.Contains(previousSelectedObject))
                    platformSpace = previousSelectedObject;

                if (platform != null && platformSpace != null)
                {
                    UpdatePlayerMoney(-GameplayPriceConstants.MovePlatformCost);

                    var newPlatformPosition = platformSpace.transform.position;
                    var newPlatformSpacePosition = platform.transform.position;

                    platform.transform.position = newPlatformPosition;
                    platformSpace.transform.position = newPlatformSpacePosition;

                    Debug.Log("Moved platform");
                }
            }
        }

        /// <summary>
        ///     Updates the player's current money value
        /// </summary>
        /// <param name="amount"> Amount to add (or subtract if the given int is negative) </param>
        public void UpdatePlayerMoney(int amount)
        {
            if (GameUtils.IsGameInProgress())
            {
                currentPlayerMoney += amount;

                if (amount < 0)
                    if (GameSettingsUtils.IsSoundEnabled())
                        GetComponent<AudioSource>().PlayOneShot(spendMoneySoundEffect);

                if (currentPlayerMoney < 0) Debug.Log("You're bankrupt?");

                GameUtils.GetBaseGameUI().UpdateMoneyCountText(currentPlayerMoney);
            }
        }

        /// <summary>
        ///     Updates the player's current health and handles 'death' / game over
        /// </summary>
        /// <param name="amount"> Amount to add (or subtract if the given int is negative) </param>
        public void UpdatePlayerHealth(int amount)
        {
            if (GameUtils.IsGameInProgress())
            {
                currentPlayerHealth += amount;

                if (currentPlayerHealth <= 0)
                {
                    currentPlayerHealth = 0;
                    GameUtils.Gameplay_GameOver();
                }

                GameUtils.GetBaseGameUI().UpdateHealthCountText(currentPlayerHealth);
            }
        }
    }
}