using System;
using UnityEngine;
using WibeSoft.Core.Bootstrap;
using WibeSoft.Core.Managers;
namespace WibeSoft.UI.Controllers
{
    public class PopupController : MonoBehaviour
    {

        private void Awake()
        {
            GameEvents.OnPopupOpened += OnPopupOpened;
            gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            GameEvents.OnPopupOpened -= OnPopupOpened;
        }

        private void OnPopupOpened()
        {
            gameObject.SetActive(true);
            UIManager.Instance.IsPopupOpen=true;
        }


        public void ClosePopup()
        {
            GameEvents.TriggerClosePopup();
            gameObject.SetActive(false);
            UIManager.Instance.IsPopupOpen=false;
        }
    }
}
