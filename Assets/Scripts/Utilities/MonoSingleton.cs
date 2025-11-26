using UnityEngine;

namespace Utilities
{
    public class MonoSingleton : MonoBehaviour
    {
        
    }
    public class MonoSingleton<T> : MonoSingleton where T : MonoSingleton<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    if (!_instance.IsInitialized) _instance.Init();
                    return _instance;
                }
                _instance = FindFirstObjectByType<T>();
                return _instance;
            }
        }

        protected bool IsInitialized;

        protected virtual void Init()
        {
            IsInitialized = true;
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance.IsInitialized && _instance != (T)this)
            {
                Destroy(this.gameObject);
                return;
            }
            if (!IsInitialized)
            {
                Init();
            }
        }
    }
}
