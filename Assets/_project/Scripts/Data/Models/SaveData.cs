using System;
using System.Collections.Generic;

namespace WibeSoft.Data.Models
{
    [Serializable]
    public class SaveData
    {
        public PlayerSaveData PlayerData { get; set; }
        public GridSaveData GridData { get; set; }
        public InventorySaveData InventoryData { get; set; }
        public DateTime LastSaveTime { get; set; }
    }

    [Serializable]
    public class PlayerSaveData
    {
        public int MaxExp { get; set; }
        public int CurrentExp { get; set; }
        public int Level { get; set; }
        public string Username { get; set; }
        public int Gold { get; set; }
        public int Gem { get; set; }
    }

    [Serializable]
    public class GridSaveData
    {
        public List<CellSaveData> Cells { get; set; }
    }

    [Serializable]
    public class CellSaveData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
    }

    [Serializable]
    public class InventorySaveData
    {
        public Dictionary<string, InventoryItemSaveData> Items { get; set; }
    }

    [Serializable]
    public class InventoryItemSaveData
    {
        public int Amount { get; set; }
        public int Value { get; set; }
    }
} 