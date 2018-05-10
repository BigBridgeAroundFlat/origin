using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Common.Dialog
{
    public static class DialogUtility
    {
        #region param

        public enum DialogType
        {
            None = 0,

            MessageDialog,
        }

        public class DialogInfo
        {
            public DialogInfo(){}
            public DialogInfo(DialogType type)
            {
                DialogType = type;
            }

            // type
            public DialogType DialogType;

            // text
            public string Title;
            public string Message;

            //button
            public bool UnenableCancelButton;
            public Action<bool> OkCancelButtonCallback;
        }

        #endregion

        #region func

        /*
         * スクロールリスト取得（破棄）演出：演出固定
         */
        public static void RemoveInfiniteScrollCellAction(GameObject removeTargetObj, InfiniteScroll.InfiniteScroll infiniteScroll, Action callback)
        {
            // アクション設定
            var actionTime = 0.3f;
            var waitTime = actionTime + 0.2f;
            var moveX = 800;

            // ターゲット
            var removeTargetRectTransform = removeTargetObj.GetComponent<RectTransform>();
            var moveTargetRectTransformList = new List<RectTransform>();
            {
                var checkPosY = removeTargetRectTransform.anchoredPosition.y;
                foreach (var rectTransform in infiniteScroll.itemList)
                {
                    if (rectTransform.gameObject.activeSelf == false)
                    {
                        continue;
                    }

                    if (checkPosY <= rectTransform.anchoredPosition.y)
                    {
                        continue;
                    }

                    moveTargetRectTransformList.Add(rectTransform);
                }
            }

            // アクション開始
            infiniteScroll.ChangeIsPlayAction(true);

            var seq = DOTween.Sequence();
            {
                // ①削除対象のスクロールバーを画面左に移動
                seq.Append(removeTargetRectTransform.DOLocalMoveX(-moveX, actionTime).SetRelative());

                // スクロールバーつめる必要があるならつめる
                if (moveTargetRectTransformList.IsEmpty() == false)
                {
                    // ②削除対象より下にあるのスクロールバーを上につめる
                    seq.AppendCallback(() =>
                    {
                        foreach (var moveTargetRectTransform in moveTargetRectTransformList)
                        {
                            moveTargetRectTransform.DOLocalMoveY(infiniteScroll.itemScale, actionTime).SetRelative();
                        }
                    });

                    // ③スクロールバーつめるまで待機
                    seq.AppendInterval(waitTime);
                }

                // ④アクション終了コールバック
                seq.AppendCallback(() =>
                {
                    if (callback != null)
                    {
                        callback();
                    }

                    // アクション終了
                    infiniteScroll.ChangeIsPlayAction(false);
                });
            }
        }

        #endregion
    }
}
