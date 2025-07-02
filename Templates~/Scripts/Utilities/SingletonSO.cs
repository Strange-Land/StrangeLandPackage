using UnityEngine;

namespace Core.Utilities
{
    public abstract class SingletonSO<T> : ScriptableObject where T : ScriptableObject
    {
        private static T instance;
        
        private const string ResourceSubfolder = "StrangeLandSO";

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    string resourcePath = $"{ResourceSubfolder}/{typeof(T).Name}";
                    instance = Resources.Load<T>(resourcePath);

                    if (instance == null)
                    {
                        Debug.LogError($"SingletonSO<{typeof(T).Name}> not found at path: 'Resources/{resourcePath}'");
                    }
                }
                return instance;
            }
        }

        protected virtual void OnDisable()
        {
            instance = null;
        }
    }
}