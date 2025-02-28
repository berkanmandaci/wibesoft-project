using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Core.Managers;
using WibeSoft.Core.Bootstrap;

namespace WibeSoft.UI.Controllers.HudControllers
{
    public class HudController : SingletonBehaviour<HudController>
    {
        [Header("Sub Controllers")]
        [SerializeField] private LevelContainerController _levelContainerController;
        [SerializeField] private WalletController _walletController;

        private PlayerManager _playerManager => PlayerManager.Instance;
        private LogManager _logger => LogManager.Instance;

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
            GameEvents.OnGameReady += HandleGameReady;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnGameReady -= HandleGameReady;
        }

        public async UniTask Initialize()
        {
            _logger.LogInfo("Initializing HUD Controller", "HudController");

            if (_levelContainerController == null)
            {
                _logger.LogError("LevelContainerController reference is missing!", "HudController");
                return;
            }

            if (_walletController == null)
            {
                _logger.LogError("WalletController reference is missing!", "HudController");
                return;
            }

            await InitializeSubControllers();
            _logger.LogInfo("HUD Controller initialized", "HudController");
        }

        private async UniTask InitializeSubControllers()
        {
            try
            {
                // Level Container başlatma
                var playerData = _playerManager.PlayerData;
                _levelContainerController.UpdateView(
                    playerData.MaxExp,
                    playerData.CurrentExp,
                    playerData.Level,
                    playerData.Username
                );

                // Wallet başlatma
                _walletController.UpdateWalletView();

                _logger.LogInfo("Sub-controllers initialized successfully", "HudController");
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error initializing sub-controllers: {ex.Message}", "HudController");
                throw;
            }
        }

        private void HandleGameReady()
        {
            _logger.LogInfo("Game ready event received, refreshing HUD", "HudController");
            InitializeSubControllers().Forget();
        }
    }
} 