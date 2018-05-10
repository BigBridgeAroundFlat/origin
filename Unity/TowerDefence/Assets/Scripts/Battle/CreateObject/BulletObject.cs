using Battle.Manager;
using UnityEngine;

namespace Battle.CreateObject
{
    public class BulletObject : ObjectBase
    {
        // cash
        private BattleParam.BulletType _bulletType = BattleParam.BulletType.None;
        private float _bulletScalerate = 1.0f;

        // system
        private void OnDestroy()
        {
            if(BulletManager.Instance) BulletManager.Instance.RemoveScriptList(this, CreateObjectInfo.Affiliation);
        }

        // 初期化
        protected override void InitCore()
        {
            UpdateBulletInfo();
        }
        private void UpdateBulletInfo()
        {
            _bulletType = (BattleParam.BulletType)CreateObjectInfo.TypeId;

            // Sprite
            SpriteRenderer.sprite = BattleFunc.CalcSprite(_bulletType);

            // Scale
            _bulletScalerate = CreateObjectInfo.AttackRange * CreateObjectInfo.AttackRange;
            transform.localScale *= _bulletScalerate;

            // Add Force
            Rigidbody2D.AddForce(CreateObjectInfo.Rigidbody2DMoveVec2);
        }

        // 効果範囲内のキャラクターに効果発動
        private void BulletActivate()
        {
            // 爆発エフェクト
            EffectManager.Instance.CreateEffect(BattleParam.EffectType.Explode, transform.position, _bulletScalerate);

            // 範囲内の全作成物に効果発動：敵味方関係無し
            {
                // プレイヤー：キャラクター
                {
                    var playerCreateObjectList = BattleCharacterManager.Instance.GetScriptList(BattleParam.Affiliation.Player);
                    foreach (var playerCreateObject in playerCreateObjectList)
                    {
                        var targetPos = playerCreateObject.gameObject.transform.position;
                        var distance = Vector2.Distance(transform.position, targetPos);
                        if (distance < CreateObjectInfo.AttackRange)
                        {
                            playerCreateObject.Damage(CreateObjectInfo.AttackDamage);
                            playerCreateObject.AddAbnormalStatusInfo(CreateObjectInfo.AbnormalStatusInfo);
                        }
                    }
                }

                // プレイヤー：建築物
                {
                    var playerCreateObjectList = BuildingManager.Instance.GetScriptList(BattleParam.Affiliation.Player);
                    foreach (var playerCreateObject in playerCreateObjectList)
                    {
                        var targetPos = playerCreateObject.gameObject.transform.position;
                        var distance = Vector2.Distance(transform.position, targetPos);
                        if (distance < CreateObjectInfo.AttackRange)
                        {
                            if (0 < CreateObjectInfo.AttackDamage)
                            {
                                playerCreateObject.Damage(CreateObjectInfo.AttackDamage);
                            }
                        }
                    }
                }

                // エネミー：キャラクター
                {
                    var enemyCreateObjectList = BattleCharacterManager.Instance.GetScriptList(BattleParam.Affiliation.Enemy);
                    foreach (var enemyCreateObject in enemyCreateObjectList)
                    {
                        var targetPos = enemyCreateObject.gameObject.transform.position;
                        var distance = Vector2.Distance(transform.position, targetPos);
                        if (distance < CreateObjectInfo.AttackRange)
                        {
                            enemyCreateObject.Damage(CreateObjectInfo.AttackDamage);
                            enemyCreateObject.AddAbnormalStatusInfo(CreateObjectInfo.AbnormalStatusInfo);
                        }
                    }
                }

                // エネミー：建築物
                {
                    var enemyCreateObjectList = BuildingManager.Instance.GetScriptList(BattleParam.Affiliation.Enemy);
                    foreach (var enemyCreateObject in enemyCreateObjectList)
                    {
                        var targetPos = enemyCreateObject.gameObject.transform.position;
                        var distance = Vector2.Distance(transform.position, targetPos);
                        if (distance < CreateObjectInfo.AttackRange)
                        {
                            if (0 < CreateObjectInfo.AttackDamage)
                            {
                                enemyCreateObject.Damage(CreateObjectInfo.AttackDamage);
                            }
                        }
                    }
                }
            }
        }

        #region override

        // 当たり判定
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == CreateObjectInfo.CreaterGameObject)
            {
                return;
            }

            // 弾効果発動
            BulletActivate();

            // 弾破棄
            Destroy(gameObject);
        }

        #endregion
    }
}
