using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.ScriptableObjects;

namespace WibeSoft.Core.Managers
{
    public class CropService : SingletonBehaviour<CropService>
    {
        private CropDatabase _database;
        private LogManager _logger => LogManager.Instance;
        private InventoryManager _inventory => InventoryManager.Instance;

        public async UniTask Initialize()
        {
            _logger.LogInfo("Initializing CropService", "CropService");
            await LoadCropDatabase();
        }

        private async UniTask LoadCropDatabase()
        {
            _database = Resources.Load<CropDatabase>("Configs/Databases/CropDatabase");
            if (_database == null)
            {
                _logger.LogError("Failed to load CropDatabase!", "CropService");
                throw new System.Exception("CropDatabase not found in Resources folder!");
            }
            await UniTask.CompletedTask;
        }
        public CropConfig GetCropConfig(string cropId)
        {
            var crop = _database.GetCropById(cropId);
            return crop;
        }

        public Sprite GetCropIcon(string cropId)
        {
            var crop = _database.GetCropById(cropId);
            return crop?.CropIcon;
        }

        public int GetPurchasePrice(string cropId)
        {
            var crop = _database.GetCropById(cropId);
            return crop?.PurchasePrice ?? 0;
        }

        public int GetSellPrice(string cropId)
        {
            var crop = _database.GetCropById(cropId);
            return crop?.SellPrice ?? 0;
        }

        public float GetGrowthTime(string cropId)
        {
            var crop = _database.GetCropById(cropId);
            return crop?.GrowthTime ?? 0;
        }

        public Mesh GetGrowthStageMesh(string cropId, int stage)
        {
            var crop = _database.GetCropById(cropId);
            if (crop == null || stage < 0 || stage >= crop.GrowthStages)
            {
                return null;
            }
            return crop.GrowthStageMeshes[stage];
        }

        public Material GetCropMaterial(string cropId)
        {
            var crop = _database.GetCropById(cropId);
            return crop?.CropMaterial;
        }

        public bool CanPurchaseCrop(string cropId)
        {
            var price = GetPurchasePrice(cropId);
            var playerGold = PlayerManager.Instance.PlayerData.Currency.Gold;
            return playerGold >= price;
        }

        public async UniTask<bool> PurchaseCrop(string cropId, int amount = 1)
        {
            if (amount <= 0)
            {
                _logger.LogWarning($"Invalid purchase amount: {amount}", "CropService");
                return false;
            }

            var totalPrice = GetPurchasePrice(cropId) * amount;
            if (!CanPurchaseCrop(cropId))
            {
                _logger.LogWarning("Insufficient gold for purchase", "CropService");
                return false;
            }

            // Remove gold from player
            PlayerManager.Instance.RemoveGold(totalPrice);

            // Add item to inventory
            _inventory.AddItem(cropId, amount, GetSellPrice(cropId));
            
            _logger.LogInfo($"Purchased {amount} {cropId} for {totalPrice} gold", "CropService");
            return true;
        }

        public async UniTask<bool> SellCrop(string cropId, int amount = 1)
        {
            if (amount <= 0)
            {
                _logger.LogWarning($"Invalid sell amount: {amount}", "CropService");
                return false;
            }

            if (_inventory.GetItemAmount(cropId) < amount)
            {
                _logger.LogWarning("Insufficient crop amount for sale", "CropService");
                return false;
            }

            var totalPrice = GetSellPrice(cropId) * amount;

            // Remove from inventory
            if (_inventory.RemoveItem(cropId, amount))
            {
                // Add gold to player
                PlayerManager.Instance.AddGold(totalPrice);
                _logger.LogInfo($"Sold {amount} {cropId} for {totalPrice} gold", "CropService");
                return true;
            }

            return false;
        }
       
    }
} 