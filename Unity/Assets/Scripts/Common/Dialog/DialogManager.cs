using System.Collections.Generic;
using Common.FrameWork;
using Common.FrameWork.Singleton;
using Common.Scene;
using Unity.Linq;
using UnityEngine;

namespace Common.Dialog
{
    public class DialogManager : OnlyOneBehavior<DialogManager>
    {
        // ref
        [SerializeField] private GameObject _dialogParent;
        [SerializeField] private float _modalTargetAlpha = 1.0f;
        public float GetModalTargetAlpha()
        {
            return _modalTargetAlpha;
        }

        // 表示ダイアログリスト
        private readonly List<GameObject> _createDialogList = new List<GameObject>();
        public int GetCreateDialogListCount()
        {
            return _createDialogList.Count;
        }

        private void Start()
        {
            // シーン遷移通知
            TransitionSceneManager.Instance.AddNotifyChangeScene(NotifyChanegScene);

            // DialogCanvas設定
            UpdateDialogCanvas();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (TransitionSceneManager.Instance != null)
            {
                TransitionSceneManager.Instance.RemoveNotifyChangeScene(NotifyChanegScene);
            }
        }

        // シーン遷移通知
        private void NotifyChanegScene(string sceneName)
        {
            UpdateDialogCanvas();
        }

        // DialogCanvas設定
        private void UpdateDialogCanvas()
        {
            _dialogParent = GameObject.FindGameObjectWithTag("DialogCanvas");
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

        public bool CreateDialog(DialogUtility.DialogType dialogType)
        {
            return CreateDialog(new DialogUtility.DialogInfo(dialogType));
        }
        public bool CreateDialog(DialogUtility.DialogInfo dialogInfo)
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
        private bool CreateDialogCore(DialogUtility.DialogInfo info)
        {
            if (AddChildDialog(info) == false)
            {
                return false;
            }

            // モーダル更新
            UpdateDialogModalEnable();
            return true;
        }
        private GameObject InstantiateDialog(DialogUtility.DialogInfo info)
        {
            var filePath = string.Empty;
            {
                switch (info.DialogType)
                {
                    case DialogUtility.DialogType.MessageDialog: filePath = "Prefabs/Dialog/OkCancelDialog"; break;
                }
            }

            var dialogObj = ResourceManager.LoadAndInstantiate(filePath);
            return dialogObj;
        }

#endregion

#region dialog list

        private bool AddChildDialog(DialogUtility.DialogInfo info)
        {
            var dialogObj = InstantiateDialog(info);
            if (dialogObj == null)
            {
                Debug.Assert(false, "DialogManager.CreateDialog : type = " + info.DialogType);
                return false;
            }

            if (_dialogParent == null)
            {
                Debug.Assert(false, "DialogManager.CreateDialog : _dialogParent = null");
                return false;
            }

            // parent設定
            dialogObj.SetActive(true);
            dialogObj.transform.SetParent(_dialogParent.transform, false);
            _createDialogList.Add(dialogObj);

            // ダイアログ初期化
            var dialogBaseScript = dialogObj.DescendantsAndSelf().OfComponent<DialogBase>().FirstOrDefault();
            dialogBaseScript.Init(info);
            return true;
        }
        public void NotifyCloseDialog(GameObject dialogObj)
        {
            if (_createDialogList.Remove(dialogObj))
            {
                UpdateDialogModalEnable();
            }
        }
        private void UpdateDialogModalEnable()
        {
            var max = _createDialogList.Count;
            for (var index = 0; index < max; index++)
            {
                // 一番手前のダイアログのModalだけ有効
                var isEnable = index == max - 1;

                var dialogObj = _createDialogList[index];
                var dialogBaseScript = dialogObj.DescendantsAndSelf().OfComponent<DialogBase>().FirstOrDefault();
                dialogBaseScript.ChangeEnableModalImage(isEnable);
            }
        }

        #endregion

        #region utility

        public void FrameOutAllCreateDialog()
        {
            foreach (var createDialog in _createDialogList)
            {
                createDialog.GetComponent<DialogBase>().FrameOut();
            }
            _createDialogList.Clear();
        }

#endregion
    }
}
