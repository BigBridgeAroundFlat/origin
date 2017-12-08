using Define;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using UnityEngine;
using Engine;

namespace Dialog
{
    public class DialogManager : OnlyOneBehavior<DialogManager>
    {
        // ref
        public Canvas ScreenSpaceOverlayCanvas;
        [SerializeField] private float _modalTargetAlpha = 1.0f;
        public float GetModalTargetAlpha()
        {
            return _modalTargetAlpha;
        }

        // 表示ダイアログリスト
        private readonly List<GameObject> _createDialogList = new List<GameObject>();
        private readonly List<GameObject> _dontDestroyDialogList = new List<GameObject>();
        public int GetCreateDialogListCount()
        {
            return _createDialogList.Count;
        }

        // コールバック：ダイアログ状態通知
        private CommonData.IntCallback _notifyChangeDialogCountCallbackCallback;
        public void SetNotifyChangeDialogActiveCountCallbackCallback(CommonData.IntCallback callback)
        {
            _notifyChangeDialogCountCallbackCallback = callback;
        }

        private void Start()
        {
            var canvas = GetComponentInParent<Canvas>();
            GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        }
        public bool PushBackKey()
        {
            if (_createDialogList.Count == 0)
            {
                return false;
            }

            var dialogObj = _createDialogList[_createDialogList.Count - 1];
            var dialogBaseScript = dialogObj.DescendantsAndSelf().OfComponent<DialogBase>().FirstOrDefault();
            dialogBaseScript.PushBackKey();
            return true;
        }

#region create dialog

        public bool CreateDialog(DialogParam.DialogType dialogType)
        {
            return CreateDialog(new DialogData.DialogInfo(dialogType));
        }
        public bool CreateDialog(DialogData.DialogInfo dialogInfo)
        {
            // 一番手前に表示されてたら作成不要
            {
                if (0 < _createDialogList.Count)
                {
                    var dialogObj = _createDialogList[_createDialogList.Count - 1];
                    var dialogBaseScript = dialogObj.DescendantsAndSelf().OfComponent<DialogBase>().FirstOrDefault();
                    var dialogType = dialogBaseScript.GetDialogType();
                    if (dialogType == dialogInfo.DialogType)
                    {
                        return false;
                    }
                }
            }

            return CreateDialogCore(dialogInfo);
        }
        private bool CreateDialogCore(DialogData.DialogInfo info)
        {
            var dialogObj = GetDontDestroyDialog(info);

            // 新規作成
            if (dialogObj == null)
            {
                dialogObj = InstantiateDialog(info);
                if (dialogObj == null)
                {
                    Debug.Assert(false, "DialogManager.CreateDialog : type = " + info.DialogType);
                    return false;
                }
            }
            // 使いまわし
            else
            {
                dialogObj.SetActive(true);
            }

            AddChildDialog(dialogObj, info);

            // ダイアログ初期化
            var dialogBaseScript = dialogObj.DescendantsAndSelf().OfComponent<DialogBase>().FirstOrDefault();
            dialogBaseScript.Init(info);

            // モーダル更新
            UpdateDialogModalEnable();
            return true;
        }
        private GameObject GetDontDestroyDialog(DialogData.DialogInfo info)
        {
            return _dontDestroyDialogList.FirstOrDefault(x=>x.GetComponent<DialogBase>().GetDialogType() == info.DialogType);
        }
        private GameObject InstantiateDialog(DialogData.DialogInfo info)
        {
            GameObject dialogObj = null;

            var filePath = string.Empty;
            {
                switch (info.DialogType)
                {
                    case DialogParam.DialogType.OkDialog: filePath = "Prefabs/Dialog/OkDialog"; break;
                    case DialogParam.DialogType.OkCancelDialog: filePath = "Prefabs/Dialog/OkCancelDialog"; break;
                }
            }

            // ResourceからロードしてInstantiate
            dialogObj = ResourceManager.LoadAndInstantiate(filePath);
            return dialogObj;
        }

#endregion

#region dialog list

        private void AddChildDialog(GameObject obj, DialogData.DialogInfo info)
        {
            obj.SetActive(true);
            obj.transform.SetParent(gameObject.transform, false);
            _createDialogList.Add(obj);

            // 通知：ダイアログ追加
            {
                if (_notifyChangeDialogCountCallbackCallback != null)
                {
                    _notifyChangeDialogCountCallbackCallback(_createDialogList.Count);
                }
            }
        }
        public void NotifyCloseDialog(GameObject dialogObj)
        {
            if (_createDialogList.Remove(dialogObj))
            {
                UpdateDialogModalEnable();

                // 通知：ダイアログ除去
                {
                    if (_notifyChangeDialogCountCallbackCallback != null)
                    {
                        _notifyChangeDialogCountCallbackCallback(_createDialogList.Count);
                    }
                }
            }
        }
        private void UpdateDialogModalEnable()
        {
            var openDialogType = DialogParam.DialogType.None;

            var max = _createDialogList.Count;
            for (var index = 0; index < max; index++)
            {
                // 一番手前のダイアログのModalだけ有効
                var isEnable = index == max - 1;

                var dialogObj = _createDialogList[index];
                var dialogBaseScript = dialogObj.DescendantsAndSelf().OfComponent<DialogBase>().FirstOrDefault();
                dialogBaseScript.ChangeEnableModalImage(isEnable);

                // 通知：一番手前に表示
                if (isEnable)
                {
                    openDialogType = dialogBaseScript.GetDialogType();
                }
            }
        }

        #endregion

        #region input unenable

        public void RequestChangeInputEnable(bool isEnable)
        {
            if (isEnable)
            {
                InputManager.Instance.EnableAllInput("dialog");
            }
            else
            {
                InputManager.Instance.DisableAllInput("dialog");
            }
        }

        #endregion
    }
}
