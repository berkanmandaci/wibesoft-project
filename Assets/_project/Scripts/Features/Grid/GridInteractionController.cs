using UnityEngine;
using WibeSoft.Core.Managers;
using WibeSoft.Core.Bootstrap;
using UnityEngine.UI;

namespace WibeSoft.Features.Grid
{
    public class GridInteractionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LayerMask _gridLayer;
        [SerializeField] private Image _dragImage;

        private Camera _mainCamera;
        private GridManager _gridManager => GridManager.Instance;
        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;
        private InventoryManager _inventory => InventoryManager.Instance;

        // Drag & Drop state
        private bool _isPlantingMode;
        private string _selectedCropId;
        private GameObject _currentDragObject;

        private Cell _hoveredCell;
        private Cell _lastSelectedCell;

        private void Awake()
        {
            _mainCamera = Camera.main;

            _logger.LogInfo("GridInteractionController initialized", "GridInteractionController");
        }



        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnPlanting += PlantCrop;
            GameEvents.OnInventoryItemSelected += StartPlantingMode;
            GameEvents.OnPopupClosed += EndPlantingMode;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPlanting -= PlantCrop;
            GameEvents.OnInventoryItemSelected -= StartPlantingMode;
            GameEvents.OnPopupClosed -= EndPlantingMode;
        }

        private async void PlantCrop(string cropId)
        {
            await CropManager.Instance.PlantCrop(_gridManager.SelectedCell, cropId);
            _inventory.RemoveItem(cropId, 1);
        }

        private void Update()
        {
            if (_isPlantingMode)
            {
                HandlePlantingMode();
            }
        }

        private void StartPlantingMode(string cropId)
        {
            _selectedCropId = cropId;
            _isPlantingMode = true;
            _gridManager.DisableSelection();

            // CreateDragObject();

            GameEvents.TriggerPlantingModeStarted();
            _logger.LogInfo($"Planting mode started with crop: {cropId}", "GridInteractionController");
        }

        private void EndPlantingMode()
        {
            if (!_isPlantingMode) return;

            _isPlantingMode = false;
            _selectedCropId = null;
            _gridManager.EnableSelection();

            if (_currentDragObject != null)
            {
                Destroy(_currentDragObject);
                _currentDragObject = null;
                _dragImage = null;
            }

            ClearHoveredCell();

            GameEvents.TriggerPlantingModeEnded();
            _logger.LogInfo("Planting mode ended", "GridInteractionController");
        }

        private void HandlePlantingMode()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _gridLayer))
            {
                UpdateDragObjectPosition(hit.point);

                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null)
                {
                    HandleCellHover(cell);

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (IsValidPlantingCell(cell))
                        {
                            TryPlantCrop(cell);
                        }
                    }
                }
            }
            else
            {
                ClearHoveredCell();
            }

            // Sağ tık ile iptal
            if (Input.GetMouseButtonDown(1))
            {
                EndPlantingMode();
            }
        }

        // private void CreateDragObject()
        // {
        //     if (_dragItemPrefab != null)
        //     {
        //         _currentDragObject = Instantiate(_dragItemPrefab, _worldSpaceCanvas.transform);
        //         _dragImage = _currentDragObject.GetComponent<Image>();
        //         
        //         if (_dragImage != null)
        //         {
        //             _dragImage.sprite = _cropService.GetCropIcon(_selectedCropId);
        //             _dragImage.SetNativeSize();
        //             
        //             // Boyutu ayarla
        //             var rectTransform = _dragImage.rectTransform;
        //             rectTransform.sizeDelta = rectTransform.sizeDelta * 0.5f; // Boyutu yarıya indir
        //         }
        //     }
        // }

        private void UpdateDragObjectPosition(Vector3 hitPoint)
        {
            if (_currentDragObject != null)
            {
                _currentDragObject.transform.position = hitPoint;
                // Billboard effect - her zaman kameraya bak
                // _currentDragObject.transform.rotation = Quaternion.LookRotation(_mainCamera.transform.forward);
            }
        }

        private void HandleCellHover(Cell cell)
        {
            if (_hoveredCell == cell) return;

            ClearHoveredCell();
            _hoveredCell = cell;

            bool isValid = IsValidPlantingCell(cell);
            if (isValid)
            {
                cell.ShowPlantingPreview(true);
                GameEvents.TriggerValidCellHovered(cell.Position);
            }
            else
            {
                cell.ShowInvalidPlanting(true);
                GameEvents.TriggerInvalidCellHovered(cell.Position);
            }
        }

        private void ClearHoveredCell()
        {
            if (_hoveredCell != null)
            {
                _hoveredCell.ShowPlantingPreview(false);
                _hoveredCell.ShowInvalidPlanting(false);
                _hoveredCell = null;
            }
        }

        private bool IsValidPlantingCell(Cell cell)
        {
            return cell.Type == CellType.Farm && cell.State == CellState.Empty;
        }

        private async void TryPlantCrop(Cell cell)
        {
            if (!IsValidPlantingCell(cell))
            {
                GameEvents.TriggerPlantingFailed("Geçersiz ekim hücresi");
                return;
            }

            if (_inventory.GetItemAmount(_selectedCropId) <= 0)
            {
                GameEvents.TriggerPlantingFailed("Yetersiz tohum");
                EndPlantingMode();
                return;
            }

            await CropManager.Instance.PlantCrop(cell, _selectedCropId);
            _inventory.RemoveItem(_selectedCropId, 1);
        }
    }
}
