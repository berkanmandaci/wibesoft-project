using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class FarmingManager : SingletonBehaviour<FarmingManager>
    {
        private Dictionary<Vector2Int, CropData> _crops = new Dictionary<Vector2Int, CropData>();
        private GridManager _gridManager => GridManager.Instance;
        private InventoryManager _inventoryManager => InventoryManager.Instance;
        private LogManager _logger => LogManager.Instance;

        public async UniTask Initialize()
        {
            _crops.Clear();
            _logger.LogInfo("FarmingManager initialized", "FarmingManager");
            await UniTask.CompletedTask;
        }

        public void PlantCrop(Vector2Int position, string cropType)
        {
            if (_gridManager.IsCellOccupied(position))
            {
                if (!_crops.ContainsKey(position))
                {
                    var cropData = new CropData
                    {
                        Type = cropType,
                        PlantTime = Time.time,
                        State = CropState.Seed
                    };

                    _crops[position] = cropData;
                    _logger.LogInfo($"Planted {cropType} at position: {position}", "FarmingManager");
                }
                else
                {
                    _logger.LogWarning($"Cell at position {position} already has a crop", "FarmingManager");
                }
            }
            else
            {
                _logger.LogWarning($"Cannot plant at position {position} - cell is not available", "FarmingManager");
            }
        }

        public void HarvestCrop(Vector2Int position)
        {
            if (_crops.TryGetValue(position, out CropData crop))
            {
                if (crop.State == CropState.Ready)
                {
                    _inventoryManager.AddItem(crop.Type, 1);
                    _crops.Remove(position);
                    _logger.LogInfo($"Harvested {crop.Type} at position: {position}", "FarmingManager");
                }
                else
                {
                    _logger.LogWarning($"Crop at position {position} is not ready for harvest", "FarmingManager");
                }
            }
            else
            {
                _logger.LogWarning($"No crop found at position {position}", "FarmingManager");
            }
        }

        public void UpdateCrops()
        {
            foreach (var kvp in _crops)
            {
                var position = kvp.Key;
                var crop = kvp.Value;
                var growthTime = Time.time - crop.PlantTime;

                // Simple growth logic - can be expanded based on crop types
                if (growthTime > 60f && crop.State == CropState.Seed)
                {
                    crop.State = CropState.Ready;
                    _logger.LogInfo($"Crop at position {position} is ready for harvest", "FarmingManager");
                }
            }
        }
    }

    public class CropData
    {
        public string Type { get; set; }
        public float PlantTime { get; set; }
        public CropState State { get; set; }
    }

    public enum CropState
    {
        Seed,
        Ready
    }
} 