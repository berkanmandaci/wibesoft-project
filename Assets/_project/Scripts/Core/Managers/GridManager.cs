using UnityEngine;
using System.Collections.Generic;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class GridManager : SingletonBehaviour<GridManager>
    {
        private Dictionary<Vector2Int, GameObject> _grid = new Dictionary<Vector2Int, GameObject>();
        private GameObject _cellPrefab;
        private LogManager _logger=> LogManager.Instance;


        public void InitializeGrid(int width, int height, GameObject cellPrefab)
        {
            _cellPrefab = cellPrefab;

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
            var cell = Instantiate(_cellPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
            cell.transform.parent = transform;
            _grid[position] = cell;
            _logger.LogInfo($"Cell created at position: {position}", "GridManager");
        }

        public GameObject GetCell(Vector2Int position)
        {
            if (_grid.TryGetValue(position, out GameObject cell))
            {
                return cell;
            }
            _logger.LogWarning($"No cell found at position: {position}", "GridManager");
            return null;
        }

        public bool IsCellOccupied(Vector2Int position)
        {
            return _grid.ContainsKey(position);
        }
    }
} 