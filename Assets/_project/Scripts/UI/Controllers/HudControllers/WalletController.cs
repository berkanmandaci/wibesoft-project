using UnityEngine;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Core.Managers;
using WibeSoft.UI.Views.HudViews;

namespace WibeSoft.UI.Controllers
{
    public class WalletController : MonoBehaviour
    {
        [SerializeField] private WalletContainerView _walletView;
        
        private InventoryManager _inventoryManager => InventoryManager.Instance;
        private LogManager _logger => LogManager.Instance;

        private void OnEnable()
        {
            GameEvents.OnMoneyChanged += HandleMoneyChanged;
            GameEvents.OnInventoryItemChanged += HandleInventoryItemChanged;
            
            UpdateWalletView();
        }

        private void OnDisable()
        {
            GameEvents.OnMoneyChanged -= HandleMoneyChanged;
            GameEvents.OnInventoryItemChanged -= HandleInventoryItemChanged;
        }

        private void HandleMoneyChanged(int amount)
        {
            UpdateWalletView();
            _logger.LogInfo($"Money updated: {amount}", "WalletController");
        }

        private void HandleInventoryItemChanged(string itemId, int amount)
        {
            if (itemId.ToLower() == "gem")
            {
                UpdateWalletView();
                _logger.LogInfo($"Gems updated: {amount}", "WalletController");
            }
        }

        private void UpdateWalletView()
        {
            var coins = _inventoryManager.GetItemAmount("coin");
            var gems = _inventoryManager.GetItemAmount("gem");
            _walletView.Init(coins, gems);
        }
    }
} 