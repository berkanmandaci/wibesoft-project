using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WibeSoft.Core.Managers
{
    public class PlayerPrefsDataService : SingletonBehaviour<PlayerPrefsDataService>
    {
        private const string PLAYER_DATA_KEY = "player_data";
        private const string GRID_DATA_KEY = "grid_data";
        private const string INVENTORY_DATA_KEY = "inventory_data";
        private const string LAST_SAVE_TIME_KEY = "last_save_time";

        private SaveData _cachedData;
        private LogManager _logger => LogManager.Instance;

        public async UniTask Initialize()
        {
            _logger.LogInfo("PlayerPrefsDataService başlatılıyor", "PlayerPrefsDataService");
            await LoadData();
        }

        public SaveData GetAllData()
        {
            return _cachedData ?? CreateNewSaveData();
        }

        public PlayerSaveData GetPlayerData()
        {
            return _cachedData?.PlayerData ?? CreateNewSaveData().PlayerData;
        }

        public GridSaveData GetGridData()
        {
            return _cachedData?.GridData ?? CreateNewSaveData().GridData;
        }

        public InventorySaveData GetInventoryData()
        {
            return _cachedData?.InventoryData ?? CreateNewSaveData().InventoryData;
        }

        public async UniTask<bool> SaveAllData(SaveData data)
        {
            try
            {
                _cachedData = data;
                _cachedData.LastSaveTime = DateTime.Now;

                SaveToPlayerPrefs();
                _logger.LogInfo("Tüm veriler başarıyla kaydedildi", "PlayerPrefsDataService");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Veri kaydedilirken hata oluştu: {ex.Message}", "PlayerPrefsDataService");
                return false;
            }
        }
        public async UniTask SaveGridData(GridSaveData gridData)
        {
            _cachedData.GridData = gridData;
            if (_cachedData.GridData != null)
                PlayerPrefs.SetString(GRID_DATA_KEY, JsonConvert.SerializeObject(_cachedData.GridData));
        }
        public async UniTask SaveInventoryData(InventorySaveData saveData)
        {
            _cachedData.InventoryData = saveData;
            if (_cachedData.InventoryData != null)
                PlayerPrefs.SetString(INVENTORY_DATA_KEY, JsonConvert.SerializeObject(_cachedData.InventoryData));
        }
        public async  UniTask SavePlayerData(PlayerSaveData saveData)
        {
            _cachedData.PlayerData = saveData;
            if (_cachedData.PlayerData != null)
                PlayerPrefs.SetString(PLAYER_DATA_KEY, JsonConvert.SerializeObject(_cachedData.PlayerData));
        }
        private void SaveToPlayerPrefs()
        {
            if (_cachedData.PlayerData != null)
                PlayerPrefs.SetString(PLAYER_DATA_KEY, JsonConvert.SerializeObject(_cachedData.PlayerData));

            if (_cachedData.GridData != null)
                PlayerPrefs.SetString(GRID_DATA_KEY, JsonConvert.SerializeObject(_cachedData.GridData));

            if (_cachedData.InventoryData != null)
                PlayerPrefs.SetString(INVENTORY_DATA_KEY, JsonConvert.SerializeObject(_cachedData.InventoryData));

            PlayerPrefs.SetString(LAST_SAVE_TIME_KEY, JsonConvert.SerializeObject(_cachedData.LastSaveTime));
            PlayerPrefs.Save();
        }

        private async UniTask LoadData()
        {
            try
            {
                _cachedData = new SaveData();

                // Player Data
                string playerJson = PlayerPrefs.GetString(PLAYER_DATA_KEY, "");
                _cachedData.PlayerData = !string.IsNullOrEmpty(playerJson) 
                    ? JsonConvert.DeserializeObject<PlayerSaveData>(playerJson) 
                    : CreateNewSaveData().PlayerData;

                // Grid Data
                string gridJson = PlayerPrefs.GetString(GRID_DATA_KEY, "");
                _cachedData.GridData = !string.IsNullOrEmpty(gridJson) 
                    ? JsonConvert.DeserializeObject<GridSaveData>(gridJson) 
                    : CreateNewSaveData().GridData;

                // Inventory Data
                string inventoryJson = PlayerPrefs.GetString(INVENTORY_DATA_KEY, "");
                _cachedData.InventoryData = !string.IsNullOrEmpty(inventoryJson) 
                    ? JsonConvert.DeserializeObject<InventorySaveData>(inventoryJson) 
                    : CreateNewSaveData().InventoryData;

                // Last Save Time
                string timeJson = PlayerPrefs.GetString(LAST_SAVE_TIME_KEY, "");
                _cachedData.LastSaveTime = !string.IsNullOrEmpty(timeJson) 
                    ? JsonConvert.DeserializeObject<DateTime>(timeJson) 
                    : DateTime.Now;

                _logger.LogInfo("Veriler başarıyla yüklendi", "PlayerPrefsDataService");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Veri yüklenirken hata oluştu: {ex.Message}", "PlayerPrefsDataService");
                _cachedData = CreateNewSaveData();
            }
        }

        private SaveData CreateNewSaveData()
        {
            var cells = new List<CellSaveData>();
            
            // 10x10 grid için merkez noktaları hesapla
            int centerX = 10 / 2;
            int centerY = 10 / 2;
            
            // 10x10 boyutunda grid oluştur
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var cell = new CellSaveData
                    {
                        X = x,
                        Y = y,
                        Type = "Ground",
                        State = "Empty"
                    };
                    
                    // Merkezdeki 6 hücreyi (2x3) çiftlik alanı olarak ayarla
                    if (x >= centerX - 1 && x <= centerX && 
                        y >= centerY - 1 && y <= centerY + 1)
                    {
                        cell.Type = "Farm";
                    }
                    
                    cells.Add(cell);
                }
            }

            // Başlangıç envanter öğelerini oluştur
            var inventoryItems = new Dictionary<string, InventoryItemSaveData>
            {
                ["Carrot"] = new InventoryItemSaveData { Amount = 10, Value = 100 },
                ["Corn"] = new InventoryItemSaveData { Amount = 10, Value = 150 }
            };

            return new SaveData
            {
                PlayerData = new PlayerSaveData
                {
                    MaxExp = 1000,
                    CurrentExp = 0,
                    Level = 1,
                    Username = "Player",
                    Gold = 10000,
                    Gem = 100
                },
                GridData = new GridSaveData
                {
                    Cells = cells
                },
                InventoryData = new InventorySaveData
                {
                    Items = inventoryItems
                },
                LastSaveTime = DateTime.Now
            };
        }

        public void ClearAllData()
        {
            PlayerPrefs.DeleteKey(PLAYER_DATA_KEY);
            PlayerPrefs.DeleteKey(GRID_DATA_KEY);
            PlayerPrefs.DeleteKey(INVENTORY_DATA_KEY);
            PlayerPrefs.DeleteKey(LAST_SAVE_TIME_KEY);
            PlayerPrefs.Save();
            
            _cachedData = CreateNewSaveData();
            _logger.LogInfo("Tüm veriler silindi ve varsayılan değerlere sıfırlandı", "PlayerPrefsDataService");
        }

       
    }
} 