using UnityEngine;

namespace Common.Other
{
    public static class CommonUtility
    {
        /*
         * よく使うCallback
         */
        public delegate void VoidCallback();
        public delegate void IntCallback(int value);
        public delegate void FloatCallback(float value);

        /*
         * 座標変換
         */
        public static Vector3 ConvertWorldPosFromCanvasPos(Canvas canvas, RectTransform rect)
        {
            //UI座標からスクリーン座標に変換
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.position);

            //ワールド座標
            Vector3 result = Vector3.zero;

            //スクリーン座標→ワールド座標に変換
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPos, canvas.worldCamera, out result);

            return result;
        }
        public static Vector3 ConvertCanvasPosFromWorldPos(GameObject canvas, Vector3 worldPos)
        {
            var canvasRect = canvas.GetComponent<RectTransform>();
            var screenPos = Camera.main.WorldToViewportPoint(worldPos);
            var worldObjectScreenPosition = new Vector2((screenPos.x * canvasRect.sizeDelta.x), (screenPos.y * canvasRect.sizeDelta.y));
            return new Vector3(worldObjectScreenPosition.x, worldObjectScreenPosition.y, 0);
        }
    }
}
