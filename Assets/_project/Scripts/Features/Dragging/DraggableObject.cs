using UnityEngine;
using WibeSoft.Core.Managers;
using WibeSoft.Features.Grid;
using System.Collections.Generic;

namespace WibeSoft.Features.Dragging
{
    public class DraggableObject : MonoBehaviour
    {
        private bool _isDragging;
        private Camera _mainCamera;
        private float _yPosition;
        private Vector3 _initialPosition;
        private GridManager _gridManager => GridManager.Instance;
        private LogManager _logger => LogManager.Instance;

        private List<Cell> _lastHighlightedCells = new List<Cell>();
        private Vector2Int _size = new Vector2Int(2, 2); // 2x2 grid boyutu
        private const float CELL_SIZE = 2f; // Grid hücre boyutu

        private void Start()
        {
            _mainCamera = Camera.main;
            _yPosition = transform.position.y;
            _initialPosition = transform.position;
        }

        private void OnMouseDown()
        {
            _isDragging = true;
            _gridManager.DisableSelection();
        }

        private void OnMouseUp()
        {
            _isDragging = false;
            ClearHighlights();

            var centerCell = GetCenterCell();
            if (centerCell != null && CanPlaceAt(centerCell.X, centerCell.Y))
            {
                // Tam grid pozisyonuna yerleştir
                var snapPos = _gridManager.GetCell(centerCell.X, centerCell.Y).transform.position;
                transform.position = new Vector3(snapPos.x - 1, _yPosition, snapPos.z - 1);
                _initialPosition = transform.position;
                _logger.LogInfo($"Bina {centerCell.X}, {centerCell.Y} koordinatlarına yerleştirildi", "DraggableObject");
            }
            else
            {
                transform.position = _initialPosition;
                _logger.LogInfo("Yerleştirme başarısız, ilk pozisyona geri dönüldü", "DraggableObject");
            }

            _gridManager.EnableSelection();
        }

        private void Update()
        {
            if (!_isDragging) return;

            // Objeyi mouse ile hareket ettir
            var mouseRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, _yPosition, 0));

            if (groundPlane.Raycast(mouseRay, out float distance))
            {
                Vector3 hitPoint = mouseRay.GetPoint(distance);
                transform.position = new Vector3(hitPoint.x, _yPosition, hitPoint.z);
                UpdateHighlight();
            }
        }

        private Cell GetCenterCell()
        {
            // Objenin dünya pozisyonunu grid koordinatlarına çevir
            Vector3 pos = transform.position;
            int x = Mathf.RoundToInt(pos.x / CELL_SIZE);
            int z = Mathf.RoundToInt(pos.z / CELL_SIZE);

            // // Grid merkezine göre offset ekle
            x += 5; // 10x10 grid için merkez offseti
            z += 5;

            return _gridManager.GetCell(x, z);
        }

        private bool CanPlaceAt(int centerX, int centerY)
        {
            int startX = centerX - (_size.x / 2);
            int startY = centerY - (_size.y / 2);

            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    var cell = _gridManager.GetCell(startX + x, startY + y);
                    if (cell == null || cell.Type != CellType.Ground)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private Cell _lastCenterCell;
        private void UpdateHighlight()
        {
          

            var centerCell = GetCenterCell();
            if (centerCell == null|| centerCell== _lastCenterCell) return;
         
            ClearHighlights();
            int startX = centerCell.X - (_size.x / 2);
            int startY = centerCell.Y - (_size.y / 2);

            bool canPlace = true;
            List<Cell> cellsToHighlight = new List<Cell>();

            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    var cell = _gridManager.GetCell(startX + x, startY + y);
                    if (cell == null || cell.Type != CellType.Ground)
                    {
                        canPlace = false;
                    }
                    if (cell != null)
                    {
                        cellsToHighlight.Add(cell);
                    }
                }
            }

            foreach (var cell in cellsToHighlight)
            {
                if (canPlace)
                {
                    cell.ShowPlantingPreview(true);
                }
                else
                {
                    cell.ShowInvalidPlanting(true);
                }
                _lastHighlightedCells.Add(cell);
            }
            _lastCenterCell = centerCell;
        }

        private void ClearHighlights()
        {
            foreach (var cell in _lastHighlightedCells)
            {
                if (cell != null)
                {
                    cell.ShowPlantingPreview(false);
                    cell.ShowInvalidPlanting(false);
                }
            }
            _lastHighlightedCells.Clear();
        }
    }
}
