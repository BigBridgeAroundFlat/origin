using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.Other
{
    [RequireComponent(typeof(Image))]
    public class DragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // ref
        [SerializeField] protected GameObject DraggingPrefab;
        [SerializeField] protected Image BaseImage;

        // cash
        protected Canvas Canvas;
        protected CanvasScaler CanvasScaler;

        // ドラッグ対象
        protected GameObject DraggingObject;
        protected RectTransform DraggingRectTransform;

        // ドラッグイメージ上書き？
        protected bool IsOverrideDragImage;

        protected virtual void Awake()
        {
            Canvas = GetComponentInParent<Canvas>();
            CanvasScaler = Canvas.gameObject.GetComponent<CanvasScaler>();
        }

        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            if (CanDrag() == false)
            {
                return;
            }

            CreateDraggingObject();
            DraggingRectTransform.anchoredPosition = CalcTouchPoint(pointerEventData.position);
        }
        public void OnDrag(PointerEventData pointerEventData)
        {
            if (DraggingRectTransform == null)
            {
                return;
            }

            DraggingRectTransform.anchoredPosition = CalcTouchPoint(pointerEventData.position);
        }
        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if (DraggingRectTransform == null)
            {
                return;
            }

            // 建造物作成
            NotifyOnEndDrag();

            // 破棄
            DestroyDraggingObject();
        }

        private Vector2 CalcTouchPoint(Vector2 touchPos)
        {
#if UNITY_EDITOR
            return touchPos;
#else
            var touchPosX = touchPos.x * (CanvasScaler.referenceResolution.x / Screen.width);
            var touchPosY = touchPos.y * (CanvasScaler.referenceResolution.y / Screen.height);
            return new Vector2(touchPosX, touchPosY);
#endif
        }

        // DraggingObject破棄
        private void DestroyDraggingObject()
        {
            Destroy(DraggingObject);
            DraggingRectTransform = null;
        }

        // ドラッグオブジェクト作成
        private void CreateDraggingObject()
        {
            DraggingObject = Instantiate(DraggingPrefab);
            {
                DraggingObject.SetActive(true);
                DraggingObject.transform.SetParent(Canvas.gameObject.transform);
                DraggingObject.transform.localScale = Vector3.one;

                // override dragImage
                if (IsOverrideDragImage)
                {
                    var draggingImage = DraggingObject.GetComponent<Image>();
                    draggingImage.sprite = BaseImage.sprite;
                    draggingImage.rectTransform.sizeDelta = BaseImage.rectTransform.sizeDelta;
                    draggingImage.color = BaseImage.color;
                    draggingImage.material = BaseImage.material;
                }
            }

            DraggingRectTransform = DraggingObject.GetComponent<RectTransform>();
        }

        /*
         * ドラッグ中（OnDrag）にバックグラウンドに遷移すると、ドラッグ終了（OnEndDrag）が呼ばれない
         * バッググラウンドに遷移した時点で、ドラッグ処理は無効にしてドラッグオブジェクトは破棄
         */
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                DestroyDraggingObject();
            }
        }

        // ドラッグ可能？
        protected virtual bool CanDrag(){ return true; }

        // ドラッグ終了（ドロップ）時の処理
        protected virtual void NotifyOnEndDrag(){}
    }
}