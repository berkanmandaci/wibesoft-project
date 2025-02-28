using System;

namespace WibeSoft.Data.Models
{
    public class PlayerData
    {
        private int _maxExp;
        private int _currentExp;
        private int _level;
        private string _username;
        private CurrencyData _currencyData;

        public int MaxExp
        {
            get => _maxExp;
            private set
            {
                if (value <= 0) throw new ArgumentException("MaxExp must be greater than 0");
                _maxExp = value;
            }
        }

        public int CurrentExp
        {
            get => _currentExp;
            private set
            {
                if (value < 0) throw new ArgumentException("CurrentExp cannot be negative");
                if (value > MaxExp) throw new ArgumentException("CurrentExp cannot be greater than MaxExp");
                _currentExp = value;
            }
        }

        public int Level
        {
            get => _level;
            private set
            {
                if (value < 1) throw new ArgumentException("Level must be at least 1");
                _level = value;
            }
        }

        public string Username
        {
            get => _username;
            private set
            {
                if (string.IsNullOrEmpty(value)) throw new ArgumentException("Username cannot be empty");
                _username = value;
            }
        }

        public CurrencyData Currency => _currencyData;

        public PlayerData(int maxExp, int currentExp, int level, string username, int gold = 0, int gem = 0)
        {
            MaxExp = maxExp;
            Level = level;
            CurrentExp = currentExp;
            Username = username;
            _currencyData = new CurrencyData(gold, gem);
        }

        public void UpdateExperience(int maxExp, int currentExp, int level)
        {
            MaxExp = maxExp;
            Level = level;
            CurrentExp = currentExp;
        }

        public void SetUsername(string newUsername)
        {
            Username = newUsername;
        }

        public bool AddGold(int amount)
        {
            try
            {
                _currencyData.AddGold(amount);
                return true;
            }
            catch (System.ArgumentException)
            {
                return false;
            }
        }

        public bool RemoveGold(int amount)
        {
            try
            {
                _currencyData.RemoveGold(amount);
                return true;
            }
            catch (System.ArgumentException)
            {
                return false;
            }
        }

        public bool AddGem(int amount)
        {
            try
            {
                _currencyData.AddGem(amount);
                return true;
            }
            catch (System.ArgumentException)
            {
                return false;
            }
        }

        public bool RemoveGem(int amount)
        {
            try
            {
                _currencyData.RemoveGem(amount);
                return true;
            }
            catch (System.ArgumentException)
            {
                return false;
            }
        }
    }
} 