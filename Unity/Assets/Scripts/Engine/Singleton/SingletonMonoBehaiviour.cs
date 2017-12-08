using UnityEngine;

namespace Engine
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<T>();

	                if(_instance == null)
	                {
	                    GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
	                    singleton.name = "(singleton) " + typeof(T).ToString();

	                    DontDestroyOnLoad(singleton);
	                }
                }
                return _instance;
            }
        }


        protected void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                Init();
            }
        }

        protected virtual void Init()
        {
            
        }
    }
}
