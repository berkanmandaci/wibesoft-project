using UnityEngine;
using WibeSoft.Core.Managers;
using WibeSoft.UI.Views.HudViews;
using WibeSoft.Core.Bootstrap;

namespace WibeSoft.UI.Controllers.HudControllers
{
    public class LevelContainerController : MonoBehaviour
    {
        [SerializeField] private LevelContainerView _levelContainerView;
        
        private PlayerManager _playerManager => PlayerManager.Instance;
        private LogManager _logger => LogManager.Instance;

        private void OnEnable()
        {
            GameEvents.OnPlayerLevelChanged += HandlePlayerLevelChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerLevelChanged -= HandlePlayerLevelChanged;
        }

        
        private void HandlePlayerLevelChanged(int level, int currentExp, int maxExp)
        {
            UpdateView(maxExp, currentExp, level, _playerManager.PlayerData.Username);
        }

        public void UpdateView(int maxExp, int currentExp, int level, string username)
        {
            _levelContainerView.Init(maxExp, currentExp, level, username);
            _logger.LogInfo($"Level container updated. Level: {level}, Exp: {currentExp}/{maxExp}", "LevelContainerController");
        }
    }
} 