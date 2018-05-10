using Battle.Manager;
using Battle.UI;
using Common.Dialog;
using Common.FrameWork.Singleton;
using Common.Scene;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Battle
{
    public class BattleController : OnlyOneBehavior<BattleController>
    {
        // ref:Button
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _giveUpButton;

        // ref:Text
        [SerializeField] private Text _stageIdText;
        [SerializeField] private Text _manaText;
        [SerializeField] private Text _coinText;
        [SerializeField] private Text _limitTimeText;

        // ref:MapObject
        [SerializeField] private GameObject _stage1MapObjectParent;
        [SerializeField] private GameObject _stage2MapObjectParent;
        [SerializeField] private GameObject _stage3MapObjectParent;

        // limit time
        private float _limitTime = BattleParam.StageLimitTime;

        // energy
        private float _currentEnergyTime = BattleParam.AddEnergyTimeInterval;

        // BattleInfoManager
        private readonly BattleData.EnergyInfo _playerEnergyInfo = new BattleData.EnergyInfo();

        // create building icon
        [SerializeField] private GameObject _createObjectIconParent;
        private List<CreateObjectIcon> _createObjectIconList;

        // effect prefab
        [SerializeField] private GameObject _popupTextEffectPrefab;

        // battle state
        public enum BattleState
        {
            None,
            SelectCreateObject,
            Play,
            Finish,
        }
        public BattleState CurrentBattleState { get; private set; }

        // enemy auto create
        private float _currentEnemyAutoActionInterval;

        // stage info
        private BattleData.StageInfo _currentStageInfo;

        private void Start()
        {
            // stage info
            {
                _currentStageInfo = BattleFunc.CalcStageDataInfo(BattleParam.CurrentStageId);
                _stageIdText.text = "STAGE-" + _currentStageInfo.StageId;

                switch (_currentStageInfo.StageId)
                {
                    case 1: _stage1MapObjectParent.SetActive(true); break;
                    case 2: _stage2MapObjectParent.SetActive(true); break;
                    case 3: _stage3MapObjectParent.SetActive(true); break;
                }
            }

            // button
            {
                _startButton.OnClickEtension(PushStartButton);
                _giveUpButton.OnClickEtension(PushGiveUpButton);
            }

            // 作成アイコン初期化
            InitCreateObjectIcon();

            UpdateCoinText();
            UpdateManaText();
            UpdateLimitTimeText();
            UpdateCreateBuildingIcon();

            // 作成オブジェクト選択
            CurrentBattleState = BattleState.SelectCreateObject;
        }

        private void InitCreateObjectIcon()
        {
            _createObjectIconList = _createObjectIconParent.Descendants().OfComponent<CreateObjectIcon>().ToList();

            var index = 0;
            foreach (var createObjectIcon in _createObjectIconList)
            {
                var createObjectInfo = new BattleData.CreateObjectInfo();
                {
                    createObjectInfo.Affiliation = BattleParam.Affiliation.Player;
                }

                var objectType = BattleParam.ObjectType.Building;
                var typeId = (int) BattleParam.BuildingType.KohakuHouse;

                // check save data
                {
                    // object type
                    {
                        var key = "ObjectType" + index;
                        if (PlayerPrefs.HasKey(key))
                        {
                            objectType = (BattleParam.ObjectType) PlayerPrefs.GetInt(key);
                        }
                    }

                    // type id
                    {
                        var key = "TypeId" + index;
                        if (PlayerPrefs.HasKey(key))
                        {
                            typeId = PlayerPrefs.GetInt(key);
                        }
                    }
                }

                switch (objectType)
                {
                    case BattleParam.ObjectType.Building: createObjectInfo = BattleFunc.GetCreateObjectInfo((BattleParam.BuildingType)typeId); break;
                    case BattleParam.ObjectType.Bullet: createObjectInfo = BattleFunc.GetCreateObjectInfo((BattleParam.BulletType)typeId); break;
                    case BattleParam.ObjectType.BattleCharacter: createObjectInfo = BattleFunc.GetCreateObjectInfo((BattleParam.BattleCharacterType)typeId); break;
                }

                createObjectIcon.SetCreateObjectInfo(createObjectInfo);
                index++;
            }
        }

        private void Update()
        {
            if (CurrentBattleState != BattleState.Play)
            {
                return;
            }

            UpdateLimitTime();
            UpdateEnergyTime();
            UpdateEnemyAction();

            // object
            {
                BuildingManager.Instance.UpdateScriptList();
                BattleCharacterManager.Instance.UpdateScriptList();
            }
        }

#region energy

        private void UpdateEnergyTime()
        {
            var isAddEnergyBonus = _limitTime < BattleParam.BonusTimeAddEnergy;

            var timeIntervalRate = isAddEnergyBonus ? BattleParam.BonusRateAddEnergy : 1.0f;
            _currentEnergyTime -= Time.deltaTime * timeIntervalRate;

            if (_currentEnergyTime <= 0)
            {
                _currentEnergyTime = BattleParam.AddEnergyTimeInterval;

                // 追加
                AddCoin(BattleParam.AddEnergyTimeValue);
                AddMana(BattleParam.AddEnergyTimeValue);
            }
        }

        public void AddCoin(int count, BattleParam.Affiliation affiliation = BattleParam.Affiliation.Player)
        {
            if (affiliation != BattleParam.Affiliation.Player)
            {
                return;
            }

            _playerEnergyInfo.Coin += count;
            UpdateCoinText();
            UpdateCreateBuildingIcon();
        }
        private void UpdateCoinText()
        {
            _coinText.text = _playerEnergyInfo.Coin.ToString();
        }

        public void AddMana(int count, BattleParam.Affiliation affiliation = BattleParam.Affiliation.Player)
        {
            if (affiliation != BattleParam.Affiliation.Player)
            {
                return;
            }

            _playerEnergyInfo.Mana += count;
            UpdateManaText();
            UpdateCreateBuildingIcon();
        }
        private void UpdateManaText()
        {
            _manaText.text = _playerEnergyInfo.Mana.ToString();
        }

        private void UpdateCreateBuildingIcon()
        {
            foreach (var createBuildingIcon in _createObjectIconList)
            {
                createBuildingIcon.UpdateFromEnergyInfo(_playerEnergyInfo);
            }
        }

#endregion

#region limit time

        private void UpdateLimitTime()
        {
            _limitTime -= Time.deltaTime;
            if (_limitTime <= 0)
            {
                _limitTime = 0;
                StageResult(false);
            }

            // UI更新
            UpdateLimitTimeText();
        }
        private void UpdateLimitTimeText()
        {
            var limitTimeInt = (int)_limitTime;
            var minute = limitTimeInt / 60;
            var second = limitTimeInt % 60;
            _limitTimeText.text = minute + ":" + second.ToString("00");
        }

        #endregion

#region enemy action

        private void UpdateEnemyAction()
        {
            var isCreateEnemyIntervalBonus = _limitTime < BattleParam.BonusTimeCreateEnemyInterval;
            var isCreateEnemyAngryBonus = _limitTime < BattleParam.BonusTimeCreateEnemyAngry;

            var timeIntervalRate = isCreateEnemyIntervalBonus ? BattleParam.BonusRateCreateEnemyInterval : 1.0f;
            _currentEnemyAutoActionInterval -= Time.deltaTime * timeIntervalRate;

            if (_currentEnemyAutoActionInterval <= 0)
            {
                _currentEnemyAutoActionInterval = Random.Range(_currentStageInfo.CreateEnemyIntervalMin, _currentStageInfo.CreateEnemyIntervalMax);

                // エネミーキャラ作成
                {
                    var createBattleCharacterType = _currentStageInfo.CreateBattleCharacterTypeList.GetAtRandom();
                    var info = BattleFunc.GetCreateObjectInfo(createBattleCharacterType);
                    {
                        info.Affiliation = BattleParam.Affiliation.Enemy;
                        info.Pos = BattleFunc.GetScreenTopRight() + Vector3.right;
                    }

                    var script = BattleCharacterManager.Instance.CreateObject(info);

                    // check angry
                    {
                        var angryRate = isCreateEnemyAngryBonus ? BattleParam.BonusRateCreateEnemyAngry : 1.0f;
                        var angryPercent = _currentStageInfo.CreateEnemyAngryPercent * angryRate;
                        var randomPercent = Random.Range(0, 100);
                        if (randomPercent < angryPercent)
                        {
                            var abnormalStatusInfo = new BattleData.AbnormalStatusInfo();
                            {
                                abnormalStatusInfo.AbnormalStatusType = BattleParam.AbnormalStatusType.Angry;
                                abnormalStatusInfo.LimitTime = 999.0f;
                                abnormalStatusInfo.ChangeSpeedRate = 2.0f;
                            }
                            script.AddAbnormalStatusInfo(abnormalStatusInfo);
                        }
                    }
                }
            }
        }

        #endregion

        #region other

        // バトル終了
        public void StageResult(bool isGameOver)
        {
            CurrentBattleState = BattleState.Finish;

            var seq = DOTween.Sequence();
            {
                seq.AppendInterval(1.0f);
                seq.AppendCallback(() =>
                {
                    Time.timeScale = 0;

                    var dialogInfo = new DialogUtility.DialogInfo();
                    {
                        dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                        dialogInfo.Message = isGameOver ? "Continue?" : "Congratulation!";
                        dialogInfo.UnenableCancelButton = !isGameOver;
                        dialogInfo.OkCancelButtonCallback = (bool isOk) =>
                        {
                            var sceneName = "Top";
                            if (isGameOver && isOk)
                            {
                                sceneName = "Battle";
                            }

                            TransitionSceneManager.Instance.TransitionScene(sceneName);
                        };
                    }
                    DialogManager.Instance.CreateDialog(dialogInfo);
                });
            }
        }

        // エフェクト作成：ダメージテキスト
        public void CreateDamageTextEffect(int damageValue, GameObject parent)
        {
            var effectObj = Instantiate(_popupTextEffectPrefab, Vector3.zero, Quaternion.identity);
            {
                effectObj.SetActive(true);
                effectObj.transform.SetParent(parent.transform, false);

                var textScript = effectObj.GetComponent<Text>();
                {
                    textScript.text = damageValue.ToString();
                    textScript.color = damageValue < 0 ? Color.white : Color.black;
                }
            }
        }

        private void PushStartButton()
        {
            // BattleState変更
            CurrentBattleState = BattleState.Play;

            // ボタン無効
            {
                _startButton.gameObject.SetActive(false);
                foreach (var createObjectIcon in _createObjectIconList)
                {
                    createObjectIcon.ChangeEnableSelectCreateIconDialogButton(false);
                }
            }

            // 編成保存
            {
                var index = 0;
                foreach (var createObjectIcon in _createObjectIconList)
                {
                    var info = createObjectIcon.CreateObjectInfo;

                    // object type
                    {
                        var key = "ObjectType" + index;
                        var value = (int)info.ObjectType;
                        PlayerPrefs.SetInt(key, value);
                    }

                    // type id
                    {
                        var key = "TypeId" + index;
                        var value = info.TypeId;
                        PlayerPrefs.SetInt(key, value);
                    }

                    index++;
                }
            }

            // 初期配置
            {
                foreach (var initCreateObjectInfo in _currentStageInfo.InitCreateObjectInfoList)
                {
                    BuildingManager.Instance.CreateObject(initCreateObjectInfo);
                }
            }
        }
        private void PushGiveUpButton()
        {
            var dialogInfo = new DialogUtility.DialogInfo();
            {
                dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                dialogInfo.Message = "Back Top Scene ?";
                dialogInfo.IsStopTimeScale = true;
                dialogInfo.OkCancelButtonCallback = (bool result) =>
                {
                    if (result)
                    {
                        TransitionSceneManager.Instance.TransitionScene("Top");
                    }
                };
            }
            DialogManager.Instance.CreateDialog(dialogInfo);
        }

#endregion
    }
}
