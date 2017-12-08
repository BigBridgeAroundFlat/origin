using UnityEngine;

namespace Utility
{
    public class AutoDestroy : MonoBehaviour
    {
        public float DestroyTime = 1.0f;

        // Use this for initialization
        void Start()
        {
            Destroy(gameObject, DestroyTime);
        }
    }
}