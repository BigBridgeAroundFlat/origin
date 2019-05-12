using System.Collections.Generic;
using Battle;
using System.Linq;
using Common.Dialog;
using Common.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace AnimationViewer
{
    public class AnimationViewerController : MonoBehaviour
    {
        [SerializeField] private ViewerCharacter _viewerCharacter;
        [SerializeField] private ToggleGroup _chracterToggleGroup;
        [SerializeField] private List<Toggle> _chracterToggleList;
        [SerializeField] private ToggleGroup _animationToggleGroup;
        [SerializeField] private List<Toggle> _animationToggleList;
        [SerializeField] private Button _closeButton;

        // Use this for initialization
        private void Start ()
        {
            foreach (var chracterToggle in _chracterToggleList)
            {
                chracterToggle.onValueChanged.AddListener((bool isOn) =>
                {
                    if (isOn)
                    {
                        PlayAnimation();
                    }
                });
            }
            foreach (var animationToggle in _animationToggleList)
            {
                animationToggle.onValueChanged.AddListener((bool isOn) =>
                {
                    if (isOn)
                    {
                        PlayAnimation();
                    }
                });
            }

            _closeButton.OnClickEtension(() =>
            {
                var dialogInfo = new DialogUtility.DialogInfo();
                {
                    dialogInfo.DialogType = DialogUtility.DialogType.MessageDialog;
                    dialogInfo.Message = "Finish Animation View ?";
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
        }

        private void PlayAnimation()
        {
            // character type
            {
                var characterType = CalCharacterType();
                _viewerCharacter.SetCharacterType(characterType);
            }

            // animation
            {
                var characterState = CalCharacterState();
                _viewerCharacter.PlayAnimatin(characterState);
            }
        }

        private GameInfoManager.CharacterType CalCharacterType()
        {
            var type = GameInfoManager.CharacterType.None;

            var toggle = _chracterToggleGroup.ActiveToggles().FirstOrDefault();
            if (toggle)
            {
                switch (toggle.name)
                {
                    case "Kohaku": type = GameInfoManager.CharacterType.Kohaku; break;
                    case "Heroine": type = GameInfoManager.CharacterType.Heroine; break;
                }
            }

            return type;
        }
        private Character.CharacterState CalCharacterState()
        {
            var type = Character.CharacterState.Idle;

            var toggle = _animationToggleGroup.ActiveToggles().FirstOrDefault();
            if (toggle)
            {
                switch (toggle.name)
                {
                    case "Idle": type = Character.CharacterState.Idle; break;
                    case "Move": type = Character.CharacterState.Move; break;
                    case "Jump": type = Character.CharacterState.Jump; break;
                    case "Damage": type = Character.CharacterState.Damage; break;
                    case "Down": type = Character.CharacterState.Down; break;
                    case "Attack1": type = Character.CharacterState.Attack1; break;
                    case "Attack2": type = Character.CharacterState.Attack2; break;
                    case "Attack3": type = Character.CharacterState.Attack3; break;
                    case "Appeal": type = Character.CharacterState.Appeal; break;
                }
            }

            return type;
        }

    }
}
