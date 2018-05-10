using Common.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace SceneController
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _sceneButton;
        [SerializeField] private Button _animeButton;

        // Use this for initialization
        void Start ()
        {
            _startButton.OnClickEtension(PushStartButton);
            _sceneButton.OnClickEtension(PushSceneButton);
            _animeButton.OnClickEtension(PushAnimeButton);
        }

        private void UnenableAllButton()
        {
            _startButton.enabled = false;
            _sceneButton.enabled = false;
            _animeButton.enabled = false;
        }

        private void PushStartButton()
        {
            UnenableAllButton();
            TransitionSceneManager.Instance.TransitionScene("CharacterSelect");
        }
        private void PushSceneButton()
        {
            UnenableAllButton();
            TransitionSceneManager.Instance.TransitionScene("SceneSelect");
        }
        private void PushAnimeButton()
        {
            UnenableAllButton();
            TransitionSceneManager.Instance.TransitionScene("AnimationViewer");
        }
    }
}
