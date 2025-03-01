using System;
using UnityEngine;
using WibeSoft.Core.Managers;

namespace WibeSoft.Data.Models
{
    [Serializable]
    public class CropData
    {
        public string CropId { get; private set; }
        public DateTime PlantedTime { get; private set; }
        public float GrowthProgress { get; private set; }
        public CropState State { get; private set; }

        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;

        public CropData(string cropId)
        {
            CropId = cropId;
            PlantedTime = DateTime.Now;
            GrowthProgress = 0f;
            State = CropState.Growing;
            
            _logger.LogInfo($"New crop created: {cropId}", "CropData");
        }

        public void UpdateGrowth()
        {
            var cropConfig = _cropService.GetCropConfig(CropId);
            if (cropConfig == null) return;

            var elapsedTime = (float)(DateTime.Now - PlantedTime).TotalSeconds;
            GrowthProgress = Mathf.Clamp01(elapsedTime / cropConfig.GrowthTime);

            if (GrowthProgress >= 1f && State == CropState.Growing)
            {
                State = CropState.ReadyToHarvest;
                _logger.LogInfo($"Crop ready to harvest: {CropId}", "CropData");
            }
        }

        public void UpdateFromSaveData(DateTime plantedTime, float progress, CropState state)
        {
            PlantedTime = plantedTime;
            GrowthProgress = progress;
            State = state;
        }
    }

    public enum CropState
    {
        Growing,
        ReadyToHarvest,
        Dead
    }
} 