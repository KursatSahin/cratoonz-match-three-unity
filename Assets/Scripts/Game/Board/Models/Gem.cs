using System;
using UnityEngine;

namespace Game.Board
{
    [Serializable]
    public class Gem
    {
        public Action<Point, float> PositionChanged;
        public Action DestroyGem;

        private Point _position;
        private GemColor _color;
        private bool _isSelected;
        private bool _isSwapped;
        private bool _destroyed;
        private bool _isModified;
        
        public Point Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    var diff = Math.Max(_position.Y - 8, 1);
                    var durationFactor = 1 + Mathf.Log(diff);
                    _position = value;
                    PositionChanged?.Invoke(value, durationFactor);
                }
            }
        }

        public GemColor Color
        {
            get => _color;
            private set => _color = value;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        public bool IsSwapped
        {
            get => _isSwapped;
            set => _isSwapped = value;
        }

        public bool Destroyed
        {
            get => _destroyed;
            set
            {
                if (_destroyed == value) return;

                _destroyed = value;
                
                if (value)
                {
                    DestroyGem?.Invoke();
                }
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set => _isModified = value;
        }

        public Gem(Point position, GemColor color)
        {
            _position = position;
            _color = color;
            _isSelected = false;
        }

        public enum GemColor
        {
            Blue = 0,
            Yellow = 1,
            Green = 2,
            Red = 3,
            None = 99
        }
    }
}