using Battle.Manager;
using Common.Dialog;
using Common.FrameWork;
using Common.Other;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.UI
{
    public class CreateObjectIcon : DragImage 
    {
        // ref:Button
        [SerializeField] private Button _selectCreateIconDialogButton;

        // ref:Mana
        [SerializeField] private GameObject _consumeManaObj;
        [SerializeField] private Text _consumeManaText;

        // ref:Coin
        [SerializeField] private GameObject _consumeCoinObj;
        [SerializeField] private Text _consumeCoinText;

        // 有効か
        private bool _isEnable;

        // 建造物作成情報
        public BattleData.CreateObjectInfo CreateObjectInfo { get; private set; }

        public void Start()
        {
            _selectCreateIconDialogButton.OnClickEtension(PushSelectCreateIconDialogButton);
        }

        public void ChangeEnableSelectCreateIconDialogButton(bool enable)
        {
            _selectCreateIconDialogButton.enabled = enable;
        }
        private void PushSelectCreateIconDialogButton()
        {
            var info = new DialogUtility.DialogInfo();
            {
                info.DialogType = DialogUtility.DialogType.SelectCreateObjectIconDialog;
                info.IsStopTimeScale = true;
                info.CreateObjectInfoCallback = SetCreateObjectInfo;
            }
            DialogManager.Instance.CreateDialog(info);
        }

        public void SetCreateObjectInfo(BattleData.CreateObjectInfo info)
        {
            CreateObjectInfo = info;
            CreateObjectInfo.Affiliation = BattleParam.Affiliation.Player;

            BaseImage.sprite = BattleFunc.CalcSprite(CreateObjectInfo);
            IsOverrideDragImage = info.ObjectType != BattleParam.ObjectType.Bullet;

            UpdateConsumeManaText();
            UpdateConsumeCoinText();
        }

        // consume
        private void UpdateConsumeManaText()
        {
            var consumeMana = CreateObjectInfo.ConsumeMana;
            _consumeManaObj.SetActive(0 < consumeMana);
            _consumeManaText.text = consumeMana.ToString();
        }
        private void UpdateConsumeCoinText()
        {
            var consumeCoin = CreateObjectInfo.ConsumeCoin;
            _consumeCoinObj.SetActive(0 < consumeCoin);
            _consumeCoinText.text = consumeCoin.ToString();
        }

        // ドラッグ可能？
        protected override bool CanDrag()
        {
            return _isEnable;
        }

        // ドラッグ終了（ドロップ）時の処理
        protected override void NotifyOnEndDrag()
        {
            CreateObject();
        }

        // 建造物作成
        private void CreateObject()
        {
            var createPos = CommonUtility.ConvertWorldPosFromCanvasPos(Canvas, DraggingRectTransform);

            // ray：地面に当たってたら無効
            {
                var hit = Physics2D.Raycast(createPos, Vector3.forward, 100);
                if (hit.collider != null)
                {
                    var layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
                    if (layerName == "Ground")
                    {
                        return;
                    }
                }
// デバッグ用
#if false
                // 可視化
                Debug.DrawRay(createPos, new Vector3(0, 0, 100), Color.blue, 1);

                // コンソールにhitしたGameObjectを出力
                Debug.Log(hit.collider);
#endif
            }

            // 作成
            switch (CreateObjectInfo.ObjectType)
            {
                case BattleParam.ObjectType.Building:
                {
                    CreateObjectInfo.Pos = createPos;
                    BuildingManager.Instance.CreateObject(CreateObjectInfo);
                }
                break;

                // 画面外上部から落ちてくる
                case BattleParam.ObjectType.Bullet:
                {
                    CreateObjectInfo.Pos = BattleFunc.ConvertToCreateScreenOutPos(createPos);
                    BulletManager.Instance.CreateObject(CreateObjectInfo);
                }
                    break;

                case BattleParam.ObjectType.BattleCharacter:
                {
                    CreateObjectInfo.Pos = createPos;
                    BattleCharacterManager.Instance.CreateObject(CreateObjectInfo);
                }
                    break;
            }

            // 素材消費
            {
                BattleController.Instance.AddCoin(-CreateObjectInfo.ConsumeCoin);
                BattleController.Instance.AddMana(-CreateObjectInfo.ConsumeMana);
            }
        }

        // 状態更新：エネルギー依存
        public void UpdateFromEnergyInfo(BattleData.EnergyInfo info)
        {
            _isEnable = CreateObjectInfo.ConsumeCoin <= info.Coin && CreateObjectInfo.ConsumeMana <= info.Mana;
            var alpha = _isEnable ? 1.0f : 0.5f;
            FadeUtility.ChangeComponentAlpha(gameObject, FadeUtility.FadeTargetComponent.Image, alpha);
        }

    }
}