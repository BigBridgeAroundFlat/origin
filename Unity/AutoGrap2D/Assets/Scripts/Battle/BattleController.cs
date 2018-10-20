using Common.Dialog;
using Common.FrameWork.Singleton;
using Common.Scene;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Linq;
using System.Linq;

namespace Battle
{
    public class BattleController : OnlyOneBehavior<BattleController>
    {
        // ref
        [SerializeField] private Text _limitTimeText;
        [SerializeField] private Button _subMenuButton;
        [SerializeField] private Button _speedUpButton;
        [SerializeField] private Image _limitTouchModal;

        // ref image text
        [SerializeField] private Image _imageTextStart;
        [SerializeField] private Image _imageTextFinish;
        [SerializeField] private Image _imageTextTimeUp;

        // ref character
        [SerializeField] private PlayerUI _playerUi;
        [SerializeField] private Enemy _enemyScript;

        // limit time
        [SerializeField] private float _limitTime = 99.0f;

        // バトル中？
        public bool IsPlayBattle { get; private set; }

        // ノベルシーン
        private AsyncOperation _novelSceneAsyncOperation = null;
        [SerializeField] private Image _fuckImage;

        private void Start()
        {
            // ノベルシーン作成
            {
                _novelSceneAsyncOperation = SceneManager.LoadSceneAsync("Novel", LoadSceneMode.Additive);
                _novelSceneAsyncOperation.allowSceneActivation = false;
            }

            // UI系初期化
            {
                _subMenuButton.OnClickEtension(PushSubMenuButton);
                _speedUpButton.OnClickEtension(PushSpeedUpButton);
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

        public void NotifyDeath(bool isEnemy, GameObject obj)
        {
            if (isEnemy == false)
            {
                _playerUi.NotifyDeathMonster(obj);
                if (_playerUi.CheckGameOver() == false)
                {
                    return;
                }
            }

            IsPlayBattle = false;
            StageResultWait(isEnemy);
        }
        private void StageResultWait(bool isGameClear)
        {
            if (_enemyScript.CanStageResult() == false)
            {
                _enemyScript.StopMove();
            }

            var playerList = _playerUi.GetPlayerScriptList();
            foreach (var player in playerList)
            {
                if(player.CanStageResult() == false)
                {
                    player.StopMove();
                }
            }

            if (isGameClear)
            {
                GameClear();
            }
            else
            {
                GameOver();
            }
        }
        private void GameOver()
        {
            _enemyScript.AppealWin();

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
                        StartCoroutine(ChangeScene(sceneName));
                    };
                }
                DialogManager.Instance.CreateDialog(dialogInfo);
            });
        }
        private void GameClear()
        {
            // set novel info
            {
                var novelInfo = new GameInfoManager.NovelInfo();
                {
                    novelInfo.Type = GameInfoManager.NovelInfo.NovelType.Special;
                    novelInfo.No = 1;
                    novelInfo.IsSceneView = false;
                }
                GameInfoManager.SetCurrentNovelInfo(novelInfo);
            }

            // 移動＆アニメーション
            {
                var playerList = _playerUi.GetPlayerScriptList();
                foreach(var player in playerList)
                {
                    player.ChangeGravityEnable(false);
                }
                _enemyScript.ChangeGravityEnable(false);
                _fuckImage.gameObject.SetActive(true);

                var seq = DOTween.Sequence();
                {
                    seq.Append(playerList[0].gameObject.transform.DOMove(new Vector3(-6.0f, -1.5f, 0), 1.0f));
                    seq.Join(_enemyScript.gameObject.transform.DOMove(new Vector3(-3.0f, -1.5f, 0), 1.0f));
                    seq.Join(_fuckImage.DOFade(1.0f, 1.0f));
                    seq.AppendCallback(() =>
                    {
                        playerList[0].AppealWin();
                        _enemyScript.AppealDown();
                        _novelSceneAsyncOperation.allowSceneActivation = true;
                    });
                }
            }
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
            IsPlayBattle = false;

            var dialogInfo = new DialogUtility.DialogInfo();
            {
                dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                dialogInfo.Message = "Retry?";
                dialogInfo.OkCancelButtonCallback = (bool result) =>
                {
                    Time.timeScale = 1.0f;

                    if (result)
                    {
                        StartCoroutine(ChangeScene("Battle"));
                    }
                    else
                    {
                        IsPlayBattle = true;
                    }
                };
            }
            DialogManager.Instance.CreateDialog(dialogInfo);
        }
        private void PushSpeedUpButton()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            GameInfoManager.IsSpeedUp = !GameInfoManager.IsSpeedUp;

            var alpha = GameInfoManager.IsSpeedUp ? 1.0f : 0.5f;
            _speedUpButton.GetComponent<CanvasGroup>().alpha = alpha;

            // 速度変更
            Time.timeScale = GameInfoManager.IsSpeedUp ? 3.0f : 1.0f;
        }

        System.Collections.IEnumerator ChangeScene(string sceneName)
        {
            if(_novelSceneAsyncOperation.isDone == false)
            {
                _novelSceneAsyncOperation.allowSceneActivation = true;
                yield return null;

                SceneManager.UnloadSceneAsync("Novel");
            }

            TransitionSceneManager.Instance.TransitionScene(sceneName);
        }
        #endregion
    }
}
