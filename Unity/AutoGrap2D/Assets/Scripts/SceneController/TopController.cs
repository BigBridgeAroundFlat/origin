using Common.Scene;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Top
{
    public class TopController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Start()
        {
            Debug.Log("TopController");
            _canvasGroup.alpha = 0;

            var seq = DOTween.Sequence();
            {
                seq.Append(_canvasGroup.DOFade(1.0f, 1.0f));
                seq.AppendInterval(1.0f);
                seq.Append(_canvasGroup.DOFade(0.0f, 1.0f));
                seq.AppendCallback(() =>
                {
                    TransitionSceneManager.Instance.TransitionScene("Title");
                });
            }
        }
    }
}
