using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Title
{
    public class TitleController : MonoBehaviour
    {
        public List<Button> ButtonList = new List<Button>();

        // Use this for initialization
        void Start()
        {
            Debug.Log("start");
            var count = 0;
            foreach (var button in ButtonList)
            {
                count++;
                var no = count;
                button.OnClickEtension(() =>
                {
                    PushButton(no);
                });

            }
        }

        private void PushButton(int no)
        {
            switch (no)
            {
                case 1:
                    {
                        Engine.TransitionSceneManager.Instance.TransitionScene("Battle");
                    }
                    break;

                case 2:
                    {
                        var dialogInfo = new Define.DialogData.DialogInfo();
                        {
                            dialogInfo.DialogType = Define.DialogParam.DialogType.OkDialog;
                            dialogInfo.Message = "Coming Soon";
                            dialogInfo.UnenableCancelButton = true;
                        }
                        Dialog.DialogManager.Instance.CreateDialog(dialogInfo);
                    }
                    break;

                case 3:
                    {
                        Application.Quit();
                    }
                    break;
            }
        }
    }
}

