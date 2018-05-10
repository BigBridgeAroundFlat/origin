using System;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Dialog
{
    public class DialogAnimation : MonoBehaviour
    {
        public enum DialogAnimationType
        {
            None = 0,

            Scale,
            SlideX,
            SlideY,
        }

        // ref : setting
        public DialogAnimationType AnimationType = DialogAnimationType.None;
        public bool IsFrameOut;
        public Image TargetImage;
        public float AnimationTime;
        public AnimationCurve AnimationCurve;
        public AnimationCurve FadeCurve;

        // animation curve
        private float _currentAnimationTime;
        private float _currentCurveRate;

        // check finish
        private bool _isUpdateEnable;
        private Action _finishCallback;

        // default
        private Vector3 _defaultPosVec = Vector3.zero;
        private Vector3 _defaultScaleVec = Vector3.zero;

        // modal
        private Image _modalImage;
        public void SetModalImage(Image modal)
        {
            _modalImage = modal;
        }

        private void Awake()
        {
            if (TargetImage != null)
            {
                _defaultPosVec = TargetImage.rectTransform.position;
                _defaultScaleVec = TargetImage.rectTransform.localScale;

                // CanvasGroup設定されてないなら設定
                if (TargetImage.gameObject.GetComponent<CanvasGroup>() == null)
                {
                    TargetImage.gameObject.AddComponent<CanvasGroup>();
                }

                // フレームインアニメーション設定されてるならActiveはfalse
                if (IsFrameOut == false)
                {
                    TargetImage.gameObject.SetActive(false);
                }
            }
        }

        // 共通アニメーション：DialogManagerから設定
        public void SetDefaultDialogAnimationSetting(GameObject dialogBg, DialogAnimation script)
        {
            // target image
            {
                TargetImage = dialogBg.GetComponent<Image>();
                _defaultPosVec = TargetImage.rectTransform.position;
                _defaultScaleVec = TargetImage.rectTransform.localScale;
                dialogBg.SetActive(false);

                // CanvasGroup設定されてないなら設定
                if (TargetImage.gameObject.GetComponent<CanvasGroup>() == null)
                {
                    TargetImage.gameObject.AddComponent<CanvasGroup>();
                }
            }

            // setting
            {
                AnimationType = script.AnimationType;
                IsFrameOut = script.IsFrameOut;
                AnimationTime = script.AnimationTime;
                AnimationCurve = script.AnimationCurve;
                FadeCurve = script.FadeCurve;
            }
        }

        #region play animation

        public void PlayDialogAnimation(Action callback)
        {
            _finishCallback = callback;

            _currentAnimationTime = 0;
            _currentCurveRate = 0;
            _isUpdateEnable = true;

            TargetImage.gameObject.SetActive(true);
            UpdateCore();
        }
        public bool CanPlayAnimation(bool isFrameOut)
        {
            if (IsFrameOut != isFrameOut)
            {
                return false;
            }

            if (AnimationType == DialogAnimationType.None)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region update

        private void Update()
        {
            if (_isUpdateEnable == false)
            {
                return;
            }

            UpdateCore();
        }
        private void UpdateCore()
        {
            // カーブレート
            _currentAnimationTime += Time.unscaledDeltaTime;
            _currentCurveRate = Mathf.Min(_currentAnimationTime / AnimationTime, 1.0f);

            // アニメーション更新
            {
                UpdateFade();
                UpdateAnimation();
            }


            // 終了
            {
                if (1.0f <= _currentCurveRate)
                {
                    if (_finishCallback != null)
                    {
                        _finishCallback();
                        _finishCallback = null;
                    }

                    _isUpdateEnable = false;
                }
            }
        }
        private void UpdateFade()
        {
            // fade
            {
                var fadeRate = FadeCurve.Evaluate(_currentCurveRate);

                var canvasGroup = TargetImage.gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = fadeRate;
                }
            }

            // modal
            {
                if (_modalImage != null)
                {
                    var modalAlpha = DialogManager.Instance.GetModalTargetAlpha();
                    var color = _modalImage.color;

                    var colorRate = IsFrameOut ? 1.0f - _currentCurveRate : _currentCurveRate;
                    color.a = modalAlpha * colorRate;
                    _modalImage.color = color;
                }
            }
        }
        private void UpdateAnimation()
        {
            var animationRate = AnimationCurve.Evaluate(_currentCurveRate);
            switch (AnimationType)
            {
                case DialogAnimationType.Scale:
                {
                    TargetImage.rectTransform.localScale = _defaultScaleVec * animationRate;
                }
                    break;

                case DialogAnimationType.SlideX:
                {
                    var imageWidth = TargetImage.rectTransform.sizeDelta.x;
                    var addPosX = imageWidth * (animationRate - 1.0f);
                    TargetImage.rectTransform.localPosition = new Vector3(_defaultPosVec.x + addPosX, _defaultPosVec.y, _defaultPosVec.z);
                }
                    break;

                case DialogAnimationType.SlideY:
                {
                    var imageHeight = TargetImage.rectTransform.sizeDelta.y;
                    var addPosY = imageHeight * (animationRate - 1.0f);
                    TargetImage.rectTransform.localPosition = new Vector3(_defaultPosVec.x, _defaultPosVec.y + addPosY, _defaultPosVec.z);
                }
                    break;
            }
        }

        #endregion
    }
}
