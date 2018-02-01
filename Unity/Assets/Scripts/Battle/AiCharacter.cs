using UnityEngine;

namespace Battle
{
    public class CharacterAi : Character
    {
        // ターゲット
        [SerializeField] private Character _targetScript;

        // AI思考時間
        [SerializeField] private float _updateAiTimeMin;
        [SerializeField] private float _updateAiTimeMax;
        private float _currentUpdateAiTime;
        private float _currentAiTime;

        // AI思考パラメータ
        private const float DistanceMax = 16.0f;
        private const float DistanceMiddle = DistanceMax * 0.5f;
        private const float DistanceMin = 1.5f;
        private const float AttackRange = 3.0f;

        // AIアクション
        public enum AiActionType
        {
            Idle = 0,
            MoveNear,
            MoveFar,
            AttackCombo1,
            AttackCombo3,
            Appeal,
        }
        private AiActionType _currentAiActionType = AiActionType.Idle;

        protected override void Start()
        {
            base.Start();

            // AI思考タイマーリセット
            ResetAiTimer();
        }

        protected virtual void Update()
        {
            if (CanUpdate() == false)
            {
                return;
            }

            // ジャンプさせるなら接地判定処理必要
            //            UpdateGroundRayCastHit2D();

            UpdateMove();
            UpdateAi();
        }
        private void UpdateMove()
        {
            if (CanMove() == false)
            {
                StopMove();
                return;
            }

            var isRight = CheckTargetRight();
            switch (_currentAiActionType)
            {
                case AiActionType.MoveNear:
                {
                    var distance = CalcTragetDistance();
                    if (distance < DistanceMin)
                    {
                        _currentAiActionType = AiActionType.Idle;
                    }
                    else
                    {
                        Move(isRight);
                    }
                }
                    break;

                case AiActionType.MoveFar:
                    Move(!isRight);
                    break;

                default:
                    StopMove();
                    break;
            }
        }

        #region ai

        private bool CanUpdateAi()
        {
            if (CanUpdate() == false)
            {
                return false;
            }

            var state = CalcCurrentCharacterState();
            switch (state)
            {
                case CharacterState.Idle:
                case CharacterState.Move:
                    break;

                default:
                    return false;
            }

            return true;
        }
        private void UpdateAi()
        {
            // 思考時間
            {
                _currentAiTime += Time.deltaTime;
                if (_currentAiTime < _currentUpdateAiTime)
                {
                    return;
                }
            }

            // AI思考可能？
            if (CanUpdateAi() == false)
            {
                return;
            }

            // AI思考&アクション
            var aiActionType = CalcAiActionType();
            PlayAiAction(aiActionType);

            // AI思考タイマーリセット
            ResetAiTimer();
        }

        private void ResetAiTimer()
        {
            _currentAiTime = 0;
            _currentUpdateAiTime = Random.Range(_updateAiTimeMin, _updateAiTimeMax);
        }
        private AiActionType CalcAiActionType()
        {
            /*
             * AI思考に必要なパラメータ
             * targetCharacterState:相手の状態
             * distance：ステージ全体=16、攻撃範囲=2
             */
            var targetCharacterState = _targetScript.CalcCurrentCharacterState();
            var distance = CalcTragetDistance();

            /*
             * AI思考
             * 近距離：待機orアピール、離れる、攻撃
             * 中距離：待機orアピール、近づく
             * 遠距離：近づく
             */
            if (distance < AttackRange)
            {
                // アピール
                {
                    var appealPercent = 10;
                    switch (targetCharacterState)
                    {
                        case CharacterState.Down:
                            appealPercent = 80;
                            break;
                    }

                    if (CheckPercent(appealPercent))
                    {
                        return AiActionType.Appeal;
                    }
                }

                // 待機
                if (CheckPercent(20))
                {
                    return AiActionType.Idle;
                }

                //離れる
                {
                    var percent = 20;
                    switch (_currentAiActionType)
                    {
                        case AiActionType.AttackCombo1:
                        case AiActionType.AttackCombo3:
                            percent = 50;
                            break;
                    }

                    if (CheckPercent(percent))
                    {
                        return AiActionType.MoveFar;
                    }
                }

                //コンボ攻撃
                if (CheckPercent(30))
                {
                    return AiActionType.AttackCombo1;
                }

                return AiActionType.AttackCombo3;
            }
            else if (distance < DistanceMiddle)
            {
                // アピール
                {
                    var appealPercent = 20;
                    switch (targetCharacterState)
                    {
                        case CharacterState.Down:
                            appealPercent = 50;
                            break;
                    }

                    if (CheckPercent(appealPercent))
                    {
                        return AiActionType.Appeal;
                    }
                }

                // 待機
                if (CheckPercent(50))
                {
                    return AiActionType.Idle;
                }

                //近づく
                return AiActionType.MoveNear;
            }
            else
            {
                return AiActionType.MoveNear;
            }
        }
        private void PlayAiAction(AiActionType type)
        {
            _currentAiActionType = type;

            switch (type)
            {
                case AiActionType.Idle:
                    LookAtTarget();
                    break;

                case AiActionType.MoveNear:
                case AiActionType.MoveFar:
                    UpdateMove();
                    break;

                case AiActionType.AttackCombo1:
                    LookAtTarget();
                    AttackCombo(1);
                    break;

                case AiActionType.AttackCombo3:
                    LookAtTarget();
                    AttackCombo(3);
                    break;

                case AiActionType.Appeal:
                    LookAtTarget();
                    AppealNormal();
                    break;
            }
        }
        private void LookAtTarget()
        {
            var isRight = CheckTargetRight();
            var currentScaleVec2 = transform.localScale;

            if (isRight != 0 < currentScaleVec2.x)
            {
                currentScaleVec2.x *= -1;
                transform.localScale = currentScaleVec2;
            }
        }

        #endregion

        #region action

        private void AttackCombo(int comboCount)
        {
            if (CanAction() == false)
            {
                return;
            }

            for (var i = 0; i < comboCount; i++)
            {
                var key = "Attack" + (i + 1);
                CharacterAnimator.SetTrigger(key);
            }
        }

        #endregion

        #region utility

        private float CalcTragetDistance()
        {
            return Mathf.Abs(_targetScript.gameObject.transform.position.x - transform.position.x);
        }
        private bool CheckTargetRight()
        {
            return transform.position.x < _targetScript.gameObject.transform.position.x;
        }
        private bool CheckPercent(int percent)
        {
            return Random.Range(0, 100) < percent;
        }

        #endregion
    }
}
