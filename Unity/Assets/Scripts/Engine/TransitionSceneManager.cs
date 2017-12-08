using System.Collections;
using System.Collections.Generic;
using Define;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine
{
    public class TransitionSceneManager : Engine.SingletonMonoBehaviour<TransitionSceneManager>
    {
        private const float TransitionSceneFadeInterval = 0.5f;

        // 黒テクスチャ
        private Texture2D _blackTexture;

        // フェード情報
        private float _alpha;
        private FadeParam.FadeState _fadeState = FadeParam.FadeState.None;
        private float _fadeInterval = TransitionSceneFadeInterval;


        // シーン遷移ログ
        private readonly List<string> _preSceneNameList = new List<string>();

        // フェードアウト終了コールバック
        private CommonData.VoidCallback _fadeOutFinishCallback;

        // シーン遷移コールバック
        public delegate void ChangeSceneCallback(bool isStart, string preSceneName, string changeSceneName);
        private ChangeSceneCallback _changeSceneCallback;
        public void SetChangeSceneCallback(ChangeSceneCallback callback)
        {
            _changeSceneCallback += callback;
        }

        #region common

        /*
        * 初期化
        * ・CreateBlackTexture：黒テクスチャ作成
        * ・OnGUI：黒テクスチャのα値変更
        */
        protected override void Init()
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
            if (_fadeState == FadeParam.FadeState.None)
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
                    time += Time.deltaTime;
                    yield return 0;
                }

                _alpha = 1.0f;
            }

            // scene change
            {
                var preSceneName = GetCurrentSceneName();
                if (_changeSceneCallback != null)
                {
                    _changeSceneCallback(true, preSceneName, sceneName);
                }

                // load empty scene
                {
                    SceneManager.LoadScene("Empty");
                    yield return null;
                }

                // GC Collect
                {
                    System.GC.Collect();
                    yield return null;
                }

                // load next scene
                {
                    _fadeState = FadeParam.FadeState.FadeinFinish;
                    SceneManager.LoadScene(sceneName);
                    yield return null;
                }

                if (_changeSceneCallback != null)
                {
                    _changeSceneCallback(false, preSceneName, sceneName);
                }
            }

            /*
             * 遷移シーン先で準備完了通知呼ぶルール削除、シーン遷移処理後に即フェードアウト開始
             */
            {
                _fadeState = FadeParam.FadeState.FadeoutStart;
                StartCoroutine(FadeOut());
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
                    time += Time.deltaTime;
                    yield return 0;
                }

                _alpha = 0.0f;
            }

            InputManager.Instance.EnableAllInput("scene");

            {
                _fadeState = FadeParam.FadeState.FadeoutFinish;
                gameObject.SetActive(false);
            }

            // フェードアウト終了コールバック
            {
                if (_fadeOutFinishCallback != null)
                {
                    _fadeOutFinishCallback();
                    _fadeOutFinishCallback = null;
                }
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
        public bool TransitionScene(string sceneName, bool isPushScene = false, CommonData.VoidCallback callback = null)
        {
            if (TransitionSceneCore(sceneName))
            {
                if (isPushScene)
                {
                    var currentSceneName = GetCurrentSceneName();
                    AddPreSceneName(currentSceneName);
                }

                _fadeOutFinishCallback = callback;
                return true;
            }

            return false;
        }
        public bool TransitionPreScene(string sceneName, CommonData.VoidCallback callback = null)
        {
            var preSceneName = PopPreSceneName();
            var changeSceneName = preSceneName == string.Empty ? sceneName : preSceneName;
            if (TransitionSceneCore(changeSceneName))
            {
                _fadeOutFinishCallback = callback;
                return true;
            }

            return false;
        }
        private bool TransitionSceneCore(string sceneName)
        {
            if (_fadeState != FadeParam.FadeState.None && _fadeState != FadeParam.FadeState.FadeoutFinish)
            {
                return false;
            }

            InputManager.Instance.DisableAllInput("scene");

            {
                _alpha = 0;
                _fadeInterval = TransitionSceneFadeInterval;
                _fadeState = FadeParam.FadeState.FadeinStart;
                gameObject.SetActive(true);
                StartCoroutine(FadeIn(sceneName));
            }

            return true;
        }

        public void TransitionSceneImmediate(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
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
            return SceneManager.GetActiveScene().name;
        }

        #endregion
    }
}