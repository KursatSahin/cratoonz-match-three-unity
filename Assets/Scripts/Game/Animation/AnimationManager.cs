using System.Collections.Generic;
using Core.Events;
using Core.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Game.Animation
{
    public class AnimationManager : IAnimationManager
    {
        #region Private Fields

        private Queue<Tween> _mainSequence = new Queue<Tween>();

        private Sequence _swapSequence;
        private Sequence _destroySequence;
        private Sequence _gravitySequence;
        private Sequence _rollbackSequence;

        private IEventDispatcher _eventDispatcher;
        
        #endregion

        #region Properties
        public bool IsPlaying
        {
            get
            {
                if (_mainSequence.Count < 1)
                    return false;
                var current = _mainSequence.Peek();
                return (current != null) && _mainSequence.Peek().IsPlaying();
            }
        }
        
        #endregion
        
        #region Publlic Functions
        
        public AnimationManager()
        {
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }

        public void Enqueue(AnimGroup animGroup, Tween tween, float startingTime = 0f)
        {
            switch (animGroup)
            {
                case AnimGroup.Swap:
                    EnqueueSwap(tween, startingTime);
                    break;
                case AnimGroup.Destroy:
                    EnqueueDestroy(tween, startingTime);
                    break;
                case AnimGroup.Gravity:
                    EnqueueGravity(tween, startingTime);
                    break;
                case AnimGroup.Rollback:
                    EnqueueRollback(tween, startingTime);
                    break;
            }
        }
        
        public void Reset()
        {
            if (_swapSequence == null && _destroySequence == null && _gravitySequence == null && _rollbackSequence == null)
            {
                return;
            }

            var sequence = DOTween.Sequence().Pause();
            if (_gravitySequence != null)
            {
                sequence.Append(_gravitySequence);
            }
            if (_destroySequence != null)
            {
                sequence.Append(_destroySequence);
            }
            if (_swapSequence != null)
            {
                sequence.Append(_swapSequence);
            }
            if (_rollbackSequence != null)
            {
                sequence.Append(_rollbackSequence);
            }
            
            _mainSequence.Enqueue(sequence);

            _gravitySequence = null;
            _destroySequence = null;
            _swapSequence = null;
            _rollbackSequence = null;
        }

        public void Play()
        {
            if (_mainSequence.Count > 0 && !_mainSequence.Peek().IsPlaying())
            {
                _mainSequence.Peek().Play().OnComplete(() =>
                {
                    _mainSequence.Dequeue();
                    if (_mainSequence.Count < 1)
                    {
                        _eventDispatcher.Fire(GameEventType.UnblockInputHandler);
                    }
                });
            }
        }

        public async UniTask Cancel()
        {
            await UniTask.Yield();
            
            if (_mainSequence.Count < 1)
                return;

            var currentSubSequence = _mainSequence.Peek();
            
            if (currentSubSequence.IsPlaying())
                return;

            currentSubSequence.Goto(float.MaxValue);
            _mainSequence.Clear();

            await UniTask.Yield();
        }

        public async UniTask Wait()
        {
            await UniTask.Yield();

            while (IsPlaying)
            {
                await UniTask.WaitUntil(() => !IsPlaying);
                // Wait for 1 more frame because animation manager runs 1 frame behind the game logic
                await UniTask.Yield();
            }
        }
        
        #endregion

        #region Private Functions

        private void EnqueueGravity(Tween tween, float startingTime = 0f)
        {
            if (_gravitySequence == null)
            {
                _gravitySequence = DOTween.Sequence().Pause();
            }

            _gravitySequence.Insert(startingTime, tween);
        }

        private void EnqueueDestroy(Tween tween, float startingTime = 0f)
        {
            if (_destroySequence == null)
            {
                _destroySequence = DOTween.Sequence().Pause();
            }

            _destroySequence.Insert(startingTime, tween);
        }

        private void EnqueueSwap(Tween tween, float startingTime = 0f)
        {
            if (_swapSequence == null)
            {
                _swapSequence = DOTween.Sequence().Pause();
            }

            _swapSequence.Insert(startingTime, tween);
        }

        private void EnqueueRollback(Tween tween, float startingTime = 0f)
        {
            if (_rollbackSequence == null)
            {
                _rollbackSequence = DOTween.Sequence().Pause();
            }
        
            _rollbackSequence.Insert(startingTime, tween);
        }
        
        #endregion
    }
}