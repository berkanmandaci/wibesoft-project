using UnityEngine;
using System.Collections.Generic;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using System.Linq;

namespace WibeSoft.Core.Managers
{
    public class GridManager : SingletonBehaviour<GridManager>
    {
        private Dictionary<Vector2Int, GameObject> _grid = new Dictionary<Vector2Int, GameObject>();
        private Dictionary<Vector2Int, Cell> _cells = new Dictionary<Vector2Int, Cell>();
        private GameObject _cellPrefab;
        private LogManager _logger => LogManager.Instance;

        public void InitializeGrid(int width, int height, GameObject cellPrefab)
        {
            _cellPrefab = cellPrefab;
            _grid.Clear();
            _cells.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateCell(new Vector2Int(x, y));
                }
            }

            _logger.LogInfo($"Grid initialized with dimensions: {width}x{height}", "GridManager");
        }

        private void CreateCell(Vector2Int position)
        {
            var cell = new Cell(position.x, position.y);
            var cellObject = Instantiate(_cellPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
            cellObject.transform.parent = transform;
            
            _grid[position] = cellObject;
            _cells[position] = cell;
            
            _logger.LogInfo($"Cell created at position: {position}", "GridManager");
        }

        public Cell GetCell(int x, int y)
        {
            var position = new Vector2Int(x, y);
            return _cells.TryGetValue(position, out Cell cell) ? cell : null;
        }

        public GameObject GetCellObject(Vector2Int position)
        {
            return _grid.TryGetValue(position, out GameObject cellObject) ? cellObject : null;
        }

        public bool IsCellOccupied(Vector2Int position)
        {
            return _cells.ContainsKey(position);
        }

        public IEnumerable<Cell> GetAllCells()
        {
            return _cells.Values;
        }

        public void ClearGrid()
        {
            foreach (var cellObject in _grid.Values)
            {
                if (cellObject != null)
                {
                    Destroy(cellObject);
                }
            }

            _grid.Clear();
            _cells.Clear();
            _logger.LogInfo("Grid cleared", "GridManager");
        }
    }
} 