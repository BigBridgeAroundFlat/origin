using Define;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
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
        public DialogAnimationType _dialogAnimationType = DialogAnimationType.None;
        public bool _isFrameOut;
        public Image _targetImage;
        public float _animationTime;
        public AnimationCurve _animationCurve;
        public AnimationCurve _fadeCurve;

        // animation curve
        private float _currentAnimationTime;
        private float _currentCurveRate;

        // check finish
        private bool _isUpdateEnable;
        private CommonData.VoidCallback _finishCallback;

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
            if (_targetImage != null)
            {
                _defaultPosVec = _targetImage.rectTransform.position;
                _defaultScaleVec = _targetImage.rectTransform.localScale;

                // CanvasGroup設定されてないなら設定
                if (_targetImage.gameObject.GetComponent<CanvasGroup>() == null)
                {
                    _targetImage.gameObject.AddComponent<CanvasGroup>();
                }

                // フレームインアニメーション設定されてるならActiveはfalse
                if (_isFrameOut == false)
                {
                    _targetImage.gameObject.SetActive(false);
                }
            }
        }

        // 共通アニメーション：DialogManagerから設定
        public void SetDefaultDialogAnimationSetting(GameObject dialogBg, DialogAnimation script)
        {
            // target image
            {
                _targetImage = dialogBg.GetComponent<Image>();
                _defaultPosVec = _targetImage.rectTransform.position;
                _defaultScaleVec = _targetImage.rectTransform.localScale;
                dialogBg.SetActive(false);

                // CanvasGroup設定されてないなら設定
                if (_targetImage.gameObject.GetComponent<CanvasGroup>() == null)
                {
                    _targetImage.gameObject.AddComponent<CanvasGroup>();
                }
            }

            // setting
            {
                _dialogAnimationType = script._dialogAnimationType;
                _isFrameOut = script._isFrameOut;
                _animationTime = script._animationTime;
                _animationCurve = script._animationCurve;
                _fadeCurve = script._fadeCurve;
            }
        }

        #region play animation

        public void PlayDialogAnimation(CommonData.VoidCallback callback)
        {
            _finishCallback = callback;

            _currentAnimationTime = 0;
            _currentCurveRate = 0;
            _isUpdateEnable = true;

            _targetImage.gameObject.SetActive(true);
            UpdateCore();
        }
        public bool CanPlayAnimation(bool isFrameOut)
        {
            if (_isFrameOut != isFrameOut)
            {
                return false;
            }

            if (_dialogAnimationType == DialogAnimationType.None)
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
            _currentAnimationTime += Time.deltaTime;
            _currentCurveRate = Mathf.Min(_currentAnimationTime / _animationTime, 1.0f); ;

            // アニメーション更新
            {
                UpdateFade();
                UpdateAnimation();
            }


            // 終了
            {
                if (_currentCurveRate == 1.0f)
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
                var fadeRate = _fadeCurve.Evaluate(_currentCurveRate);

                var canvasGroup = _targetImage.gameObject.GetComponent<CanvasGroup>();
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

                    var colorRate = _isFrameOut ? 1.0f - _currentCurveRate : _currentCurveRate;
                    color.a = modalAlpha * colorRate;
                    _modalImage.color = color;
                }
            }
        }
        private void UpdateAnimation()
        {
            var animationRate = _animationCurve.Evaluate(_currentCurveRate);
            switch (_dialogAnimationType)
            {
                case DialogAnimationType.Scale:
                {
                    _targetImage.rectTransform.localScale = _defaultScaleVec * animationRate;
                }
                    break;

                case DialogAnimationType.SlideX:
                {
                    var imageWidth = _targetImage.rectTransform.sizeDelta.x;
                    var addPosX = imageWidth * (animationRate - 1.0f);
                    _targetImage.rectTransform.localPosition = new Vector3(_defaultPosVec.x + addPosX, _defaultPosVec.y, _defaultPosVec.z);
                }
                    break;

                case DialogAnimationType.SlideY:
                {
                    var imageHeight = _targetImage.rectTransform.sizeDelta.y;
                    var addPosY = imageHeight * (animationRate - 1.0f);
                    _targetImage.rectTransform.localPosition = new Vector3(_defaultPosVec.x, _defaultPosVec.y + addPosY, _defaultPosVec.z);
                }
                    break;
            }
        }

        #endregion
    }
}
