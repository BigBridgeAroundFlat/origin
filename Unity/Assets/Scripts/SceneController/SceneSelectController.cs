using Common.Scene;
using System.Linq;
using Common.Dialog;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SceneController
{
    public class SceneSelectController : MonoBehaviour
    {
        [SerializeField] private GameObject _buttonParent;
        [SerializeField] private Button _closeButton;

        // Use this for initialization
        void Start ()
        {
            _closeButton.OnClickEtension(() =>
            {
                var dialogInfo = new DialogUtility.DialogInfo();
                {
                    dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                    dialogInfo.Message = "Finish Scene Select ?";
                    dialogInfo.OkCancelButtonCallback = (bool isOk) =>
                    {
                        if (isOk)
                        {
                            TransitionSceneManager.Instance.TransitionScene("Title");
                        }
                    };
                }
                DialogManager.Instance.CreateDialog(dialogInfo);
            });

            var buttonList = _buttonParent.Descendants().OfComponent<Button>().ToList();
            foreach (var button in buttonList)
            {
                var buttonTemp = button;
                button.OnClickEtension(() =>
                {
                    var novelType = GameInfoManager.NovelInfo.NovelType.None;
                    var novelNo = 0;

                    var objName = buttonTemp.gameObject.name;
                    if (objName.StartsWith("Special"))
                    {
                        novelType = GameInfoManager.NovelInfo.NovelType.Special;
                        objName = objName.Remove(0, 7);
                        novelNo = int.Parse(objName);
                    }
                    else if (objName.StartsWith("Normal"))
                    {
                        novelType = GameInfoManager.NovelInfo.NovelType.Normal;
                        objName = objName.Remove(0, 6);
                        novelNo = int.Parse(objName);
                    }

                    if (novelType != GameInfoManager.NovelInfo.NovelType.None && novelNo != 0)
                    {
                        var novelInfo = new GameInfoManager.NovelInfo();
                        {
                            novelInfo.Type = novelType;
                            novelInfo.No = novelNo;
                            novelInfo.IsSceneView = true;
                        }
                        GameInfoManager.SetCurrentNovelInfo(novelInfo);

                        // シーン遷移
                        TransitionSceneManager.Instance.TransitionScene("Novel");
                    }
                });
            }
        }

    }
}
