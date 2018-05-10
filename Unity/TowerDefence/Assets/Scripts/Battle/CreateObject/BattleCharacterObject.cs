using Battle.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.CreateObject
{
    public class BattleCharacterObject : DamageObjectBase
    {
        // ref 
        public GameObject AttackCollision;

        // ref : Abnormal Status Icon
        public GameObject PoisonStatusIcon;
        public GameObject SpeedUpStatusIcon;
        public GameObject SpeedDownStatusIcon;
        public GameObject AngryStatusIcon;

        // simple animation
        [SerializeField] private SimpleAnimation _battleCharacterSimpleAnimation;
        [SerializeField] private BattleParam.CharacterState _currentCharacterState = BattleParam.CharacterState.Idle;
        private void ChangeCharacterState(BattleParam.CharacterState state)
        {
            if (_currentCharacterState == state)
            {
                return;
            }

            _currentCharacterState = state;
            _battleCharacterSimpleAnimation.Play(_currentCharacterState.ToString());
        }

        // cash
        private BattleParam.BattleCharacterType _battleCharacterType = BattleParam.BattleCharacterType.None;
        private readonly List<BattleParam.CharacterState> _randomAttackActionStateList = new List<BattleParam.CharacterState>();

        // 状態異常
        private readonly List<BattleData.AbnormalStatusInfo> _abnormalStatusInfoList = new List<BattleData.AbnormalStatusInfo>();

        // 速度：移動速度＆アニメーション速度
        private float _currentSpeedrate = 1.0f;
        private void UpdateSpeedRate()
        {
            _currentSpeedrate = 1.0f;

            foreach (var abnormalStatusInfo in _abnormalStatusInfoList)
            {
                _currentSpeedrate *= abnormalStatusInfo.ChangeSpeedRate;
            }

            _battleCharacterSimpleAnimation.animator.speed = _currentSpeedrate;
        }


        // Ai
        private enum AiActionType
        {
            Idle,
            Move,
            Fall,
            Attack,
        }

        // system
        private void OnDestroy()
        {
            if (BattleCharacterManager.Instance) BattleCharacterManager.Instance.RemoveScriptList(this, CreateObjectInfo.Affiliation);
        }

        // 初期化
        protected override void InitCore()
        {
            base.InitCore();

            UpdateBattleCharacterInfo();
        }
        private void UpdateBattleCharacterInfo()
        {
            _battleCharacterType = (BattleParam.BattleCharacterType)CreateObjectInfo.TypeId;

            // ランダムアクション
            {
                _randomAttackActionStateList.Clear();

                switch (_battleCharacterType)
                {
                    case BattleParam.BattleCharacterType.Kohaku:
                    {
                        _randomAttackActionStateList.Add(BattleParam.CharacterState.Attack1);
                        _randomAttackActionStateList.Add(BattleParam.CharacterState.Attack2);
                        _randomAttackActionStateList.Add(BattleParam.CharacterState.Attack3);
                    }
                        break;

                    default:
                        _randomAttackActionStateList.Add(BattleParam.CharacterState.Attack1);
                        break;
                }
            }

            // エネミー対応
            if (IsEnemy())
            {
                // Sprite反転
                ReverseSprite();

                // 真っ黒
                SpriteRenderer.color = Color.black;

                // AttackCollisionのTag,Layer変更
                AttackCollision.tag = "EnemyAttack";
                AttackCollision.layer = LayerMask.NameToLayer("EnemyAttack");
            }
        }

        #region ai action

        private bool CanUpdateAi()
        {
            // 死亡
            if (IsDead())
            {
                return false;
            }

            // 状態
            switch (_currentCharacterState)
            {
                case BattleParam.CharacterState.Idle:
                case BattleParam.CharacterState.Walk:
                case BattleParam.CharacterState.Fall:
                    break;

                default:
                {
                    // アニメーション中はAI思考しない：行動中断しない
                    var targetState = _battleCharacterSimpleAnimation.GetState(_currentCharacterState.ToString());
                    if (targetState.enabled)
                    {
                        return false;
                    }

                    // アニメーション終了してたら待機状態に移行してAI思考
                    ChangeCharacterState(BattleParam.CharacterState.Idle);
                }
                    break;

            }

            return true;
        }
        public void UpdateAi()
        {
            if (CanUpdateAi() == false)
            {
                return;
            }

            var aiActionType = CalcAiAiActionType();
            switch (aiActionType)
            {
                case AiActionType.Idle: AiActionIdle(); break;
                case AiActionType.Move: AiActionMove(); break;
                case AiActionType.Fall: AiActionFall(); break;
                case AiActionType.Attack: AiActionAttack(); break;
            }
        }
        private AiActionType CalcAiAiActionType()
        {
            // 落下中
            if (IsFalling())
            {
                return AiActionType.Fall;
            }

            // 敵対所属の作成物全チェック
            var targetAffiliation = IsEnemy() ? BattleParam.Affiliation.Player : BattleParam.Affiliation.Enemy;
            var isExistTarget = false;


            // キャラクター
            {
                var targetScriptList = BattleCharacterManager.Instance.GetScriptList(targetAffiliation);
                foreach (var targetScript in targetScriptList)
                {
                    // 死亡演出は無視
                    if (targetScript.IsDead())
                    {
                        continue;
                    }

                    // ターゲット
                    if (targetScript.IsVisible() == false)
                    {
                        continue;
                    }
                    isExistTarget = true;

                    // 攻撃
                    var targetPos = targetScript.gameObject.transform.position;
                    var distanceX = Mathf.Abs(transform.position.x - targetPos.x);
                    if (distanceX < CreateObjectInfo.AttackRange)
                    {
                        var distanceY = Mathf.Abs(transform.position.y - targetPos.y);
                        if (distanceY < BattleParam.BattleCharacterAttackHeightRange)
                        {
                            return AiActionType.Attack;
                        }
                    }
                }
            }

            // 建造物
            {
                var targetScriptList = BuildingManager.Instance.GetScriptList(targetAffiliation);
                foreach (var targetScript in targetScriptList)
                {
                    // 死亡演出は無視
                    if (targetScript.IsDead())
                    {
                        continue;
                    }

                    // ターゲット
                    if (targetScript.IsVisible() == false)
                    {
                        continue;
                    }
                    isExistTarget = true;

                    // 攻撃
                    var targetPos = targetScript.gameObject.transform.position;
                    var distanceX = Mathf.Abs(transform.position.x - targetPos.x);
                    if (distanceX < CreateObjectInfo.AttackRange)
                    {
                        var distanceY = Mathf.Abs(transform.position.y - targetPos.y);
                        if (distanceY < BattleParam.BattleCharacterAttackHeightRange)
                        {
                            return AiActionType.Attack;
                        }
                    }
                }
            }

            return isExistTarget ? AiActionType.Move : AiActionType.Idle;
        }
        private void AiActionAttack()
        {
            var randomActionState = _randomAttackActionStateList.GetAtRandom();
            ChangeCharacterState(randomActionState);
        }
        private void AiActionMove()
        {
            // 移動
            var moveX = IsEnemy() ? -CreateObjectInfo.Rigidbody2DMoveVec2.x : CreateObjectInfo.Rigidbody2DMoveVec2.x;
            moveX *= _currentSpeedrate;
            Rigidbody2D.velocity = new Vector2(moveX, Rigidbody2D.velocity.y);

            // Animation
            ChangeCharacterState(BattleParam.CharacterState.Walk);
        }
        private void AiActionFall()
        {
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);

            // Animation
            ChangeCharacterState(BattleParam.CharacterState.Fall);
        }
        private void AiActionIdle()
        {
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);

            // Animation
            ChangeCharacterState(BattleParam.CharacterState.Idle);
        }

        #endregion

        #region abnormal

        public void AddAbnormalStatusInfo(BattleData.AbnormalStatusInfo info)
        {
            if (info.AbnormalStatusType == BattleParam.AbnormalStatusType.None)
            {
                return;
            }

            // 同種ステータス：上書き
            _abnormalStatusInfoList.RemoveAll(x => x.AbnormalStatusType == info.AbnormalStatusType);
            _abnormalStatusInfoList.Add(new BattleData.AbnormalStatusInfo(info));
            UpdateAbnormalStatusIcon();
        }
        private void UpdateAbnormalStatusIcon()
        {
            // 全部false
            {
                PoisonStatusIcon.SetActive(false);
                SpeedUpStatusIcon.SetActive(false);
                SpeedDownStatusIcon.SetActive(false);
                AngryStatusIcon.SetActive(false);
            }

            var max = _abnormalStatusInfoList.Count;
            for (var i = 0; i < max; i++)
            {
                var abnormalStatusInfo = _abnormalStatusInfoList[i];
                switch (abnormalStatusInfo.AbnormalStatusType)
                {
                    case BattleParam.AbnormalStatusType.Poison: PoisonStatusIcon.SetActive(true); break;
                    case BattleParam.AbnormalStatusType.SpeedUp: SpeedUpStatusIcon.SetActive(true); break;
                    case BattleParam.AbnormalStatusType.SpeedDown: SpeedDownStatusIcon.SetActive(true); break;
                    case BattleParam.AbnormalStatusType.Angry: AngryStatusIcon.SetActive(true); break;
                }
            }
        }

        public void UpdateAbnormalStatus()
        {
            if (_abnormalStatusInfoList.IsEmpty())
            {
                return;
            }

            var max = _abnormalStatusInfoList.Count;
            for (var i = 0; i < max; i++)
            {
                var abnormalStatusInfo = _abnormalStatusInfoList[i];
                abnormalStatusInfo.LimitTime -= Time.deltaTime;

                switch (abnormalStatusInfo.AbnormalStatusType)
                {
                    // 一定時間毎にダメージ
                    case BattleParam.AbnormalStatusType.Poison:
                    {
                        abnormalStatusInfo.DamageInterval -= Time.deltaTime;
                        if (abnormalStatusInfo.DamageInterval <= 0)
                        {
                            abnormalStatusInfo.DamageInterval = abnormalStatusInfo.DamageIntervalMax;
                            Damage(abnormalStatusInfo.DamageValue);
                        }
                    }
                        break;

                    // スピード変更
                    case BattleParam.AbnormalStatusType.SpeedUp:
                    case BattleParam.AbnormalStatusType.SpeedDown:
                    case BattleParam.AbnormalStatusType.Angry:
                    {
                        if (abnormalStatusInfo.IsUpdateSpeedRate == false)
                        {
                            abnormalStatusInfo.IsUpdateSpeedRate = true;
                            UpdateSpeedRate();
                        }
                    }
                    break;

                }
            }

            // 終了した状態異常削除
            if (_abnormalStatusInfoList.RemoveAll(x => x.IsFinish()) != 0)
            {
                UpdateAbnormalStatusIcon();
                UpdateSpeedRate();
            }
        }

#endregion

#region utility

        public bool IsScreenOut()
        {
            return SpriteRenderer.isVisible == false;
        }

#endregion

#region overridea

        // ダメージ
        protected override void Death()
        {
            // ダメージコライダー無効化
            DamageCollider2D.enabled = false;

            // Animation
            ChangeCharacterState(BattleParam.CharacterState.Down);
            Destroy(gameObject, 1.0f);
        }

#endregion
    }
}
