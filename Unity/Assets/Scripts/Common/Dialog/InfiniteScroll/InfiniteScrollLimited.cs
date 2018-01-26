using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.Dialog.InfiniteScroll
{
    [RequireComponent(typeof(InfiniteScroll))]
    public class InfiniteScrollLimited : UIBehaviour, IInfiniteScrollSetup {

        // 制限スクロールの最大数
        [SerializeField, Range(1, 9999)] private int _scrollItemMax = 30;

        // 更新処理
        public delegate void UpdateCallback(int count, GameObject obj);
        private UpdateCallback _updateCallback;


        // 初期化
        public void Init(int max, UpdateCallback callback)
        {
            _scrollItemMax = max;
            _updateCallback = callback;
        }

        public void OnPostSetupItems()
        {
            var infiniteScroll = GetComponent<InfiniteScroll>();
            infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);
            GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;

            UpdateScrollRectTransform();
        }
        private void UpdateScrollRectTransform()
        {
            var infiniteScroll = GetComponent<InfiniteScroll>();

            // 前後の余白取るんで2倍
            var addMargin = infiniteScroll.margin * 2 - infiniteScroll.itemScaleMargin;

            var rectTransform = GetComponent<RectTransform>();
            var delta = rectTransform.sizeDelta;
            if (infiniteScroll.direction == InfiniteScroll.ScrollDirection.Vertical)
            {
                delta.y = infiniteScroll.itemScale * _scrollItemMax + addMargin;
            }
            else
            {
                delta.x = infiniteScroll.itemScale * _scrollItemMax + addMargin;
            }
            rectTransform.sizeDelta = delta;
        }

        public void UpdateCurrentScroll(int scrollItemMaxCount)
        {
            // スクロール最大数更新
            {
                _scrollItemMax = scrollItemMaxCount;
                UpdateScrollRectTransform();
            }

            // スクロールバー更新
            GetComponent<InfiniteScroll>().UpdateCurrentItem();
        }

        public void OnUpdateItem(int count, GameObject obj)
        {
            if(count < 0 || count >= _scrollItemMax)
            {
                obj.SetActive (false);
            }
            else
            {
                obj.SetActive (true);

                if (_updateCallback != null)
                {
                    _updateCallback(count, obj);
                }
            }
        }
    }
}
