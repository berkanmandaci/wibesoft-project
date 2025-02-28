using System;

namespace WibeSoft.Data.Models
{
    public class PlayerLevelData
    {
        private readonly int _maxExp;
        private readonly int _currentExp;
        private readonly int _level;
        private readonly string _username;

        public int MaxExp => _maxExp;
        public int CurrentExp => _currentExp;
        public int Level => _level;
        public string Username => _username;

        public PlayerLevelData(int maxExp, int currentExp, int level, string username)
        {
            if (maxExp <= 0) throw new ArgumentException("MaxExp must be greater than 0");
            if (currentExp < 0) throw new ArgumentException("CurrentExp cannot be negative");
            if (currentExp > maxExp) throw new ArgumentException("CurrentExp cannot be greater than MaxExp");
            if (level < 1) throw new ArgumentException("Level must be at least 1");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username cannot be empty");

            _maxExp = maxExp;
            _currentExp = currentExp;
            _level = level;
            _username = username;
        }
    }
} 