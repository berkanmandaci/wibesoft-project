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
        private const int GRID_SIZE = 20;
        private const float CELL_SPACING = 1.1f;
        
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

        public async UniTask Initialize()
        {
            _logger = LogManager.Instance;
            _jsonDataService = JsonDataService.Instance;
            
            _logger.LogInfo("Initializing GridManager", "GridManager");
            
            ValidateReferences();
            await CreateGridContainer();
            await CreateGrid();
            await LoadGridData();
            
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
                _logger.LogError("Cell material is not assigned!", "GridManager");
                throw new System.Exception("Cell material is not assigned!");
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

        private async UniTask CreateGrid()
        {
            _logger.LogInfo("Creating grid...", "GridManager");
            
            _grid = new Cell[GRID_SIZE, GRID_SIZE];
            
            for (int x = 0; x < GRID_SIZE; x++)
            {
                for (int y = 0; y < GRID_SIZE; y++)
                {
                    Vector3 position = CalculateCellPosition(x, y);
                    _grid[x, y] = await CreateCell(position, x, y);
                }
            }
            
            _logger.LogInfo($"Grid created with size: {GRID_SIZE}x{GRID_SIZE}", "GridManager");
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

        private async UniTask LoadGridData()
        {
            _logger.LogInfo("Loading grid data...", "GridManager");
            
            var gridData = _jsonDataService.GetGridData();
            if (gridData != null && gridData.Cells != null)
            {
                foreach (var cellData in gridData.Cells)
                {
                    if (IsValidPosition(cellData.X, cellData.Y))
                    {
                        var cell = _grid[cellData.X, cellData.Y];
                        await cell.LoadFromData(cellData);
                    }
                }
                _logger.LogInfo("Grid data loaded successfully", "GridManager");
            }
            else
            {
                _logger.LogInfo("No saved grid data found, using default configuration", "GridManager");
            }
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
    }
} 