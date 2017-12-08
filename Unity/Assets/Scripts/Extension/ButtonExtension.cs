using System;
using Sound;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtension
{
    /// <summary>
    /// ボタン標準機能
    /// </summary>
    public static void OnClickEtension(this Button button, Action onNext, string seFileName = "se_kettei")
    {
        button.OnClickAsObservable()
            .Do(s => SoundManager.Instance.PlaySe(seFileName))
            // .ThrottleFirst(TimeSpan.FromSeconds(1)) 連打対策
            .Subscribe(__ =>
            {
                if (onNext != null) onNext();
            }, ex => Debug.LogError(ex.Message))
            .AddTo(button.gameObject);
    }
}