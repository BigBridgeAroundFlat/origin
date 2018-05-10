using Common.Dialog;
using Common.FrameWork.Singleton;
using Common.Scene;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class MainController : OnlyOneBehavior<MainController>
    {
        // ref ui
        [SerializeField] private GameObject LimitTouchModal;
        [SerializeField] private Text ScoreValueText;
        [SerializeField] private Text LimitTimeText;
        [SerializeField] private Button SubMenuButton;
        [SerializeField] private Image ImageStart;

        // param
        private float limitTime;
        private int scoreForGame;
        private int scoreForUi;

        public bool _isStart { get; private set; }

        // for score animation
        public int Number
        {
            set
            {
                scoreForUi = value;
                ScoreValueText.text = scoreForUi.ToString();
            }
            get
            {
                return scoreForUi;
            }
        }

        // Use this for initialization
        void Start ()
        {
            limitTime = 99.0f;
            SubMenuButton.OnClickEtension(PushSubMenuButton);
            UpdateScoreText();

            // Start演出
            var seq = DOTween.Sequence();
            {
                seq.Append(ImageStart.DOFade(1.0f, 0.3f));
                seq.AppendInterval(1.0f);
                seq.Append(ImageStart.DOFade(0.0f, 0.3f));
                seq.AppendCallback(() =>
                {
                    ImageStart.gameObject.SetActive(false);
                    _isStart = true;
                });
            }
        }

        private void Update()
        {
            if (_isStart == false)
            {
                return;
            }

            UpdateLimitTime();
        }

        public void AddScore(int value)
        {
            if (0 < value)
            {
                scoreForGame += value;
                UpdateScoreText();
            }
        }
        private void UpdateScoreText()
        {
            DOTween.To(() => Number, (x) => Number = x, scoreForGame, 1);
        }

        private void UpdateLimitTime()
        {
            limitTime -= Time.deltaTime;
            limitTime = Mathf.Max(limitTime, 0);

            var limitTimeInt = (int) limitTime;
            LimitTimeText.text = limitTimeInt.ToString();
        }

        private void PushSubMenuButton()
        {
            Time.timeScale = 0;

            var dialogInfo = new DialogUtility.DialogInfo();
            {
                dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                dialogInfo.Message = "Retry?";
                dialogInfo.OkCancelButtonCallback = (bool result) =>
                {
                    if (result)
                    {
                        LimitTouchModal.SetActive(true);
                        TransitionSceneManager.Instance.TransitionScene("Main");
                    }
                    else
                    {
                        Time.timeScale = 1.0f;
                    }
                };
            }
            DialogManager.Instance.CreateDialog(dialogInfo);
        }

        public void Gameover()
        {
            Time.timeScale = 0;

            var dialogInfo = new DialogUtility.DialogInfo();
            {
                dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                dialogInfo.Message = "Continue?";
                dialogInfo.OkCancelButtonCallback = (bool result) =>
                {
                    LimitTouchModal.SetActive(true);

                    if (result)
                    {
                        TransitionSceneManager.Instance.TransitionScene("Main");
                    }
                    else
                    {
                        TransitionSceneManager.Instance.TransitionScene("Top");
                    }
                };
            }
            DialogManager.Instance.CreateDialog(dialogInfo);
        }

        public void GameClear()
        {
            Time.timeScale = 0;

            var dialogInfo = new DialogUtility.DialogInfo();
            {
                dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                dialogInfo.Title = "Score " + scoreForGame.ToString();
                dialogInfo.Message = "Retry?";
                dialogInfo.OkCancelButtonCallback = (bool result) =>
                {
                    LimitTouchModal.SetActive(true);

                    if (result)
                    {
                        TransitionSceneManager.Instance.TransitionScene("Main");
                    }
                    else
                    {
                        TransitionSceneManager.Instance.TransitionScene("Top");
                    }
                };
            }
            DialogManager.Instance.CreateDialog(dialogInfo);
        }
    }
}
