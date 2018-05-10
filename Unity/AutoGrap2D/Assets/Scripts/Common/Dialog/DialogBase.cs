using System;
using System.Collections;
using System.Linq;
using Common.FrameWork;
using Common.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Dialog
{
    public abstract class DialogBase : MonoBehaviour
    {
        // ref
        public Image ModalImage;
        public GameObject DialogBg;
        public Animator FrameInAnimator;
        public Animator FrameOutAnimator;

        // ダイアログ情報
        protected DialogUtility.DialogInfo DialogInfo;
        public DialogUtility.DialogType GetDialogType()
        {
            if (DialogInfo == null)
            {
                return DialogUtility.DialogType.None;
            }

            return DialogInfo.DialogType;
        }

        // フェード状態
        private FadeUtility.FadeState _fadeState = FadeUtility.FadeState.None;

        // ボタン有効／無効
        private bool _unenableButtonFlag;

        // 抽象関数
        public abstract void PushBackKey();
        protected abstract void InitCore();

        public virtual void Init(DialogUtility.DialogInfo info)
        {
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
        private void OnDestroy()
        {
            NotifyCloseDialog();
        }
        private void NotifyCloseDialog()
        {
            var dialogManager = DialogManager.Instance;
            if (dialogManager != null)
            {
                dialogManager.NotifyCloseDialog(gameObject);
            }
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

            return _fadeState == FadeUtility.FadeState.FadeinFinish;
        }

        #endregion

        #region frame in

        public void FrameIn(Action callback = null)
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
        private void FrameInCore(Action callback, Image modalImage = null)
        {
            // check state
            {
                if (_fadeState != FadeUtility.FadeState.None && _fadeState != FadeUtility.FadeState.FadeoutFinish)
                {
                    return;
                }

                _fadeState = FadeUtility.FadeState.FadeinStart;
                gameObject.SetActive(true);
            }

            // SE
            SoundManager.Instance.PlaySe("window");

            // Animator > DialogAnimation
            {
                if (FrameInAnimator != null)
                {
                    FrameInAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

                    StartCoroutine(WaitAnimatorFinish(FrameInAnimator, () =>
                    {
                        FrameInFinish(callback);
                    }));
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
        private void FrameInDefault(Action callback, Image modalImage)
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
        public void FrameInFinish(Action callback)
        {
            if (callback != null)
            {
                callback();
            }

            _fadeState = FadeUtility.FadeState.FadeinFinish;
            NotifyFrameInFinish();
        }
        protected virtual void NotifyFrameInFinish()
        {
            
        }

#endregion

#region frame out

        public void FrameOut(Action callback = null)
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
        private void FrameOutCore(Action callback, Image modalImage = null)
        {
            // check state
            {
                if (_fadeState != FadeUtility.FadeState.FadeinFinish)
                {
                    return;
                }

                _fadeState = FadeUtility.FadeState.FadeoutStart;
            }

            // SE
            SoundManager.Instance.PlaySe("cancel");

            // Animator(this or parent) > DialogAnimation(this) > Default
            {
                if (FrameOutAnimator != null)
                {
                    FrameOutAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

                    StartCoroutine(WaitAnimatorFinish(FrameOutAnimator, () =>
                    {
                        FrameOutFinish(callback);
                    }));
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
        private void FrameOutDefault(Action callback, Image modalImage = null)
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
        private void FrameOutFinish(Action callback)
        {
            if (callback != null)
            {
                callback();
            }

            _fadeState = FadeUtility.FadeState.FadeoutFinish;

            Destroy(gameObject);
        }

#endregion

#region utility

        /*
         * Animator終了検知
         */
        IEnumerator WaitAnimatorFinish(Animator checkAnimator, Action callback)
        {
            yield return null;
            yield return new WaitForAnimation(checkAnimator, 0);

            if (callback != null)
            {
                callback();
            }
        }

#endregion
    }
}
