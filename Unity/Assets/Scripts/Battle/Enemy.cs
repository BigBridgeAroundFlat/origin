using Common.FrameWork;

namespace Battle
{
    public class Enemy : CharacterAi
    {
        protected override void Start()
        {
            base.Start();

            var characterType = GameInfoManager.EnemySelectCharacterType;
            SetCharacterParameter(characterType);

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
                BattleController.Instance.NotifyDeath(true);
            }
        }
    }
}
