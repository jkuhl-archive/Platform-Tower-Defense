using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace Menus
{
    public class GameLoadingLogic : MonoBehaviour
    {
        private const string MainMenuSceneName = "MainMenu";
        private const string GameplaySceneName = "GameplayScene";
        private const float ScreenFadeTime = 1.5f;

        [Header("Loading Screen GameObjects")]
        [SerializeField] private CanvasGroup mainLoadingScreenCanvasGroup;
        [SerializeField] private CanvasGroup loadingUiCanvasGroup;
        [SerializeField] private GameObject loadingScreenCanvas;
        [SerializeField] private GameObject loadingProgressBar;

        [Header("Load Main Menu Toggle")]
        [SerializeField] private bool loadMainMenuOnStart;

        [Header("Debug Variables")]
        [SerializeField] private bool debugGameplay;
        [SerializeField] private string debugGameModeName;
        [SerializeField] private string debugGameMapName;

        // Start is called before the first frame update
        private void Start()
        {
            // Register the GameLoadingObject in GameUtils so we always have access to it,
            // destroy it if another is already registered
            var success = GameUtils.SetGameLoadingObject(gameObject);
            if (!success) Destroy(gameObject);

            // Prevent GameLoadingObject from being destroyed when a new scene is loaded
            DontDestroyOnLoad(gameObject);

            // If we are loading the main menu on object start, do so and then return
            if (loadMainMenuOnStart)
            {
                LoadMainMenu();
                return;
            }

            // If we are loading directly to a debug gameplay scene do so now
            if (debugGameplay)
            {
                InitializationBuffer.ResetBuffer();
                InitializationBuffer.SelectGameMode(debugGameModeName);
                InitializationBuffer.SelectGameMap(debugGameMapName);
                LoadGameplay();
            }
        }

        /// <summary>
        ///     Fades the given canvas group in or out
        /// </summary>
        /// <param name="canvasGroup"> Canvas group that we want to fade </param>
        /// <param name="enable"> If true fade in, if false date out </param>
        /// <returns> null while still in progress </returns>
        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, bool enable)
        {
            var startValue = canvasGroup.alpha;
            float targetValue = 0;
            float time = 0;

            if (enable) targetValue = 1;

            while (time < ScreenFadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / ScreenFadeTime);
                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetValue;
        }
        
        /// <summary>
        ///     Loads the background music prefab into the gameplay scene
        /// </summary>
        private void Gameplay_LoadBackgroundMusic()
        {
            var backgroundMusicPrefab = InitializationBuffer.GetBackgroundMusic();
            var backgroundMusic = Instantiate(backgroundMusicPrefab, backgroundMusicPrefab.transform.position,
                backgroundMusicPrefab.transform.rotation);
            backgroundMusic.name = backgroundMusicPrefab.name;
        }

        /// <summary>
        ///     Loads the game logic prefab into the gameplay scene
        /// </summary>
        private void Gameplay_LoadGameLogic()
        {
            var gameLogicPrefab = InitializationBuffer.GetGameLogic();
            var gameLogic = Instantiate(gameLogicPrefab, gameLogicPrefab.transform.position,
                gameLogicPrefab.transform.rotation);
            gameLogic.name = gameLogicPrefab.name;
        }

        /// <summary>
        ///     Loads the game UI prefab into the gameplay scene
        /// </summary>
        private void Gameplay_LoadGameUI()
        {
            var gameUiPrefab = InitializationBuffer.GetGameUI();
            var gameUi = Instantiate(gameUiPrefab, gameUiPrefab.transform.position, gameUiPrefab.transform.rotation);
            gameUi.name = gameUiPrefab.name;
        }

        /// <summary>
        ///     Loads the lighting prefab into the gameplay scene
        /// </summary>
        private void Gameplay_LoadLighting()
        {
            var lightingPrefab = InitializationBuffer.GetLighting();
            var lighting = Instantiate(lightingPrefab, lightingPrefab.transform.position,
                lightingPrefab.transform.rotation);
            lighting.name = lightingPrefab.name;
        }

        /// <summary>
        ///     Loads the main camera prefab into the gameplay scene
        /// </summary>
        private void Gameplay_LoadMainCamera()
        {
            var mainCameraPrefab = InitializationBuffer.GetMainCamera();
            var mainCamera = Instantiate(mainCameraPrefab, mainCameraPrefab.transform.position,
                mainCameraPrefab.transform.rotation);
            mainCamera.name = mainCameraPrefab.name;
        }

        /// <summary>
        ///     Loads the map prefab into the gameplay scene
        /// </summary>
        private void Gameplay_LoadMap()
        {
            var mapPrefab = InitializationBuffer.GetMap();
            var gameMap = Instantiate(mapPrefab, mapPrefab.transform.position, mapPrefab.transform.rotation);
            gameMap.name = mapPrefab.name;
        }
        
        /// <summary>
        ///     Loads the given scene with a fancy loading screen.
        ///     Also handles loading gameplay specific objects if loading the gameplay scene
        /// </summary>
        /// <param name="sceneName"> Name of the scene we want to load </param>
        /// <returns> null while still in progress </returns>
        private IEnumerator LoadNewScene(string sceneName)
        {
            // Sub method for updating the loading progress bar
            IEnumerator UpdateLoadingProgressBar()
            {
                var progressBar = loadingProgressBar.GetComponent<Slider>();
                var currentValue = progressBar.value;
                progressBar.value = Mathf.Clamp01(currentValue + .1f);

                // Wait for a few milliseconds to ensure the progress bar visually updates
                float waitTime = 0;
                while (waitTime < ScreenFadeTime / 4)
                {
                    waitTime += Time.deltaTime;
                    yield return null;
                }
            }

            // Fade out to loading screen
            yield return StartCoroutine(LoadingScreenFade(true));

            // Load new scene asynchronously
            Debug.Log($"Loading new scene: '{sceneName}'");
            var sceneLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!sceneLoad.isDone)
            {
                loadingProgressBar.GetComponent<Slider>().value = sceneLoad.progress / 2;
                yield return null;
            }

            // If we are loading the gameplay scene, load all the gameplay objects in
            if (sceneName == GameplaySceneName)
            {
                // Load map specific objects
                Gameplay_LoadBackgroundMusic();
                Gameplay_LoadLighting();
                Gameplay_LoadMainCamera();
                Gameplay_LoadMap();
                yield return StartCoroutine(UpdateLoadingProgressBar());

                // Load game mode specific objects
                Gameplay_LoadGameLogic();
                Gameplay_LoadGameUI();
                yield return StartCoroutine(UpdateLoadingProgressBar());
            }

            // Finish loading progress bar
            while (loadingProgressBar.GetComponent<Slider>().value < 1f)
                yield return StartCoroutine(UpdateLoadingProgressBar());

            // Fade in to new scene from loading screen
            yield return StartCoroutine(LoadingScreenFade(false));

            // If we are loading the gameplay scene, start the game
            if (sceneName == GameplaySceneName)
            {
                GameUtils.Gameplay_ResetGame();
                GameUtils.Gameplay_StartGame();
            }
        }

        /// <summary>
        ///     Handles fading in or out of the loading screen
        ///     Fades loading screen UI elements separately from background to produce a smooth effect
        /// </summary>
        /// <param name="enable"> If true fade in to the loading screen, if false fade out </param>
        /// <returns> null while still in progress </returns>
        private IEnumerator LoadingScreenFade(bool enable)
        {
            // Fade in loading screen
            if (enable)
            {
                loadingScreenCanvas.SetActive(true);
                loadingProgressBar.GetComponent<Slider>().value = 0;

                // Fade in loading screen canvas
                yield return StartCoroutine(FadeCanvasGroup(mainLoadingScreenCanvasGroup, true));

                // Wait a few milliseconds for smoothness
                float waitTime = 0;
                while (waitTime < ScreenFadeTime / 4)
                {
                    waitTime += Time.deltaTime;
                    yield return null;
                }

                // Fade in loading UI elements
                yield return StartCoroutine(FadeCanvasGroup(loadingUiCanvasGroup, true));
            }

            // Fade out loading screen
            if (!enable)
            {
                // Fade out loading UI elements
                yield return StartCoroutine(FadeCanvasGroup(loadingUiCanvasGroup, false));

                // Wait a few milliseconds for smoothness
                float waitTime = 0;
                while (waitTime < ScreenFadeTime / 4)
                {
                    waitTime += Time.deltaTime;
                    yield return null;
                }

                // Fade out loading screen canvas
                yield return StartCoroutine(FadeCanvasGroup(mainLoadingScreenCanvasGroup, false));

                loadingScreenCanvas.SetActive(false);
            }
        }

        /// <summary>
        ///     Loads the gameplay scene
        /// </summary>
        public void LoadGameplay()
        {
            StartCoroutine(LoadNewScene(GameplaySceneName));
        }

        /// <summary>
        ///     Loads the main menu
        /// </summary>
        public void LoadMainMenu()
        {
            StartCoroutine(LoadNewScene(MainMenuSceneName));
        }
    }
}