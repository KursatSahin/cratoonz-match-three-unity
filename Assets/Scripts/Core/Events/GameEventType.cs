namespace Core.Events
{
    public enum GameEventType : ushort
    {
        
        
        //Input Events
        BlockInputHandler,
        UnblockInputHandler,
        SwipeInputDetected,
        LongPressInputDetected,
        TapInputDetected,
        
        //Board Events
        NewGemGenerated,
    }
}