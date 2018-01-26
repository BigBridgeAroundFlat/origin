using Common.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace Top
{
    public class TopController : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.OnClickEtension(() =>
            {
                TransitionSceneManager.Instance.TransitionScene("CharacterSelect");
            });
        }
    }
}
