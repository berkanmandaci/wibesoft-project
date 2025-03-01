using System;
using UnityEngine;
using WibeSoft.Features.Grid;

namespace WibeSoft.Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Crop Config", menuName = "WibeSoft/Configs/Crop Config")]
    public class CropConfig : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] private string _cropId;
        [SerializeField] private string _cropName;
        [SerializeField] private Sprite _cropIcon;

        [Header("Economy")]
        [SerializeField] private int _purchasePrice; // Price to buy from shop
        [SerializeField] private int _sellPrice;     // Price when harvested

        [Header("Growth Settings")]
        [SerializeField] private float _growthTime; // Growth time in seconds
        [SerializeField] private int _growthStages; // Number of growth stages

        [Header("Visual Settings")]
        [SerializeField] private Mesh[] _growthStageMeshes; // Mesh for each stage
        [SerializeField] private Material _cropMaterial;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _plantEffect;
        [SerializeField] private ParticleSystem _growthEffect;
        [SerializeField] private ParticleSystem _harvestEffect;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip _plantSound;
        [SerializeField] private AudioClip _growthSound;
        [SerializeField] private AudioClip _harvestSound;

        // Public Properties
        public string CropId => _cropId;
        public string CropName => _cropName;
        public Sprite CropIcon => _cropIcon;
        public int PurchasePrice => _purchasePrice;
        public int SellPrice => _sellPrice;
        public float GrowthTime => _growthTime;
        public int GrowthStages => _growthStages;
        public Mesh[] GrowthStageMeshes => _growthStageMeshes;
        public Material CropMaterial => _cropMaterial;
        public ParticleSystem PlantEffect => _plantEffect;
        public ParticleSystem GrowthEffect => _growthEffect;
        public ParticleSystem HarvestEffect => _harvestEffect;
        public AudioClip PlantSound => _plantSound;
        public AudioClip GrowthSound => _growthSound;
        public AudioClip HarvestSound => _harvestSound;
        

        private void OnValidate()
        {
            // Validation checks
            if (_purchasePrice < 0)
                _purchasePrice = 0;

            if (_sellPrice < 0)
                _sellPrice = 0;

            if (_growthTime < 1)
                _growthTime = 1;

            if (_growthStages < 1)
                _growthStages = 1;

            if (_growthStageMeshes != null && _growthStageMeshes.Length != _growthStages)
                Debug.LogWarning($"Number of growth stage meshes ({_growthStageMeshes.Length}) does not match number of growth stages ({_growthStages})!", this);
        }
    }
} 