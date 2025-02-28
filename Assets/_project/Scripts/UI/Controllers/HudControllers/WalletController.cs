using UnityEngine;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Core.Managers;
using WibeSoft.UI.Views.HudViews;

namespace WibeSoft.UI.Controllers.HudControllers
{
    public class WalletController : MonoBehaviour
    {
        [SerializeField] private WalletContainerView _walletContainerView;
        
        private PlayerManager _playerManager => PlayerManager.Instance;
        private LogManager _logger => LogManager.Instance;

        private void OnEnable()
        {
            GameEvents.OnCurrencyChanged += HandleCurrencyChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnCurrencyChanged -= HandleCurrencyChanged;
        }

        private void HandleCurrencyChanged(int gold, int gem)
        {
            UpdateWalletView();
            _logger.LogInfo($"Wallet updated: Gold: {gold}, Gem: {gem}", "WalletController");
        }

        public void UpdateWalletView()
        {
            var gold = _playerManager.PlayerData.Currency.Gold;
            var gem = _playerManager.PlayerData.Currency.Gem;
            _walletContainerView.Init(gold, gem);
        }
    }
} 