using Game.Board;
using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "CMatch/Board/BoardSettingsContainer", fileName = nameof(BoardSettingsContainer))]
    public class BoardSettingsContainer : SingletonScriptableObject<BoardSettingsContainer>
    {
        [Header("BoardSettings")]
        public BoardSettings BoardSettings;
    }
}