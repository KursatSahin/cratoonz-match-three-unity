using Core.Services;

namespace Core.Events
{
    public abstract class EventDispatcherListener : IEventDispatcherListener
    {
        protected IEventDispatcher _eventDispatcher;
        
        protected EventDispatcherListener()
        {
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }

        public virtual void SubscribeEvents() { }

        public virtual void UnsubscribeEvents() { }
    }
}