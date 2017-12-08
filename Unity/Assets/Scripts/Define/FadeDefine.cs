using UnityEngine;
using UnityEngine.UI;


namespace Define
{
    public static class FadeParam
    {
        public enum FadeAction
        {
            None = 0,

            FadeIn,
            FadeOut,
            FadeInOut,
            FadeOutIn,
        }
        public enum FadeState
        {
            None = 0,

            FadeinStart,
            FadeinFinish,
            FadeoutStart,
            FadeoutFinish,
        }
        public enum FadeTargetComponent
        {
            None = 0,

            Image,
            Text,
            Sprite_2D,
        }
    }

    public static class FadeData
    {
        public class FadeInfo
        {
            // const
            const float DefaultFadeWait = 1.0f;
            const float DefaultAddFadeAlpha = 0.1f;

            // target
            public FadeParam.FadeTargetComponent FadeTargetComponent = FadeParam.FadeTargetComponent.None;

            // action
            public FadeParam.FadeAction FadeAction = FadeParam.FadeAction.None;

            // param
            public float Delay = 0;
            public float ChangeInterval = DefaultFadeWait;
            public float AddFadeAlpha = DefaultAddFadeAlpha;

            // finish
            public bool IsDestroy = true;
            public CommonData.VoidCallback FinishCallback = null;
        }
    }

    public static class FadeFunc
    {
        /*
         * α値変更
         */ 
        public static void ChangeComponentAlpha(GameObject obj, FadeParam.FadeTargetComponent type, float alpha)
        {
            if (obj == null || type == FadeParam.FadeTargetComponent.None)
            {
                return;
            }

            switch (type)
            {
                case FadeParam.FadeTargetComponent.Image:
                {
                    var image = obj.GetComponent<Image>();
                    if (image != null)
                    {
                        var currentColor = image.color;
                        image.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    }
                }
                    break;

                case FadeParam.FadeTargetComponent.Text:
                {
                    var text = obj.GetComponent<Text>();
                    if (text != null)
                    {
                        var currentColor = text.color;
                        text.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    }
                }
                    break;

                case FadeParam.FadeTargetComponent.Sprite_2D:
                {
                    var spriteRender = obj.GetComponent<SpriteRenderer>();
                    if (spriteRender != null)
                    {
                        var currentColor = spriteRender.color;
                        spriteRender.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    }
                }
                    break;
            }
        }
    }
}