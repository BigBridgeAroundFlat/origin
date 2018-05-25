using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpFace : MonoBehaviour
{
    [SerializeField] private Sprite _damageFaceSprite1;
    [SerializeField] private Sprite _damageFaceSprite2;
    [SerializeField] private Sprite _damageFaceSprite3;
    [SerializeField] private Image _faceSpriteImage;

    public void UpdateHpFace(int currentHp, int maxHp)
    {
        var changeSprite = _damageFaceSprite1;

        var percent = (float)currentHp / (float)maxHp * 100.0f;
        if (percent < 15.0f)
        {
            changeSprite = _damageFaceSprite3;
        }
        else if (percent < 50.0f)
        {
            changeSprite = _damageFaceSprite2;
        }

        _faceSpriteImage.sprite = changeSprite;
    }

}
