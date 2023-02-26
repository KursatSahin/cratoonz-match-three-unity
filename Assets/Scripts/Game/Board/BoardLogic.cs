using System.Collections.Generic;
using Bootsrapper;
using Containers;
using Core.Events;
using Core.Events.GameEvents;
using Core.Services;

namespace Game.Board
{
    public class BoardLogic
    {
        #region Public Fields
        public Gem [,] GemMap { get; set; }
        
        #endregion

        #region Private Fields

        private int _minSolutionLength = 3;
        private BoardSettings _boardSettings;
        private IEventDispatcher _eventDispatcher;
        private IBoardGenerator _boardGenerator;
        
        private List<Gem> _destroyedCollector = new List<Gem>();
        private HashSet<int> _emptyColumns = new HashSet<int>();
        
        private bool _isBoardModified = false;
        
        #endregion

        #region Public Functions

        public BoardLogic(Gem[,] gemMap)
        {
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            _boardSettings = AppBootstrapper.Containers.BoardSettingsContainer.BoardSettings;
            _boardGenerator = ServiceLocator.Instance.Get<IBoardGenerator>();
            GemMap = gemMap;
        }

        /// <summary>
        /// Swap given gems
        /// </summary>
        /// <param name="firstGem">First gem</param>
        /// <param name="secondGem">Second gem</param>
        public void SwapGems(Gem firstGem, Gem secondGem)
        {
            // Block input helper during animations done 
            _eventDispatcher.Fire(GameEventType.BlockInputHandler);

            // Swap gems on array
            GemMap[firstGem.Position.Y, firstGem.Position.X] = secondGem;
            GemMap[secondGem.Position.Y, secondGem.Position.X] = firstGem;
         
            // Mark gems is swapped
            firstGem.IsSwapped = true;
            secondGem.IsSwapped = true;
            
            // Swap gems on position data this refers their position on board array. Also this trigger gem view to move to new position
            (firstGem.Position, secondGem.Position) = (secondGem.Position, firstGem.Position);

            // Mark gems is modified
            firstGem.IsModified = true;
            secondGem.IsModified = true;
            
            // Mark board is modified
            _isBoardModified = true;
        }
        
        /// <summary>
        /// Swap last swapped gems back to their previous positions
        /// </summary>
        /// <param name="firstGem"></param>
        /// <param name="secondGem"></param>
        public void RollBack(Gem firstGem, Gem secondGem)
        {
            // Swap gems on array
            GemMap[firstGem.Position.Y, firstGem.Position.X] = secondGem;
            GemMap[secondGem.Position.Y, secondGem.Position.X] = firstGem;

            // Swap gems on position data this refers their position on board array. Also this trigger gem view to move to new position
            (firstGem.Position, secondGem.Position) = (secondGem.Position, firstGem.Position);
        }

        /// <summary>
        /// Find mathed gems and destroy them.
        /// Starting point of finding matches are gems which were marked as swapped. From these points the function traversing same color adjacents and try to find matches
        /// If match found at all matched gems destroy at the end of function.  
        /// </summary>
        public void FindMatchesAndClear()
        {
            if (!_isBoardModified)
                return;

            var _isRollBackNeeded = true;
            
            // Get modified gems
            var modifiedGems = GetModifiedGems();
            
            List<Gem> mathedGems = new List<Gem>();
            
            // Check each modified gem and its adjacents for matches
            foreach (var gem in modifiedGems)
            {
                mathedGems.Clear();
                
                // Check this modified gem and its adjacents for matches
                var isMatchFound = CheckGemForMatch(gem, mathedGems);
                
                // If match found at all gems on mathedGems list destroy and collect from _destroyedCollector
                if (isMatchFound)
                {
                    foreach (var gemData in mathedGems)
                    {
                        // Mark gem as destroyed
                        gemData.Destroyed = true;
                        _destroyedCollector.Add(gemData);
                    }
                }

                // If match not found and gem is swapped and also other modified gem are on same situation then roll back gem to its previous position
                _isRollBackNeeded = _isRollBackNeeded && (gem.IsSwapped && !isMatchFound);
                
                // 
                gem.IsSwapped = false;
                gem.IsModified = false;
            }

            // If roll back needed then roll back modified gems
            if (_isRollBackNeeded)
            {
                RollBack(modifiedGems[0], modifiedGems[1]);
            }

            // Marked board as not modified
            _isBoardModified = false;
        }
        
        /// <summary>
        /// After matches are found and destroyed, this function make all pieces fall to destroyed down-adjacents position
        /// </summary>
        public void SettleBoard()
        {
            // Get modified columns
            HashSet<int> fallingColumns = new HashSet<int>();
            foreach (var destroyedGem in _destroyedCollector)
            {
                fallingColumns.Add(destroyedGem.Position.X);
            }
            
            // Check if modified columns are empty
            if (fallingColumns.Count < 1)
                return;

            foreach (var column in fallingColumns)
            {
                int fallingDistance = 0;

                // Traverse the column from bottom row to top row
                for (int row = 0; row < _boardSettings.BoardHeight; row++)
                {
                    var gemData = GemMap[row, column];
                    
                    // If gem is destroyed then increment falling distance and set board array null
                    if (gemData.Destroyed)
                    {
                        GemMap[row, column] = null;
                        fallingDistance++;
                    }
                    // If gem is not destroyed
                    else
                    {
                        
                        if (fallingDistance > 0)
                        {
                            // Calculate landing position of gem after falling complete
                            Point landingPosition = new Point( column, row - fallingDistance );
                            
                            // Update relevant slot of board array with this gemData 
                            GemMap[landingPosition.Y, landingPosition.X] = gemData;
                            
                            // Set null to board slot which holds gemData previously
                            GemMap[row, column] = null;
                            
                            // Update gem data with landing position. Also this trigger gem view to move to new the position
                            gemData.Position = landingPosition;
                            
                            // Mark gem as modified 
                            gemData.IsModified = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill all empty slots with new gems
        /// This function traverse only empty columns from bottom row to top row, and fill empty slots at the top of column 
        /// </summary>
        public void FillEmptySlots()
        {
            _emptyColumns.Clear();
            
            // Get modified columns
            foreach (var destroyedGem in _destroyedCollector)
            {
                _emptyColumns.Add(destroyedGem.Position.X);
            }

            // Check if modified columns are empty
            if (_emptyColumns.Count < 1)
                return;
            
            foreach (var column in _emptyColumns)
            {
                // Set starting generation position row index  
                var generatorRowIndex = _boardSettings.BoardHeight;
                
                // Traverse the column from bottom row to top row
                for (int row = 0; row < _boardSettings.BoardHeight; row++)
                {
                    var gemData = GemMap[row, column];

                    // If gemData is is null create a new gem data for this empty slot
                    if (gemData == null)
                    {
                        // Create a new gemData for this position
                        gemData = _boardGenerator.CreateGem(_boardSettings.colorRatioPairs, null, column, row);
                        
                        // Change gemData position to same column but generator row. This means gem view spawn the top of same column and will fall down to this position
                        gemData.Position = new Point(column, generatorRowIndex++);

                        // Created gemData set it to board array
                        GemMap[row, column] = gemData;
                        
                        // Spawn gem view with this gemData
                        var newGemGeneratedEvent = new NewGemGeneratedEvent(gemData);
                        _eventDispatcher.Fire(GameEventType.NewGemGenerated, newGemGeneratedEvent);

                        // Update gem data with original position. Also this trigger gem view to move to new the position
                        gemData.Position = new Point(column, row);
                        
                        // Mark gem as modified
                        gemData.IsModified = true;
                    }
                }
            }
            
            _destroyedCollector.Clear();
            _isBoardModified = true;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Check gem and its adjacents for possible match
        /// </summary>
        /// <param name="gem">Gem instance</param>
        /// <param name="result">List of matched gems which is using for returning parameter</param>
        /// <returns>returns TRUE if matched gems count more than minimum solution count, else FALSE</returns>
        private bool CheckGemForMatch(Gem gem, List<Gem> result)
        {
            var current = gem;
            result.Add(current);
            Point nextPosition;
            Gem next;
             
            List<Gem> horizontalMatches = new List<Gem>{current};
            List<Gem> verticalMatches = new List<Gem>{current};

            // Check up left for horizontal matches
            if (current.Position.X > 0)
            {
                nextPosition = current.Position + Point.Left;
                next = GemMap[nextPosition.Y, nextPosition.X];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Left, horizontalMatches);
            }

            // Check up right for horizontal matches
            if (current.Position.X < _boardSettings.BoardWidth - 1)
            {
                nextPosition = current.Position + Point.Right;
                next = GemMap[nextPosition.Y, nextPosition.X];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Right, horizontalMatches);
            }

            // Check down adjacents for vertical matches
            if (current.Position.Y > 0)
            {
                nextPosition = current.Position + Point.Down;
                next = GemMap[nextPosition.Y, nextPosition.X];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Down, verticalMatches);
            }

            // Check up adjacents for vertical matches 
            if (current.Position.Y < _boardSettings.BoardHeight - 1)
            {
                nextPosition = current.Position + Point.Up;
                next = GemMap[nextPosition.Y, nextPosition.X];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Up, verticalMatches);
            }

            // remove first gem from the list because of avoiding duplicates current gem
            horizontalMatches.RemoveAt(0);
            if (horizontalMatches.Count >= _minSolutionLength-1)
            {
                result.AddRange(horizontalMatches);
            }
            
            // remove first gem from the list because of avoiding duplicate current gem
            verticalMatches.RemoveAt(0);
            if (verticalMatches.Count >= _minSolutionLength-1)
            {
                result.AddRange(verticalMatches);
            }
            
            return result.Count >= _minSolutionLength;
        }
        
        /// <summary>
        /// Recursive helper function for CheckGemForMatch.
        /// This function will check the gem at the given position and adjacent gems on only one direction of the same color.
        /// </summary>
        /// <param name="gem"></param>
        /// <param name="direction"></param>
        /// <param name="result"></param>
        private void CheckGemForMatchHelper(Gem gem, Point direction, List<Gem> result)
        {
            var current = gem;
            result.Add(current);
            
            var nextPosition = current.Position + direction;
            
            if (nextPosition.X < 0 || nextPosition.X >= _boardSettings.BoardWidth || nextPosition.Y < 0 || nextPosition.Y >= _boardSettings.BoardHeight)
                return;
            
            var next = GemMap[nextPosition.Y, nextPosition.X];
            
            if (next.Color == current.Color)
            {
                CheckGemForMatchHelper(next, direction, result);
            }
        }
        
        /// <summary>
        /// Get modified gems on board
        /// </summary>
        /// <returns>List of gemData</returns>
        private List<Gem> GetModifiedGems()
        {
            var result = new List<Gem>();

            foreach (var gem in GemMap)
            {
                if (gem.IsModified)
                {
                    result.Add(gem);
                }
            }
            return result;
        }
        #endregion
    }
}