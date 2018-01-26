using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Scene
{
    public static class SceneUtility
    {
        // 常駐シーン名
        private const string ResidentSystemSceneName = "ResidentSystem";

        // シーン切り替え時にカメラが無い状態を防ぐためのカメラ
        private static GameObject _dummyCamera;

        /// <summary>
        /// ダミーカメラ設定
        /// </summary>
        /// <param name="camera"></param>
        public static void InitializeDummyCamera(GameObject camera)
        {
            _dummyCamera = camera;
            _dummyCamera.SetActive(false);
        }

        /// <summary>
        /// システム常駐シーンを追加
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="callback"></param>
        public static void LoadResidentScene(MonoBehaviour behaviour, Action callback = null)
        {
            // 既にシーンが存在する場合は何もしない
            var scene = SceneManager.GetSceneByName(ResidentSystemSceneName);
            if (scene.IsValid())
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
                return;
            }

            behaviour.StartCoroutine(LoadSceneAsync(ResidentSystemSceneName, callback));
        }


        /// <summary>
        /// シーン読み込み
        /// </summary>
        public static void LoadScene(MonoBehaviour behaviour, string sceneName, Action completeCb = null)
        {
            LoadSceneByName(behaviour, sceneName, completeCb);
        }

        /// <summary>
        /// シーンの破棄と読み込み
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="sceneName"></param>
        /// <param name="completeCb"></param>
        private static void LoadSceneByName(MonoBehaviour behaviour, string sceneName, Action completeCb)
        {
            _dummyCamera.SetActive(true);

            behaviour.StartCoroutine(DelayedUnloadAll(() =>
            {
                // シーンを破棄した後にGCを行う
                GC.Collect();

                behaviour.StartCoroutine(LoadSceneAsync(sceneName, () =>
                {
                    _dummyCamera.SetActive(false);

                    if (completeCb != null)
                    {
                        completeCb.Invoke();
                    }
                }));
            }));
        }

        /// <summary>
        /// 非同期読み込み
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private static IEnumerator LoadSceneAsync(string sceneName, Action callback)
        {
            var ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            ao.allowSceneActivation = false;
            while (!ao.isDone)
            {
                if (ao.progress.Equals(0.9f))
                {
                    ao.allowSceneActivation = true;
                }
                yield return null;
            }

            // アクティブにする アクティブシーンがInstantiate対象になるため
            var result = SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            Debug.Assert(result);

            if (callback != null)
            {
                callback.Invoke();
            }
        }

        /// <summary>
        /// 常駐シーン以外をすべて破棄
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        private static IEnumerator DelayedUnloadAll(Action cb)
        {
            var targetRemovalNamelist = new List<string>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var curScene = SceneManager.GetSceneAt(i);
                if (!curScene.name.Equals(ResidentSystemSceneName))
                {
                    targetRemovalNamelist.Add(curScene.name);
                }
            }
            foreach (var item in targetRemovalNamelist)
            {
                var sc = SceneManager.GetSceneByName(item);
                SceneManager.UnloadSceneAsync(sc);
                while (sc.isLoaded)
                {
                    yield return null;
                }
            }
            cb.Invoke();
        }


        /// <summary>
        /// 常駐シーン以外のアクティブシーン名を取得
        /// UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentSceneName()
        {
            var namelist = new List<string>();

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var curScene = SceneManager.GetSceneAt(i);
                if (!curScene.isLoaded || !curScene.IsValid())
                {
                    continue;
                }
                if (!curScene.name.Equals(ResidentSystemSceneName))
                {
                    namelist.Add(curScene.name);
                }
            }

            // 常駐シーン以外は1個だけの想定
            Debug.Assert(namelist.Count == 1);

            return namelist[0];
        }

    }
}