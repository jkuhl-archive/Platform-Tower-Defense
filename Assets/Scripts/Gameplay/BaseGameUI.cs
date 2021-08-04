using Data;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Gameplay
{
    public class BaseGameUI : MonoBehaviour
    {
        // Game UI element names
        private const string BuyPlatformButtonName = "BuyPlatformButton";
        private const string MovePlatformButtonName = "MovePlatformButton";
        private const string BuyCannonTowerButtonName = "BuyCannonTowerButton";
        private const string BuyBleedTowerButtonName = "BuyBleedTowerButton";
        private const string MoneyCounterTextName = "MoneyCounter";
        private const string HealthCounterTextName = "HealthCounter";
        private const string WaveCounterTextName = "WaveCounter";
        private const string WaveStatusTextName = "WaveStatus";
        private const string PlatformCounterTextName = "PlatformCounter";

        [Header("UI Sound Effects")]
        [SerializeField] private AudioClip buttonClickSoundEffect;

        // Start is called before the first frame update
        private void Start()
        {
            UpdateCostTextObjects();
            UpdatePlatformCountText();
            gameObject.GetComponent<Canvas>().worldCamera =
                GameUtils.GetRootGameObjectByName("MainCamera").GetComponent<Camera>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameUtils.IsGameInProgress() && !GameUtils.IsGamePaused())
            {
                UpdateButtonStatus();
                UpdateWaveTextObjects();
            }
        }

        /// <summary>
        ///     Enables and disabled button objects based on the player's ability to use them
        /// </summary>
        private void UpdateButtonStatus()
        {
            // Attempt to update buy platform button
            var canBuyPlatform = GameUtils.GetPlayerLogic().CanBuyPlatform();
            var buyPlatformButton = GetUiObjectByName(BuyPlatformButtonName).GetComponent<Button>();
            if (buyPlatformButton.interactable != canBuyPlatform) buyPlatformButton.interactable = canBuyPlatform;

            // Attempt to update move platform button
            var canMovePlatform = GameUtils.GetPlayerLogic().CanMovePlatform();
            var movePlatformButton = GetUiObjectByName(MovePlatformButtonName).GetComponent<Button>();
            if (movePlatformButton.interactable != canMovePlatform) movePlatformButton.interactable = canMovePlatform;

            // Attempt to update buy cannon tower button
            var canBuyCannonTower = GameUtils.GetPlayerLogic().CanBuyTower(GameplayPriceConstants.BuyCannonTowerCost);
            var buyCannonTowerButton = GetUiObjectByName(BuyCannonTowerButtonName).GetComponent<Button>();
            if (buyCannonTowerButton.interactable != canBuyCannonTower)
                buyCannonTowerButton.interactable = canBuyCannonTower;
            
            // Attempt to update buy bleed tower button
            var canBuyBleedTower = GameUtils.GetPlayerLogic().CanBuyTower(GameplayPriceConstants.BuyBleedTowerCost);
            var buyBleedTowerButton = GetUiObjectByName(BuyBleedTowerButtonName).GetComponent<Button>();
            if (buyBleedTowerButton.interactable != canBuyBleedTower)
                buyBleedTowerButton.interactable = canBuyBleedTower;
        }

        /// <summary>
        ///     Updates cost related text objects based on current game status
        /// </summary>
        private void UpdateCostTextObjects()
        {
            // Attempt to update buy platform button text
            var buyPlatformString = $"Buy Platform: ${GameplayPriceConstants.BuyPlatformCost}";
            var buyPlatformText = GetUiObjectByName(BuyPlatformButtonName).GetComponentInChildren<Text>();
            if (buyPlatformText.text != buyPlatformString) buyPlatformText.text = buyPlatformString;

            // Attempt to update move platform text
            var movePlatformString = $"Move Platform: ${GameplayPriceConstants.MovePlatformCost}";
            var movePlatformText = GetUiObjectByName(MovePlatformButtonName).GetComponentInChildren<Text>();
            if (movePlatformText.text != movePlatformString) movePlatformText.text = movePlatformString;

            // Attempt to update buy cannon tower text
            var buyCannonTowerString = $"Buy Cannon Tower: ${GameplayPriceConstants.BuyCannonTowerCost}";
            var buyCannonTowerText = GetUiObjectByName(BuyCannonTowerButtonName).GetComponentInChildren<Text>();
            if (buyCannonTowerText.text != buyCannonTowerString) buyCannonTowerText.text = buyCannonTowerString;
            
            // Attempt to update buy bleed tower text
            var buyBleedTowerString = $"Buy Bleed Tower: ${GameplayPriceConstants.BuyBleedTowerCost}";
            var buyBleedTowerText = GetUiObjectByName(BuyBleedTowerButtonName).GetComponentInChildren<Text>();
            if (buyBleedTowerText.text != buyBleedTowerString) buyBleedTowerText.text = buyBleedTowerString;
        }
        
        /// <summary>
        ///     Updates wave related text objects based on current wave status
        /// </summary>
        private void UpdateWaveTextObjects()
        {
            // Attempt to update wave count text
            var waveCounterString = $"Wave: {GameUtils.GetWaveManager().GetCurrentWaveNumber()}";
            var waveCounterText = GetUiObjectByName(WaveCounterTextName).GetComponent<Text>();

            if (waveCounterText.text != waveCounterString) waveCounterText.text = waveCounterString;

            // Attempt to update wave info text
            var waveWaveStatusString = GameUtils.GetWaveManager().GetWaveStatusMessage();
            var waveInfoText = GetUiObjectByName(WaveStatusTextName).GetComponent<Text>();
            if (waveInfoText.text != waveWaveStatusString) waveInfoText.text = waveWaveStatusString;
        }
        
        /// <summary>
        ///     Handler for all 'Buy Tower' buttons
        /// </summary>
        /// <param name="towerPrefab"> Prefab for the tower we want to buy </param>
        public void BuyTowerButtonHandler(GameObject towerPrefab)
        {
            Debug.Log($"Clicked buy button for tower: '{towerPrefab.name}'");
            if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(buttonClickSoundEffect);

            GameUtils.GetPlayerLogic().BuyTower(towerPrefab);
        }

        /// <summary>
        ///     Gets the GameObject for the given UI element by name
        ///     Only able to access child GameObjects of this object
        /// </summary>
        /// <param name="objectName"> Name of the UI element that we want to get </param>
        /// <returns> GameObject for the UI element, or null if it cannot be found </returns>
        public GameObject GetUiObjectByName(string objectName)
        {
            var childUiObjects = GetComponentsInChildren<Transform>();
            foreach (var child in childUiObjects)
                if (child.gameObject.name == objectName)
                    return child.gameObject;

            return null;
        }

        /// <summary>
        ///     Handles button actions
        /// </summary>
        /// <param name="buttonGameObject"> GameObject for the button that was selected </param>
        public void UiButtonHandler(GameObject buttonGameObject)
        {
            Debug.Log($"Clicked button: '{buttonGameObject.name}'");
            if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(buttonClickSoundEffect);

            switch (buttonGameObject.name)
            {
                // Platform interaction buttons
                case "BuyPlatformButton":
                    GameUtils.GetPlayerLogic().BuyPlatform();
                    break;
                case "MovePlatformButton":
                    GameUtils.GetPlayerLogic().MovePlatform();
                    break;

                // Main menu buttons
                case "ReturnToMainMenuButton":
                    GameUtils.Gameplay_PauseGame(true);
                    GameUtils.GetGameLoadingLogic().LoadMainMenu();
                    break;
            }
        }
        
        /// <summary>
        ///     Updates the health text object to match the player's current health
        /// </summary>
        /// <param name="currentPlayerHealth"> Player's current health as an int </param>
        public void UpdateHealthCountText(int currentPlayerHealth)
        {
            var healthString = $"Health: {currentPlayerHealth} / {GameplayPlayerConstants.PlayerStartingHealth}";
            var healthCounterText = GetUiObjectByName(HealthCounterTextName).GetComponent<Text>();
            if (currentPlayerHealth == 0) healthString = "Game Over!";

            if (healthCounterText.text != healthString) healthCounterText.text = healthString;
        }

        /// <summary>
        ///     Updates the money text object to match the player's current money
        /// </summary>
        /// <param name="currentPlayerMoney"> Player's current money as an int </param>
        public void UpdateMoneyCountText(int currentPlayerMoney)
        {
            var moneyString = $"Money: ${currentPlayerMoney}";
            var moneyCounterText = GetUiObjectByName(MoneyCounterTextName).GetComponent<Text>();
            if (moneyCounterText.text != moneyString) moneyCounterText.text = moneyString;
        }

        /// <summary>
        ///     Updates the platform counter text object to match the amount of platforms currently in play
        /// </summary>
        public void UpdatePlatformCountText()
        {
            var platformsString =
                $"Platforms: {GameUtils.GetMapLogic().platformList.Count} / {GameUtils.GetMapLogic().GetMaxPlatformCount()}";
            var platformCountText = GetUiObjectByName(PlatformCounterTextName).GetComponent<Text>();
            if (platformCountText.text != platformsString) platformCountText.text = platformsString;
        }
    }
}