using Common.FrameWork;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private int score = 100;
        [SerializeField] private GameObject scoreTextPrefab;

        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _circleCollider2D;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _circleCollider2D = GetComponent<CircleCollider2D>();

            {
                var itemType = PlayerPrefs.GetInt("ItemType");
                if (itemType == 2)
                {
                    var filePath = "Sprites/item2";
                    _spriteRenderer.sprite = ResourceManager.LoadSprite(filePath);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 同フレームの重複対策
            if (_circleCollider2D.enabled == false)
            {
                return;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                // enable false
                {
                    _spriteRenderer.enabled = false;
                    _circleCollider2D.enabled = false;
                }

                // スコア取得
                MainController.Instance.AddScore(score);

                // スコア取得演出
                {
                    var prefab = Instantiate(scoreTextPrefab, transform);
                    {
                        prefab.SetActive(true);
                    }

                    var child = prefab.Descendants().FirstOrDefault();

                    // text
                    {
                        var text = child.GetComponent<Text>();
                        text.text = score.ToString();

                    }

                    // typeface animator
                    {
                        var typefaceAnimator = child.GetComponent<TypefaceAnimator>();
                        typefaceAnimator.onComplete.AddListener(() =>
                        {
                            // 破棄
                            Destroy(gameObject);
                        });
                    }
                }
            }
        }
    }
}
