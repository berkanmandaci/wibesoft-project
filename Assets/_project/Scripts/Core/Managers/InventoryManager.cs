using System.Collections.Generic;
using WibeSoft.Core.Bootstrap;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;

namespace WibeSoft.Core.Managers
{
    public class InventoryManager : SingletonBehaviour<InventoryManager>
    {
        private Dictionary<string, InventoryItem> _inventory = new Dictionary<string, InventoryItem>();
        private LogManager _logger => LogManager.Instance;

        public async UniTask Initialize()
        {
            _inventory.Clear();
            _logger.LogInfo("Initializing InventoryManager", "InventoryManager");
            await UniTask.CompletedTask;
            _logger.LogInfo("InventoryManager initialized", "InventoryManager");
        }

        public async UniTask LoadData(Dictionary<string, InventoryItemSaveData> items)
        {
            _logger.LogInfo("Loading inventory data", "InventoryManager");

            _inventory.Clear();
            foreach (var item in items)
            {
                _inventory[item.Key] = new InventoryItem
                {
                    Amount = item.Value.Amount,
                    Value = item.Value.Value
                };
            }

            _logger.LogInfo("Inventory data loaded", "InventoryManager");
        }

        public void AddItem(string itemId, int amount, int value)
        {
            if (_inventory.ContainsKey(itemId))
            {
                _inventory[itemId].Amount += amount;
            }
            else
            {
                _inventory[itemId] = new InventoryItem
                {
                    Amount = amount,
                    Value = value
                };
            }

            _logger.LogInfo($"Added {amount} {itemId} to inventory", "InventoryManager");
        }

        public bool RemoveItem(string itemId, int amount)
        {
            if (!_inventory.ContainsKey(itemId) || _inventory[itemId].Amount < amount)
            {
                _logger.LogWarning($"Insufficient {itemId} amount", "InventoryManager");
                return false;
            }

            _inventory[itemId].Amount -= amount;
            if (_inventory[itemId].Amount <= 0)
            {
                _inventory.Remove(itemId);
            }

            _logger.LogInfo($"Removed {amount} {itemId} from inventory", "InventoryManager");
            return true;
        }

        public Dictionary<string, InventoryItem> GetAllItems()
        {
            return new Dictionary<string, InventoryItem>(_inventory);
        }

        public int GetItemAmount(string itemType)
        {
            return _inventory.TryGetValue(itemType, out InventoryItem item) ? item.Amount : 0;
        }

        public int GetItemValue(string itemType)
        {
            return _inventory.TryGetValue(itemType, out InventoryItem item) ? item.Value : 0;
        }
    }

    public class InventoryItem
    {
        public int Amount { get; set; }
        public int Value { get; set; }
    }
} 