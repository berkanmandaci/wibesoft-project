using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WibeSoft.Core.Managers;

namespace WibeSoft.UI.Views.InventoryViews
{
    public class ItemSlotView : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Button _button;

        private string _itemId;
        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;

        private void Awake()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnSlotClicked);
            }
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnSlotClicked);
            }
        }

        public void Init(string itemId, int amount)
        {
            _itemId = itemId;
            
            if (_itemIcon != null)
            {
                var icon = _cropService.GetCropIcon(itemId);
                _itemIcon.sprite = icon;
                _itemIcon.enabled = icon != null;
            }

            if (_amountText != null)
            {
                _amountText.text = amount.ToString();
                _amountText.enabled = amount > 0;
            }

            _logger.LogInfo($"Item slot initialized: {itemId} x{amount}", "ItemSlotView");
        }

        public void UpdateAmount(int amount)
        {
            if (_amountText != null)
            {
                _amountText.text = amount.ToString();
                _amountText.enabled = amount > 0;
            }
        }

        private void OnSlotClicked()
        {
            _logger.LogInfo($"Item slot clicked: {_itemId}", "ItemSlotView");
            // TODO: Implement click handling (show item details, use item, etc.)
        }
    }
}
