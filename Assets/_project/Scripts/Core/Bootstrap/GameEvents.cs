using System;
using UnityEngine;
using WibeSoft.Features.Grid;

namespace WibeSoft.Core.Bootstrap
{
    /// <summary>
    /// Static class that manages game events
    /// </summary>
    public static class GameEvents
    {
        // Bootstrap Events
        public static event Action OnGameReady;
        public static event Action<Exception> OnBootstrapError;

        // Game State Events
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameQuit;

        // Player Events
        public static event Action<int> OnMoneyChanged;
        public static event Action<string, int> OnInventoryItemChanged;
        public static event Action<int, int, int> OnPlayerLevelChanged;
        public static event Action<int, int> OnCurrencyChanged;

        // Grid Events
        public static event Action<Cell> OnCellSelected;
        public static event Action<Vector2Int, string> OnCropPlanted;
        public static event Action<Vector2Int> OnCropHarvested;
        public static event Action<Vector2Int, string> OnCropStateChanged;
        public static event Action<Cell> OnPlantingRequested;
        public static event Action<Cell> OnCropInfoPopup;
        public static event Action<Cell> OnHarvestPopup;

        // Planting Events
        public static event Action<string> OnInventoryItemSelected;
        
        public static event Action<string> OnPlanting;
        public static event Action OnPlantingModeStarted;
        public static event Action OnPlantingModeEnded;
        public static event Action<Vector2Int> OnValidCellHovered;
        public static event Action<Vector2Int> OnInvalidCellHovered;
        public static event Action<Vector2Int, string> OnPlantingStarted;
        public static event Action<string> OnPlantingFailed;

        // UI Events
        public static event Action OnInventoryOpened;
        public static event Action OnInventoryClosed;
        public static event Action OnPopupClosed;
        public static event Action OnPopupOpened;

        // Save Events
        public static event Action OnGameSaved;
        public static event Action OnGameLoaded;

        #region Event Triggers

        public static void TriggerGameReady()
        {
            OnGameReady?.Invoke();
            Debug.Log("Game Ready event triggered");
        }

        public static void TriggerBootstrapError(Exception ex)
        {
            OnBootstrapError?.Invoke(ex);
            Debug.LogError($"Bootstrap Error event triggered: {ex.Message}");
        }

        public static void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
            Debug.Log("Game Paused event triggered");
        }

        public static void TriggerGameResumed()
        {
            OnGameResumed?.Invoke();
            Debug.Log("Game Resumed event triggered");
        }

        public static void TriggerGameQuit()
        {
            OnGameQuit?.Invoke();
            Debug.Log("Game Quit event triggered");
        }

        public static void TriggerMoneyChanged(int amount)
        {
            OnMoneyChanged?.Invoke(amount);
            Debug.Log($"Money Changed event triggered: {amount}");
        }

        public static void TriggerInventoryItemChanged(string itemId, int amount)
        {
            OnInventoryItemChanged?.Invoke(itemId, amount);
            Debug.Log($"Inventory Item Changed event triggered: {itemId} = {amount}");
        }

        public static void TriggerInventoryItemSelected(string itemId)
        {
            OnInventoryItemSelected?.Invoke(itemId);
            Debug.Log($"Inventory Item Selected event triggered: {itemId}");
        }

        public static void TriggerCellSelected(Cell position)
        {
            OnCellSelected?.Invoke(position);
            Debug.Log($"Cell Selected event triggered: {position}");
        }

        public static void TriggerCropPlanted(Vector2Int position, string cropType)
        {
            OnCropPlanted?.Invoke(position, cropType);
            Debug.Log($"Crop Planted event triggered: {cropType} at {position}");
        }

        public static void TriggerCropHarvested(Vector2Int position)
        {
            OnCropHarvested?.Invoke(position);
            Debug.Log($"Crop Harvested event triggered at {position}");
        }

        public static void TriggerCropStateChanged(Vector2Int position, string state)
        {
            OnCropStateChanged?.Invoke(position, state);
            Debug.Log($"Crop State Changed event triggered: {state} at {position}");
        }

        public static void TriggerPlantingRequested(Cell cell)
        {
            OnPlantingRequested?.Invoke(cell);
            Debug.Log($"Planting requested at position: {cell}");
        }
        public static void TriggerCropInfoPopup(Cell cell)
        {
            OnCropInfoPopup?.Invoke(cell);
        }
        
        public static void TriggerHarvestPopup(Cell cell)
        {
            OnHarvestPopup?.Invoke(cell);
        }

        public static void TriggerInventoryOpened()
        {
            OnInventoryOpened?.Invoke();
            Debug.Log("Inventory Opened event triggered");
        }

        public static void TriggerInventoryClosed()
        {
            OnInventoryClosed?.Invoke();
            Debug.Log("Inventory Closed event triggered");
        }
        public static void TriggerClosePopup()
        {
            OnPopupClosed?.Invoke();
            Debug.Log("Popup Closed event triggered");
        }
        public static void TriggerOpenPopup()
        {
            OnPopupOpened?.Invoke();
            Debug.Log("Popup Closed event triggered");
        }
        

        public static void TriggerGameSaved()
        {
            OnGameSaved?.Invoke();
            Debug.Log("Game Saved event triggered");
        }

        public static void TriggerGameLoaded()
        {
            OnGameLoaded?.Invoke();
            Debug.Log("Game Loaded event triggered");
        }

        public static void TriggerPlayerLevelChanged(int level, int currentExp, int maxExp)
        {
            OnPlayerLevelChanged?.Invoke(level, currentExp, maxExp);
            Debug.Log($"Player Level Changed event triggered: Level {level}, Exp: {currentExp}/{maxExp}");
        }

        public static void TriggerCurrencyChanged(int gold, int gem)
        {
            OnCurrencyChanged?.Invoke(gold, gem);
            Debug.Log($"Currency Changed event triggered: Gold {gold}, Gem {gem}");
        }
        

        public static void TriggerPlanting(string cropId)
        {
            OnPlanting?.Invoke(cropId);
            Debug.Log("Planting");
        }
        public static void TriggerPlantingModeStarted()
        {
            OnPlantingModeStarted?.Invoke();
            Debug.Log("Planting mode started");
        }

        public static void TriggerPlantingModeEnded()
        {
            OnPlantingModeEnded?.Invoke();
            Debug.Log("Planting mode ended");
        }

        public static void TriggerValidCellHovered(Vector2Int position)
        {
            OnValidCellHovered?.Invoke(position);
        }

        public static void TriggerInvalidCellHovered(Vector2Int position)
        {
            OnInvalidCellHovered?.Invoke(position);
        }

    

        public static void TriggerPlantingFailed(string reason)
        {
            OnPlantingFailed?.Invoke(reason);
            Debug.Log($"Planting failed: {reason}");
        }

        #endregion


       
    }
} 