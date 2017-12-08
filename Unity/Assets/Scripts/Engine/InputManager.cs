using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Engine
{
    public class InputManager : SingletonMonoBehaviour<InputManager> 
	{
	    public enum TouchState
	    {
	        None,
	        Down,
	        Move,
	        Up
	    }
	    public TouchState State{ get; protected set; }


		const int FINGER_ID_NONE = -1;

	    public Vector3 TouchPosition{ get; protected set; }

        public float TouchMoveLen{ get; protected set; }
		Vector3 lastTouchPosition;
		int touchFingerId = FINGER_ID_NONE;

		Vector2 TouchOnePosition = new Vector2(0, 0);
		Vector2 prevTouchOnePosition = new Vector2(0, 0);
		int touchOneFingerId = FINGER_ID_NONE;

		protected RaycastHit hit;

	    protected bool isDownInCurrentFrame;

        public bool IsMultiTouch{ get; private set; }
		public float PinchLen{ get; private set; }

		public bool IsBackKeyTouch{ get; private set; }
		bool IsEnableInput = true;

        // 入力有効・無効コマンドリスト
        private List<string> _disableInputCommandList = new List<string>();

        protected override void Init()
        {
            hit = new RaycastHit();
        }

		Vector2 GetScreenRatio( Vector2 position)
		{
			Vector2 ratio;

			int width = Screen.currentResolution.width;
			int height = Screen.currentResolution.height;

			ratio.x = position.x / width;
			ratio.y = position.y / height;
			return ratio;
		}

	    void Update () 
	    {
	        //reset state
	        State = TouchState.None;
	        isDownInCurrentFrame = false;

			IsMultiTouch = HandleTouch();

			//
			// Escapeキー（AndroidのBackkey）
			// どこかTouchしている場合は常に無効
			//
			if (State == TouchState.None) {
				IsBackKeyTouch = HandleKeyTouch();
			} else {
				IsBackKeyTouch = false;
			}
		}

		Dictionary<int, int> fingerIds = new Dictionary<int, int>();
		bool HandleTouch()
		{
			// 入力のFingerIdを取得
			fingerIds.Clear();
			int index = 0;
			foreach (Touch touch in Input.touches)
			{
				fingerIds.Add(touch.fingerId, index++);
			}
			index = 0;
			// 先にシングルタッチ処理
			if (Input.touchCount > 0)
			{
				if (touchFingerId != FINGER_ID_NONE){
					if(fingerIds.ContainsKey(touchFingerId)){
						index = fingerIds[touchFingerId];
						//Debug.Log("find Finger id : "+touchFingerId);

					} else {
						// 該当IDが無いので、一旦入力を終わらせる
						index = -1;
						//Debug.Log("none Finger id : "+touchFingerId);
					}
				}
				if(index == -1){
					Touch touch = Input.GetTouch(0);
					HandleTouch(touch.fingerId, touch.position, TouchPhase.Ended);
				} else {
					Touch touch = Input.GetTouch(index);
					HandleTouch(touch.fingerId, touch.position, touch.phase);
				}
			}

			// ２点以上は無視
			if (Input.touchCount >= 2 && touchFingerId != FINGER_ID_NONE)
			{
				// singleとは違うIndex
				int idx = (index == 1) ? 0 : 1;
				if (touchOneFingerId != FINGER_ID_NONE)
				{
					if (fingerIds.ContainsKey(touchOneFingerId))
					{
						idx = fingerIds[touchOneFingerId];
					}
				}

				// ２点目だけ取得
				prevTouchOnePosition = TouchOnePosition;
				// Store both touches.
				Touch touchOne = Input.GetTouch(idx);
				if (prevTouchOnePosition == Vector2.zero)
					prevTouchOnePosition = touchOne.position;
				TouchOnePosition = touchOne.position;
				touchOneFingerId = touchOne.fingerId;

				//Debug.Log("m : "+touchOneFingerId+" : "+idx);

				Vector2 ratioPrevZero = GetScreenRatio(lastTouchPosition);
				Vector2 ratioPrevOne = GetScreenRatio(prevTouchOnePosition);
				Vector2 ratioZero = GetScreenRatio(TouchPosition);
				Vector2 ratioOne = GetScreenRatio(TouchOnePosition);

				float prevTouchDeltaMag = (ratioPrevZero - ratioPrevOne).magnitude;
				float touchDeltaMag = (ratioZero - ratioOne).magnitude;

				PinchLen = (prevTouchDeltaMag - touchDeltaMag) * 500.0f;

				// ２点目によるState遷移：Endedは取らない
				switch (touchOne.phase)
				{
					case TouchPhase.Began:
						State = TouchState.Down;
						isDownInCurrentFrame = true;
						break;

					case TouchPhase.Moved:
						if (!isDownInCurrentFrame)
						{
							State = TouchState.Move;
						}
						break;
					case TouchPhase.Ended:
						touchOneFingerId = FINGER_ID_NONE;
						break;
				}
				//Debug.Log ("pinch : "+PinchLen);
				return true;
			}
			else
			{
				if (Input.touchCount == 0)
				{
					if (Input.GetMouseButtonDown(0))
					{
						HandleTouch(10, Input.mousePosition, TouchPhase.Began);
					}
					if (Input.GetMouseButton(0))
					{
						HandleTouch(10, Input.mousePosition, TouchPhase.Moved);
					}
					if (Input.GetMouseButtonUp(0))
					{
						HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
					}
					// 何も入力がなかったらクリア
					if(State == TouchState.None){
						lastTouchPosition = Vector2.zero;
						TouchPosition = Vector2.zero;
						touchFingerId = FINGER_ID_NONE;
					}
				}
				PinchLen = 0;
				prevTouchOnePosition = Vector2.zero;
				TouchOnePosition = Vector2.zero;
				touchOneFingerId = FINGER_ID_NONE;
				return false;
			}

		}

	    void HandleTouch(int _touchFingerId, Vector3 _touchPosition, TouchPhase _touchPhase) 
	    {
			lastTouchPosition = TouchPosition;
			TouchPosition = _touchPosition;
			touchFingerId = _touchFingerId;

	        switch (_touchPhase) 
	        {
	            case TouchPhase.Began:
	                State = TouchState.Down;
	                isDownInCurrentFrame = true;
					//lastTouchPosition = TouchPosition;
					TouchMoveLen = 0.0f;
	                break;

	            case TouchPhase.Moved:
	                if(!isDownInCurrentFrame)
	                {
	                    State = TouchState.Move;
	                }
					TouchMoveLen += Vector3.Distance(lastTouchPosition,TouchPosition);
					//lastTouchPosition = TouchPosition;
	                break;

	            case TouchPhase.Ended:
	                State = TouchState.Up;
					touchFingerId = FINGER_ID_NONE;
	                break;
	        }
	    }

		bool HandleKeyTouch()
		{
			if (!IsEnableInput) {
				return false;
			}
			return Input.GetKeyUp (KeyCode.Escape);
		}


		public RaycastHit GetRayHit(int layerMask = -1,Camera camera = null)
	    {
			if(camera == null) camera = Camera.main;
			Ray ray = camera.ScreenPointToRay(TouchPosition);
	        Physics.Raycast(ray, out hit, 2000, layerMask);
	        return hit;
	    }

		public RaycastHit[] GetRayHitAll(int layerMask = -1, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			Ray ray = camera.ScreenPointToRay(TouchPosition);
			var hits = Physics.RaycastAll(ray, 2000, layerMask);
			return hits;
		}

		public GameObject TouchObject2D ()
		{
			Vector2 tapPoint = Camera.main.ScreenToWorldPoint (TouchPosition);
			Collider2D collider = Physics2D.OverlapPoint (tapPoint);
			if (collider != null) {
				return collider.transform.gameObject;
			}
			return null;
		}


        public GameObject[] TouchObject2DAll ()
        {
            Vector2 tapPoint = Camera.main.ScreenToWorldPoint (TouchPosition);
            Collider2D[] colliders = Physics2D.OverlapPointAll(tapPoint);
            if (colliders != null && colliders.Length > 0) 
            {
                GameObject[] ret = new GameObject[ colliders.Length ];
                for (int i = 0; i < colliders.Length; i++)
                {
                    ret[i] = colliders[i].transform.gameObject;
                }
                return ret;
            }
            return null;
        }



        public Vector2 ViewportPosition()
		{
			return ViewportPosition(TouchPosition);
		}

		public Vector2 ViewportPosition(Vector3 v)
		{
			return new Vector2(
				v.x / Screen.width,
				v.y / Screen.height
			);
		}

		public Vector2 ViewportMove()
		{
			if(State == TouchState.Move)
			{
                return (TouchPosition - lastTouchPosition) / Screen.height;
				//return ViewportPosition(TouchPosition) - ViewportPosition(lastTouchPosition);
			}
			return Vector2.zero;
		}


        public float DeltaTouchMoveLength()
        {
            return (TouchPosition - lastTouchPosition).magnitude;
        }

        public Vector3 TouchMove()
        {
            return (TouchPosition - lastTouchPosition);
        }


        public bool IsPointerOverUIObject() 
        {
            //TODO
            //Is it sure? return true?
            if (EventSystem.current == null) return true;

            // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
            // the ray cast appears to require only eventData.position.
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }


		// for Multitouch position
		public Vector2 ViewportOnePosition()
		{
			return ViewportPosition(TouchOnePosition);
		}
		public Vector2 ViewportOneMove()
		{
			return (TouchOnePosition - prevTouchOnePosition) / Screen.height;
		}


		public void DisableAllInput(string key)
        {
            // 無効・有効命令：1対1
            {
                _disableInputCommandList.Add(key);
            }

            // 入力無効
            {
                EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
                if (eventSystem != null)
                {
                    eventSystem.enabled = false;
                }
                IsEnableInput = false;
            }
        }

        public void EnableAllInput(string key)
        {
            // 無効命令が残っているならreturn
            {
                _disableInputCommandList.Remove(key);
                if (0 < _disableInputCommandList.Count)
                {
                    return;
                }
            }

            // 入力有効
            {
                EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
                if (eventSystem != null)
                {
                    eventSystem.enabled = true;
                }
                IsEnableInput = true;
            }
        }


    }
}