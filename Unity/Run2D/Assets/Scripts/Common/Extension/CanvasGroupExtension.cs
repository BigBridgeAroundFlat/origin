using System;
using Common;
using UnityEngine;

public static class CanvasGroupExtension
{
    public const float ENABLE_ALPHA = 1.0f;
    public const float DISABLE_ALPHA = 0.0f;

    public static void Show(this CanvasGroup canvasGroup)
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (Math.Abs(canvasGroup.alpha - ENABLE_ALPHA) > 0.001)
        {
            canvasGroup.alpha = ENABLE_ALPHA;
        }

        if (!canvasGroup.interactable)
        {
            canvasGroup.interactable = true;
        }

        if (!canvasGroup.blocksRaycasts)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }

    public static void Hide(this CanvasGroup canvasGroup)
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (Math.Abs(canvasGroup.alpha - DISABLE_ALPHA) > 0.001)
        {
            canvasGroup.alpha = DISABLE_ALPHA;
        }

        if (canvasGroup.interactable)
        {
            canvasGroup.interactable = false;
        }

        if (canvasGroup.blocksRaycasts)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
}
