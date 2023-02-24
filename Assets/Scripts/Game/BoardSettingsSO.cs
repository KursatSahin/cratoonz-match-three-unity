using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "CMatch/Board/BoardSettingsSO", fileName = nameof(BoardSettingsSO))]
    public class BoardSettingsSO : ScriptableObject
    {
        [Header("BoardSettings")]
        public int BoardWidth = 8;
        public int BoardHeight = 8;
    }
}