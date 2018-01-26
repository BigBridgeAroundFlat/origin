using Common.Dialog;
using DG.Tweening;
using UnityEngine;

namespace Common.Scene
{
    public class ResidentSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _dummyCamera;
        private void Start()
        {
            SceneUtility.InitializeDummyCamera(_dummyCamera);
        }
    }
}
