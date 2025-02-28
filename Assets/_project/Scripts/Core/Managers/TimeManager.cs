using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    public class TimeManager : SingletonBehaviour<TimeManager>
    {
        private float _lastSaveTime;
        private LogManager _logger => LogManager.Instance;

        public async UniTask Initialize()
        {
            _lastSaveTime = Time.time;
            _logger.LogInfo("TimeManager initialized", "TimeManager");
            await UniTask.CompletedTask;
        }

        public void UpdateGameTime()
        {
            _logger.LogInfo("Updating game time...", "TimeManager");
        }

        public float GetOfflineTime()
        {
            var currentTime = Time.time;
            var offlineTime = currentTime - _lastSaveTime;
            _logger.LogInfo($"Offline time: {offlineTime} seconds", "TimeManager");
            return offlineTime;
        }
    }
} 