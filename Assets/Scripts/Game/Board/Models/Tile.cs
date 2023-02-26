using System;

namespace Game.Board
{
    [Serializable]
    public class Tile
    {
        public TileType Type { get; }
        public Tile(TileType type)
        {
            Type = type;
        }
        
        public enum TileType
        {
            Normal = 0,
            Empty = 1,
            Obstacle = 2,
        }
    }
}