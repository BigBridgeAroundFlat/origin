using Define;
using System.Collections.Generic;
using UnityEngine;
using Engine;

namespace Sound
{
    public class SoundManager : Engine.SingletonMonoBehaviour<SoundManager>
    {
        // BGM
        [SerializeField] private AudioSource _bgmAudioSource = new AudioSource();
        private SoundParam.BgmType _currentBgmType = SoundParam.BgmType.None;

        // SE：同時再生最大数 = リスト要素数
        [SerializeField] private List<AudioSource> _seAudioSourceList = new List<AudioSource>();
        private int _currentSeIndex;

        // SE : 外部用、シーン遷移のタイミングで破棄
        private List<AudioSource> _externalAudioSourceList = new List<AudioSource>();

        // SeBank
        private SeBank _commonSeBank;
        private SeBank _sceneSeBank;


        private void OnEnable()
        {
            // 全シーン共通SeBank設定
            _commonSeBank = GetComponent<SeBank>();

            // 現在のシーンのSEBank更新
            UpdateSceneSeBank();

            // シーン遷移コールバック
            TransitionSceneManager.Instance.SetChangeSceneCallback(NotifyChangeScene);
        }

        // シーン遷移通知
        public void NotifyChangeScene(bool isStart, string preSceneName, string changeSceneName)
        {
            // シーン遷移直前
            if (isStart)
            {
                // 外部用AudioSource破棄
                {
                    foreach (var externalAudioSource in _externalAudioSourceList)
                    {
                        Destroy(externalAudioSource);
                    }
                    _externalAudioSourceList.Clear();
                }
            }
            // シーン遷移直後
            else
            {
                UpdateSceneSeBank();
            }
        }

        // シーン固有SeBank更新
        private void UpdateSceneSeBank()
        {
            var obj = GameObject.FindGameObjectWithTag("SeBank");
            if (obj != null)
            {
                _sceneSeBank = obj.GetComponent<SeBank>();
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

        public void PlayBgm(SoundParam.BgmType type)
        {
            if (_currentBgmType == type)
            {
                return;
            }

            // 変更前のBGM停止
            _bgmAudioSource.Stop();

            // 再生
            var filePath = SoundFunc.GetBgmFilePath(type);
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
            return;

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