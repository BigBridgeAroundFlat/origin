using Common.Other;
using UnityEngine;

namespace Battle
{
    public class AttackCollisionInfo : MonoBehaviour
    {
        public float DamageValue;
        public bool IsDownAttack;
        public int UniqueId { get; private set; }

        private void OnEnable()
        {
            UniqueId = SimpleUniqueId.Id();
        }
    }
}
