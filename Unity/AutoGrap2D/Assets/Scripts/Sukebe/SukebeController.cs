using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Scene;
using Common.Dialog;

public class SukebeController : MonoBehaviour
{
    [SerializeField] private Button _watchButton;
    [SerializeField] private Button _touchButton;
    [SerializeField] private Button _nextPageButton;
    [SerializeField] private Button _prevPageButton;
    [SerializeField] private Image sukebeImage;

    private enum SukebeMode
    {
        None = 0,
        Watch,
        Touch,
    }
    private SukebeMode sukebeMode = SukebeMode.None;
    private void UpdateButton()
    {
        _watchButton.GetComponent<Image>().color = sukebeMode == SukebeMode.Watch ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
        _touchButton.GetComponent<Image>().color = sukebeMode == SukebeMode.Touch ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
    }

    [SerializeField] private Button _targetCancelButton;
    [SerializeField] private Button _targetFaceButton;
    [SerializeField] private Button _targetBustButton;
    [SerializeField] private Button _targetHipButton;
    private enum TargetMode
    {
        None = 0,
        Face,
        Bust,
        Hip,
    }
    private TargetMode targetMode = TargetMode.None;
    private void ChangeTargetMode(TargetMode mode)
    {
        if(targetMode == TargetMode.None)
        {
            if (mode == TargetMode.None)
            {
                return;
            }
        }
        else
        {
            if (mode != TargetMode.None)
            {
                return;
            }
        }

        targetMode = mode;

        switch (targetMode)
        {
            case TargetMode.None:
                sukebeImage.gameObject.transform.localScale = Vector3.one;
                sukebeImage.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                break;

            case TargetMode.Face:
                sukebeImage.gameObject.transform.localScale = Vector3.one * 2;
                sukebeImage.gameObject.transform.localPosition = new Vector3(300, -640, 0);
                break;

            case TargetMode.Bust:
                sukebeImage.gameObject.transform.localScale = Vector3.one * 2;
                sukebeImage.gameObject.transform.localPosition = new Vector3(200, 0, 0);
                break;

            case TargetMode.Hip:
                sukebeImage.gameObject.transform.localScale = Vector3.one * 2;
                sukebeImage.gameObject.transform.localPosition = new Vector3(0,640,0);
                break;
        }
    }


    [SerializeField] private List<Sprite> _spriteList = new List<Sprite>();
    private int _pageNo;
    private void ChangePage(bool isNext)
    {
        if(isNext)
        {
            var max = _spriteList.Count - 1;
            if (_pageNo == max)
            {
                var dialogInfo = new DialogUtility.DialogInfo();
                {
                    dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                    dialogInfo.Message = "CFinish?";
                    dialogInfo.OkCancelButtonCallback = (bool isOk) =>
                    {
                        if(isOk)
                        {
                            TransitionSceneManager.Instance.TransitionScene("Title");
                        }
                    };
                }
                DialogManager.Instance.CreateDialog(dialogInfo);

            }
            else if (_pageNo < max)
            {
                _pageNo++;
            }
        }
        else
        {
            if (0 < _pageNo)
            {
                _pageNo--;
            }
        }

        sukebeImage.sprite = _spriteList[_pageNo];
    }

    void Start ()
    {
        _watchButton.OnClickEtension(() =>
        {
            sukebeMode = SukebeMode.Watch;
            UpdateButton();
        });
        _touchButton.OnClickEtension(() =>
        {
            sukebeMode = SukebeMode.Touch;
            UpdateButton();
        });
        _nextPageButton.OnClickEtension(() =>
        {
            ChangePage(true);
        });
        _prevPageButton.OnClickEtension(() =>
        {
            ChangePage(false);
        });

        _targetCancelButton.OnClickEtension(() =>
        {
            ChangeTargetMode(TargetMode.None);
        }, string.Empty);

        _targetFaceButton.OnClickEtension(() =>
        {
            ChangeTargetMode(TargetMode.Face);
        }, string.Empty);

        _targetBustButton.OnClickEtension(() =>
        {
            ChangeTargetMode(TargetMode.Bust);
        }, string.Empty);

        _targetHipButton.OnClickEtension(() =>
        {
            ChangeTargetMode(TargetMode.Hip);
        }, string.Empty);
    }
}
