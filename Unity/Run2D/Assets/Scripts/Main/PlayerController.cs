using System;
using DG.Tweening;
using TouchScript.Gestures;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using ResourceManager = Common.FrameWork.ResourceManager;

namespace Main
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject TouchGestureModal;
        private Rigidbody2D _rigidbody2D;
        private Animator _anim;

        // 移動
        public float MaxSpeed = 5; //歩くスピード
        public float Accel = 1; //歩くスピード
        private float _speed;

        // ジャンプ
        public float JumpPower = 300; //ジャンプ力
        public float SuperJumpPower = 500; //ジャンプ力
        public float FallPower = 10; //落下加算
        public LayerMask GroundLayer; //Linecastで判定するLayer
        private RaycastHit2D groundRaycastHit2D;

        // Use this for initialization
        void Start ()
        {
            //cash
            {
                _anim = GetComponent<Animator>();
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }

            {
                TouchGestureModal.GetComponent<FlickGesture>().Flicked += OnFlicked;
                TouchGestureModal.GetComponent<TapGesture>().Tapped += OnTapped;
            }
        }

        private bool IsStart()
        {
            return MainController.Instance._isStart;
        }
        private void Update() 
        {
            if (IsStart() == false)
            {
                return;   
            }

            UpdateCore();
        }

        private void UpdateCore()
        {
            // ground
            {
                groundRaycastHit2D = Physics2D.Linecast(
                    transform.position + transform.up * 1,
                    transform.position - transform.up * 0.05f,
                    GroundLayer);

                if (groundRaycastHit2D == false)
                {
                    var beasePos = transform.position + new Vector3(-0.25f, 0);
                    groundRaycastHit2D = Physics2D.Linecast(
                        beasePos + transform.up * 1,
                        beasePos - transform.up * 0.05f,
                        GroundLayer);
                }
            }

#if false
            if (Input.GetKeyDown("space"))
            {
                Jump(false);
            }
#endif
            UpdateMove();
            UpdateJump();
        }
#if false
        private void FixedUpdate()
        {
            if (IsStart() == false)
            {
                return;   
            }

            //左キー: -1、右キー: 1
            float x = Input.GetAxisRaw("Horizontal");
            //左か右を入力したら
            if (x != 0)
            {
                var isRight = 0 < x;
                Run(isRight);
            }
            else
            {
                StopRun();
            }
        }
#endif

        private void OnTapped(object sender, EventArgs e)
        {
            if (IsStart() == false)
            {
                return;
            }

            Jump(false);
        }

        private void OnFlicked(object sender, EventArgs eventArgs)
        {
            if (IsStart() == false)
            {
                return;
            }

            var gesture = sender as FlickGesture;

            if (gesture.State != Gesture.GestureState.Recognized)
                return;

            if (gesture.ScreenFlickVector.y < 0)
            {
                if (groundRaycastHit2D && groundRaycastHit2D.collider.CompareTag("UpperGround"))
                {
                    var tempCollider = groundRaycastHit2D.collider;
                    var seq = DOTween.Sequence();
                    {
                        seq.AppendCallback(() =>
                        {
                            tempCollider.enabled = false;
                        });
                        seq.AppendInterval(1.0f);
                        seq.AppendCallback(() =>
                        {
                            tempCollider.enabled = true;
                        });
                    }
                }
            }
            else if (gesture.ScreenFlickVector.y > 0)
            {
                Jump(true);
            }
        }

        private void Run(bool isRight)
        {
            // 速度
            {
                var addAccel = isRight ? Accel : -Accel;
                _speed += addAccel;
                _speed = Mathf.Max(_speed, -MaxSpeed);
                _speed = Mathf.Min(_speed, MaxSpeed);
                _rigidbody2D.velocity = new Vector2(_speed, _rigidbody2D.velocity.y);
            }

            // 反転
            {
                Vector2 temp = transform.localScale;
                temp.x = isRight ? 1.0f : -1.0f;
                transform.localScale = temp;
            }

            // animator
            {
                _anim.SetBool("run", true);
            }
        }

        private void StopRun()
        {
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            _anim.SetBool("run", false);
        }

        private void Jump(bool isSuper)
        {
            if (groundRaycastHit2D == false)
            {
                return;
            }

            _anim.SetTrigger("jump");

            //AddForceにて上方向へ力を加える
            var addPower = isSuper ? SuperJumpPower : JumpPower;
            _rigidbody2D.AddForce(Vector2.up * addPower);

            UpdateJump();
        }

        private void UpdateJump()
        {
            //上下への移動速度を取得
            float velY = _rigidbody2D.velocity.y;
            //移動速度が0.1より大きければ上昇
            bool isJumping = velY > 0.1f;

            //移動速度が-0.1より小さければ下降
            bool isFalling = velY < -0.1f; ;


            //結果をアニメータービューの変数へ反映する
            _anim.SetBool("jumping", isJumping);
            _anim.SetBool("falling", isFalling);

            if (isJumping || isFalling)
            {
                _rigidbody2D.AddForce(Vector2.down * FallPower);
            }
        }

        private void UpdateMove()
        {
            Run(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 同フレームの重複対策
            if (gameObject.activeSelf == false)
            {
                return;
            }

            if (other.gameObject.CompareTag("GoalBlock"))
            {
                MainController.Instance.GameClear();
            }
            else if (other.gameObject.CompareTag("DeathBlock") || other.gameObject.CompareTag("Enemy"))
            {
                gameObject.SetActive(false);

                var filePath = "Prefabs/Explode";
                var prefab = ResourceManager.LoadAndInstantiate(filePath);
                {
                    prefab.transform.position = transform.position + new Vector3(0,0.6f,0);
                    prefab.OnDestroyAsObservable()
                        .Subscribe(_ =>
                        {
                            MainController.Instance.Gameover();
                        });
                }
            }
        }
    }
}
