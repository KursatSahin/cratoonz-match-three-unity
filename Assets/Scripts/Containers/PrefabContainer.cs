using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "CMatch/Containers/PrefabContainer", fileName = nameof(PrefabContainer))]
    public class PrefabContainer : SingletonScriptableObject<PrefabContainer>
    {
        public GameObject GemPrefab;
        public GameObject TilePrefab;
    }
}