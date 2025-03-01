using System;
using UnityEngine;
using WibeSoft.Core.Managers;
using WibeSoft.Data.ScriptableObjects;

namespace WibeSoft.Data.Models
{
    [Serializable]
    public class CropData
    {
        public string CropId { get; private set; }
        public DateTime PlantedTime { get; private set; }
        
        public int Step =>  GrowthProgress < 0.5f ? 0 : GrowthProgress < 0.95f ? 1 : 2;

        public CropConfig GetCropConfig => _cropService.GetCropConfig(CropId);

        public float ElapsedTime => (float)(DateTime.Now - PlantedTime).TotalSeconds;
        public float GrowthProgress => ElapsedTime / GrowthTime;
        
        public float GrowthTime => GetCropConfig.GrowthTime;
        
        public CropState GetCurrentState()
        {
            return ElapsedTime < GrowthTime ? CropState.Growing : CropState.ReadyToHarvest;
        }


        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;

        public CropData(string cropId)
        {
            CropId = cropId;
            PlantedTime = DateTime.Now;
            _logger.LogInfo($"New crop created: {cropId}", "CropData");
        }

      

        public void UpdateFromSaveData(DateTime plantedTime)
        {
            PlantedTime = plantedTime;
        }
    }

    public enum CropState
    {
        Growing,
        ReadyToHarvest,
    }
}
