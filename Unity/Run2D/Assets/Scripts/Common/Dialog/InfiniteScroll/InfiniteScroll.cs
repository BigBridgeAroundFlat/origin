using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.Dialog.InfiniteScroll
{
    public class InfiniteScroll : UIBehaviour
    {
        [SerializeField]
        private RectTransform itemPrototype;

        [SerializeField, Range(0, 30)]
        int instantateItemCount = 9;

        public ScrollDirection direction;
        public int margin;

        public OnItemPositionChange onUpdateItem = new OnItemPositionChange();

        [System.NonSerialized]
        public LinkedList<RectTransform> itemList = new LinkedList<RectTransform>();

        protected float diffPreFramePosition = 0;
        protected int currentItemNo = 0;
        protected Vector3 defaultPos = Vector3.zero;

        protected bool isUpdate = false;

        public bool isPlayAction { get; private set; }
        public void ChangeIsPlayAction(bool result)
        {
            isPlayAction = result;
        }


        public enum ScrollDirection
        {
            Vertical,
            Horizontal,
        }

        protected override void Start()
        {
            itemPrototype.gameObject.SetActive(false);
        }

        // cache component

        private RectTransform _rectTransform;
        protected RectTransform rectTransform {
            get {
                if(_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private float anchoredPosition
        {
            get {
                return direction == ScrollDirection.Vertical ? -rectTransform.anchoredPosition.y : rectTransform.anchoredPosition.x;
            }
        }

        private float _itemScale = -1;
        public float itemScale {
            get {
                if(itemPrototype != null && _itemScale == -1) {
                    _itemScale = direction == ScrollDirection.Vertical ? itemPrototype.sizeDelta.y : itemPrototype.sizeDelta.x;
                }
                return _itemScale;
            }
        }

        public void CreateScroll ()
        {
            var controllers = GetComponents<MonoBehaviour>()
                .Where(item => item is IInfiniteScrollSetup)
                .Select(item => item as IInfiniteScrollSetup)
                .ToList();

            // create items
            var scrollRect = GetComponentInParent<ScrollRect>();
            scrollRect.horizontal = direction == ScrollDirection.Horizontal;
            scrollRect.vertical = direction == ScrollDirection.Vertical;
            scrollRect.content = rectTransform;

            itemPrototype.gameObject.SetActive(false);
		
            for(int i = 0; i < instantateItemCount; i++)
            {
                var item = GameObject.Instantiate(itemPrototype) as RectTransform;
                item.SetParent(transform, false);
                item.name = i.ToString();
                if (direction == ScrollDirection.Vertical)
                {
                    item.anchoredPosition = new Vector2(0, -itemScale * i - margin);
                }
                else
                {
                    item.anchoredPosition = new Vector2(itemScale * i + margin, 0);
                }
                itemList.AddLast(item);

                item.gameObject.SetActive(true);

                foreach(var controller in controllers)
                {
                    controller.OnUpdateItem(i, item.gameObject);
                }
            }

            foreach(var controller in controllers){
                controller.OnPostSetupItems();
            }

            isUpdate = true;
            defaultPos = transform.position;
        }

        public void ResetScroll()
        {
            currentItemNo = 0;
            diffPreFramePosition = 0;
            transform.position = defaultPos;
            ;
        }
        public void UpdateCurrentItem()
        {
            int targetCount = currentItemNo;
            foreach (var item in itemList)
            {
                var pos = itemScale * targetCount;
                item.anchoredPosition = (direction == ScrollDirection.Vertical) ? new Vector2(0, -pos- margin) : new Vector2(pos+ margin, 0);

                onUpdateItem.Invoke(targetCount, item.gameObject);
                targetCount++;
            }
        }

        private List<int> addLastTargetIndexList = new List<int>();
        private List<int> addFrontTargetIndexList = new List<int>();

        private bool IsUpdateAddLast()
        {
            return anchoredPosition - diffPreFramePosition - margin < -itemScale * 2;
        }
        private bool IsUpdateAddFront()
        {
            return anchoredPosition - diffPreFramePosition + margin > 0;
        }
        void Update()
        {
            if (isUpdate == false)
            {
                return;
            }

            // スクロール対象取得
            {
                if (IsUpdateAddLast())
                {
                    addFrontTargetIndexList.Clear();

                    while (IsUpdateAddLast())
                    {
                        diffPreFramePosition -= itemScale;
                        addLastTargetIndexList.Add(currentItemNo);
                        currentItemNo++;
                    }

                    while (true)
                    {
                        if (instantateItemCount < addLastTargetIndexList.Count)
                        {
                            addLastTargetIndexList.RemoveAt(0);
                            continue;
                        }

                        break;
                    }
                }
                else if (IsUpdateAddFront())
                {
                    addLastTargetIndexList.Clear();

                    while (IsUpdateAddFront())
                    {
                        diffPreFramePosition += itemScale;
                        currentItemNo--;
                        addFrontTargetIndexList.Add(currentItemNo);
                    }

                    while (true)
                    {
                        if (instantateItemCount < addFrontTargetIndexList.Count)
                        {
                            addFrontTargetIndexList.RemoveAt(0);
                            continue;
                        }

                        break;
                    }
                }
            }

            // update scroll bar
            {
                if (0 < addLastTargetIndexList.Count)
                {
                    var targetCount = addLastTargetIndexList.GetAndRemove(0);

                    var item = itemList.First.Value;
                    itemList.RemoveFirst();
                    itemList.AddLast(item);

                    var pos = itemScale * instantateItemCount + itemScale * targetCount;
                    item.anchoredPosition = (direction == ScrollDirection.Vertical) ? new Vector2(0, -pos- margin) : new Vector2(pos + margin, 0);

                    onUpdateItem.Invoke(targetCount + instantateItemCount, item.gameObject);
                }
                else if (0 < addFrontTargetIndexList.Count)
                {
                    var targetCount = addFrontTargetIndexList.GetAndRemove(0);

                    var item = itemList.Last.Value;
                    itemList.RemoveLast();
                    itemList.AddFirst(item);

                    var pos = itemScale * targetCount;
                    item.anchoredPosition = (direction == ScrollDirection.Vertical) ? new Vector2(0, -pos- margin) : new Vector2(pos + margin, 0);
                    onUpdateItem.Invoke(targetCount, item.gameObject);
                }
            }
        }

        [System.Serializable]
        public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject> {}
    }
}
