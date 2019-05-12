using Common.FrameWork;
using DG.Tweening;
using UnityEngine;
using Unity.Linq;

namespace Battle
{
    public class ViewerCharacter : MonoBehaviour
    {
        [SerializeField] private GameInfoManager.CharacterType _currentCharacterType = GameInfoManager.CharacterType.None;

        // cash
        private Animator _characterAnimator;

        private void Start()
        {
            _characterAnimator = GetComponent<Animator>();
            SetCharacterType(_currentCharacterType);
        }

        public void SetCharacterType(GameInfoManager.CharacterType characterType)
        {
            if (characterType == GameInfoManager.CharacterType.None)
            {
                return;
            }

            // animation controller
            {
                var filePath = GameInfoManager.CalcCharacterAnimationControllerFilePath(characterType);
                var animationController = ResourceManager.LoadRuntimeAnimatorController(filePath);
                _characterAnimator.runtimeAnimatorController = animationController;
            }

            if (characterType == GameInfoManager.CharacterType.Heroine)
            {
                _characterAnimator.gameObject.DescendantsAndSelf().OfComponent<SpriteRenderer>().FirstOrDefault().flipX = true;
                transform.localScale = Vector3.one;
                transform.position = new Vector3(4.5f, 0, 0);
            }
            else
            {
                _characterAnimator.gameObject.DescendantsAndSelf().OfComponent<SpriteRenderer>().FirstOrDefault().flipX = false;
                transform.localScale = Vector3.one * 8.0f;
                transform.position = new Vector3(4.5f, -3.2f, 0);
            }
        }

        public void PlayAnimatin(Character.CharacterState state)
        {
            if (_characterAnimator == null)
            {
                return;
            }

            _characterAnimator.SetBool("Walk", state == Character.CharacterState.Move);

            switch (state)
            {
                case Character.CharacterState.Jump: JumpAnimation(); break;
                case Character.CharacterState.Damage: DamageAnimation(false); break;
                case Character.CharacterState.Down: DamageAnimation(true); break;
                case Character.CharacterState.Attack1: AttackComboAnimation(1); break;
                case Character.CharacterState.Attack2: AttackComboAnimation(2); break;
                case Character.CharacterState.Attack3: AttackComboAnimation(3); break;
                case Character.CharacterState.Appeal: _characterAnimator.SetTrigger("AppealNormal"); break;
            }
        }

        private void JumpAnimation()
        {
            _characterAnimator.SetTrigger("Jump");
            _characterAnimator.SetBool("IsJumping", true);
            _characterAnimator.SetBool("IsFalling", false);

            var seq = DOTween.Sequence();
            {
                seq.AppendInterval(0.5f);
                seq.AppendCallback(() =>
                {
                    _characterAnimator.SetBool("IsJumping", false);
                    _characterAnimator.SetBool("IsFalling", true);
                });
                seq.AppendInterval(0.5f);
                seq.AppendCallback(() =>
                {
                    _characterAnimator.SetBool("IsFalling", false);
                });
            }
        }
        private void DamageAnimation(bool isDown)
        {
            _characterAnimator.SetTrigger("Damage");
            _characterAnimator.SetBool("Down", isDown);
            _characterAnimator.SetBool("WakeUp", isDown);
        }
        private void AttackComboAnimation(int comboCount, bool isCombo = true)
        {
            isCombo = false;

            if(isCombo)
            {
                for (var i = 0; i < comboCount; i++)
                {
                    var key = "Attack" + (i + 1);
                    _characterAnimator.SetTrigger(key);
                }
            }
            else
            {
                var key = "Attack" + comboCount;
                _characterAnimator.SetTrigger(key);
            }
        }
    }
}
