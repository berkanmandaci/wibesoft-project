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
    public class CropManager : SingletonBehaviour<CropManager>
    {
        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;
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

        private void StartGrowthTimer(Cell cell)
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
                while (cell.HasCrop && cell.CurrentCrop.State == CropState.Growing)
                {
                    cell.CurrentCrop.UpdateGrowth();

                    var stage = Mathf.FloorToInt(cell.CurrentCrop.GrowthProgress * (cropConfig.GrowthStages - 1));
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

                    if (cell.CurrentCrop.State == CropState.ReadyToHarvest)
                    {
                        cell.UpdateState(CellState.ReadyToHarvest);
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

        public void ProcessOfflineGrowth(TimeSpan offlineTime)
        {
            var gridManager = GridManager.Instance;
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var cell = gridManager.GetCell(x, y);
                    if (cell != null && cell.HasCrop)
                    {
                        cell.CurrentCrop.UpdateGrowth();
                        if (cell.CurrentCrop.State == CropState.ReadyToHarvest)
                        {
                            cell.UpdateState(CellState.ReadyToHarvest);
                        }
                    }
                }
            }

            _logger.LogInfo($"Processed offline growth for duration: {offlineTime.TotalHours:F1} hours", "CropManager");
        }

        public async UniTask HarvestCrop(Cell cell)
        {
            if (!cell.HasCrop || cell.CurrentCrop.State != CropState.ReadyToHarvest)
            {
                _logger.LogWarning($"Cannot harvest: No ready crop at position {cell.Position}", "CropManager");
                return;
            }

            var cropId = cell.CurrentCrop.CropId;
            cell.ClearCrop();

            GameEvents.TriggerCropHarvested(cell.Position);
            _logger.LogInfo($"Crop harvested: {cropId} at position {cell.Position}", "CropManager");
        }
    }
}
