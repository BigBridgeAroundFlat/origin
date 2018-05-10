using Common.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.UI
{
    public class SelectCreateObjectIconDialog : DialogBase
    {
        // ref
        [SerializeField] private GameObject _selectCreateIconPrefab;
        [SerializeField] private GameObject _selectCreateIconParent;
        [SerializeField] private Button _closeButton;

        private void Start()
        {
            _closeButton.OnClickEtension(PusgCloseButton);
        }
        public override void PushBackKey()
        {
            PusgCloseButton();
        }

        protected override void InitCore()
        {
            // building
            CreateBuildingIcon(BattleParam.BuildingType.KohakuHouse);
            CreateBuildingIcon(BattleParam.BuildingType.TokoHouse);
            CreateBuildingIcon(BattleParam.BuildingType.Cannon);
            CreateBuildingIcon(BattleParam.BuildingType.CannonCheap);
            CreateBuildingIcon(BattleParam.BuildingType.Wall);
            CreateBuildingIcon(BattleParam.BuildingType.WallCheap);

            // bullet
            CreateBulletIcon(BattleParam.BulletType.DropBomb);
            CreateBulletIcon(BattleParam.BulletType.RecoveryBomb);
            CreateBulletIcon(BattleParam.BulletType.PoisonBomb);
            CreateBulletIcon(BattleParam.BulletType.SpeedUpBomb);
            CreateBulletIcon(BattleParam.BulletType.SpeedDownBomb);

            // character
            CreateCharacterIcon(BattleParam.BattleCharacterType.Kohaku);
            CreateCharacterIcon(BattleParam.BattleCharacterType.Toko);
        }

        private void CreateBuildingIcon(BattleParam.BuildingType type)
        {
            var obj = Instantiate(_selectCreateIconPrefab, _selectCreateIconParent.transform);
            {
                obj.SetActive(true);
                obj.GetComponent<SelectCreateObjectIcon>().Init(type, PushSelectIconButton);
            }
        }
        private void CreateBulletIcon(BattleParam.BulletType type)
        {
            var obj = Instantiate(_selectCreateIconPrefab, _selectCreateIconParent.transform);
            {
                obj.SetActive(true);
                obj.GetComponent<SelectCreateObjectIcon>().Init(type, PushSelectIconButton);
            }
        }
        private void CreateCharacterIcon(BattleParam.BattleCharacterType type)
        {
            var obj = Instantiate(_selectCreateIconPrefab, _selectCreateIconParent.transform);
            {
                obj.SetActive(true);
                obj.GetComponent<SelectCreateObjectIcon>().Init(type, PushSelectIconButton);
            }
        }

        private void PusgCloseButton()
        {
            if (IsEnableButton() == false)
            {
                return;
            }

            FrameOut();
        }

        private void PushSelectIconButton(BattleData.CreateObjectInfo info)
        {
            if (IsEnableButton() == false)
            {
                return;
            }

            if (DialogInfo.CreateObjectInfoCallback != null)
            {
                DialogInfo.CreateObjectInfoCallback(info);
            }

            FrameOut();
        }
    }
}