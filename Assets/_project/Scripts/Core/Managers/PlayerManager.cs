using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using WibeSoft.Core.Bootstrap;

namespace WibeSoft.Core.Managers
{
    public class PlayerManager : SingletonBehaviour<PlayerManager>
    {
        private PlayerData _playerData;
        private LogManager _logger => LogManager.Instance;

        public PlayerData PlayerData => _playerData;

        public async UniTask Initialize()
        {
            // TODO: Bu veriler SaveManager'dan yüklenecek
            _playerData = new PlayerData(1000, 0, 1, "Oyuncu");
            _logger.LogInfo("PlayerManager initialized", "PlayerManager");
            await UniTask.CompletedTask;
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                _logger.LogWarning($"Invalid experience amount: {amount}", "PlayerManager");
                return;
            }

            int newExp = _playerData.CurrentExp + amount;
            int newLevel = _playerData.Level;
            int maxExp = _playerData.MaxExp;

            // Level atlama kontrolü
            while (newExp >= maxExp)
            {
                newExp -= maxExp;
                newLevel++;
                maxExp = CalculateMaxExpForLevel(newLevel);
            }

            _playerData = new PlayerData(maxExp, newExp, newLevel, _playerData.Username);
            _logger.LogInfo($"Experience added: {amount}, New Level: {newLevel}, New Exp: {newExp}/{maxExp}", "PlayerManager");
            
            GameEvents.TriggerPlayerLevelChanged(newLevel, newExp, maxExp);
        }

        public void SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Username cannot be empty", "PlayerManager");
                return;
            }

            _playerData = new PlayerData(
                _playerData.MaxExp,
                _playerData.CurrentExp,
                _playerData.Level,
                username
            );

            _logger.LogInfo($"Username updated to: {username}", "PlayerManager");
        }

        private int CalculateMaxExpForLevel(int level)
        {
            // Basit bir level scaling formülü: her level için %20 artış
            return (int)(1000 * Mathf.Pow(1.2f, level - 1));
        }
    }
} 