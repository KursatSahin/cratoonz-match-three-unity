using UnityEngine;

namespace Game.Utils
{
    /// <summary>
    /// Abstract class for making reload-proof singletons out of ScriptableObjects
    /// Returns the asset created on the editor, or null if there is none
    /// Based on Unite 2016 https://www.youtube.com/watch?v=VBA1QCoEAX4
    /// </summary>
    /// <typeparam name="T">Singleton type</typeparam>
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    //_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                    string name = typeof(T).Name;

                    _instance = Resources.Load<T>(name);
                }

                return _instance;
            }
        }
    }
}