using System.Collections.Generic;
using Common.FrameWork;
using Common.FrameWork.Singleton;
using Common.Scene;
using UnityEngine;

namespace Common.Sound
{
    public class SoundManager : OnlyOneBehavior<SoundManager>
    {
        // BGM
        [SerializeField] private AudioSource _bgmAudioSource = new AudioSource();
        private SoundUtility.BgmType _currentBgmType = SoundUtility.BgmType.None;

        // SE：同時再生最大数 = リスト要素数
        [SerializeField] private List<AudioSource> _seAudioSourceList = new List<AudioSource>();
        private int _currentSeIndex;

        // SE : 外部用、シーン遷移のタイミングで破棄
        private List<AudioSource> _externalAudioSourceList = new List<AudioSource>();

        // SeBank
        private SeBank _commonSeBank;
        private SeBank _sceneSeBank;

        private void Start()
        {
            // 全シーン共通SeBank
            _commonSeBank = GetComponent<SeBank>();

            // カレントシーンのSeBank
            UpdateSceneSeBank();

            // シーン遷移通知
            TransitionSceneManager.Instance.AddNotifyChangeScene(NotifyChanegScene);
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
            UpdateSceneSeBank();
        }

        // シーン固有SeBank更新
        private void UpdateSceneSeBank()
        {
            _sceneSeBank = null;

            var seBankObj = GameObject.FindGameObjectWithTag("SeBank");
            if (seBankObj != null)
            {
                _sceneSeBank = seBankObj.GetComponent<SeBank>();
            }
        }

        // 外部用AudioSource
        public AudioSource CreateExternalAudioSource()
        {
            var exterAudioSource = gameObject.AddComponent<AudioSource>();
            _externalAudioSourceList.Add(exterAudioSource);
            return exterAudioSource;
        }

        #region bgm

        public void PlayBgm(SoundUtility.BgmType type)
        {
            if (_currentBgmType == type)
            {
                return;
            }

            // 変更前のBGM停止
            _bgmAudioSource.Stop();

            // 再生
            var filePath = SoundUtility.GetBgmFilePath(type);
            var audioClip = ResourceManager.LoadAudioClip(filePath);
            _bgmAudioSource.clip = audioClip;
            _bgmAudioSource.loop = true;
            _bgmAudioSource.volume = 1.0f;
            _bgmAudioSource.Play();

            // 変更
            _currentBgmType = type;
        }
        public void StopBgm()
        {
            _bgmAudioSource.Stop();
        }

        #endregion

        #region se

        public void PlaySe(string fileName, AudioSource audioSource = null)
        {
            AudioClip seAudioClip = null;
            {
                // scene
                if (_sceneSeBank != null)
                {
                    seAudioClip = _sceneSeBank.GetAudioClip(fileName);
                }

                // common
                if (seAudioClip == null && _commonSeBank != null)
                {
                    seAudioClip = _commonSeBank.GetAudioClip(fileName);
                }
            }

            if (seAudioClip == null)
            {
                Debug.Assert(false, "Play SE Error : fileName = " + fileName);
                return;
            }

            // play audio
            {
                // SoundManagerのAudioSourceをリングバッファで使いまわし
                if (audioSource == null)
                {
                    var seAudioSource = _seAudioSourceList[_currentSeIndex];
                    if (seAudioSource == null)
                    {
                        return;
                    }

                    // 前のSE停止
                    seAudioSource.Stop();

                    // SE再生
                    seAudioSource.clip = seAudioClip;
                    seAudioSource.volume = 1.0f;
                    seAudioSource.Play();

                    //「SeAudioSourceList」 のインデックス更新
                    _currentSeIndex++;
                    if (_seAudioSourceList.Count <= _currentSeIndex)
                    {
                        _currentSeIndex = 0;
                    }
                }
                // 引数でAudioSource指定
                else
                {
                    // 前のSE停止
                    audioSource.Stop();

                    // SE再生
                    audioSource.clip = seAudioClip;
                    audioSource.volume = 1.0f;
                    audioSource.Play();
                }
            }
        }

        #endregion
    }

}//Common