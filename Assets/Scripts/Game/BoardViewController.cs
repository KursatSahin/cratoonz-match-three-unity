using System;
using Bootsrapper;
using Containers;
using Core.Events;
using Core.Events.GameEvents;
using Core.Services;
using Game.Board;
using Game.Board.Views;
using Lean.Pool;
using UnityEngine;

namespace Game
{
    public class BoardViewController : MonoEventDispatcherListener
    {
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _gemPrefab;
        
        [SerializeField] private Transform _gemParent;
        [SerializeField] private Transform _tileParent;

        private BoardSettings _boardSettings;
        private Tile[,] _tileMap;
        private Gem[,] _gemMap;
        private BoardLogic _boardLogic;
        
        private IBoardDrawHelper _boardDrawHelper;
        private IBoardGenerator _boardGenerator;
        private IRandomGenerator _randomGenerator;
        private IEventDispatcher _eventDispatcher;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Get BoardSettings from the BoardSettingsContainer
            var prefabProvider = AppBootstrapper.Containers.PrefabContainer;
            _boardSettings = AppBootstrapper.Containers.BoardSettingsContainer.BoardSettings;

            // Create and initialize RandomGenerator service, and register it to the ServiceLocator
            _randomGenerator = new RandomGenerator(); 
            ServiceLocator.Instance.RegisterService<IRandomGenerator>(_randomGenerator);
            
            // Create and initialize BoardGenerator service, and register it to the ServiceLocator
            _boardGenerator = new RandomBoardGenerator(_randomGenerator);
            ServiceLocator.Instance.RegisterService<IBoardGenerator>(_boardGenerator);
            
            // Create ands initialize BoardDrawHelper service, and register it to the ServiceLocator
            _boardDrawHelper = new BoardDrawHelper();
            ServiceLocator.Instance.RegisterService<IBoardDrawHelper>(_boardDrawHelper);
            
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }

        private void Start()
        {
            // Generate the tiles data
            _tileMap = _boardGenerator.GenerateTiles(_boardSettings);
            // Generate the gems data
            _gemMap = _boardGenerator.GenerateGems(_boardSettings, _tileMap);
            
            Vector3 boardScale = new Vector3(
                _boardSettings.BoardWidth * _boardSettings.TileSize, 
                _boardSettings.BoardHeight * _boardSettings.TileSize, 
                1);
            
            // Adjust the parent container for the tiles
            _tileParent.localScale = boardScale;
            // Adjust the parent container for the gems
            _gemParent.localScale = boardScale;

            _boardLogic = new BoardLogic(_gemMap);
            
            DrawTiles();
            DrawGems();

            _eventDispatcher.Subscribe(GameEventType.SwipeInputDetected, OnSwipe);
            _eventDispatcher.Subscribe(GameEventType.TapInputDetected, OnTap);
            _eventDispatcher.Subscribe(GameEventType.NewGemGenerated, OnNewGemGenerated);
        }

        private void OnDestroy()
        {
            _eventDispatcher.Unsubscribe(GameEventType.SwipeInputDetected, OnSwipe);
            _eventDispatcher.Unsubscribe(GameEventType.TapInputDetected, OnTap);
            _eventDispatcher.Unsubscribe(GameEventType.NewGemGenerated, OnNewGemGenerated);
        }

        private void Update()
        {
            _boardLogic.FindMatchesAndClear();
            //_boardLogic.SettleBoard();
            //_boardLogic.FillEmptySlots();
        }

        private void DrawTiles()
        {
            GameObject tmpTile;
            for (int row = 0; row < _boardSettings.BoardHeight; row++)
            {
                for (int col = 0; col < _boardSettings.BoardWidth; col++)
                {
                    tmpTile = LeanPool.Spawn(_tilePrefab, _tileParent, true);
                    tmpTile.transform.position = _boardDrawHelper.GetWorldPosition(row, col);
                }
            }
        }
        
        /// <summary>
        /// Generate gem view as much as board size
        /// </summary>
        private void DrawGems()
        {
            foreach (var gem in _gemMap)
            {
                GenerateGemView(gem);
            }
        }
        
        /// <summary>
        /// Spawns a gem and sets its data
        /// </summary>
        /// <param name="gemData"></param>
        private void GenerateGemView(Gem gemData)
        {
            if (LeanPool.Spawn(_gemPrefab, _gemParent, true).TryGetComponent(out GemView gemView))
            {
                gemView.SetGemData(gemData);
            }
        }
        
        private void OnNewGemGenerated(IEvent e)
        {
            var newGemEvent = e as NewGemGeneratedEvent;
            
            GenerateGemView(newGemEvent.GeneratedGem);
        }
        
        /// <summary>
        /// OnTap event handler, returns the tapped chip view
        /// </summary>
        /// <param name="gemView"></param>
        private void OnTap(IEvent e)
        {
            var tapEvent = e as TapInputEvent;
            var gemView = tapEvent?.OriginGemView;
            
            Debug.Log($"Tap event received ({gemView.Data.Position})");
            
            // if there is not any selected gem, select it
            if (GemView.PreviousSelected == null)
            {
                gemView.Select();
            }
            else
            {
                // if the tapped gem is already selected, deselect it
                if (GemView.PreviousSelected.Data.Position == gemView.Data.Position)
                {
                    gemView.Deselect();
                }
                else
                {
                    // if there is already a different selected gem and the tapped gem is its neighbour, swap them
                    if (gemView.GetAdjacents().Contains(GemView.PreviousSelected.Data.Position))
                    {
                        _boardLogic.SwapGems(gemView.Data, GemView.PreviousSelected.Data);
                        GemView.PreviousSelected.Deselect();
                    }
                    // if there is already a different selected gem and the tapped gem is not its neighbour, deselect previous one and select this
                    else
                    {
                        gemView.Select();
                    }
                }
            }
        }
        
        /// <summary>
        /// OnSwipe event handler, returns the chip view where the swipe started and swipe direction 
        /// </summary>
        /// <param name="gemView">Collided gem view</param>
        /// <param name="swipeDirection">Swipe direction</param>
        private void OnSwipe(IEvent e)
        {
            var swipeEvent = e as SwipeInputEvent;
            var gemView = swipeEvent?.OriginGemView;
            var swipeDirection = swipeEvent.Direction;

            Debug.Log($"Swipe event received ({gemView.Data.Position}) with direction => ({swipeDirection})");

            if (GemView.PreviousSelected != null)
            {
                GemView.PreviousSelected.Deselect();
            }

            // Calculate new gem position
            Point toPosition = gemView.Data.Position + swipeDirection;

            // check if the new position is valid or not -is there any gem at position-
            if (!IsPositionValid(toPosition)) return;

            // swap the gems at starting position of swipe and its neighbour which is found with swipe direction
            _boardLogic.SwapGems(gemView.Data, _gemMap[toPosition.Y, toPosition.X]);
        }

        /// <summary>
        /// checks if the position is within the board boundaries
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsPositionValid(Point position)
        {
            if (position.X < 0 || position.X >= _boardSettings.BoardWidth)
            {
                return false;
            }

            if (position.Y < 0 || position.Y >= _boardSettings.BoardHeight)
            {
                return false;
            }

            return true;
        }
        
    }
}

