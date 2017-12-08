using Define;
using Sound;
using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public abstract class DialogBase : MonoBehaviour
    {
        // ref
        public Image ModalImage;
        public GameObject DialogBg;
        public Animation FrameInAnimation;
        public Animation FrameOutAnimation;

        // ダイアログ情報
        protected DialogData.DialogInfo DialogInfo;
        public DialogParam.DialogType GetDialogType()
        {
            if (DialogInfo == null)
            {
                return DialogParam.DialogType.None;
            }

            return DialogInfo.DialogType;
        }

        // フェード状態
        private FadeParam.FadeState _fadeState = FadeParam.FadeState.None;

        // ボタン有効／無効
        private bool _unenableButtonFlag;

        // 抽象関数
        public abstract void PushBackKey();
        protected abstract void InitCore();

        private void OnDestroy()
        {
            var dialogManager = DialogManager.Instance;
            if (dialogManager != null)
            {
                dialogManager.NotifyCloseDialog(gameObject);
            }
        }

        public virtual void Init(DialogData.DialogInfo info)
        {
            // ModalのRGB固定：NEW_ZOO-211 ダイアログ関連の修正
            if (ModalImage != null)
            {
                ModalImage.color = new Color(0,0,0,DialogManager.Instance.GetModalTargetAlpha());

                // canvas group
                {
                    if (ModalImage.gameObject.GetComponent<CanvasGroup>() == null)
                    {
                        ModalImage.gameObject.AddComponent<CanvasGroup>();
                    }

                    ModalImage.GetComponent<CanvasGroup>().ignoreParentGroups = true;
                }
            }

            DialogInfo = info;
            InitCore();

            FrameIn();
        }

        #region modal

        public void ChangeEnableModalImage(bool enable)
        {
            if (ModalImage != null)
            {
                ModalImage.enabled = enable;
            }
        }

        #endregion

        #region button

        public void UnenableButton()
        {
            _unenableButtonFlag = true;
        }
        public bool IsEnableButton()
        {
            if (_unenableButtonFlag)
            {
                return false;
            }

            return _fadeState == FadeParam.FadeState.FadeinFinish;
        }

        #endregion

        #region frame in

        public void FrameIn(CommonData.VoidCallback callback = null)
        {
            // 後ろにダイアログ無いならモーダルのフェードアクション行う
            var createDialogListCount = DialogManager.Instance.GetCreateDialogListCount();
            if (createDialogListCount == 1)
            {
                if (ModalImage != null)
                {
                    ModalImage.color = Color.clear;
                }
                FrameInCore(callback, ModalImage);
            }
            else
            {
                FrameInCore(callback);
            }
        }
        private void FrameInCore(CommonData.VoidCallback callback, Image modalImage = null)
        {
            // check state
            {
                if (_fadeState != FadeParam.FadeState.None && _fadeState != FadeParam.FadeState.FadeoutFinish)
                {
                    return;
                }

                DialogManager.Instance.RequestChangeInputEnable(false);
                _fadeState = FadeParam.FadeState.FadeinStart;
                gameObject.SetActive(true);
            }

            // SE
            SoundManager.Instance.PlaySe("window");

            // Animator > DialogAnimation
            {
                if (FrameInAnimation != null && FrameInAnimation.clip != null)
                {
                    var animationClip = FrameInAnimation.clip;

                    // アニメーション再生
                    FrameInAnimation.Play(animationClip.name);

                    // 終了Callback
                    var seq = DOTween.Sequence();
                    {
                        seq.AppendInterval(animationClip.length);
                        seq.AppendCallback(() =>
                        {
                            FrameInFinish(callback);
                        });
                    }
                }
                else
                {
                    var dialogAnimationScript = GetComponents<DialogAnimation>().FirstOrDefault(x => x.CanPlayAnimation(false));
                    if (dialogAnimationScript != null)
                    {
                        dialogAnimationScript.SetModalImage(modalImage);
                        dialogAnimationScript.PlayDialogAnimation(() =>
                        {
                            FrameInFinish(callback);
                        });
                    }
                    else
                    {
                        FrameInDefault(callback, modalImage);
                    }
                }
            }
        }
        private void FrameInDefault(CommonData.VoidCallback callback, Image modalImage)
        {
            // dialogManagerにアタッチされているDialogAnimationをアタッチ
            var defaultDialogAnimationScript = DialogManager.Instance.GetComponents<DialogAnimation>().FirstOrDefault(x => x.CanPlayAnimation(false));
            if (defaultDialogAnimationScript != null)
            {
                var addScript = gameObject.AddComponent<DialogAnimation>();
                addScript.SetDefaultDialogAnimationSetting(DialogBg, defaultDialogAnimationScript);
                addScript.SetModalImage(modalImage);
                addScript.PlayDialogAnimation(() =>
                {
                    FrameInFinish(callback);
                });
            }
        }
        public void FrameInFinish(CommonData.VoidCallback callback)
        {
            if (callback != null)
            {
                callback();
            }

            DialogManager.Instance.RequestChangeInputEnable(true);
            _fadeState = FadeParam.FadeState.FadeinFinish;
            NotifyFrameInFinish();
        }
        protected virtual void NotifyFrameInFinish()
        {
            
        }

        #endregion

        #region frame out

        public void FrameOut(CommonData.VoidCallback callback = null)
        {
            // 後ろにダイアログ無いならモーダルのフェードアクション行う
            var createDialogListCount = DialogManager.Instance.GetCreateDialogListCount();
            if (createDialogListCount == 1)
            {
                FrameOutCore(callback, ModalImage);
            }
            else
            {
                FrameOutCore(callback);
            }
        }
        private void FrameOutCore(CommonData.VoidCallback callback, Image modalImage = null)
        {
            // check state
            {
                if (_fadeState != FadeParam.FadeState.FadeinFinish)
                {
                    return;
                }

                DialogManager.Instance.RequestChangeInputEnable(false);
                _fadeState = FadeParam.FadeState.FadeoutStart;
            }

            // SE
            SoundManager.Instance.PlaySe("cancel");

            // Animator(this or parent) > DialogAnimation(this) > Default
            {
                if (FrameOutAnimation != null && FrameOutAnimation.clip != null)
                {
                    var animationClip = FrameOutAnimation.clip;

                    // アニメーション再生
                    FrameOutAnimation.Play(animationClip.name);

                    // 終了Callback
                    var seq = DOTween.Sequence();
                    {
                        seq.AppendInterval(animationClip.length);
                        seq.AppendCallback(() =>
                        {
                            FrameOutFinish(callback);
                        });
                    }
                }
                else
                {
                    var dialogAnimationScript = GetComponents<DialogAnimation>().FirstOrDefault(x=>x.CanPlayAnimation(true));
                    if (dialogAnimationScript != null)
                    {
                        dialogAnimationScript.SetModalImage(modalImage);
                        dialogAnimationScript.PlayDialogAnimation(() =>
                        {
                            FrameOutFinish(callback);
                        });
                    }
                    else
                    {
                        FrameOutDefault(callback, modalImage);
                    }
                }
            }
        }
        private void FrameOutDefault(CommonData.VoidCallback callback, Image modalImage = null)
        {
            // dialogManagerにアタッチされているDialogAnimationをアタッチ
            var defaultDialogAnimationScript = DialogManager.Instance.GetComponents<DialogAnimation>().FirstOrDefault(x => x.CanPlayAnimation(true));
            if (defaultDialogAnimationScript != null)
            {
                var addScript = gameObject.AddComponent<DialogAnimation>();
                addScript.SetDefaultDialogAnimationSetting(DialogBg, defaultDialogAnimationScript);
                addScript.SetModalImage(modalImage);
                addScript.PlayDialogAnimation(() =>
                {
                    FrameOutFinish(callback);
                });
            }
        }
        private void FrameOutFinish(CommonData.VoidCallback callback)
        {
            if (callback != null)
            {
                callback();
            }

            DialogManager.Instance.RequestChangeInputEnable(true);
            _fadeState = FadeParam.FadeState.FadeoutFinish;

            Destroy(gameObject);
        }

        #endregion
    }
}
