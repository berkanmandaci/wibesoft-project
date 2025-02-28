using System;
using UnityEngine;

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

        // Grid Events
        public static event Action<Vector2Int> OnCellSelected;
        public static event Action<Vector2Int, string> OnCropPlanted;
        public static event Action<Vector2Int> OnCropHarvested;
        public static event Action<Vector2Int, string> OnCropStateChanged;

        // UI Events
        public static event Action OnInventoryOpened;
        public static event Action OnInventoryClosed;

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

        public static void TriggerCellSelected(Vector2Int position)
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

        #endregion
    }
} 