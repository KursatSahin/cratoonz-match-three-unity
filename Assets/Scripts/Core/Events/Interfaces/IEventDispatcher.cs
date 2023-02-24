using Core.Services;

namespace Core.Events
{
    public delegate void EventAction(IEvent e);
    
    public interface IEventDispatcher : IService
    {
        void Subscribe(GameEventType gameEventType, EventAction listener);
        void Unsubscribe(GameEventType gameEventType, EventAction listener);
        void Fire(GameEventType gameEventType, IEvent e = null);
    }
}