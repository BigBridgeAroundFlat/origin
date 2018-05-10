using UnityEngine;

namespace Common.Other
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float destroyTime;

        // Use this for initialization
        void Start ()
        {
            if (0 < destroyTime)
            {
                DestroyCore(destroyTime);
            }
        }

        public void DestroyCore(float time)
        {
            Destroy(gameObject, time);
        }
    }
}
