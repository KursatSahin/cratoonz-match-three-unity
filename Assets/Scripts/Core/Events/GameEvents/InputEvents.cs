using Game.Board;
using Game.Board.Views;
using UnityEngine;

namespace Core.Events.GameEvents
{
    public class SwipeInputEvent : IEvent
    {
        public GemView OriginGemView { get; set; }
        public Point Direction { get; set; }

        public SwipeInputEvent(GemView originGemView, Point direction)
        {
            OriginGemView = originGemView;
            Direction = direction;
        }
    }

    public class TapInputEvent : IEvent
    {
        public GemView OriginGemView { get; set; }

        public TapInputEvent(GemView originGemView)
        {
            OriginGemView = originGemView;
        }
    }
}