using System.Collections.Generic;
using Bootsrapper;
using Containers;
using Core.Services;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Game.Board.Views
{
    public class GemView : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private GemContainer _gemContainer;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _selectedColor;
        #endregion

        #region Private Fields
        private SpriteRenderer _spriteRenderer;
        private static IBoardDrawHelper _boardDrawHelper;
        private static BoardSettings _boardSettings;
        #endregion 

        #region Properties
        public Gem Data { get; private set; }
        #endregion
    
        #region Unity Events
        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _boardSettings = AppBootstrapper.Containers.BoardSettingsContainer.BoardSettings;
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
            transform.position = _boardDrawHelper.GetWorldPosition(data.Position.X, data.Position.Y);
            
            Data.PositionChanged += OnPositionChanged;
            Data.DestroyGem += OnDestroyGem;
        }

        #endregion

        #region Private Functions
        
        private void OnDestroyGem()
        {
            
        }

        private void OnPositionChanged(Point position, float durationFactor)
        {
            
        }

        #endregion
        
    }
}