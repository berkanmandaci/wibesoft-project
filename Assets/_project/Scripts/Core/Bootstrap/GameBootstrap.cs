using Cysharp.Threading.Tasks;
using System;
using WibeSoft.Core.Managers;
using WibeSoft.Core.Singleton;
using UnityEngine;

namespace WibeSoft.Core.Bootstrap
{
    /// <summary>
    /// Game bootstrap class - Manages initialization of all systems
    /// </summary>
    public class GameBootstrap : SingletonBehaviour<GameBootstrap>
    {
        private LogManager _logger => LogManager.Instance;

        private void Start()
        {
            Init().Forget();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnGameQuit += HandleGameQuit;
            GameEvents.OnCropPlanted += HandleCropPlanted;
            GameEvents.OnCropHarvested += HandleCropHarvested;
            GameEvents.OnCropStateChanged += HandleCropStateChanged;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnGameQuit -= HandleGameQuit;
            GameEvents.OnCropPlanted -= HandleCropPlanted;
            GameEvents.OnCropHarvested -= HandleCropHarvested;
            GameEvents.OnCropStateChanged -= HandleCropStateChanged;
        }

        private async void HandleGameQuit()
        {
            await SaveAllGameData();
            _logger.LogInfo("Game data saved on quit", "GameBootstrap");
        }

        private async void HandleCropPlanted(Vector2Int position, string cropId)
        {
            await SaveAllGameData();
            _logger.LogInfo("Game data saved after planting", "GameBootstrap");
        }

        private async void HandleCropHarvested(Vector2Int position)
        {
            await SaveAllGameData();
            _logger.LogInfo("Game data saved after harvesting", "GameBootstrap");
        }

        private async void HandleCropStateChanged(Vector2Int position, string state)
        {
            await SaveAllGameData();
            _logger.LogInfo("Game data saved after crop state change", "GameBootstrap");
        }

        private async UniTask SaveAllGameData()
        {
            try
            {
                await GridManager.Instance.SaveGridData();
                await InventoryManager.Instance.SaveInventoryData();
                await PlayerManager.Instance.SavePlayerData();
                GameEvents.TriggerGameSaved();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving game data: {ex.Message}", "GameBootstrap");
            }
        }

        private async UniTask Init()
        {
            try
            {
                await InitializeCore();
                await InitializeData();
                await InitializeUI();
                await InitializeGameplay();

                OnBootstrapComplete();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Bootstrap error: {ex.Message}", "Bootstrap");
                await HandleBootstrapError(ex);
            }
        }

        private async UniTask InitializeCore()
        {
            await UniTask.DelayFrame(1);
            
            try
            {
                _logger.LogInfo("Initializing core systems...", "Bootstrap");

                // TimeManager
                await TimeManager.Instance.Initialize();

                // PoolManager
                await PoolManager.Instance.Initialize();

                // JsonDataService
                await JsonDataService.Instance.Initialize();

                // AudioManager
                var audioManager = AudioManager.Instance;

                // PlayerManager
                await PlayerManager.Instance.Initialize();

                _logger.LogInfo("Core systems initialized", "Bootstrap");
            }
            catch (Exception ex)
            {
                throw new BootstrapException("Failed to initialize core systems", ex);
            }
        }

        private async UniTask InitializeData()
        {
            await UniTask.DelayFrame(1);
            _logger.LogInfo("Loading data...", "Bootstrap");

            try
            {
                // JsonDataService initialization
                await JsonDataService.Instance.Initialize();

                // ScriptableObject configs
                await LoadConfigurations();

                // Player Manager initialization
                await PlayerManager.Instance.Initialize();

                _logger.LogInfo("Data loaded successfully", "Bootstrap");
            }
            catch (Exception ex)
            {
                throw new BootstrapException("Failed to load data", ex);
            }
        }

        private async UniTask InitializeUI()
        {
            await UniTask.DelayFrame(1);
            _logger.LogInfo("Initializing UI...", "Bootstrap");

            try
            {
                // UI Manager
                await UIManager.Instance.Initialize();

                _logger.LogInfo("UI initialized", "Bootstrap");
            }
            catch (Exception ex)
            {
                throw new BootstrapException("Failed to initialize UI", ex);
            }
        }

        private async UniTask InitializeGameplay()
        {
            await UniTask.DelayFrame(1);
            _logger.LogInfo("Initializing gameplay systems...", "Bootstrap");

            try
            {
                // Crop service
                await CropService.Instance.Initialize();

                // Grid system
                await GridManager.Instance.Initialize();

                // Inventory system
                await InventoryManager.Instance.Initialize();

                _logger.LogInfo("Gameplay systems initialized", "Bootstrap");
            }
            catch (Exception ex)
            {
                throw new BootstrapException("Failed to initialize gameplay systems", ex);
            }
        }

        #region Helper Methods

        private async UniTask LoadConfigurations()
        {
            // TODO: Load ScriptableObject configurations
            await UniTask.CompletedTask;
        }

        private async UniTask HandleBootstrapError(Exception ex)
        {
            _logger.LogError($"Bootstrap error caught: {ex.Message}", "Bootstrap");

            // Show error screen
            await ShowErrorScreen(ex.Message);

            // Retry mechanism
            if (await ShouldRetryBootstrap())
            {
                await UniTask.DelayFrame(60); // Wait for 1 second
                await RetryBootstrap();
            }
        }

        private async UniTask ShowErrorScreen(string message)
        {
            _logger.LogError($"Error screen should show: {message}", "Bootstrap");
            await UniTask.CompletedTask;
        }

        private async UniTask<bool> ShouldRetryBootstrap()
        {
            // TODO: Implement retry logic
            await UniTask.CompletedTask;
            return false;
        }

        private async UniTask RetryBootstrap()
        {
            await Init();
        }

        private void OnBootstrapComplete()
        {
            _logger.LogInfo("Bootstrap completed successfully", "Bootstrap");
            GameEvents.TriggerGameReady();
        }

        #endregion
    }

    public class BootstrapException : Exception
    {
        public BootstrapException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
