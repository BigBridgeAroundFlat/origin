using Battle.Manager;
using UnityEngine;

namespace Battle.CreateObject
{
    public class BuildingObject : DamageObjectBase
    {
        // cash
        private BattleParam.BuildingType _buildingType = BattleParam.BuildingType.None;

        // インターバル：アクション
        private float _currentActionInterval;

        // system
        private void OnDestroy()
        {
            if(BuildingManager.Instance) BuildingManager.Instance.RemoveScriptList(this, CreateObjectInfo.Affiliation);
        }

        // パラメータ
        protected override void InitCore()
        {
            base.InitCore();

            UpdateBuildingInfo();
        }
        private void UpdateBuildingInfo()
        {
            _buildingType = (BattleParam.BuildingType)CreateObjectInfo.TypeId;

            // Sprite
            SpriteRenderer.sprite = BattleFunc.CalcSprite(_buildingType);

            // インターバル：アクション
            _currentActionInterval = CreateObjectInfo.AutoActionInterval;

            // エネミー対応
            if (IsEnemy())
            {
                // Sprite反転
                ReverseSprite();
            }
        }

        // 自動行動
        public void UpdateAutoAction()
        {
            if (CanUpdateAutoAction() == false)
            {
                return;
            }

            _currentActionInterval -= Time.deltaTime;
            if (_currentActionInterval <= 0)
            {
                // インターバル初期化
                _currentActionInterval = CreateObjectInfo.AutoActionInterval;

                // オートアクション実行
                PlayAutoAction();

                // HP減少
                Damage(CreateObjectInfo.AutoActionReduceHp, false);
            }
        }

        private bool CanUpdateAutoAction()
        {
            if (IsFalling())
            {
                return false;
            }

            return true;
        }
        private void PlayAutoAction()
        {
            switch (_buildingType)
            {
                // キャラ生産
                case BattleParam.BuildingType.KohakuHouse:
                case BattleParam.BuildingType.TokoHouse:
                {
                    var createBattleCharacterType = GetBattleCharacterType();
                    var info = BattleFunc.GetCreateObjectInfo(createBattleCharacterType);
                    {
                        info.Affiliation = CreateObjectInfo.Affiliation;
                        info.Level = CreateObjectInfo.Level;
                        info.Pos = transform.position + CreateObjectInfo.CreatePointOffset;
                        info.CreaterGameObject = gameObject;
                    }
                        BattleCharacterManager.Instance.CreateObject(info);
                }
                    break;

                // 弾作成
                case BattleParam.BuildingType.Cannon:
                case BattleParam.BuildingType.CannonCheap:
                {
                    var createBulletType = GetBulletType();
                    var info = BattleFunc.GetCreateObjectInfo(createBulletType);
                    {
                        info.Affiliation = CreateObjectInfo.Affiliation;
                        info.Level = CreateObjectInfo.Level;
                        info.Pos = transform.position + CreateObjectInfo.CreatePointOffset;
                        info.CreaterGameObject = gameObject;
                    }
                    BulletManager.Instance.CreateObject(info);
                }
                    break;
           }
        }
        private BattleParam.BattleCharacterType GetBattleCharacterType()
        {
            switch (_buildingType)
            {
                case BattleParam.BuildingType.KohakuHouse: return BattleParam.BattleCharacterType.Kohaku;
                case BattleParam.BuildingType.TokoHouse: return BattleParam.BattleCharacterType.Toko;
            }

            Debug.Assert(false, "GetBattleCharacterType : _buildingType = " + _buildingType);
            return BattleParam.BattleCharacterType.None;
        }
        private BattleParam.BulletType GetBulletType()
        {
            switch (_buildingType)
            {
                case BattleParam.BuildingType.Cannon: return BattleParam.BulletType.CannonBullet;
                case BattleParam.BuildingType.CannonCheap: return BattleParam.BulletType.CheapCannonBullet;
            }

            Debug.Assert(false, "GetBulletType : _buildingType = " + _buildingType);
            return BattleParam.BulletType.None;
        }

        #region override

        // ダメージ
        protected override void Death()
        {
            // ダメージコライダー無効化
            DamageCollider2D.enabled = false;

            // 城破壊通知：城が破壊されたら負け
            if (_buildingType == BattleParam.BuildingType.Castle)
            {
                var isGameOver = IsEnemy() == false;
                BattleController.Instance.StageResult(isGameOver);
            }

            // 爆発エフェクト
            EffectManager.Instance.CreateEffect(BattleParam.EffectType.Explode, transform.position);

            // 破棄
            Destroy(gameObject);
        }

        #endregion
    }
}
