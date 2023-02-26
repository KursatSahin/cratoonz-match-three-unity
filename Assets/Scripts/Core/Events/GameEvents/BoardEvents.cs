using Game.Board;

namespace Core.Events.GameEvents
{
    public class NewGemGeneratedEvent : IEvent
    {
        public Gem GeneratedGem { get; set; }

        public NewGemGeneratedEvent(Gem generatedGem)
        {
            GeneratedGem = generatedGem;
        }
    }
}