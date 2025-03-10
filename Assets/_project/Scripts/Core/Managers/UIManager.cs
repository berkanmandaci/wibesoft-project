using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.UI.Controllers.HudControllers;

namespace WibeSoft.Core.Managers
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        private Canvas _mainCanvas;
        private GameObject _currentPanel;
        private LogManager _logger => LogManager.Instance;
        public bool IsPopupOpen;

        public async UniTask Initialize()
        {
            // Create main canvas if not exists
            if (_mainCanvas == null)
            {
                var canvasObj = new GameObject("MainCanvas");
                _mainCanvas = canvasObj.AddComponent<Canvas>();
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                _mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _logger.LogInfo("Main canvas created", "UIManager");
            }

            // Initialize HUD Controller
            await HudController.Instance.Initialize();
            
            return;
        }

        public void ShowPanel(GameObject panelPrefab)
        {
            if (_currentPanel != null)
            {
                HideCurrentPanel();
            }

            _currentPanel = Instantiate(panelPrefab, _mainCanvas.transform);
            _logger.LogInfo($"Showing panel: {panelPrefab.name}", "UIManager");
        }

        public void HideCurrentPanel()
        {
            if (_currentPanel != null)
            {
                Destroy(_currentPanel);
                _currentPanel = null;
                _logger.LogInfo("Current panel hidden", "UIManager");
            }
        }
    }
}
