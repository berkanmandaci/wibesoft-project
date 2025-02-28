using TMPro;
using UnityEngine;
namespace WibeSoft.UI.Views.HudViews
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _currencyValueText;

        public void Init(long amount)
        {
            _currencyValueText.text = amount.ToString();
        }
    }
}
