using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using WibeSoft.Core.Bootstrap;

namespace WibeSoft.Core.Managers
{
    public class PlayerLevelManager : SingletonBehaviour<PlayerLevelManager>
    {
        private PlayerLevelData _playerLevelData;
        private LogManager _logger => LogManager.Instance;

        public PlayerLevelData PlayerLevelData => _playerLevelData;

        public async UniTask Initialize()
        {
            // TODO: Bu veriler SaveManager'dan yüklenecek
            _playerLevelData = new PlayerLevelData(1000, 0, 1, "Oyuncu");
            _logger.LogInfo("PlayerLevelManager initialized", "PlayerLevelManager");
            await UniTask.CompletedTask;
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                _logger.LogWarning($"Invalid experience amount: {amount}", "PlayerLevelManager");
                return;
            }

            int newExp = _playerLevelData.CurrentExp + amount;
            int newLevel = _playerLevelData.Level;
            int maxExp = _playerLevelData.MaxExp;

            // Level atlama kontrolü
            while (newExp >= maxExp)
            {
                newExp -= maxExp;
                newLevel++;
                maxExp = CalculateMaxExpForLevel(newLevel);
            }

            _playerLevelData = new PlayerLevelData(maxExp, newExp, newLevel, _playerLevelData.Username);
            _logger.LogInfo($"Experience added: {amount}, New Level: {newLevel}, New Exp: {newExp}/{maxExp}", "PlayerLevelManager");
            
            GameEvents.TriggerPlayerLevelChanged(newLevel, newExp, maxExp);
        }

        public void SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Username cannot be empty", "PlayerLevelManager");
                return;
            }

            _playerLevelData = new PlayerLevelData(
                _playerLevelData.MaxExp,
                _playerLevelData.CurrentExp,
                _playerLevelData.Level,
                username
            );

            _logger.LogInfo($"Username updated to: {username}", "PlayerLevelManager");
        }

        private int CalculateMaxExpForLevel(int level)
        {
            // Basit bir level scaling formülü: her level için %20 artış
            return (int)(1000 * Mathf.Pow(1.2f, level - 1));
        }
    }
} 