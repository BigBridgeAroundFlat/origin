using DG.Tweening;
using UnityEngine;

namespace Common.Scene
{
    /// <summary>
    /// 起動用Behaviour
    /// </summary>
    public class AwakeScene : MonoBehaviour
    {
        private void Start()
        {
            SceneUtility.LoadResidentScene(this, () =>
            {
                var seq = DOTween.Sequence();
                {
                    seq.AppendInterval(0.1f);
                    seq.AppendCallback(() =>
                    {
                        TransitionSceneManager.Instance.TransitionSceneImmediate("Top");
                    });
                }

            });
        }
    }
}
