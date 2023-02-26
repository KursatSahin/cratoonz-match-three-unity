using System;
using Core.Events;
using Core.Events.GameEvents;
using Core.Services;
using Game.Board;
using Game.Board.Views;
using Lean.Touch;
using UnityEngine;
using Utils;

public class InputHandler : MonoBehaviour
    {
        #region Private Fields
        private Camera _mainCamera;

        private bool _isBlocked = false;
        
        private IEventDispatcher _eventDispatcher;
        
        #endregion

        #region Public Functions
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
            _isBlocked = true;
        }
        
        public void OnEnable()
        {
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerSwipe += OnFingerSwipe;
            _eventDispatcher.Subscribe(GameEventType.BlockInputHandler, OnBlockInputHandler);
            _eventDispatcher.Subscribe(GameEventType.UnblockInputHandler, OnUnblockInputHandler);
            
            _isBlocked = false;
        }
        
        public void OnDisable()
        {
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerSwipe -= OnFingerSwipe;
            _eventDispatcher.Unsubscribe(GameEventType.BlockInputHandler, OnBlockInputHandler);
            _eventDispatcher.Unsubscribe(GameEventType.UnblockInputHandler, OnUnblockInputHandler);
        }
        
        #endregion

        #region Private Functions
        /// <summary>
        /// OnFingerSwipe event handler
        /// </summary>
        /// <param name="finger"></param>
        private void OnFingerSwipe(LeanFinger finger)
        {
            if (_isBlocked) return;
            
            var gemView = GetGem(finger);
            if (gemView == null) return;

            Point direction = Point.Zero;
            
            var swipe = finger.SwipeScreenDelta;
            if (swipe.x < -Mathf.Abs(swipe.y)) direction = Point.Left;
            if (swipe.x > Mathf.Abs(swipe.y)) direction = Point.Right;
            if (swipe.y < -Mathf.Abs(swipe.x)) direction = Point.Down;
            if (swipe.y > Mathf.Abs(swipe.x)) direction = Point.Up;

            var swipeInputEvent = new SwipeInputEvent(gemView, direction);
            _eventDispatcher.Fire(GameEventType.SwipeInputDetected, swipeInputEvent);
        }
        
        /// <summary>
        /// OnFingerTap event handler
        /// </summary>
        /// <param name="finger"></param>
        private void OnFingerTap(LeanFinger finger)
        {
            if (_isBlocked) return;
            
            GemView gemView = GetGem(finger);
            if (gemView == null) return;

            var tapInputEvent = new TapInputEvent(gemView);
            _eventDispatcher.Fire(GameEventType.TapInputDetected, tapInputEvent);
        }
        
        /// <summary>
        /// Get gem view by hit by finger
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        private GemView GetGem(LeanFinger finger)
        {
            var hit = Physics2D.Raycast(_mainCamera.ScreenPointToRay(finger.StartScreenPosition).origin, Vector2.zero);

            if (hit.transform != null && hit.transform.gameObject.HasComponent<GemView>())
            {
                return hit.transform.GetComponent<GemView>();
            }

            return null;
        }
        
        /// <summary>
        /// OnUnblockInput event handler
        /// </summary>
        /// <param name="e"></param>
        private void OnUnblockInputHandler(IEvent e)
        {
            _isBlocked = false;
        }

        /// <summary>
        /// OnBlockInput event handler
        /// </summary>
        /// <param name="e"></param>
        private async void OnBlockInputHandler(IEvent e)
        {
            _isBlocked = true;
        }
        
        #endregion
    }