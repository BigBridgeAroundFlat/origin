﻿using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneEditor : Editor
{
    [MenuItem("Tools/Scene/Awake")]
    public static void LoadAwakeScene(){LoadWithResidentSystem("Awake");}

    [MenuItem("Tools/Scene/Top")]
    public static void LoadTopScene(){LoadWithResidentSystem("Top");}

    [MenuItem("Tools/Scene/Battle")]
    public static void LoadBattleScene() { LoadWithResidentSystem("Battle"); }

    private static void LoadWithResidentSystem(string sceneName)
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ResidentSystem.unity", OpenSceneMode.Single);
        EditorSceneManager.OpenScene(string.Format("Assets/Scenes/{0}.unity", sceneName), OpenSceneMode.Additive);

        // アクティブにする アクティブシーンがInstantiate対象になるため ライティング等も対象
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName));
    }
}