using System.Collections.Generic;
using Containers;
using Core.Services;
using Game.Board;

namespace Game
{
    public interface IBoardGenerator : IService
    {
        public Gem[,] GenerateGems(BoardSettings boardSettings);
        public Gem[,] GenerateGems(BoardSettings boardSettings, Tile[,] tiles);
        public Tile[,] GenerateTiles(BoardSettings boardSettings);
        
        //public Gem[,] GenerateTiles(int rowCount, int columnCount, Tile[] tiles);
        
        public Gem CreateGem(ColorRatioPair[] colorRatioPairs, List<Gem.GemColor> avoidedColors, int columnIndex, int rowIndex);
    }
}