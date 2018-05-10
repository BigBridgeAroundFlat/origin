using System.Collections.Generic;
using Common.FrameWork;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Common.Sound
{
    public class BgmFade : MonoBehaviour
    {
        const float FadeTime = 2.0f;

        /*
         * BGMフェード情報
         */ 
        private class BgmFadeInfo
        {
            public BgmFadeInfo(FadeUtility.FadeState state)
            {
                State = state;
            }

            public FadeUtility.FadeState State;
            public float Time;
        }
        private readonly Dictionary<int, BgmFadeInfo> _bgmFadeInfoDic = new Dictionary<int, BgmFadeInfo>();

        private AudioSource _targetBgmAudioSource;
        public void SetTargetBgmAudioSource(AudioSource bgm)
        {
            _targetBgmAudioSource = bgm;
        }

        private void Start()
        {
            /*
             * BGMフェード更新
             */
            this.UpdateAsObservable()
                .Where(x => 0 < _bgmFadeInfoDic.Count)
                .Subscribe(_ => UpdateBgmFade())
                .AddTo(this);
        }

        #region update bgm fade

        /*
         * BGMフェード処理
         */
        private void UpdateBgmFade()
        {
            // update & delete
            {
                var deleteList = new List<int>();
                foreach (var pair in _bgmFadeInfoDic)
                {
                    int key = pair.Key;
                    if (UpdateFade(key))
                    {
                        deleteList.Add(key);
                    }
                }

                foreach (var key in deleteList)
                {
                    _bgmFadeInfoDic.Remove(key);
                }
            }
        }
        private bool UpdateFade(int key)
        {
            var info = _bgmFadeInfoDic[key];
            if (_targetBgmAudioSource == null || info == null)
            {
                // dictionary から削除
                return true;
            }

            // volume fade
            {
                switch (info.State)
                {
                    case FadeUtility.FadeState.FadeinStart:
                    {
                        info.Time += Time.deltaTime;
                        _targetBgmAudioSource.volume = info.Time / FadeTime;
                        if (info.Time >= FadeTime)
                        {
                            _targetBgmAudioSource.volume = 1.0f;
                            info.State = FadeUtility.FadeState.FadeinFinish;
                            return true;
                        }
                    }
                        break;

                    case FadeUtility.FadeState.FadeoutStart:
                    {
                        info.Time += Time.deltaTime;
                        _targetBgmAudioSource.volume = 1.0f - (info.Time / FadeTime);
                        if (info.Time >= FadeTime)
                        {
                            _targetBgmAudioSource.volume = 0.0f;
                            info.State = FadeUtility.FadeState.FadeoutFinish;
                            return true;
                        }
                    }
                        break;

                    default:
                    {
                        Debug.Assert(false, "BgmFade::updateFade : info.state = " + info.State.ToString());
                    }
                        break;
                }
            }

            return false;
        }

        #endregion

        #region bgm dictionary

        public void BgmFadeIn(int index)
        {
            AddBgmStateDictionary(index, FadeUtility.FadeState.FadeinStart);
        }
        public void BgmFadeOut(int index)
        {
            AddBgmStateDictionary(index, FadeUtility.FadeState.FadeoutStart);
        }
        private void AddBgmStateDictionary(int index, FadeUtility.FadeState state)
        {
            // 上書き
            foreach (int key in _bgmFadeInfoDic.Keys)
            {
                if (key == index)
                {
                    _bgmFadeInfoDic[key] = new BgmFadeInfo(state);
                    return;
                }
            }

            _bgmFadeInfoDic.Add(index, new BgmFadeInfo(state));
        }

        #endregion
    }

}// Common