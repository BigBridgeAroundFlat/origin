using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Sprite _playerDamageEffectSprite;
    [SerializeField] private Sprite _enemyDamageEffectSprite;

    public void Init(GameInfoManager.CharacterType characterType, int damage)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();

        // pos & scale
        {
            transform.localPosition += new Vector3(0, 0.4f, 0);

            var scale = 1.0f;

            switch (damage)
            {
                case 20: scale = 1.2f; break;
                case 30: scale = 1.5f; break;
                case 100: scale = 2.0f; break;
            }

            transform.localScale *= scale;
        }

        // sprite
        {
            spriteRenderer.sprite = characterType == GameInfoManager.CharacterType.Kohaku ? _playerDamageEffectSprite : _enemyDamageEffectSprite;
        }

        // action
        {
            var seq = DOTween.Sequence();
            {
                seq.Append(spriteRenderer.DOFade(1.0f, 0.2f));
                seq.Append(spriteRenderer.DOFade(0.0f, 0.2f));
                seq.AppendCallback(() =>
                {
                    Destroy(gameObject);
                });
            }
        }
    }
}
