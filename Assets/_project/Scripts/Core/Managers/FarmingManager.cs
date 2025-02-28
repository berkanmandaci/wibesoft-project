using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WibeSoft.Data.Models;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class FarmingManager : SingletonBehaviour<FarmingManager>
    {
        private LogManager _logger;
        private Dictionary<Cell, Crop> _crops;
        private GridManager _gridManager;
        private InventoryManager _inventoryManager;
        
        public async UniTask Initialize()
        {
            _logger = LogManager.Instance;
            _logger.LogInfo("Initializing FarmingManager", "FarmingManager");
        
            _crops = new Dictionary<Cell, Crop>();
            _gridManager = GridManager.Instance;
            _inventoryManager = InventoryManager.Instance;
        
            _logger.LogInfo("FarmingManager initialized", "FarmingManager");
        }
        
        public async UniTask LoadCrop(Cell cell, string type, float plantTime, string state)
        {
            _logger.LogInfo($"Loading crop: {type} at ({cell.X}, {cell.Y})", "FarmingManager");
        
            var crop = new Crop
            {
                Type = type,
                PlantTime = plantTime,
                State = (CropState)Enum.Parse(typeof(CropState), state)
            };
        
            _crops[cell] = crop;
            _logger.LogInfo($"Crop loaded: {type} at ({cell.X}, {cell.Y})", "FarmingManager");
        }
        
        public bool HasCrop(Cell cell)
        {
            return _crops.ContainsKey(cell);
        }
        
        public Crop GetCrop(Cell cell)
        {
            return _crops[cell];
        }
        
        public void PlantCrop(Cell cell, string cropType)
        {
            if (HasCrop(cell))
            {
                _logger.LogWarning($"Cell already has a crop: ({cell.X}, {cell.Y})", "FarmingManager");
                return;
            }
        
            var crop = new Crop
            {
                Type = cropType,
                PlantTime = Time.time,
                State = CropState.Growing
            };
        
            _crops[cell] = crop;
            _logger.LogInfo($"Planted {cropType} at ({cell.X}, {cell.Y})", "FarmingManager");
        }
        
        public void HarvestCrop(Cell cell)
        {
            if (!HasCrop(cell))
            {
                _logger.LogWarning($"No crop found at: ({cell.X}, {cell.Y})", "FarmingManager");
                return;
            }
        
            var crop = GetCrop(cell);
            if (crop.State != CropState.Ready)
            {
                _logger.LogWarning($"Crop not ready for harvest at: ({cell.X}, {cell.Y})", "FarmingManager");
                return;
            }
        
            _inventoryManager.AddItem(crop.Type, 1, 100);
            _crops.Remove(cell);
            _logger.LogInfo($"Harvested {crop.Type} at ({cell.X}, {cell.Y})", "FarmingManager");
        }
        
        public void UpdateCrops()
        {
            foreach (var kvp in _crops)
            {
                var cell = kvp.Key;
                var crop = kvp.Value;
        
                if (crop.State == CropState.Growing && Time.time - crop.PlantTime >= 60f)
                {
                    crop.State = CropState.Ready;
                    _logger.LogInfo($"Crop ready for harvest: {crop.Type} at ({cell.X}, {cell.Y})", "FarmingManager");
                }
            }
        }
    }

    public class Crop
    {
        public string Type { get; set; }
        public float PlantTime { get; set; }
        public CropState State { get; set; }
    }

    public enum CropState
    {
        Growing,
        Ready
    }
} 