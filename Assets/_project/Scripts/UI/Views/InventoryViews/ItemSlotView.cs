using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Core.Managers;

namespace WibeSoft.UI.Views.InventoryViews
{
    public class ItemSlotView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private Button _button;
        [SerializeField] private float _longPressThreshold = 0.5f; // Uzun basma için gereken süre

        private string _itemId;
        private LogManager _logger => LogManager.Instance;
        private CropService _cropService => CropService.Instance;
        
        private bool _isPointerDown;
        private float _pointerDownTimer;

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

        private void Update()
        {
            if (_isPointerDown)
            {
                _pointerDownTimer += Time.deltaTime;
                if (_pointerDownTimer >= _longPressThreshold)
                {
                    OnLongPress();
                    _isPointerDown = false;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
            _pointerDownTimer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isPointerDown && _pointerDownTimer < _longPressThreshold)
            {
                OnSlotClicked();
            }
            _isPointerDown = false;
            _pointerDownTimer = 0f;
        }

        private void OnLongPress()
        {
            _logger.LogInfo($"Item slot long pressed: {_itemId}", "ItemSlotView");
            GameEvents.TriggerInventoryItemSelected(_itemId);
            GameEvents.TriggerClosePopup();
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
            GameEvents.TriggerPlanting(_itemId);
            GameEvents.TriggerClosePopup();
        }
    }
}
