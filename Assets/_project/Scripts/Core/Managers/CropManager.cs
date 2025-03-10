using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using WibeSoft.Features.Grid;
using WibeSoft.Core.Bootstrap;
using System;
using System.Collections.Generic;

namespace WibeSoft.Core.Managers
{
    public class CropManager : Singleton<CropManager>
    {
        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;
        private PlayerManager _playerManager => PlayerManager.Instance;
        private Dictionary<Vector2Int, UniTask> _growthTasks = new Dictionary<Vector2Int, UniTask>();

        public async UniTask Initialize()
        {
            _logger.LogInfo("Initializing CropManager", "CropManager");
            await UniTask.CompletedTask;
        }

        public async UniTask PlantCrop(Cell cell, string cropId)
        {
            var cropData = new CropData(cropId);
            cell.SetCrop(cropData);

            StartGrowthTimer(cell);

            _logger.LogInfo($"Crop planted: {cropId} at position {cell.Position}", "CropManager");
            GameEvents.TriggerCropPlanted(cell.Position, cropId);
        }

        public void StartGrowthTimer(Cell cell)
        {
            if (_growthTasks.ContainsKey(cell.Position))
            {
                return;
            }

            _growthTasks[cell.Position] = MonitorGrowth(cell);
        }


        private async UniTask MonitorGrowth(Cell cell)
        {
            try
            {
                var cropConfig = _cropService.GetCropConfig(cell.CurrentCrop.CropId);
                if (cropConfig == null) return;

                int lastStage = -1;
                while (cell.HasCrop && (cell.CurrentCrop.GetCurrentState() == CropState.Growing || cell.CurrentCrop.GetCurrentState() == CropState.ReadyToHarvest ))
                {

                    var stage = cell.CurrentCrop.Step;
                    if (stage != lastStage)
                    {
                        var cropObject = cell.transform.Find("CropMesh");
                        if (cropObject != null)
                        {
                            var meshFilter = cropObject.GetComponent<MeshFilter>();
                            if (meshFilter != null)
                            {
                                meshFilter.mesh = _cropService.GetGrowthStageMesh(cell.CurrentCrop.CropId, stage);
                                _logger.LogInfo($"Updated crop stage to {stage} at position {cell.Position}", "CropManager");
                            }
                        }
                        lastStage = stage;
                    }

                    if (cell.CurrentCrop.GetCurrentState() == CropState.ReadyToHarvest)
                    {
                        GameEvents.TriggerCropStateChanged(cell.Position, "ReadyToHarvest");
                        break;
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error monitoring crop growth: {ex.Message}", "CropManager");
            }
            finally
            {
                _growthTasks.Remove(cell.Position);
            }
        }


        public async UniTask HarvestCrop(Cell cell)
        {
            if (!cell.HasCrop || cell.CurrentCrop.GetCurrentState() != CropState.ReadyToHarvest)
            {
                _logger.LogWarning($"Cannot harvest: No ready crop at position {cell.Position}", "CropManager");
                return;
            }

            var cropId = cell.CurrentCrop.CropId;
            var cropConfig = _cropService.GetCropConfig(cropId);
            
            // Para ve exp ver
            _playerManager.AddGold(cropConfig.SellPrice);
            _playerManager.AddExperience(10);
            
            // Ekini temizle
            cell.ClearCrop();

            GameEvents.TriggerCropHarvested(cell.Position);
            _logger.LogInfo($"Crop harvested: {cropId} at position {cell.Position}", "CropManager");
        }
    }
}
