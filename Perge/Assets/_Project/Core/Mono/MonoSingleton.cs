using UnityEngine;

namespace Core.Mono
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        
        public static void Prepare()
        {
            GetInstance();
        }

        public static T Instance => GetInstance();

        public static T GetInstance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var gameObject = new GameObject(typeof(T).ToString());
                    gameObject.hideFlags = HideFlags.HideInHierarchy;
                    
                    _instance = gameObject.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}