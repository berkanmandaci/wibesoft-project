using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using System;

namespace WibeSoft.Core.Managers
{
    public class TimeManager : SingletonBehaviour<TimeManager>
    {
        private LogManager _logger => LogManager.Instance;
        private JsonDataService _jsonDataService => JsonDataService.Instance;

        public async UniTask Initialize()
        {
            _logger.LogInfo("Initializing TimeManager", "TimeManager");
            await UniTask.CompletedTask;
            _logger.LogInfo("TimeManager initialized", "TimeManager");
        }

        public void UpdateGameTime()
        {
            _logger.LogInfo("Updating game time...", "TimeManager");
        }

        public TimeSpan GetOfflineTime()
        {
            var lastSaveTime = _jsonDataService.GetAllData().LastSaveTime;
            var offlineTime = DateTime.Now - lastSaveTime;
            _logger.LogInfo($"Offline time: {offlineTime.TotalSeconds} seconds", "TimeManager");
            return offlineTime;
        }
    }
} 