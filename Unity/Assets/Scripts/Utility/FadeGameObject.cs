using System.Collections;
using Define;
using UnityEngine;


namespace Utility
{
    public class FadeGameObject : MonoBehaviour
    {
        private FadeData.FadeInfo _fadeInfo;

        public void Init(FadeData.FadeInfo info)
        {
            _fadeInfo = info;

            bool isFadeIn = false;
            {
                switch (_fadeInfo.FadeAction)
                {
                    case FadeParam.FadeAction.FadeIn:
                    case FadeParam.FadeAction.FadeInOut:
                        {
                            isFadeIn = true;
                            FadeFunc.ChangeComponentAlpha(gameObject, _fadeInfo.FadeTargetComponent, 0);
                        }
                        break;
                }
            }

            StartCoroutine(FadeUpdate(_fadeInfo.Delay, isFadeIn, () =>
            {
                switch (_fadeInfo.FadeAction)
                {
                    case FadeParam.FadeAction.FadeIn:
                    case FadeParam.FadeAction.FadeOut:
                        {
                            FadeFinish();
                        }
                        break;

                    case FadeParam.FadeAction.FadeOutIn:
                    case FadeParam.FadeAction.FadeInOut:
                        {
                            StartCoroutine(FadeUpdate(_fadeInfo.ChangeInterval, !isFadeIn, FadeFinish));
                        }
                        break;
                }
            }));
        }

        private void FadeFinish()
        {
            if (_fadeInfo.FinishCallback != null)
            {
                _fadeInfo.FinishCallback();
            }

            if (_fadeInfo.IsDestroy)
            {
                Destroy(gameObject);
            }
        }

        IEnumerator FadeUpdate(float delay, bool isFadeIn, CommonData.VoidCallback callback)
        {
            // delay
            {
                yield return new WaitForSeconds(delay);
            }

            // fade
            {
                if (isFadeIn)
                {
                    float alphaRate = 0.0f;
                    while (alphaRate < 1.0f)
                    {
                        alphaRate += _fadeInfo.AddFadeAlpha;
                        alphaRate = Mathf.Min(alphaRate, 1.0f);
                        FadeFunc.ChangeComponentAlpha(gameObject, _fadeInfo.FadeTargetComponent, alphaRate);

                        yield return null;
                    }
                }
                else
                {
                    float alphaRate = 1.0f;
                    while (alphaRate > 0)
                    {
                        alphaRate -= _fadeInfo.AddFadeAlpha;
                        alphaRate = Mathf.Max(alphaRate, 0);
                        FadeFunc.ChangeComponentAlpha(gameObject, _fadeInfo.FadeTargetComponent, alphaRate);

                        yield return null;
                    }
                }
            }

            if (callback != null)
            {
                callback();
            }
        }
    }
}
