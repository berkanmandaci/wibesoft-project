using System;
using UnityEngine;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Core.Managers;
using WibeSoft.Features.Grid;
using WibeSoft.UI.Views;

namespace WibeSoft.UI.Controllers
{
    public class CropInfoController : MonoBehaviour
    {
        [SerializeField] private CropInfoContainerView _cropInfoView;
        
        private LogManager _logger => LogManager.Instance;
        private GridManager _gridManager => GridManager.Instance;

        private void Awake()
        {
            HideInventory();
        }
        private void OnEnable()
        {
            GameEvents.OnCropInfoPopup += HandleCropInfoRequest;
            GameEvents.OnPopupClosed += HandlePopupClosed;
        }

        private void OnDisable()
        {
            GameEvents.OnCropInfoPopup -= HandleCropInfoRequest;
            GameEvents.OnPopupClosed -= HandlePopupClosed;
        }

        private void HideInventory()
        {
            _cropInfoView.gameObject.SetActive(false);
        }
        private void HandleCropInfoRequest(Cell cell)
        {
            if (cell == null || !cell.HasCrop)
            {
                _logger.LogWarning("Attempted to show crop info for invalid cell", "CropInfoController");
                return;
            }

            var crop = cell.CurrentCrop;
            _cropInfoView.Init(
                cropId: crop.CropId,
                plantedTime: crop.PlantedTime
            );


            _cropInfoView.transform.position = cell.GetScreenPosition();
            _logger.LogInfo($"Showing crop info for: {crop.CropId}", "CropInfoController");
            _cropInfoView.gameObject.SetActive(true);
            GameEvents.TriggerOpenPopup();
        }

        private void HandlePopupClosed()
        {
            _cropInfoView.gameObject.SetActive(false);
            _logger.LogInfo("Crop info popup closed", "CropInfoController");
        }
    }
} 