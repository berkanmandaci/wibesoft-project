using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WibeSoft.Core.Managers;
using WibeSoft.Core.Bootstrap;
using System;

namespace WibeSoft.UI.Views
{
    /// <summary>
    /// Ekin bilgilerini gösteren UI bileşeni
    /// </summary>
    public class CropInfoContainerView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text _cropNameText;
        [SerializeField] private TMP_Text _cropHarvestTimeText;
        [SerializeField] private Slider _cropGrowthSlider;

        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;

        private string _currentCropId;
        private float _currentGrowthProgress;
        private DateTime _plantedTime;
        private float _growthTime;
        private bool _isGrowing;

        private void OnEnable()
        {
            GameEvents.OnCropStateChanged += HandleCropStateChanged;
            _isGrowing = true;
        }

        private void OnDisable()
        {
            GameEvents.OnCropStateChanged -= HandleCropStateChanged;
            _isGrowing = false;
        }

        private void Update()
        {
            if (_isGrowing && _currentCropId != null)
            {
                UpdateRemainingTime();
            }
        }

        /// <summary>
        /// Ekin bilgilerini güncelleyen ana metod
        /// </summary>
        public void Init(string cropId, DateTime plantedTime)
        {
            _currentCropId = cropId;
            _plantedTime = plantedTime;



            var cropConfig = _cropService.GetCropConfig(cropId);
            if (cropConfig == null)
            {
                _logger.LogError($"Crop config not found for ID: {cropId}", "CropInfoContainerView");
                return;
            }

            var elapsedTime = (float)(DateTime.Now - _plantedTime).TotalSeconds;
            

            _growthTime = cropConfig.GrowthTime;

            UpdateCropName(cropConfig.CropName);
            UpdateGrowthProgress(elapsedTime, _growthTime);
            UpdateRemainingTime();

        }

        private void HandleCropStateChanged(Vector2Int position, string state)
        {
            if (_currentCropId != null)
            {
                _logger.LogInfo($"Crop state changed: {state}", "CropInfoContainerView");
                if (state == "ReadyToHarvest")
                {
                    _isGrowing = false;
                    _cropHarvestTimeText.text = "Hasada Hazır!";
                }
            }
        }

        private void UpdateCropName(string cropName)
        {
            if (_cropNameText != null)
            {
                _cropNameText.text = cropName;
            }
        }

        private void UpdateRemainingTime()
        {
            if (_cropHarvestTimeText != null)
            {
                var elapsedTime = (float)(DateTime.Now - _plantedTime).TotalSeconds;
                var remainingTime = Mathf.Max(0, _growthTime - elapsedTime);

                if (remainingTime <= 0)
                {
                    _cropHarvestTimeText.text = "Hasada Hazır!";
                    _isGrowing = false;
                }
                else
                {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
                    _cropHarvestTimeText.text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
            }
        }

        private void UpdateGrowthProgress(float progress, float maxGrowth)
        {
            if (_cropGrowthSlider != null)
            {
                _cropGrowthSlider.maxValue = maxGrowth;
                _cropGrowthSlider.value = progress;
            }
        }
    }
}
