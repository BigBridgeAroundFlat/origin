using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Define
{
    public static class CommonParam
    {
        public enum Direction
        {
            None = 0,

            Up,
            Down,
            Left,
            Right,
        }
	}

    public static class CommonData
    {
        public delegate void VoidCallback();
        public delegate void IntCallback(int value);
        public delegate void FloatCallback(float value);
        public delegate void StringCallback(string str);

    }

    public static class CommonFunc
    {
        #region calc min max

        /*
         * リストの最大値、最小値取得
         */
        public static int CalcMaxValue(List<int> intList)
        {
            if (intList.Count == 0)
            {
                return -1;
            }

            var result = intList[0];

            return intList.Aggregate(result, Mathf.Max);
        }
        public static int CalcMinValue(List<int> intList)
        {
            if (intList.Count == 0)
            {
                return -1;
            }

            var result = intList[0];

            return intList.Aggregate(result, Mathf.Min);
        }

        #endregion

        #region angle

        /*
         * 角度から方向取得
         */
        public static CommonParam.Direction CalcDirection(float angle)
        {
            CommonParam.Direction dir;
            {
                // up
                if (45 <= angle && angle <= 135)
                {
                    dir = CommonParam.Direction.Up;
                }
                // left
                else if (135 <= angle && angle <= 225)
                {
                    dir = CommonParam.Direction.Left;
                }
                // down
                else if (225 <= angle && angle <= 315)
                {
                    dir = CommonParam.Direction.Down;
                }
                // right
                else
                {
                    dir = CommonParam.Direction.Right;
                }
            }
            return dir;
        }

        /*
         * 方向から方向ベクトル取得
         */
        public static Vector3 CalcDirectionVector(CommonParam.Direction dir)
        {
            var dirVec = Vector3.zero;

            switch (dir)
            {
                case CommonParam.Direction.Up: dirVec = Vector3.up; break;
                case CommonParam.Direction.Down: dirVec = Vector3.down; break;
                case CommonParam.Direction.Right: dirVec = Vector3.right; break;
                case CommonParam.Direction.Left: dirVec = Vector3.left; break;
            }

            return dirVec;
        }

        /*
         * 2点のベクトル間の角度計算（0～360°）
         */
        public static float CalcAngleFromVector3(Vector3 p1, Vector3 p2)
        {
            float angle;

            var distanceX = p2.x - p1.x;
            var distanceY = p2.y - p1.y;
            var radius = Mathf.Atan2(distanceY, distanceX);
            if (radius * Mathf.Rad2Deg < 0)
            {
                angle = radius * Mathf.Rad2Deg + 360;
            }
            else
            {
                angle = radius * Mathf.Rad2Deg;
            }

            return angle;
        }

        #endregion

        #region convert pos

        /*
         * ワールド座標->スクリーン座標に変換
         */
        public static Vector3 ConvertCanvasPosFromWorldPos(GameObject canvas, Vector3 worldPos)
        {
            var canvasRect = canvas.GetComponent<RectTransform>();
            var screenPos = Camera.main.WorldToViewportPoint(worldPos);
            var worldObjectScreenPosition = new Vector2((screenPos.x * canvasRect.sizeDelta.x), (screenPos.y * canvasRect.sizeDelta.y));
            return new Vector3(worldObjectScreenPosition.x, worldObjectScreenPosition.y, 0);
        }

        #endregion

        #region check process speed 

        public static void CheckProcessSpeed(CommonData.VoidCallback callback)
        {
            // 現在の経過時間を取得
            var checkTime = Time.realtimeSinceStartup;

            // nullチェック無し
            callback();

            // 処理完了後の経過時間から、保存していた経過時間を引く＝処理時間
            checkTime = Time.realtimeSinceStartup - checkTime;

            Debug.Log("check time : " + checkTime.ToString("0.00000"));
        }

        #endregion
    }
}
