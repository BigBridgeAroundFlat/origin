using Common.Dialog;
using Common.FrameWork.Singleton;
using Common.Scene;
using DG.Tweening;
using System;
using Common.FrameWork;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class BattleController : OnlyOneBehavior<BattleController>
    {
        // ref
        [SerializeField] private Text _limitTimeText;
        [SerializeField] private Button _subMenuButton;
        [SerializeField] private Button _playerAiModeButton;
        [SerializeField] private Image _limitTouchModal;

        // ref image text
        [SerializeField] private Image _imageTextStart;
        [SerializeField] private Image _imageTextFinish;
        [SerializeField] private Image _imageTextTimeUp;

        // ref character
        [SerializeField] private Character _playerScript;
        [SerializeField] private Character _enemyScript;


        // limit time
        [SerializeField] private float _limitTime = 99.0f;

        // バトル中？
        public bool IsPlayBattle { get; private set; }

        private void Start()
        {
            // UI系初期化
            {
                _subMenuButton.OnClickEtension(PushSubMenuButton);
                _playerAiModeButton.OnClickEtension(PushPlayerAiMode);
                UpdateLimitTimeUi();
            }

            // Start演出
            DoImageTextEffect(_imageTextStart, () =>
            {
                IsPlayBattle = true;
            });
        }

        private void Update()
        {
            if (IsPlayBattle == false)
            {
                return;
            }

            // 時間経過
            {
                _limitTime -= Time.deltaTime;
                if (_limitTime <= 0)
                {
                    _limitTime = 0;
                    TimeUp();
                }

                // UI更新
                UpdateLimitTimeUi();
            }
        }
        private void UpdateLimitTimeUi()
        {
            var limitTimeInt = (int)_limitTime;
            _limitTimeText.text = limitTimeInt.ToString();
        }


#region battle result

        public void NotifyDeath(bool isEnemy)
        {
            IsPlayBattle = false;

            // animation
            {
                var targetScript = isEnemy ? _playerScript : _enemyScript;
                targetScript.AppealWin();
            }

            // text演出&ダイアログ
            {
                var seq = DOTween.Sequence();
                {
                    seq.AppendInterval(2.0f);
                    seq.AppendCallback(() =>
                    {
                        if (isEnemy)
                        {
                            GameClear();
                        }
                        else
                        {
                            GameOver();
                        }
                    });
                }
            }
        }
        private void GameOver()
        {
            DoImageTextEffect(_imageTextFinish, () =>
            {
                Time.timeScale = 0;

                var dialogInfo = new DialogUtility.DialogInfo();
                {
                    dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                    dialogInfo.Title = "Game Over";
                    dialogInfo.Message = "Continue?";
                    dialogInfo.OkCancelButtonCallback = (bool isOk) =>
                    {
                        var sceneName = isOk ? "Battle" : "Top";
                        TransitionSceneManager.Instance.TransitionScene(sceneName);
                    };
                }
                DialogManager.Instance.CreateDialog(dialogInfo);
            });
        }
        private void GameClear()
        {
            DoImageTextEffect(_imageTextFinish, () =>
            {
                Time.timeScale = 0;

                var dialogInfo = new DialogUtility.DialogInfo();
                {
                    dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                    dialogInfo.Title = "Game Clear";
                    dialogInfo.Message = "Go ChomeChome?";
                    dialogInfo.OkCancelButtonCallback = (bool isOk) =>
                    {
                        var sceneName = isOk ? "Top" : "Battle";
                        TransitionSceneManager.Instance.TransitionScene(sceneName);
                    };
                }
                DialogManager.Instance.CreateDialog(dialogInfo);
            });
        }

        private void TimeUp()
        {
            IsPlayBattle = false;
            DoImageTextEffect(_imageTextTimeUp, GameOver);
        }

#endregion

#region utility

        private void DoImageTextEffect(Image image, Action callback)
        {
            {
                _limitTouchModal.gameObject.SetActive(true);
                image.gameObject.SetActive(true);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            var seq = DOTween.Sequence();
            {
                seq.Append(image.DOFade(1.0f, 0.3f));
                seq.AppendInterval(1.0f);
                seq.Append(image.DOFade(0.0f, 0.3f));
                seq.AppendCallback(() =>
                {
                    _limitTouchModal.gameObject.SetActive(false);
                    image.gameObject.SetActive(false);

                    if (callback != null)
                    {
                        callback();
                    }
                });
            }
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
                        IsPlayBattle = false;
                        TransitionSceneManager.Instance.TransitionScene("Battle");
                    }
                    else
                    {
                        Time.timeScale = 1.0f;
                    }
                };
            }
            DialogManager.Instance.CreateDialog(dialogInfo);
        }
        private void PushPlayerAiMode()
        {
            GameInfoManager.IsPlayerAiMode = !GameInfoManager.IsPlayerAiMode;

            var alpha = GameInfoManager.IsPlayerAiMode ? 1.0f : 0.5f;
            FadeUtility.ChangeComponentAlpha(_playerAiModeButton.gameObject, FadeUtility.FadeTargetComponent.Image, alpha);
        }

        #endregion
    }
}
