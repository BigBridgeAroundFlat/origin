using UnityEngine;
using UnityEngine.UI;

namespace Battle.UI
{
    public class SelectCreateObjectIcon : MonoBehaviour 
    {
        // ref:button
        [SerializeField] private Image _selectIconImage;
        [SerializeField] private Button _selectButton;

        // ref:Mana
        [SerializeField] private GameObject _consumeManaObj;
        [SerializeField] private Text _consumeManaText;

        // ref:Coin
        [SerializeField] private GameObject _consumeCoinObj;
        [SerializeField] private Text _consumeCoinText;

        // 作成物情報
        public BattleData.CreateObjectInfo CreateObjectInfo { get; private set; }
        public delegate void CreateObjectInfoCallback(BattleData.CreateObjectInfo info);

        public void Init(BattleParam.BuildingType type, CreateObjectInfoCallback callback)
        {
            CreateObjectInfo = BattleFunc.GetCreateObjectInfo(type);
            InitCore(callback);
        }
        public void Init(BattleParam.BulletType type, CreateObjectInfoCallback callback)
        {
            CreateObjectInfo = BattleFunc.GetCreateObjectInfo(type);
            InitCore(callback);
        }
        public void Init(BattleParam.BattleCharacterType type, CreateObjectInfoCallback callback)
        {
            CreateObjectInfo = BattleFunc.GetCreateObjectInfo(type);
            InitCore(callback);
        }
        private void InitCore(CreateObjectInfoCallback callback)
        {
            _selectIconImage.sprite = BattleFunc.CalcSprite(CreateObjectInfo);
            _selectButton.OnClickEtension(() =>
            {
                if (callback != null)
                {
                    callback(CreateObjectInfo);
                }
            });

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
    }
}