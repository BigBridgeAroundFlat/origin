using Common.Scene;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Novel
{
    public class NovelController : MonoBehaviour
    {
        public void ChangeMainScene()
        {
            TransitionSceneManager.Instance.TransitionScene("Main");
        }

        public void SaveIntValue(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
    }
}
