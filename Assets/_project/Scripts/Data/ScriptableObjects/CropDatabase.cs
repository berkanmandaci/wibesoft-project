using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace WibeSoft.Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CropDatabase", menuName = "WibeSoft/Databases/Crop Database")]
    public class CropDatabase : ScriptableObject
    {
        [SerializeField] private List<CropConfig> _crops;
        private Dictionary<string, CropConfig> _cropLookup;

        private void OnEnable()
        {
            InitializeLookup();
        }

        private void OnValidate()
        {
            InitializeLookup();
            ValidateCropIds();
        }

        private void InitializeLookup()
        {
            _cropLookup = _crops?.ToDictionary(crop => crop.CropId) ?? new Dictionary<string, CropConfig>();
        }

        private void ValidateCropIds()
        {
            if (_crops == null) return;

            var duplicates = _crops
                .GroupBy(crop => crop.CropId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            foreach (var duplicate in duplicates)
            {
                Debug.LogError($"Duplicate CropId found: {duplicate}", this);
            }
        }

        public CropConfig GetCropById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("Crop ID cannot be null or empty", this);
                return null;
            }

            if (_cropLookup == null)
            {
                InitializeLookup();
            }

            if (!_cropLookup.TryGetValue(id, out var crop))
            {
                Debug.LogError($"Crop not found with ID: {id}", this);
                return null;
            }

            return crop;
        }

        public List<CropConfig> GetAllCrops()
        {
            return _crops?.ToList() ?? new List<CropConfig>();
        }

        public bool HasCrop(string id)
        {
            if (_cropLookup == null)
            {
                InitializeLookup();
            }

            return !string.IsNullOrEmpty(id) && _cropLookup.ContainsKey(id);
        }
    }
} 