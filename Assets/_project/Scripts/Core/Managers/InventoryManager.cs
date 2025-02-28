using System.Collections.Generic;
using WibeSoft.Core.Bootstrap;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class InventoryManager : SingletonBehaviour<InventoryManager>
    {
        private Dictionary<string, InventoryItem> _inventory = new Dictionary<string, InventoryItem>();
        private LogManager _logger => LogManager.Instance;

        public async UniTask Initialize()
        {
            _inventory.Clear();
            _logger.LogInfo("InventoryManager initialized", "InventoryManager");
            await UniTask.CompletedTask;
        }

        public void AddItem(string itemType, int amount)
        {
            if (!_inventory.ContainsKey(itemType))
            {
                _inventory[itemType] = new InventoryItem { Amount = 0, Value = GetItemBaseValue(itemType) };
            }

            _inventory[itemType].Amount += amount;
            _logger.LogInfo($"Added {amount} {itemType} to inventory. New total: {_inventory[itemType].Amount}", "InventoryManager");

            GameEvents.TriggerInventoryItemChanged(itemType, _inventory[itemType].Amount);
        }

        public bool RemoveItem(string itemType, int amount)
        {
            if (_inventory.TryGetValue(itemType, out InventoryItem item))
            {
                if (item.Amount >= amount)
                {
                    item.Amount -= amount;
                    _logger.LogInfo($"Removed {amount} {itemType} from inventory. Remaining: {item.Amount}", "InventoryManager");

                    GameEvents.TriggerInventoryItemChanged(itemType, item.Amount);
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Not enough {itemType} in inventory. Required: {amount}, Available: {item.Amount}", "InventoryManager");
                }
            }
            else
            {
                _logger.LogWarning($"Item {itemType} not found in inventory", "InventoryManager");
            }

            return false;
        }

        public int GetItemAmount(string itemType)
        {
            return _inventory.TryGetValue(itemType, out InventoryItem item) ? item.Amount : 0;
        }

        public int GetItemValue(string itemType)
        {
            return _inventory.TryGetValue(itemType, out InventoryItem item) ? item.Value : 0;
        }

        private int GetItemBaseValue(string itemType)
        {
            // This should be replaced with actual configuration values
            switch (itemType.ToLower())
            {
                case "wheat":
                    return 100;
                default:
                    return 50;
            }
        }
    }

    public class InventoryItem
    {
        public int Amount { get; set; }
        public int Value { get; set; }
    }
} 