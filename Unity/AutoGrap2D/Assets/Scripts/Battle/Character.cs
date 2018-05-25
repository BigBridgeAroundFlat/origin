using Boo.Lang;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class Character : MonoBehaviour
    {
        // ref
        [SerializeField] private GameObject _damageEffectPrefab;
        [SerializeField] private GameObject _groundPoint;
        [SerializeField] private Slider _hpGaugeSlider;

        // cash
        private Rigidbody2D _rigidbody2D;
        protected Animator CharacterAnimator;
        protected void SetAnimationController(RuntimeAnimatorController animationController)
        {
            CharacterAnimator.runtimeAnimatorController = animationController;
        }

        // 移動
        [SerializeField] private float speed; //歩くスピード

        // ジャンプ
        [SerializeField] private float _jumpPower; //ジャンプ力
        [SerializeField] private float _fallPower = 30;
        [SerializeField] private LayerMask _groundLayer; //Linecastで判定するLayer
        private RaycastHit2D _groundRaycastHit2D;

        // HP
        [SerializeField] private int _hpMax = 100;
        private int _hp;

        // HPゲージ
        private int _hpGaugeValue;
        private int HpGaugeInfo
        {
            set
            {
                _hpGaugeValue = value;
                if (_hpGaugeSlider)
                {
                    _hpGaugeSlider.value = _hpGaugeValue;
                }
            }
            get
            {
                return _hpGaugeValue;
            }
        }

        // ダメージ
        private int _damageUniqueId;
        protected List<string> DamageTagList = new List<string>();

        // 状態：AnimationTag
        public enum CharacterState
        {
            Idle = 0,
            Move,
            Jump,
            Damage,
            Down,
            Attack1,
            Attack2,
            Attack3,
            Appeal,
        }

        // キャラクタータイプ
        private GameInfoManager.CharacterType characterType = GameInfoManager.CharacterType.None;

        protected virtual void Start()
        {
            //cash
            {
                CharacterAnimator = GetComponent<Animator>();
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }
        }
        protected void SetCharacterParameter(GameInfoManager.CharacterType type)
        {
            characterType = type;

            switch (characterType)
            {
                case GameInfoManager.CharacterType.Kohaku:
                {
                    speed = 9;
                    _jumpPower = 900;
                    _hpMax = 300;
                }
                break;

                case GameInfoManager.CharacterType.Toko:
                {
                    speed = 8;
                    _jumpPower = 800;
                    _hpMax = 400;
                }
                break;
            }

            // Hp設定
            {
                if (_hpGaugeSlider)
                {
                    _hpGaugeSlider.maxValue = _hpMax;
                    _hpGaugeSlider.value = _hpMax;
                }
                _hp = _hpMax;
                _hpGaugeValue = _hpMax;
            }
        }

        #region Can

        protected bool CanUpdate()
        {
            if (BattleController.Instance.IsPlayBattle == false)
            {
                return false;
            }

            if (IsDeath())
            {
                return false;
            }

            return true;
        }
        protected bool CanMove()
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
                case CharacterState.Jump:
                    break;

                default:
                    return false;
            }

            return true;
        }
        protected bool CanJump()
        {
            if (CanUpdate() == false)
            {
                return false;
            }

            // 接地判定
            if (_groundRaycastHit2D == false)
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
        protected bool CanAction()
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
        protected bool CanDamage()
        {
            if (CanUpdate() == false)
            {
                return false;
            }

            var state = CalcCurrentCharacterState();
            switch (state)
            {
                case CharacterState.Down:
                    return false;
            }

            return true;
        }

        #endregion

        #region move

        protected void Move(bool isRight)
        {
            if (CanMove() == false)
            {
                return;
            }

            // 速度
            var moveX = isRight ? speed : -speed;
            _rigidbody2D.velocity = new Vector2(moveX, _rigidbody2D.velocity.y);

            // 移動時は移動方向に向ける
            if (0 < moveX != 0 < transform.localScale.x)
            {
                ReverseCharacterSprite();
            }

            // animator
            CharacterAnimator.SetBool("Walk", true);
        }
        protected void StopMove()
        {
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            CharacterAnimator.SetBool("Walk", false);
        }

        #endregion

        #region jump

        protected void Jump()
        {
            if (CanJump() == false)
            {
                return;
            }

            CharacterAnimator.SetTrigger("Jump");
            _rigidbody2D.AddForce(Vector2.up * _jumpPower);

            CharacterAnimator.SetBool("IsJumping", true);
            CharacterAnimator.SetBool("IsFalling", false);
        }
        protected void UpdateJump()
        {
            //上下への移動速度を取得
            var velY = _rigidbody2D.velocity.y;
            //移動速度が0.1より大きければ上昇
            var isJumping = velY > 0.1f;

            //移動速度が-0.1より小さければ下降
            var isFalling = velY < -0.1f;

            //結果をアニメータービューの変数へ反映する
            CharacterAnimator.SetBool("IsJumping", isJumping);
            CharacterAnimator.SetBool("IsFalling", isFalling);

            if (isJumping || isFalling)
            {
                _rigidbody2D.AddForce(Vector2.down * _fallPower);
            }
        }

        #endregion

        #region damage

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (DamageTagList.Contains(other.gameObject.tag) == false)
            {
                return;
            }

            var script = other.gameObject.GetComponent<AttackCollisionInfo>();
            if (script == null)
            {
                Debug.Assert(false, "AttackCollisionInfo is null");
                return;
            }

            // 同じ攻撃で連続ダメージ受けない
            if (_damageUniqueId == script.UniqueId)
            {
                return;
            }
            _damageUniqueId = script.UniqueId;

            // 被ダメージ処理
            var damageInt = Mathf.CeilToInt(script.DamageValue);
            Damage(damageInt, script.IsDownAttack);
        }

        protected virtual void Damage(int value, bool isDownAttack)
        {
            if (CanDamage() == false)
            {
                return;
            }

            // HP減少
            {
                _hp = Mathf.Max(_hp - value, 0);
                DOTween.To(() => HpGaugeInfo, (x) => HpGaugeInfo = x, _hp, 1);
            }

            //アニメーション
            {
                CharacterAnimator.SetTrigger("Damage");

                if (IsDeath())
                {
                    CharacterAnimator.SetBool("Down", true);
                    CharacterAnimator.SetBool("WakeUp", false);
                    StopMove();
                }
                else
                {
                    CharacterAnimator.SetBool("Down", isDownAttack);
                    CharacterAnimator.SetBool("WakeUp", isDownAttack);
                }
            }

            // effect
            {
                var damageEffect = Instantiate(_damageEffectPrefab, transform);
                damageEffect.GetComponent<DamageEffect>().Init(characterType, value);
            }
        }
        protected bool IsDeath()
        {
            return _hp <= 0;
        }

        #endregion

        #region attack

        protected void AppealNormal()
        {
            if (CanAction() == false)
            {
                return;
            }

            CharacterAnimator.SetTrigger("AppealNormal");
        }

        /*
         * 勝利ポーズフラグは勝利した時点以降常にtrue
         * 現在、勝利ポーズアクションが出来るかどうかは関係ないので、チェック関数不要
         */
        public void AppealWin()
        {
            CharacterAnimator.SetBool("AppealWin", true);
        }

        #endregion

        #region utility

        //接地判定
        protected void UpdateGroundRayCastHit2D()
        {
            var groundPos = _groundPoint.transform.position;
            _groundRaycastHit2D = Physics2D.Linecast(groundPos, groundPos, _groundLayer);
        }

        //キャラクターSprite反転
        protected void ReverseCharacterSprite()
        {
            Vector2 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;
        }

        // 状態取得：アニメーション
        public CharacterState CalcCurrentCharacterState()
        {
            var state = CharacterState.Idle;
            var info = CharacterAnimator.GetCurrentAnimatorStateInfo(0);

            if (info.IsTag("Idle"))
            {
                state = CharacterState.Idle;
            }
            else if (info.IsTag("Move"))
            {
                state = CharacterState.Move;
            }
            else if (info.IsTag("Jump"))
            {
                state = CharacterState.Jump;
            }
            else if (info.IsTag("Damage"))
            {
                state = CharacterState.Damage;
            }
            else if (info.IsTag("Down"))
            {
                state = CharacterState.Down;
            }
            else if (info.IsTag("Attack1"))
            {
                state = CharacterState.Attack1;
            }
            else if (info.IsTag("Attack2"))
            {
                state = CharacterState.Attack2;
            }
            else if (info.IsTag("Attack3"))
            {
                state = CharacterState.Attack3;
            }
            else if (info.IsTag("Appeal"))
            {
                state = CharacterState.Appeal;
            }
            else
            {
                Debug.Assert(false, "CalcCurrentCharacterState : Not Settingg Animation Tag");
            }

            return state;
        }

        #endregion
    }
}
