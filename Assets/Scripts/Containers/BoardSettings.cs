using System;
using Game.Board;

namespace Containers
{
    [Serializable]
    public class BoardSettings
    {
        public int BoardWidth = 8;
        public int BoardHeight = 8;
        public float TileSize = 1.28f;
        
        public ColorRatioPair[] colorRatioPairs;
    }
}