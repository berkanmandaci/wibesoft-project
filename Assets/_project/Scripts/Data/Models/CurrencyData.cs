using System;

namespace WibeSoft.Data.Models
{
    public class CurrencyData
    {
        private int _gold;
        private int _gem;

        public int Gold
        {
            get => _gold;
            private set
            {
                if (value < 0) throw new ArgumentException("Gold cannot be negative");
                _gold = value;
            }
        }

        public int Gem
        {
            get => _gem;
            private set
            {
                if (value < 0) throw new ArgumentException("Gem cannot be negative");
                _gem = value;
            }
        }

        public CurrencyData(int gold = 0, int gem = 0)
        {
            Gold = gold;
            Gem = gem;
        }

        public void AddGold(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative");
            Gold += amount;
        }

        public void RemoveGold(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative");
            if (amount > Gold) throw new ArgumentException("Not enough gold");
            Gold -= amount;
        }

        public void AddGem(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative");
            Gem += amount;
        }

        public void RemoveGem(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative");
            if (amount > Gem) throw new ArgumentException("Not enough gem");
            Gem -= amount;
        }
    }
} 