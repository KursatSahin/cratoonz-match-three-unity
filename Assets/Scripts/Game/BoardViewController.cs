using Bootsrapper;
using Containers;
using Core.Events;
using Core.Services;
using Game.Board;
using Lean.Pool;
using UnityEngine;

namespace Game
{
    public class BoardViewController : MonoEventDispatcherListener
    {
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _gemPrefab;

        private BoardSettings _boardSettings;
        private Tile[,] _tileMap;
        private Gem[,] _gemMap;
        
        private IBoardDrawHelper _boardDrawHelper;
        private IBoardGenerator _boardGenerator;
        private IRandomGenerator _randomGenerator;
        
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
        }

        private void Start()
        {
            _tileMap = _boardGenerator.GenerateTiles(_boardSettings);
            _gemMap = _boardGenerator.GenerateGems(_boardSettings, _tileMap);
            
            DrawTiles();
        }
        
        private void DrawTiles()
        {
            GameObject tmpTile;
            for (int i = 0; i < _boardSettings.BoardHeight; i++)
            {
                for (int j = 0; j < _boardSettings.BoardWidth; j++)
                {
                    tmpTile = LeanPool.Spawn(_tilePrefab, _boardDrawHelper.GetWorldPosition(i, j), Quaternion.identity);
                }
            }
        }

        private void DrawGems()
        {
            
        }
    }
}

