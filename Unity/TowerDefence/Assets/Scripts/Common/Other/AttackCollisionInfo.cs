using UnityEngine;

namespace Common.Other
{
    public class AttackCollisionInfo : MonoBehaviour
    {
        public float DamageValue;
        public int UniqueId { get; private set; }

        private void OnEnable()
        {
            UniqueId = SimpleUniqueId.Id();
        }
    }
}
