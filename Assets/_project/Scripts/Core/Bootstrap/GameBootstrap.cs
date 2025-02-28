using Cysharp.Threading.Tasks;
using System;
using WibeSoft.Core.DI;
using WibeSoft.Core.Managers;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Bootstrap
{
    /// <summary>
    /// Game bootstrap class - Manages initialization of all systems
    /// </summary>
    public class GameBootstrap : SingletonBehaviour<GameBootstrap>
    {
        private LogManager _logger;

        private void Start()
        {
            Init().Forget();
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
                // LogManager
                _logger = LogManager.Instance;
                ServiceLocator.RegisterService(_logger);
                _logger.LogInfo("Initializing core systems...", "Bootstrap");

                // SaveManager
                var saveManager = SaveManager.Instance;
                ServiceLocator.RegisterService(saveManager);

                // TimeManager
                var timeManager = TimeManager.Instance;
                ServiceLocator.RegisterService(timeManager);
                await timeManager.Initialize();

                // PoolManager
                var poolManager = PoolManager.Instance;
                ServiceLocator.RegisterService(poolManager);
                await poolManager.Initialize();

                // AudioManager
                var audioManager = AudioManager.Instance;
                ServiceLocator.RegisterService(audioManager);

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
                // ScriptableObject configs
                await LoadConfigurations();

                // Save data
                var saveData = await LoadSaveData();

                // Player data
                await InitializePlayerData(saveData);

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
                var uiManager = UIManager.Instance;
                ServiceLocator.RegisterService(uiManager);
                await uiManager.Initialize();

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
                // Grid system
                var gridManager = GridManager.Instance;
                ServiceLocator.RegisterService(gridManager);

                // Farming system
                var farmingManager = FarmingManager.Instance;
                ServiceLocator.RegisterService(farmingManager);
                await farmingManager.Initialize();

                // Inventory system
                var inventoryManager = InventoryManager.Instance;
                ServiceLocator.RegisterService(inventoryManager);
                await inventoryManager.Initialize();

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

        private async UniTask<SaveData> LoadSaveData()
        {
            // TODO: Load save data
            await UniTask.CompletedTask;
            return new SaveData();
        }

        private async UniTask InitializePlayerData(SaveData saveData)
        {
            // TODO: Initialize player data
            await UniTask.CompletedTask;
        }

        private async UniTask HandleBootstrapError(Exception ex)
        {
            _logger.LogError($"Bootstrap error caught: {ex.Message}", "Bootstrap");

            // Clean up critical systems
            await CleanupSystems();

            // Show error screen
            await ShowErrorScreen(ex.Message);

            // Retry mechanism
            if (await ShouldRetryBootstrap())
            {
                await UniTask.DelayFrame(60); // Wait for 1 second
                await RetryBootstrap();
            }
        }

        private async UniTask CleanupSystems()
        {
            DIContainer.Instance.Clear();
            await UniTask.CompletedTask;
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

    public class SaveData { }
}
