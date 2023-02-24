using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "CMatch/Board/BoardSettings", fileName = nameof(BoardSettings))]
    public class BoardSettings : ScriptableObject
    {
        [Header("BoardSettings")]
        public int BoardWidth = 8;
        public int BoardHeight = 8;
    }
}