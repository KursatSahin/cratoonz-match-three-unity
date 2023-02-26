using UnityEngine;

namespace Utils
{
    public static class GameObjectUtils
    {
        public static bool HasComponent<T> (this GameObject obj)
        {
            return obj.GetComponent(typeof(T)) != null;
        }
    }
}