using System;
using System.Linq;
using Common.FrameWork;
using Common.JoyStick;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class PlayerMonster : CharacterAi
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
                BattleController.Instance.NotifyDeath(false, gameObject);
            }
        }

        protected override void UpdateTargetScript()
        {
            _targetScript = GameObject.FindGameObjectWithTag("Enemy").GetComponent<CharacterAi>();
        }
        public void Init(HpFace face)
        {
            if (characterType == GameInfoManager.CharacterType.Heroine)
            {
                gameObject.DescendantsAndSelf().OfComponent<SpriteRenderer>().FirstOrDefault().flipX = true;
                transform.localScale = Vector3.one;
            }
            else
            {
                gameObject.DescendantsAndSelf().OfComponent<SpriteRenderer>().FirstOrDefault().flipX = false;
                transform.localScale = Vector3.one * 8.0f;
            }

            _hpFace = face;
            UpdateTargetScript();
        }
    }
}
