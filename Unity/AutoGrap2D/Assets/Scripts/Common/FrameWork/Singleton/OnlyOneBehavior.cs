using UnityEngine;

namespace Common.FrameWork.Singleton
{
    /*
     * （シーン上に）一つしか存在させたくないクラス
     * ・シングルトンと異なり、シーン遷移したら破棄される
     */ 
    public class OnlyOneBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // 新しいの作成された場合は、古い物を破棄
            if (_instance != null)
            {
                Destroy(this);
            }

            _instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}
