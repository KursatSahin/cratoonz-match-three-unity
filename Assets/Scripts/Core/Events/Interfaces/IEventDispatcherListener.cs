namespace Core.Events
{
    public interface IEventDispatcherListener
    {
        void SubscribeEvents();
        void UnsubscribeEvents();
    }
}