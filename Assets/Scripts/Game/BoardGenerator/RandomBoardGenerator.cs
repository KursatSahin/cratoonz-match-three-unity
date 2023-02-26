using System;
using System.Collections.Generic;
using Containers;
using Game.Board;
using UnityEngine;
using Utils;

namespace Game
{
    public class RandomBoardGenerator : IBoardGenerator
    {
        private readonly IRandomGenerator randomGenerator;
        
        private Gem.GemColor[] _previousLeft;
        private Gem.GemColor _previousBelow;
        
        private List<Gem.GemColor> _avoidedColors;

        private List<int> _neighbourTilesIndexes;
        private List<Tile> _neighbourTiles;
        
        private Point _tempPosition = new Point();

        public RandomBoardGenerator(IRandomGenerator randomGenerator)
        {
            this.randomGenerator = randomGenerator;
            
            _avoidedColors = new List<Gem.GemColor>();
            _neighbourTilesIndexes =  new List<int>(8);
            _neighbourTiles = new List<Tile>(8);
        }

        public Gem[,] GenerateGems(BoardSettings boardSettings)
        {
            // Allocate tiles
            Gem[,] gems = new Gem[boardSettings.BoardWidth, boardSettings.BoardHeight];
            
            _previousLeft = new Gem.GemColor[boardSettings.BoardHeight];
            _previousBelow = Gem.GemColor.None;

            for (int col = 0; col < boardSettings.BoardHeight; col++)
            {
                for (int row = 0; row < boardSettings.BoardWidth; row++)
                {
                    // Convert 2d index to 1d index
                    
                    gems[row, col] = CreatePossibleGem(boardSettings.colorRatioPairs, col, row);
                }
            }
        
            Debug.Log("Creating board is done");
            
            return gems;
        }

        public Gem[,] GenerateGems(BoardSettings boardSettings, Tile[,] tiles)
        {
            // Allocate tiles
            Gem[,] gems = new Gem[boardSettings.BoardWidth, boardSettings.BoardHeight];
            
            _previousLeft = new Gem.GemColor[boardSettings.BoardHeight];
            _previousBelow = Gem.GemColor.None;

            for (int row = 0; row < boardSettings.BoardHeight; row++)
            {
                for (int col = 0; col < boardSettings.BoardWidth; col++)
                {
                    // Convert 2d index to 1d index
                    if (tiles[row, col].Type == Tile.TileType.Normal)
                    { 
                        gems[row, col] = CreatePossibleGem(boardSettings.colorRatioPairs, col, row);
                    }
                }
            }
        
            Debug.Log("Creating board is done");
            
            return gems;
        }
        
        public Tile[,] GenerateTiles(BoardSettings boardSettings)
        {
            Tile[,] tiles = new Tile[boardSettings.BoardWidth, boardSettings.BoardHeight];
            
            for (int row = 0; row < boardSettings.BoardHeight; row++)
            {
                for (int col = 0; col < boardSettings.BoardWidth; col++)
                {
                    tiles[row, col] = new Tile(Tile.TileType.Normal);
                }
            }

            return tiles;
        }

        // public Gem[,] GenerateTiles(int rowCount, int columnCount, Tile[] tiles)
        // {
        //     
        //     throw new NotImplementedException();
        // }

        public Gem CreateGem(ColorRatioPair[] colorRatioPairs, List<Gem.GemColor> avoidedColors, int columnIndex, int rowIndex)
        {
            _avoidedColors.Clear();

            if (!avoidedColors.IsNullOrEmpty()) _avoidedColors.AddRange(avoidedColors);
            
            Gem.GemColor validTileColor = GetRandomTileColor(colorRatioPairs, _avoidedColors);
            
            _tempPosition.X = columnIndex;
            _tempPosition.Y = rowIndex;
            
            Gem newGem = new Gem(_tempPosition, validTileColor);

            return newGem;
        }

        #region Private Methods

        /// <summary>
        /// This function returns a random possible color that we can assign to the tiles according to the color ratio pairs and avoided colors
        /// </summary>
        /// <param name="colorRatioPairs">A list of possible tile colors</param>
        /// <param name="avoidedColors">A list of excepted tile colors</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Color ratio pairs cannot be null or empty</exception>
        /// <exception cref="ArgumentNullException">Avoided colors cannot be null, but only be empty</exception>
        private Gem.GemColor GetRandomTileColor(ColorRatioPair[] colorRatioPairs, List<Gem.GemColor> avoidedColors)
        {
            
            if (colorRatioPairs.IsNullOrEmpty())
            {
                throw new ArgumentException("Color ratio pairs cannot be null or empty");
            }
    
            if (avoidedColors == null)
            {
                throw new ArgumentNullException("avoidedColors" + (Gem.GemColor[])null, "Avoided colors cannot be null");
            }
    
            int cumulativeProbability = 0;
    
            int colorRatioArrayLength = colorRatioPairs.Length;
            
            for (var i = 0; i < colorRatioArrayLength; i++)
            {
                var colorRatioPair = colorRatioPairs[i];
                if (!avoidedColors.Contains(colorRatioPair.color))
                {
                    cumulativeProbability += colorRatioPair.ratio;
                }
            }
            
            var uncategorizedColor = randomGenerator.GetRandomNumber(cumulativeProbability);
    
            for (var i = 0; i < colorRatioArrayLength; i++)
            {
                var colorRatioPair = colorRatioPairs[i];
                if (!avoidedColors.Contains(colorRatioPair.color))
                {
                    uncategorizedColor -= colorRatioPair.ratio;
                    if (uncategorizedColor <= 0)
                    {
                        // Tile color depends on color ratio
                        return colorRatioPair.color;
                    }
                }
            }
    
            return Gem.GemColor.None;
        }
        
        private Gem CreatePossibleGem(ColorRatioPair[] colorRatioPairs, int colIndex, int rowIndex)
        {
            _avoidedColors.Clear();
        
            _avoidedColors.Add(_previousLeft[colIndex]);
            _avoidedColors.Add(_previousBelow);

            Gem.GemColor validTileColor = GetRandomTileColor(colorRatioPairs, _avoidedColors);
            
            _tempPosition.X = colIndex;
            _tempPosition.Y = rowIndex;
            
            Gem newGem = new Gem(_tempPosition, validTileColor);

            _previousLeft[colIndex] = validTileColor;
            _previousBelow = validTileColor;

            return newGem;
        }
        
        #endregion
    }
}