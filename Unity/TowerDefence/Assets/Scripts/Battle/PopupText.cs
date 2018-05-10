using DG.Tweening;
using UnityEngine;

namespace Battle
{
    /*
     * ポップアップテキスト演出
     * ・フェードインアウト＆スケーリング
     */
    public class PopupText : MonoBehaviour
    {
        // scale
        public float ScaleTime = 0.6f;
        public float WaitTime = 0.3f;
        public Vector3 ScaleVec = Vector3.one;

        // Use this for initialization
        private void Start()
        {
            var seq = DOTween.Sequence();
            {
                seq.Append(transform.DOScale(ScaleVec, ScaleTime));
                seq.AppendInterval(WaitTime);
                seq.Append(transform.DOScale(Vector3.zero, ScaleTime));
                seq.AppendCallback(()=>Destroy(gameObject));
            }
        }
    }
}//Puzzle