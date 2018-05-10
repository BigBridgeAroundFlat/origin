using Common.Other;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.CreateObject
{
    public abstract class DamageObjectBase : ObjectBase
    {
        // ref
        [SerializeField] private GameObject _damageTextParent;

        // ref : HpBar
        [SerializeField] protected Slider HpBarSlider;
        private void UpdateHpBarSlider()
        {
            var hpRate = (float)CurrentHp / CreateObjectInfo.Hp;
            HpBarSlider.value = hpRate;
        }

        // Hp
        protected int CurrentHp;

        // ダメージ
        [SerializeField] protected Collider2D DamageCollider2D;
        private int _damageUniqueId;
        protected List<string> DamageTagList = new List<string>();

        // 死亡
        protected abstract void Death();

        // 被ダメージ
        public void Damage(int damageValue, bool isShowDamegeText =true)
        {
            if (damageValue == 0 || IsDead())
            {
                return;
            }

            // ダメージ
            {
                CurrentHp -= damageValue;
                CurrentHp = Mathf.Max(Mathf.Min(CurrentHp, CreateObjectInfo.Hp), 0);
                UpdateHpBarSlider();
            }

            // エフェクト：ダメージテキスト
            if(isShowDamegeText)
            {
                BattleController.Instance.CreateDamageTextEffect(damageValue, _damageTextParent);
            }

            // 死亡
            if (IsDead())
            {
                Death();
            }
        }

        // 死亡
        public bool IsDead()
        {
            return CurrentHp <= 0;
        }


        #region override

        // affiliation設定
        protected override void InitCore()
        {
            CurrentHp = CreateObjectInfo.Hp;
            UpdateHpBarSlider();
            UpdateDamageTagList();
        }
        private void UpdateDamageTagList()
        {
            DamageTagList.Clear();

            switch (CreateObjectInfo.Affiliation)
            {
                case BattleParam.Affiliation.Player: DamageTagList.Add("EnemyAttack"); break;
                case BattleParam.Affiliation.Enemy: DamageTagList.Add("PlayerAttack"); break;
            }
        }

        // 当たり判定
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // 被ダメージ対象でない or 既に死亡しているなら不要
            if (DamageTagList.Contains(other.gameObject.tag) == false || IsDead())
            {
                return;
            }

            // 所属同じならダメージ受けない
            {
                var objectBaseScript = other.gameObject.GetComponent<ObjectBase>();
                if (objectBaseScript)
                {
                    if (CreateObjectInfo.Affiliation == objectBaseScript.GetAffiliation())
                    {
                        return;
                    }
                }
            }

            // 被ダメージ処理
            {
                // 攻撃コリジョン情報
                var attackCollisionInfo = other.gameObject.GetComponent<AttackCollisionInfo>();
                if (attackCollisionInfo == null)
                {
                    Debug.Assert(false, "AttackCollisionInfo is null");
                    return;
                }

                // 同じ攻撃で連続ダメージ受けない
                if (_damageUniqueId == attackCollisionInfo.UniqueId)
                {
                    return;
                }
                _damageUniqueId = attackCollisionInfo.UniqueId;

                // ダメージ
                var damageInt = Mathf.CeilToInt(attackCollisionInfo.DamageValue);
                Damage(damageInt);
            }
        }

        #endregion

    }
}
