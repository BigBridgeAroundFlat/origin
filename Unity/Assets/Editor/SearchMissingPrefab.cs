using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SearchMissingPrefab : EditorWindow
{
    [MenuItem("Tools/Search Missing Prefab")]
    static void Open()
    {
        GetWindow<SearchMissingPrefab>();
    }

    private enum SEARCH_TYPE
    {
        MISSING_SPRITE = 0,
        MISSING_COMPONENT,
    }
    private static List<String> fileNameList = new List<string>();
    void OnGUI()
    {
        EditorGUILayout.Space();

        {
            if (GUILayout.Button("MissingSprite", GUILayout.Width(200.0f)))
            {
                SearchPrefabs(SEARCH_TYPE.MISSING_SPRITE);
            }
            EditorGUILayout.Space();
        }

        {
            if (GUILayout.Button("MissingComponent", GUILayout.Width(200.0f)))
            {
                SearchPrefabs(SEARCH_TYPE.MISSING_COMPONENT);
            }
            EditorGUILayout.Space();
        }

        foreach (var filePath in fileNameList)
        {
            EditorGUILayout.LabelField(filePath);
        }
    }

    private void SearchPrefabs(SEARCH_TYPE searchType)
    {
        fileNameList.Clear();

        var guids = AssetDatabase.FindAssets("t:prefab");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

            bool result = false;
            {
                switch(searchType)
                {
                    case SEARCH_TYPE.MISSING_SPRITE:    result = HasMissingSprite(obj); break;
                    case SEARCH_TYPE.MISSING_COMPONENT: result = HasMissingComponent(obj); break;
                }
            }

            if (result)
            {
                fileNameList.Add(obj.name);
            }
        }
    }

    // check missing
    private bool HasMissingSprite(GameObject obj)
    {
        var components = obj.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var component in components)
        {
            if (component.sprite == null)
            {
                return true;
            }
        }

        return false;
    }
    private bool HasMissingComponent(GameObject obj)
    {
        var components = obj.GetComponentsInChildren<Component>(true);
        foreach(var component in components)
        {
            if(component == null)
            {
                return true;
            }
        }

        return false;
    }
}
