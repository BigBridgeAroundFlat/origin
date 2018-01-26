using System.Collections;
using System.Collections.Generic;
using Common.FrameWork;
using Common.FrameWork.Singleton;
using UniRx;
using UnityEngine;

namespace Common.Scene
{
    public class TransitionSceneManager : OnlyOneBehavior<TransitionSceneManager>
    {
        private const float TransitionSceneFadeInterval = 0.5f;

        // 黒テクスチャ
        private Texture2D _blackTexture;

        // フェード情報
        private float _alpha;
        private FadeUtility.FadeState _fadeState = FadeUtility.FadeState.None;
        private float _fadeInterval = TransitionSceneFadeInterval;

        // シーン遷移ログ
        private readonly List<string> _preSceneNameList = new List<string>();

        // シーン遷移完了通知
        public delegate void NotifyChangeScene(string sceneName);
        private NotifyChangeScene _notifyChangeScene;
        public void AddNotifyChangeScene(NotifyChangeScene callback)
        {
            _notifyChangeScene += callback;
        }
        public void RemoveNotifyChangeScene(NotifyChangeScene callback)
        {
            _notifyChangeScene -= callback;
        }

        #region common

        /*
        * 初期化
        * ・CreateBlackTexture：黒テクスチャ作成
        * ・OnGUI：黒テクスチャのα値変更
        */
        private void Start()
        {
            CreateBlackTexture();
        }
        void CreateBlackTexture()
        {
            _blackTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
            _blackTexture.SetPixel(0, 0, Color.white);
            _blackTexture.Apply();
        }
        private void OnGUI()
        {
            if (_fadeState == FadeUtility.FadeState.None)
            {
                return;
            }

            //透明度を更新して黒テクスチャを描画
            GUI.color = new Color(0, 0, 0, _alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _blackTexture);
        }

        #endregion

        #region fade

        private IEnumerator FadeIn(string sceneName)
        {
            // fade in black texture
            {
                float time = 0;
                while (time <= _fadeInterval)
                {
                    _alpha = Mathf.Lerp(0f, 1f, time / _fadeInterval);
                    time += Time.unscaledDeltaTime;
                    yield return null;
                }

                _alpha = 1.0f;
            }

            // scene change
            {
                // reset time scale
                {
                    Time.timeScale = 1.0f;
                }

                _fadeState = FadeUtility.FadeState.FadeinFinish;

                // load next scene
                SceneUtility.LoadScene(this, sceneName, () =>
                {
                    // 通知
                    _notifyChangeScene(sceneName);

                    // フェードアウト開始
                    _fadeState = FadeUtility.FadeState.FadeoutStart;
                    StartCoroutine(FadeOut());
                });
            }
        }

        private IEnumerator FadeOut()
        {
            // fade out black texture
            {
                float time = 0;
                while (time <= _fadeInterval)
                {
                    _alpha = Mathf.Lerp(1f, 0f, time / _fadeInterval);
                    time += Time.unscaledDeltaTime;
                    yield return 0;
                }

                _alpha = 0.0f;
            }

            {
                _fadeState = FadeUtility.FadeState.FadeoutFinish;
                gameObject.SetActive(false);
            }
        }

        #endregion

        #region pre scene

        private void AddPreSceneName(string sceneName)
        {
            if (sceneName == null)
            {
                return;
            }

            if (_preSceneNameList.Contains(sceneName))
            {
                Debug.Assert(false, "addPreSceneName : name = " + sceneName);
            }

            _preSceneNameList.AddToNotDuplicate(sceneName);
        }
        private string PopPreSceneName()
        {
            var sceneName = string.Empty;

            int count = _preSceneNameList.Count;
            if (0 < count)
            {
                int index = count - 1;
                sceneName = _preSceneNameList[index];
                _preSceneNameList.RemoveAt(index);
            }

            return sceneName;
        }
        public void ClearPreSceneNameList()
        {
            _preSceneNameList.Clear();
        }

        #endregion

        #region change scene

        /*
         * シーン遷移
         * ・TransitionScene：指定シーン遷移
         * ・TransitionPreScene：前のシーンに戻る、前のシーン存在しない場合は指定シーン遷移
         * ・TransitionSceneCore：シーン遷移処理
         */
        public bool TransitionScene(string sceneName, bool isPushScene = false)
        {
            if (TransitionSceneCore(sceneName))
            {
                if (isPushScene)
                {
                    var currentSceneName = GetCurrentSceneName();
                    AddPreSceneName(currentSceneName);
                }
                return true;
            }

            return false;
        }


        /// 前のシーンに遷移する
        public bool TransitionPreScene(string sceneName)
        {
            var preSceneName = PopPreSceneName();
            var changeSceneName = preSceneName == string.Empty ? sceneName : preSceneName;
            if (TransitionSceneCore(changeSceneName))
            {
                return true;
            }

            return false;
        }


        private bool TransitionSceneCore(string sceneName)
        {
            if (_fadeState != FadeUtility.FadeState.None && _fadeState != FadeUtility.FadeState.FadeoutFinish)
            {
                return false;
            }

            {
                _alpha = 0;
                _fadeInterval = TransitionSceneFadeInterval;
                _fadeState = FadeUtility.FadeState.FadeinStart;
                gameObject.SetActive(true);
                StartCoroutine(FadeIn(sceneName));
            }

            return true;
        }

        /// <summary>
        /// 即時シーン切り替え フェード無し
        /// </summary>
        /// <param name="sceneName"></param>
        public void TransitionSceneImmediate(string sceneName)
        {
            SceneUtility.LoadScene(this, sceneName);
        }

        public void ReturnTopScene()
        {
            _preSceneNameList.Clear();
            TransitionScene("Top");
        }

        #endregion

        #region utility

        public string GetCurrentSceneName()
        {
            return SceneUtility.GetCurrentSceneName();
        }

        #endregion

    }
}