using UnityEngine;
namespace WibeSoft.UI.Views.HudViews
{
    public class WalletContainerView : MonoBehaviour
    {
        [SerializeField] private CurrencyView _coinView;
        [SerializeField] private CurrencyView _gemView;
        
        public void Init(long coinAmount, long gemAmount)
        {
            _coinView.Init(coinAmount);
            _gemView.Init(gemAmount);
        }
    }
}
