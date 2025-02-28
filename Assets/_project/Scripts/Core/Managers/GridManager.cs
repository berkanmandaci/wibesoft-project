using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Data.Models;
using WibeSoft.Features.Grid;

namespace WibeSoft.Core.Managers
{
    /// <summary>
    /// Manages the grid system with a 20x20 layout
    /// Handles cell creation, positioning, and state management
    /// </summary>
    public class GridManager : SingletonBehaviour<GridManager>
    {
        private const int GRID_SIZE = 10;
        private const float CELL_SPACING = 2f;

        [Header("Prefab References")]
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private GameObject _gridContainer;

        [Header("Cell Meshes")]
        [SerializeField] private Mesh _waterMesh;
        [SerializeField] private Mesh _groundMesh;
        [SerializeField] private Mesh _farmMesh;

        [Header("Cell Material")]
        [SerializeField] private Material _cellMaterial;

        private LogManager _logger;
        private JsonDataService _jsonDataService;
        private Cell[,] _grid;
        private Cell _selectedCell;
        private Camera _mainCamera;
        private bool _isSelectionEnabled = true;

        private void Update()
        {
            if (_isSelectionEnabled && Input.GetMouseButtonDown(0))
            {
                HandleCellSelection();
            }
        }

        public async UniTask Initialize()
        {
            _logger = LogManager.Instance;
            _jsonDataService = JsonDataService.Instance;
            _mainCamera = Camera.main;

            _logger.LogInfo("Initializing GridManager", "GridManager");

            ValidateReferences();
            await CreateGridContainer();
            
            // Önce JSON verilerini yükle
            var gridData = _jsonDataService.GetGridData();
            await CreateGrid(gridData);

            _logger.LogInfo("GridManager initialized successfully", "GridManager");
        }

        private void ValidateReferences()
        {
            if (_cellPrefab == null)
            {
                _logger.LogError("Cell prefab is not assigned!", "GridManager");
                throw new System.Exception("Cell prefab is not assigned!");
            }

            if (_cellMaterial == null)
            {
                _logger.LogError("One or more materials are not assigned!", "GridManager");
                throw new System.Exception("Materials are not assigned!");
            }

            if (_waterMesh == null || _groundMesh == null || _farmMesh == null)
            {
                _logger.LogError("One or more cell meshes are not assigned!", "GridManager");
                throw new System.Exception("Cell meshes are not assigned!");
            }
        }

        private async UniTask CreateGridContainer()
        {
            if (_gridContainer == null)
            {
                _gridContainer = new GameObject("GridContainer");
                _gridContainer.transform.SetParent(transform);
                _gridContainer.transform.localPosition = Vector3.zero;
            }

            await UniTask.CompletedTask;
        }

        private async UniTask CreateGrid(GridSaveData gridData)
        {
            _logger.LogInfo("Creating grid...", "GridManager");

            _grid = new Cell[GRID_SIZE, GRID_SIZE];

            // Grid'i oluştur ve JSON verilerine göre ayarla
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    Vector3 position = CalculateCellPosition(x, y);
                    _grid[x, y] = await CreateCell(position, x, y);

                    // JSON'dan veri varsa uygula
                    if (gridData?.Cells != null)
                    {
                        var cellData = gridData.Cells.Find(c => c.X == x && c.Y == y);
                        if (cellData != null)
                        {
                            await _grid[x, y].LoadFromData(cellData);
                            _logger.LogInfo($"Cell ({x}, {y}) loaded from save data: Type={cellData.Type}", "GridManager");
                        }
                    }
                    // Eğer JSON verisi yoksa ve ilk kez oluşturuluyorsa, varsayılan 6 farm field'ı oluştur
                    else if (IsDefaultFarmLocation(x, y))
                    {
                        await _grid[x, y].SwitchToFarm();
                        _logger.LogInfo($"Default farm field created at ({x}, {y})", "GridManager");
                    }
                }
            }

            _logger.LogInfo($"Grid created with size: {GRID_SIZE}x{GRID_SIZE}", "GridManager");
        }

        private bool IsDefaultFarmLocation(int x, int y)
        {
            // Merkez koordinatları
            int centerX = GRID_SIZE / 2;
            int centerY = GRID_SIZE / 2;

            // Merkez etrafında 2x3'lük alanı kontrol et
            return (x == centerX - 1 || x == centerX) && 
                   (y >= centerY - 1 && y <= centerY + 1);
        }

        private Vector3 CalculateCellPosition(int x, int y)
        {
            float xPos = (x - (GRID_SIZE - 1) / 2f) * CELL_SPACING;
            float zPos = (y - (GRID_SIZE - 1) / 2f) * CELL_SPACING;
            return new Vector3(xPos, 0f, zPos);
        }

        private async UniTask<Cell> CreateCell(Vector3 position, int x, int y)
        {
            GameObject cellObject = Instantiate(_cellPrefab, position, Quaternion.identity, _gridContainer.transform);
            cellObject.name = $"Cell_{x}_{y}";

            var cell = cellObject.GetComponent<Cell>();
            if (cell == null)
            {
                _logger.LogError($"Cell component not found on prefab for position ({x}, {y})", "GridManager");
                throw new System.Exception("Cell component not found on prefab!");
            }

            await cell.Initialize(x, y, _cellMaterial, _waterMesh, _groundMesh, _farmMesh);

            return cell;
        }

        public Cell GetCell(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _grid[x, y];
            }

            _logger.LogWarning($"Attempted to access invalid cell position: ({x}, {y})", "GridManager");
            return null;
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < GRID_SIZE && y >= 0 && y < GRID_SIZE;
        }

        public async UniTask SaveGridData()
        {
            var gridData = new GridSaveData { Cells = new System.Collections.Generic.List<CellSaveData>() };

            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    var cell = _grid[x, y];
                    gridData.Cells.Add(new CellSaveData
                    {
                        X = x,
                        Y = y,
                        Type = cell.Type.ToString(),
                        State = cell.State.ToString()
                    });
                }
            }

            await _jsonDataService.SaveGridData(gridData);
            _logger.LogInfo("Grid data saved successfully", "GridManager");
        }

        private void HandleCellSelection()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var cell = hit.collider.GetComponent<Cell>();
                if (cell != null)
                {
                    SelectCell(cell);
                }
            }
            else
            {
                ClearSelection();
            }
        }

        public void SelectCell(Cell cell)
        {
            if (_selectedCell == cell) return;

            // Clear previous selection
            ClearSelection();

            // Set new selection
            _selectedCell = cell;
            _selectedCell.SetSelected(true);

            GameEvents.TriggerCellSelected(new Vector2Int(cell.X, cell.Y));
            _logger.LogInfo($"Cell selected at ({cell.X}, {cell.Y})", "GridManager");
        }

        public void ClearSelection()
        {
            if (_selectedCell != null)
            {
                _selectedCell.SetSelected(false);
                _selectedCell = null;
                _logger.LogInfo("Cell selection cleared", "GridManager");
            }
        }

        public Cell GetSelectedCell()
        {
            return _selectedCell;
        }

        public void EnableSelection()
        {
            _isSelectionEnabled = true;
            _logger.LogInfo("Cell selection enabled", "GridManager");
        }

        public void DisableSelection()
        {
            _isSelectionEnabled = false;
            ClearSelection();
            _logger.LogInfo("Cell selection disabled", "GridManager");
        }
    }
}

