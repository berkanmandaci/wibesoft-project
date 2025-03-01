using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Managers;
using WibeSoft.Data.Models;
using WibeSoft.Core.Bootstrap;

namespace WibeSoft.Features.Grid
{
    /// <summary>
    /// Represents a single cell in the grid system
    /// Manages cell type, state, and visual representation
    /// </summary>
    public class Cell : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private Material _defaultMaterial;
        private Material _selectedMaterial;
        private Mesh _waterMesh;
        private Mesh _groundMesh;
        private Mesh _farmMesh;
        private bool _isSelected;
        private Outline _outline;
        private CropData _currentCrop;
        private GameObject _cropObject;
        
        private LogManager _logger => LogManager.Instance;

        public int X { get; private set; }
        public int Y { get; private set; }
        public CellType Type { get; private set; }
        public CellState State { get; private set; }
        public bool IsSelected => _isSelected;
        public Vector2Int Position { get; private set; }
        public CropData CurrentCrop => _currentCrop;
        public bool HasCrop => _currentCrop != null;

        private void OnValidate()
        {
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
            if (_outline == null) _outline = GetComponent<Outline>();
        }

        private void Awake()
        {
            OnValidate();

            if (_outline == null)
            {
                _outline = gameObject.AddComponent<Outline>();
                _outline.enabled = false;
            }
          
        }

        public void SetSelected(bool selected)
        {
            if (selected == _isSelected) return;
            
            _isSelected = selected;
            _outline.enabled = selected;
            
            if (selected)
            {
                if (Type == CellType.Farm&& State == CellState.Empty)
                {
                    GameEvents.TriggerPlantingRequested(this);
                }else if (Type == CellType.Farm && State == CellState.Growing)
                {
                    GameEvents.TriggerCropInfoPopup(this);
                }else if (Type == CellType.Farm && State == CellState.ReadyToHarvest)
                {
                    GameEvents.TriggerHarvestPopup(this);
                }
                GameEvents.TriggerCellSelected(this);
                _logger.LogInfo($"Cell selected at position: {Position}", "Cell");
            }
        }
        public void ShowPlantingPreview(bool show)
        {
            _outline.enabled = show;
            HighlightCell(show);
        }
        
        private void HighlightCell(bool show)
        {
            _outline.enabled = show;
            _outline.OutlineColor = Color.green;
        }
        
        public void ShowInvalidPlanting(bool show)
        {
            _outline.enabled = show;
            _outline.OutlineColor = Color.red;
        }

        public async UniTask Initialize(int x, int y, Material defaultMaterial, Mesh waterMesh, Mesh groundMesh, Mesh farmMesh, CellType type = CellType.Ground)
        {
            X = x;
            Y = y;
            Position = new Vector2Int(x, y);
            _defaultMaterial = defaultMaterial;
            _waterMesh = waterMesh;
            _groundMesh = groundMesh;
            _farmMesh = farmMesh;
            Type = type;
            State = CellState.Empty;

            await SetupComponents();
            _logger.LogInfo($"Cell initialized at position: ({X}, {Y})", "Cell");
        }

        private async UniTask SetupComponents()
        {
            if (_meshFilter == null || _meshRenderer == null)
            {
                _logger.LogError("Required components are missing!", "Cell");
                return;
            }

            // Set mesh based on type
            switch (Type)
            {
                case CellType.Water:
                    _meshFilter.mesh = _waterMesh;
                    break;
                case CellType.Ground:
                    _meshFilter.mesh = _groundMesh;
                    break;
                case CellType.Farm:
                    _meshFilter.mesh = _farmMesh;
                    break;
            }

            _meshRenderer.material = _defaultMaterial;
            await UniTask.CompletedTask;
        }

        public async UniTask LoadFromData(CellSaveData data)
        {
            try
            {
                // Parse and set type
                if (System.Enum.TryParse(data.Type, out CellType parsedType))
                {
                    Type = parsedType;
                }
                else
                {
                    _logger.LogError($"Invalid cell type in save data: {data.Type}", "Cell");
                    Type = CellType.Ground;
                }

                // Parse and set state
                if (System.Enum.TryParse(data.State, out CellState parsedState))
                {
                    State = parsedState;
                }
                else
                {
                    _logger.LogError($"Invalid cell state in save data: {data.State}", "Cell");
                    State = CellState.Empty;
                }

                // Load crop data if exists
                if (!string.IsNullOrEmpty(data.CropId))
                {
                    _currentCrop = new CropData(data.CropId);
                    if (System.Enum.TryParse(data.CropState, out CropState cropState))
                    {
                        _currentCrop.UpdateFromSaveData(data.PlantedTime, data.GrowthProgress, cropState);
                    }
                    SetCrop(_currentCrop);
                }

                await SetupComponents();
              
                _logger.LogInfo($"Cell loaded from data at position: ({X}, {Y})", "Cell");
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error loading cell data: {ex.Message}", "Cell");
                throw;
            }
        }

        public async UniTask SwitchToWater()
        {
            Type = CellType.Water;
            _meshFilter.mesh = _waterMesh;
            await UniTask.CompletedTask;
            _logger.LogInfo($"Cell switched to Water at position: ({X}, {Y})", "Cell");
        }

        public async UniTask SwitchToGround()
        {
            Type = CellType.Ground;
            _meshFilter.mesh = _groundMesh;
            await UniTask.CompletedTask;
            _logger.LogInfo($"Cell switched to Ground at position: ({X}, {Y})", "Cell");
        }

        public async UniTask SwitchToFarm()
        {
            Type = CellType.Farm;
            _meshFilter.mesh = _farmMesh;
            await UniTask.CompletedTask;
            _logger.LogInfo($"Cell switched to Farm at position: ({X}, {Y})", "Cell");
        }

        public void UpdateState(CellState newState)
        {
            State = newState;
            _logger.LogInfo($"Cell state updated to {newState} at position: ({X}, {Y})", "Cell");
            GameEvents.TriggerCropStateChanged(Position, newState.ToString());
        }

        public void SetCrop(CropData cropData)
        {
            _currentCrop = cropData;
            UpdateState(CellState.Growing);
            
            // Ekin mesh'ini oluştur
            if (_cropObject == null)
            {
                _cropObject = new GameObject("CropMesh");
                _cropObject.transform.SetParent(transform);
                _cropObject.transform.localPosition = Vector3.up * 2f; // Hücrenin üstüne yerleştir
                
                var meshFilter = _cropObject.AddComponent<MeshFilter>();
                var meshRenderer = _cropObject.AddComponent<MeshRenderer>();
                
                // CropService'den mesh ve materyal al
                meshFilter.mesh = CropService.Instance.GetGrowthStageMesh(cropData.CropId, 0);
                meshRenderer.material = CropService.Instance.GetCropMaterial(cropData.CropId);
            }
            
            _logger.LogInfo($"Crop set at position ({X}, {Y}): {cropData.CropId}", "Cell");
        }

        public void ClearCrop()
        {
            if (_cropObject != null)
            {
                Destroy(_cropObject);
                _cropObject = null;
            }
            _currentCrop = null;
            UpdateState(CellState.Empty);
            _logger.LogInfo($"Crop cleared at position ({X}, {Y})", "Cell");
        }
    }

    public enum CellType
    {
        Water,
        Ground,
        Farm
    }

    public enum CellState
    {
        Empty,
        Growing,
        ReadyToHarvest
    }
} 