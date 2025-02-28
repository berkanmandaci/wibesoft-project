using UnityEngine;
using WibeSoft.Core.Managers;

namespace WibeSoft.Data.Models
{
    /// <summary>
    /// Represents an item in the inventory system
    /// </summary>
    public class InventoryItem
    {
        public string ItemId { get; set; }
        public int Amount { get; set; }
        public int Value { get; set; }

        // Runtime-only property, will not be used in serialization
        public Sprite Icon => CropService.Instance.GetCropIcon(ItemId);

        public InventoryItem()
        {
        }

        public InventoryItem(string itemId, int amount, int value)
        {
            ItemId = itemId;
            Amount = amount;
            Value = value;
        }

        public InventoryItem Clone()
        {
            return new InventoryItem
            {
                ItemId = this.ItemId,
                Amount = this.Amount,
                Value = this.Value
            };
        }

        public override string ToString()
        {
            return $"InventoryItem[{ItemId}] x{Amount} (Value: {Value})";
        }
    }
} 