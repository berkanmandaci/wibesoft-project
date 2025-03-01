using UnityEngine;
using System.Collections.Generic;
using WibeSoft.Core.Managers;
using WibeSoft.Data.Models;
using WibeSoft.UI.Views.InventoryViews;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Features.Grid;

namespace WibeSoft.UI.Controllers.InventoryControllers
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private ItemSlotView _itemSlotPrefab;
        [SerializeField] private float _yOffset = 100f; // Envanter panelinin yukarı offset'i

        private Dictionary<string, ItemSlotView> _itemSlots = new Dictionary<string, ItemSlotView>();
        private InventoryManager _inventoryManager => InventoryManager.Instance;
        private LogManager _logger => LogManager.Instance;
        private Camera _mainCamera;
        private Vector2Int _currentCellPosition;

        private void Awake()
        {
            _mainCamera = Camera.main;
            HideInventory();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
            RefreshInventory();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            if (_inventoryManager != null)
            {
                _inventoryManager.OnItemUpdated += HandleItemUpdated;
                _inventoryManager.OnItemRemoved += HandleItemRemoved;
                _inventoryManager.OnInventoryCleared += HandleInventoryCleared;
            }
            GameEvents.OnPlantingRequested += ShowInventoryAtCell;
            GameEvents.OnPopupClosed += HideInventory;
        }

        private void UnsubscribeFromEvents()
        {
            if (_inventoryManager != null)
            {
                _inventoryManager.OnItemUpdated -= HandleItemUpdated;
                _inventoryManager.OnItemRemoved -= HandleItemRemoved;
                _inventoryManager.OnInventoryCleared -= HandleInventoryCleared;
            }
            GameEvents.OnPlantingRequested -= ShowInventoryAtCell;
            GameEvents.OnPopupClosed -= HideInventory;
        }

        private void ShowInventoryAtCell(Cell cell)
        {
           
       
            // Envanter panelini konumlandır
            _itemContainer.position =  cell.GetScreenPosition();
            
            _itemContainer.gameObject.SetActive(true);
            GameEvents.TriggerOpenPopup();
            _logger.LogInfo($"Showing inventory at cell: {cell.Position}", "InventoryController");
            RefreshInventory();
        }

        private void HideInventory()
        {
            _itemContainer.gameObject.SetActive(false);
            _logger.LogInfo("Hiding inventory", "InventoryController");
        }

        private void RefreshInventory()
        {
            ClearInventoryUI();

            var items = _inventoryManager.GetAllItems();
            foreach (var item in items)
            {
                CreateOrUpdateItemSlot(item.Key, item.Value);
            }

            _logger.LogInfo("Inventory UI refreshed", "InventoryController");
        }

        private void HandleItemUpdated(string itemId, InventoryItem item)
        {
            CreateOrUpdateItemSlot(itemId, item);
        }

        private void HandleItemRemoved(string itemId)
        {
            if (_itemSlots.TryGetValue(itemId, out ItemSlotView slot))
            {
                Destroy(slot.gameObject);
                _itemSlots.Remove(itemId);
                _logger.LogInfo($"Removed item slot: {itemId}", "InventoryController");
            }
        }

        private void HandleInventoryCleared()
        {
            ClearInventoryUI();
        }

        private void CreateOrUpdateItemSlot(string itemId, InventoryItem item)
        {
            if (_itemSlots.TryGetValue(itemId, out ItemSlotView slot))
            {
                // Update existing slot
                slot.UpdateAmount(item.Amount);
            }
            else
            {
                // Create new slot
                var newSlot = Instantiate(_itemSlotPrefab, _itemContainer);
                newSlot.Init(itemId, item.Amount);
                _itemSlots[itemId] = newSlot;
            }
        }

        private void ClearInventoryUI()
        {
            foreach (var slot in _itemSlots.Values)
            {
                if (slot != null)
                {
                    Destroy(slot.gameObject);
                }
            }
            _itemSlots.Clear();
            _logger.LogInfo("Inventory UI cleared", "InventoryController");
        }
    }
}
