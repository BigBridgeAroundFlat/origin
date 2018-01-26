using System;
using Common.FrameWork;
using UnityEngine;

namespace Battle
{
    public class Player : CharacterAi
    {
        protected override void Start()
        {
            base.Start();

            var characterType = GameInfoManager.PlayerSelectCharacterType;
            SetCharacterParameter(characterType);

            // Set Animation Controller
            {
                var filePath = GameInfoManager.CalcCharacterAnimationControllerFilePath(characterType);
                var animationController = ResourceManager.LoadRuntimeAnimatorController(filePath);
                SetAnimationController(animationController);
            }

            // ダメージTag
            {
                DamageTagList.Add("EnemyAttack");
            }
        }
        protected override void Damage(int value, bool isDownAttack)
        {
            base.Damage(value, isDownAttack);

            if (IsDeath())
            {
                BattleController.Instance.NotifyDeath(false);
            }
        }

        protected override void Update()
        {
            if (GameInfoManager.IsPlayerAiMode)
            {
                base.Update();
                return;
            }

            if (CanUpdate() == false)
            {
                return;
            }

            // 接地判定：入力処理の前に行う
            UpdateGroundRayCastHit2D();

            // 入力アクション
            UpdateInputAction();
        }

        private void FixedUpdate()
        {
            if (GameInfoManager.IsPlayerAiMode)
            {
                return;
            }

            if (CanUpdate() == false)
            {
                return;
            }

            if (UpdateMove() == false)
            {
                StopMove();
            }
        }

        #region action

        private void UpdateInputAction()
        {
            // ジャンプ
            {
                if (Input.GetKeyDown("x"))
                {
                    Jump();
                }

                UpdateJump();
            }

            // 攻撃
            {
                if (Input.GetKeyDown("z"))
                {
                    InputAction();
                }
            }

            // アピール
            {
                if (Input.GetKeyDown("c"))
                {
                    AppealNormal();
                }
            }
        }
        private void InputAction()
        {
            var state = CalcCurrentCharacterState();
            switch (state)
            {
                case CharacterState.Idle:
                case CharacterState.Move:
                    CharacterAnimator.SetTrigger("Attack1");
                    break;

                case CharacterState.Attack1:
                    CharacterAnimator.SetTrigger("Attack2");
                    break;

                case CharacterState.Attack2:
                    CharacterAnimator.SetTrigger("Attack3");
                    break;
            }
        }

        #endregion

        #region move

        private bool UpdateMove()
        {
            if (CanMove() == false)
            {
                return false;
            }

            //左キー: -1、右キー: 1
            var x = Input.GetAxisRaw("Horizontal");
            if (Math.Abs(x) < 0.01f)
            {
                return false;
            }

            var isRight = 0 < x;
            Move(isRight);
            return true;
        }

        #endregion
    }
}
