using Core.Services;
using UnityEngine;

namespace Core.Events
{
    public abstract class MonoEventDispatcherListener : MonoBehaviour, IEventDispatcherListener
    {
        protected IEventDispatcher _eventDispatcher;
        
        protected virtual void Awake()
        {
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }

        public virtual void SubscribeEvents() { }

        public virtual void UnsubscribeEvents() { }
    }
}