using UnityEngine;

namespace WibeSoft.Data.Models
{
    public class Cell
    {
        private readonly int _x;
        private readonly int _y;
        private readonly Vector2Int _position;

        public int X => _x;
        public int Y => _y;
        public Vector2Int Position => _position;

        public Cell(int x, int y)
        {
            _x = x;
            _y = y;
            _position = new Vector2Int(x, y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Cell other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _position.GetHashCode();
        }

        public override string ToString()
        {
            return $"Cell({X}, {Y})";
        }
    }
} 