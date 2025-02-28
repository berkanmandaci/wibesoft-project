using UnityEngine;
using WibeSoft.Core.Managers;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Core.Singleton;
using Cysharp.Threading.Tasks;
using WibeSoft.UI.Controllers.HudControllers;

namespace WibeSoft.UI.Controllers
{
    public class HudController : SingletonBehaviour<HudController>
    {
        [Header("HUD References")]
        [SerializeField] private LevelContainerController _levelContainer;

        private PlayerManager _playerManager => PlayerManager.Instance;
        private LogManager _logger => LogManager.Instance;

        public async UniTask Initialize()
        {
            _logger.LogInfo("Initializing HUD...", "HudController");
            
            if (_levelContainer != null)
            {
                InitializeLevelContainer();
            }
            else
            {
                _logger.LogWarning("Level container reference is missing!", "HudController");
            }

            await UniTask.CompletedTask;
        }

        private void InitializeLevelContainer()
        {
            var playerData = _playerManager.PlayerData;
            _levelContainer.UpdateView(playerData.MaxExp, playerData.CurrentExp, playerData.Level, playerData.Username);
            _logger.LogInfo("Level container initialized", "HudController");
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
            GameEvents.OnPlayerLevelChanged += HandlePlayerLevelChanged;
            GameEvents.OnGameReady += HandleGameReady;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPlayerLevelChanged -= HandlePlayerLevelChanged;
            GameEvents.OnGameReady -= HandleGameReady;
        }

        private void HandleGameReady()
        {
            InitializeLevelContainer();
        }

        private void HandlePlayerLevelChanged(int level, int currentExp, int maxExp)
        {
            if (_levelContainer != null)
            {
                _levelContainer.UpdateView(maxExp, currentExp, level, _playerManager.PlayerData.Username);
            }
        }
    }
} 