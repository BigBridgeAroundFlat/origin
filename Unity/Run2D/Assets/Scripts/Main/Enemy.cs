using DG.Tweening;
using UnityEngine;

namespace Main
{
    public class Enemy : MonoBehaviour
    {

        public enum EnemyType
        {
            None = 0,

            Hone,
            Koumori,
            Obake,
        }
        [SerializeField] private EnemyType _enemyType;

        // Use this for initialization
        void Start ()
        {
            var seq = DOTween.Sequence();
            {
                seq.SetLoops(-1);
            }

            switch (_enemyType)
            {
                case EnemyType.Hone:
                {
#if false
                        var waitTime = Random.Range(0, 5.0f);
                    seq.AppendInterval(waitTime);
                    seq.Append(transform.DOJump(transform.position, 5, 1, 3.0f));
#endif
                }
                break;

                case EnemyType.Koumori:
                {
                    seq.Append(transform.DOMoveY(2.0f, 1.0f).SetEase(Ease.Linear).SetRelative());
                    seq.Append(transform.DOMoveY(-2.0f, 1.0f).SetEase(Ease.Linear).SetRelative());
                }
                break;
            }
        }
    }
}
