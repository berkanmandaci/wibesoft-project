using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using WibeSoft.Core.Bootstrap;

namespace WibeSoft.Core.Managers
{
    public class JsonDataService : SingletonBehaviour<JsonDataService> 
    {
        private SaveData _cachedData;
        private LogManager _logger => LogManager.Instance;
        private const string SAVE_FILE_NAME = "save.json";
        private const string SAVE_DIRECTORY = "SaveData";
        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_DIRECTORY, SAVE_FILE_NAME);

        public async UniTask Initialize()
        {
            _logger.LogInfo("Initializing JsonDataService", "JsonDataService");
            await CreateSaveDirectoryIfNotExists();
            await LoadFromJson();
        }

        private async UniTask CreateSaveDirectoryIfNotExists()
        {
            var directory = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInfo("Save directory created", "JsonDataService");
            }
        }

        // GET methods
        public SaveData GetAllData() 
        {
            _logger.LogInfo("Fetching all data", "JsonDataService");
            return _cachedData;
        }

        public PlayerSaveData GetPlayerData() 
        {
            _logger.LogInfo("Fetching player data", "JsonDataService");
            return _cachedData.PlayerData;
        }

        public GridSaveData GetGridData() 
        {
            _logger.LogInfo("Fetching grid data", "JsonDataService");
            return _cachedData.GridData;
        }

        public InventorySaveData GetInventoryData() 
        {
            _logger.LogInfo("Fetching inventory data", "JsonDataService");
            return _cachedData.InventoryData;
        }

        // SAVE methods
        public async UniTask<bool> SaveAllData(SaveData data)
        {
            _logger.LogInfo("Saving all data", "JsonDataService");
            _cachedData = data;
            var result = await SaveToJson();
            if (result) GameEvents.TriggerGameSaved();
            return result;
        }

        public async UniTask<bool> SavePlayerData(PlayerSaveData data) 
        {
            _logger.LogInfo("Saving player data", "JsonDataService");
            _cachedData.PlayerData = data;
            return await SaveToJson();
        }

        public async UniTask<bool> SaveGridData(GridSaveData data) 
        {
            _logger.LogInfo("Saving grid data", "JsonDataService");
            _cachedData.GridData = data;
            return await SaveToJson();
        }

        public async UniTask<bool> SaveInventoryData(InventorySaveData data) 
        {
            _logger.LogInfo("Saving inventory data", "JsonDataService");
            _cachedData.InventoryData = data;
            return await SaveToJson();
        }

        private async UniTask<bool> SaveToJson()
        {
            try 
            {
                _cachedData.LastSaveTime = DateTime.Now;
                string json = JsonConvert.SerializeObject(_cachedData, Formatting.Indented);
                await File.WriteAllTextAsync(SavePath, json);
                _logger.LogInfo("Data saved to JSON", "JsonDataService");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"JSON save error: {ex.Message}", "JsonDataService");
                return false;
            }
        }

        private async UniTask LoadFromJson()
        {
            try 
            {
                if (File.Exists(SavePath))
                {
                    string json = await File.ReadAllTextAsync(SavePath);
                    _cachedData = JsonConvert.DeserializeObject<SaveData>(json);
                    _logger.LogInfo("Data loaded from JSON", "JsonDataService");
                    GameEvents.TriggerGameLoaded();
                }
                else 
                {
                    _cachedData = CreateNewSaveData();
                    await SaveToJson();
                    _logger.LogInfo("New save data created", "JsonDataService");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"JSON load error: {ex.Message}", "JsonDataService");
                _cachedData = CreateNewSaveData();
                await SaveToJson();
            }
        }

        private SaveData CreateNewSaveData()
        {
            var cells = new List<CellSaveData>();
            
            // Calculate center points for 10x10 grid
            int centerX = 10 / 2;
            int centerY = 10 / 2;
            
            // Create grid with 10x10 size
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
                    
                    // Set 6 cells in the center as farm area (2x3)
                    if (x >= centerX - 1 && x <= centerX && 
                        y >= centerY - 1 && y <= centerY + 1)
                    {
                        cell.Type = "Farm";
                    }
                    
                    cells.Add(cell);
                }
            }

            // Create initial inventory items
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
    }
} 