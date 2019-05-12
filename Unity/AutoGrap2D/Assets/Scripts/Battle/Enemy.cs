using Common.FrameWork;
using UnityEngine;
using Unity.Linq;

namespace Battle
{
    public class Enemy : CharacterAi
    {
        protected override void Start()
        {
            base.Start();

            var characterType = GameInfoManager.EnemySelectCharacterType;
            SetCharacterParameter(characterType);

            {
                gameObject.DescendantsAndSelf().OfComponent<SpriteRenderer>().FirstOrDefault().flipX = true;
                transform.localScale = new Vector3(1,1,1);
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }

            // Set Animation Controller
            {
                var filePath = GameInfoManager.CalcCharacterAnimationControllerFilePath(characterType);
                var animationController = ResourceManager.LoadRuntimeAnimatorController(filePath);
                SetAnimationController(animationController);
            }

            // ダメージTag
            {
                DamageTagList.Add("PlayerAttack");
            }
        }

        protected override void Damage(int value, bool isDownAttack)
        {
            base.Damage(value, isDownAttack);

            if (IsDeath())
            {
                BattleController.Instance.NotifyDeath(true, gameObject);
            }
        }

        protected override void UpdateTargetScript()
        {
            var targetList = GameObject.FindGameObjectsWithTag("Player");
            var max = targetList.Length;
            if(0 < max)
            {
                var index = Random.Range(0, max);
                _targetScript = targetList[index].GetComponent<CharacterAi>();
            }
        }

        protected override bool CanUpdate()
        {
            if(_targetScript == null)
            {
                UpdateTargetScript();
                if(_targetScript == null)
                {
                    return false;
                }
            }

            return base.CanUpdate();
        }

        protected override void Update()
        {
            if (_targetScript != null)
            {
                UpdateTargetScript();
            }

            base.Update();
        }
    }
}
