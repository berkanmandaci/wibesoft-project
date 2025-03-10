using System.Collections.Generic;
using WibeSoft.Core.Bootstrap;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using UnityEngine;

namespace WibeSoft.Core.Managers
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        private Dictionary<string, InventoryItem> _inventory = new Dictionary<string, InventoryItem>();
        private LogManager _logger => LogManager.Instance;
        private PlayerPrefsDataService _playerPrefsDataService => PlayerPrefsDataService.Instance;

        public event System.Action<string, InventoryItem> OnItemUpdated;
        public event System.Action<string> OnItemRemoved;
        public event System.Action OnInventoryCleared;

        public async UniTask Initialize()
        {
            _inventory.Clear();
            _logger.LogInfo("Initializing InventoryManager", "InventoryManager");
            
            // Load inventory data from JSON
            var saveData = _playerPrefsDataService.GetInventoryData();
            if (saveData != null && saveData.Items != null)
            {
                await LoadData(saveData.Items);
            }
            
            _logger.LogInfo("InventoryManager initialized", "InventoryManager");
        }

        public async UniTask LoadData(Dictionary<string, InventoryItemSaveData> items)
        {
            _logger.LogInfo("Loading inventory data", "InventoryManager");

            _inventory.Clear();
            OnInventoryCleared?.Invoke();

            foreach (var item in items)
            {
                _inventory[item.Key] = new InventoryItem
                {
                    ItemId = item.Key,
                    Amount = item.Value.Amount,
                    Value = item.Value.Value,
                };
                OnItemUpdated?.Invoke(item.Key, _inventory[item.Key]);
            }

            _logger.LogInfo("Inventory data loaded", "InventoryManager");
        }
        

        public void AddItem(string itemId, int amount, int value)
        {
            if (amount <= 0)
            {
                _logger.LogWarning($"Cannot add negative or zero amount: {amount}", "InventoryManager");
                return;
            }

            if (_inventory.ContainsKey(itemId))
            {
                _inventory[itemId].Amount += amount;
            }
            else
            {
                _inventory[itemId] = new InventoryItem
                {
                    ItemId = itemId,
                    Amount = amount,
                    Value = value,
                };
            }

            OnItemUpdated?.Invoke(itemId, _inventory[itemId]);
            _logger.LogInfo($"Added {amount} {itemId} to inventory", "InventoryManager");

            // Save to JSON
            SaveInventoryData().Forget();
        }

        public bool RemoveItem(string itemId, int amount)
        {
            if (!_inventory.ContainsKey(itemId))
            {
                _logger.LogWarning($"Item not found in inventory: {itemId}", "InventoryManager");
                return false;
            }

            if (_inventory[itemId].Amount < amount)
            {
                _logger.LogWarning($"Insufficient {itemId} amount", "InventoryManager");
                return false;
            }

            _inventory[itemId].Amount -= amount;
            
            if (_inventory[itemId].Amount <= 0)
            {
                _inventory.Remove(itemId);
                OnItemRemoved?.Invoke(itemId);
            }
            else
            {
                OnItemUpdated?.Invoke(itemId, _inventory[itemId]);
            }

            _logger.LogInfo($"Removed {amount} {itemId} from inventory", "InventoryManager");
            
            // Save to JSON
            SaveInventoryData().Forget();
            return true;
        }

        public async UniTask SaveInventoryData()
        {
            var saveData = new InventorySaveData
            {
                Items = new Dictionary<string, InventoryItemSaveData>()
            };

            foreach (var item in _inventory)
            {
                saveData.Items[item.Key] = new InventoryItemSaveData
                {
                    Amount = item.Value.Amount,
                    Value = item.Value.Value
                };
            }

            await _playerPrefsDataService.SaveInventoryData(saveData);
            _logger.LogInfo("Inventory data saved", "InventoryManager");
        }

        public Dictionary<string, InventoryItem> GetAllItems()
        {
            return new Dictionary<string, InventoryItem>(_inventory);
        }

        public InventoryItem GetItem(string itemId)
        {
            return _inventory.TryGetValue(itemId, out InventoryItem item) ? item : null;
        }

        public int GetItemAmount(string itemId)
        {
            return _inventory.TryGetValue(itemId, out InventoryItem item) ? item.Amount : 0;
        }

        public int GetItemValue(string itemId)
        {
            return _inventory.TryGetValue(itemId, out InventoryItem item) ? item.Value : 0;
        }
    }
} 