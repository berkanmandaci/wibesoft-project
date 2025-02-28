using UnityEngine;
using Cysharp.Threading.Tasks;
using WibeSoft.Core.Singleton;
using WibeSoft.Data.Models;
using WibeSoft.Core.Bootstrap;

namespace WibeSoft.Core.Managers
{
    public class PlayerManager : SingletonBehaviour<PlayerManager>
    {
        private LogManager _logger;
        public PlayerData PlayerData;


        public async UniTask Initialize()
        {
            _logger = LogManager.Instance;
            _logger.LogInfo("PlayerManager başlatılıyor", "PlayerManager");

            var playerData = JsonDataService.Instance.GetPlayerData();
            PlayerData = new PlayerData(
                playerData.MaxExp,
                playerData.CurrentExp,
                playerData.Level,
                playerData.Username,
                playerData.Gold,
                playerData.Gem
            );

            _logger.LogInfo("PlayerManager başlatıldı", "PlayerManager");
        }

        public async UniTask LoadData(int maxExp, int currentExp, int level, string username)
        {
            _logger.LogInfo("Loading player data", "PlayerManager");

            try
            {
                if (PlayerData == null)
                {
                    PlayerData = new PlayerData(maxExp, currentExp, level, username);
                }
                else
                {
                    PlayerData.UpdateExperience(maxExp, currentExp, level);
                    PlayerData.SetUsername(username);
                }

                GameEvents.TriggerPlayerLevelChanged(PlayerData.Level, PlayerData.CurrentExp, PlayerData.MaxExp);
                _logger.LogInfo("Player data loaded", "PlayerManager");
            }
            catch (System.ArgumentException ex)
            {
                _logger.LogError($"Invalid player data: {ex.Message}", "PlayerManager");
                throw;
            }
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                _logger.LogWarning($"Invalid experience amount: {amount}", "PlayerManager");
                return;
            }

            int newExp = PlayerData.CurrentExp + amount;
            int newLevel = PlayerData.Level;
            int maxExp = PlayerData.MaxExp;

            while (newExp >= maxExp)
            {
                newExp -= maxExp;
                newLevel++;
                maxExp = CalculateMaxExpForLevel(newLevel);
                _logger.LogInfo($"Level up! New level: {newLevel}", "PlayerManager");
            }

            PlayerData.UpdateExperience(maxExp, newExp, newLevel);
            GameEvents.TriggerPlayerLevelChanged(PlayerData.Level, PlayerData.CurrentExp, PlayerData.MaxExp);
        }

        public void SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Username cannot be empty", "PlayerManager");
                return;
            }

            PlayerData.SetUsername(username);
            _logger.LogInfo($"Username updated: {username}", "PlayerManager");
        }

        private int CalculateMaxExpForLevel(int level)
        {
            // Simple level scaling formula: 20% increase per level
            return (int)(1000 * Mathf.Pow(1.2f, level - 1));
        }

        public void AddGold(int amount)
        {
            if (PlayerData.AddGold(amount))
            {
                _logger.LogInfo($"{amount} gold eklendi. Yeni toplam: {PlayerData.Currency.Gold}", "PlayerManager");
                GameEvents.TriggerCurrencyChanged(PlayerData.Currency.Gold, PlayerData.Currency.Gem);
                
                // Save the changes
                var saveData = JsonDataService.Instance.GetPlayerData();
                saveData.Gold = PlayerData.Currency.Gold;
                JsonDataService.Instance.SavePlayerData(saveData).Forget();
            }
        }

        public void RemoveGold(int amount)
        {
            if (PlayerData.RemoveGold(amount))
            {
                _logger.LogInfo($"{amount} gold silindi. Yeni toplam: {PlayerData.Currency.Gold}", "PlayerManager");
                GameEvents.TriggerCurrencyChanged(PlayerData.Currency.Gold, PlayerData.Currency.Gem);
                
                // Save the changes
                var saveData = JsonDataService.Instance.GetPlayerData();
                saveData.Gold = PlayerData.Currency.Gold;
                JsonDataService.Instance.SavePlayerData(saveData).Forget();
            }
        }

        public void AddGem(int amount)
        {
            if (PlayerData.AddGem(amount))
            {
                _logger.LogInfo($"{amount} gem eklendi. Yeni toplam: {PlayerData.Currency.Gem}", "PlayerManager");
                GameEvents.TriggerCurrencyChanged(PlayerData.Currency.Gold, PlayerData.Currency.Gem);
                
                // Save the changes
                var saveData = JsonDataService.Instance.GetPlayerData();
                saveData.Gem = PlayerData.Currency.Gem;
                JsonDataService.Instance.SavePlayerData(saveData).Forget();
            }
        }

        public void RemoveGem(int amount)
        {
            if (PlayerData.RemoveGem(amount))
            {
                _logger.LogInfo($"{amount} gem silindi. Yeni toplam: {PlayerData.Currency.Gem}", "PlayerManager");
                GameEvents.TriggerCurrencyChanged(PlayerData.Currency.Gold, PlayerData.Currency.Gem);
                
                // Save the changes
                var saveData = JsonDataService.Instance.GetPlayerData();
                saveData.Gem = PlayerData.Currency.Gem;
                JsonDataService.Instance.SavePlayerData(saveData).Forget();
            }
        }
    }
} 