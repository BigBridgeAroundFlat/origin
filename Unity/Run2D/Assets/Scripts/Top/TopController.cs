using System;
using Common.Scene;
using DG.Tweening;
using TouchScript.Gestures;
using UnityEngine;

namespace Top
{
    public class TopController : MonoBehaviour
    {
        [SerializeField] private TapGesture tapGesture;
        // Use this for initialization
        void Start ()
        {
            tapGesture.Tapped += OnTapped;
        }

        private void OnTapped(object sender, EventArgs e)
        {
            TransitionSceneManager.Instance.TransitionScene("Novel");
        }
    }
}
