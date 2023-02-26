using System.Collections.Generic;
using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "CMatch/Containers/GemContainer", fileName = nameof(GemContainer))]
    public class GemContainer : SingletonScriptableObject<GemContainer>
    {
        public List<Sprite> Gems;
    }
}