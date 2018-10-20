using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpFace : MonoBehaviour
{
    private void Awake()
    {
        var button = GetComponent<Button>();
        if(button != null)
        {
            button.OnClickEtension(() =>
            {
                var intVlaue = (int)_currentAiIconType;
                intVlaue++;
                if (2 < intVlaue) intVlaue = 0;
                ChangAieIconType((AiIconType)intVlaue);
            });
        }
    }

    public bool IsUse { get; private set; }
    public void UnenableHpFace()
    {
        gameObject.SetActive(false);
        IsUse = false;
    }
    public void Init()
    {
        gameObject.SetActive(true);
        IsUse = true;
        _currentAiIconType = AiIconType.Normal;
        UpdateAiIcon();
    }

    public enum AiIconType
    {
        Normal = 0,
        Attack,
        Defence,
    }
    private AiIconType _currentAiIconType = AiIconType.Normal;
    public AiIconType GetCurrentAiType() { return _currentAiIconType; }
    public void ChangAieIconType(AiIconType type)
    {
        if (_currentAiIconType == type)
        {
            return;
        }
        _currentAiIconType = type;
        UpdateAiIcon();
    }

    [SerializeField] private Image _aiIconImage;
    [SerializeField] private Sprite _aiIconSprite1;
    [SerializeField] private Sprite _aiIconSprite2;
    [SerializeField] private Sprite _aiIconSprite3;
    private void UpdateAiIcon()
    {
        Sprite aiIconSprite = null;
        switch(_currentAiIconType)
        {
            case AiIconType.Normal: aiIconSprite = _aiIconSprite1; break;
            case AiIconType.Attack: aiIconSprite = _aiIconSprite2; break;
            case AiIconType.Defence: aiIconSprite = _aiIconSprite3; break;
        }
        _aiIconImage.sprite = aiIconSprite;
    }

    [SerializeField] private Image _faceSpriteImage;
    [SerializeField] private Sprite _damageFaceSprite1;
    [SerializeField] private Sprite _damageFaceSprite2;
    [SerializeField] private Sprite _damageFaceSprite3;
    [SerializeField] private Sprite _damageFaceSprite4;
    public void UpdateHpFace(int currentHp, int maxHp)
    {
        var changeSprite = _damageFaceSprite1;

        var percent = (float)currentHp / (float)maxHp * 100.0f;
        if(percent < 1)
        {
            changeSprite = _damageFaceSprite4;
            IsUse = false;
        }
        else if (percent < 15.0f)
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
