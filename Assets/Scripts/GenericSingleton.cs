using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Utilities
{
    public class GenericSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = FindObjectOfType<T>(true);

                if (instance != null) return instance;

                var obj = new GameObject { name = typeof(T).Name };
                instance = obj.AddComponent<T>();
                return instance;
            }
        }

        public void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                //Destroy(gameObj);
            }
        }
        
    }
}
