using System;
using UnityEngine;

namespace Battle.CreateObject
{
    public abstract class ObjectBase : MonoBehaviour
    {
        // 作成情報
        protected BattleData.CreateObjectInfo CreateObjectInfo;
        public BattleParam.Affiliation GetAffiliation(){ return CreateObjectInfo.Affiliation; }

        // cash
        protected SpriteRenderer SpriteRenderer;
        protected Rigidbody2D Rigidbody2D;

        protected virtual void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // 初期化
        public virtual void Init(BattleData.CreateObjectInfo info)
        {
            CreateObjectInfo = info;
            InitCore();
        }
        protected abstract void InitCore();

        // 当たり判定
        protected abstract void OnTriggerEnter2D(Collider2D other);

        #region utility

        // 落下中？
        protected bool IsFalling()
        {
            return 0.01f < Math.Abs(Rigidbody2D.velocity.y);
        }

        // 見える（描画されてる）？
        public bool IsVisible()
        {
            return SpriteRenderer.isVisible;
        }

        // エネミー？
        protected bool IsEnemy()
        {
            return CreateObjectInfo.Affiliation == BattleParam.Affiliation.Enemy;
        }

        // Sprite反転
        protected void ReverseSprite()
        {
            Vector2 temp = transform.localScale;
            if (0 < temp.x)
            {
                temp.x *= -1;
                transform.localScale = temp;
            }
        }

        #endregion

    }
}
