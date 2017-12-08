using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneEditor : Editor
{
    [MenuItem("Tools/Scene/Logo")]
    public static void LoadLogoScene()
    {
        LoadSystemMonitor("Logo");
    }

    [MenuItem("Tools/Scene/Title")]
    public static void LoadTitleScene()
    {
        LoadSystemMonitor("Title");
    }

    [MenuItem("Tools/Scene/CharacterSelect")]
    public static void LoadCharacterSelectScene()
    {
        LoadSystemMonitor("CharacterSelect");
    }

    [MenuItem("Tools/Scene/Battle")]
    public static void LoadBattleScene()
    {
        LoadSystemMonitor("Battle");
    }

    [MenuItem("Tools/Scene/AnimationEditor")]
    public static void LoadAnimationEditorScene()
    {
        LoadSystemMonitor("AnimationEditor");
    }

    private static void LoadSystemMonitor(string sceneName)
    {
        var path = string.Format("Assets/Scenes/{0}.unity", sceneName);
        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
    }
}
