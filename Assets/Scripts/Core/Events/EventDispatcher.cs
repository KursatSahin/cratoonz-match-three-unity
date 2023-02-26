using System;
using System.Collections.Generic;

namespace Core.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<GameEventType, EventAction> _eventDictionary;

        public EventDispatcher()
        {
            _eventDictionary = new Dictionary<GameEventType, EventAction>();
        }

        /// <summary>
        /// Subscribe to an event
        /// </summary>
        /// <param name="gameEventType"></param>
        /// <param name="listener"></param>
        public void Subscribe(GameEventType gameEventType, EventAction listener)
        {
            if (_eventDictionary.TryGetValue(gameEventType, out EventAction listeners))
            {
                listeners += listener;
            }
            else
            {
                _eventDictionary.Add(gameEventType, listener);
            }
        }

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        /// <param name="gameEventType"></param>
        /// <param name="listener"></param>
        public void Unsubscribe(GameEventType gameEventType, EventAction listener)
        {
            if (_eventDictionary.TryGetValue(gameEventType, out EventAction listeners))
            {
                listeners -= listener;
                
                if (listeners == null)
                {
                    _eventDictionary.Remove(gameEventType);
                }
            }
        }

        /// <summary>
        /// Fire an event
        /// </summary>
        /// <param name="gameEventType"></param>
        /// <param name="e"></param>
        public void Fire(GameEventType gameEventType, IEvent e = null)
        {
            if (_eventDictionary.TryGetValue(gameEventType, out EventAction listeners))
            {
                listeners.Invoke(e);
            }
        }
    }
}