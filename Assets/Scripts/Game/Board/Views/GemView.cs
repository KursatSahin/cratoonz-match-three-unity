using System.Collections.Generic;
using Bootsrapper;
using Containers;
using Core.Events;
using Core.Services;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Game.Board.Views
{
    public class GemView : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;
        #endregion

        #region Private Fields
        private SpriteRenderer _spriteRenderer;
        private static IBoardDrawHelper _boardDrawHelper;
        private static BoardSettings _boardSettings;
        private static GemContainer _gemContainer;
        private IEventDispatcher _eventDispatcher;
        #endregion 

        #region Properties
        public static GemView PreviousSelected { get; private set; }
        public Gem Data { get; private set; }
        #endregion
    
        #region Unity Events
        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _boardSettings = AppBootstrapper.Containers.BoardSettingsContainer.BoardSettings;
            _gemContainer = AppBootstrapper.Containers.GemContainer;
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }

        private void OnDisable()
        {
            _spriteRenderer.color = _defaultColor;
        }

        #endregion

        #region Public Functions

        public void SetGemData (Gem data)
        {
            if (_boardDrawHelper == null)
            {
                _boardDrawHelper = ServiceLocator.Instance.Get<IBoardDrawHelper>();
            }

            Data = data;
            _spriteRenderer.sprite = _gemContainer.Gems[(int)Data.Color];
            transform.position = _boardDrawHelper.GetWorldPosition(data.Position.Y, data.Position.X);
            
            Data.PositionChanged += OnPositionChanged;
            Data.DestroyGem += OnDestroyGem;
        }

        public void Select()
        {
            if (PreviousSelected)
                PreviousSelected.Deselect();

            PreviousSelected = this;
            
            _spriteRenderer.color = _selectedColor;
        }

        public void Deselect()
        {
            if (PreviousSelected == null) 
                return;

            // Hide outline
            PreviousSelected = null;
            
            _spriteRenderer.color = _defaultColor;
        }
        
        public List<Point> GetAdjacents()
        {
            var adjacents = new List<Point>();

            if (Data.Position.X > 0)  adjacents.Add(new Point(Data.Position.X - 1, Data.Position.Y));
            if (Data.Position.X < _boardSettings.BoardWidth - 1)  adjacents.Add(new Point(Data.Position.X + 1, Data.Position.Y));
            if (Data.Position.Y > 0)  adjacents.Add(new Point(Data.Position.X, Data.Position.Y - 1));
            if (Data.Position.Y < _boardSettings.BoardHeight - 1)  adjacents.Add(new Point(Data.Position.X, Data.Position.Y + 1));

            return adjacents;
        }
        
        #endregion

        #region Private Functions
        
        private void OnDestroyGem()
        {
            Data.PositionChanged -= OnPositionChanged;
            Data.DestroyGem -= OnDestroyGem;
            Data = null;
            
            _spriteRenderer.DOFade(0, 0.4f).
                OnComplete(() => LeanPool.Despawn(gameObject));
        }

        private void OnPositionChanged(Point position, float durationFactor)
        {
            transform.DOMove(_boardDrawHelper.GetWorldPosition(position.Y, position.X), 0.4f * durationFactor)
                .SetEase(Ease.OutCirc).OnComplete((() =>
                {
                    _eventDispatcher.Fire(GameEventType.UnblockInputHandler);
                }));
        }

        #endregion
        
    }
}