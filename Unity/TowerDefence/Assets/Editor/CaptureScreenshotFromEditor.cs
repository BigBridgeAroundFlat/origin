using System.Collections;
using UniRx;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unityエディタ上からGameビューのスクリーンショットを撮るEditor拡張
/// </summary>
public class CaptureScreenshotFromEditor : Editor
{
    /// <summary>
    /// キャプチャを撮る
    /// </summary>
    /// <remarks>
    /// Edit > CaptureScreenshot に追加。
    /// HotKeyは Ctrl + Shift + F12。
    /// </remarks>
    [MenuItem("Edit/CaptureScreenshot #%F12")]
    private static void CaptureScreenshot()
    {
        var fileName = System.DateTime.Now.ToString("スクリーンショット yyyy-MM-dd HH.mm.ss") + ".png";
        Observable.FromCoroutine(() => CaptureCoroutine(fileName)).Subscribe();
    }

    private static IEnumerator CaptureCoroutine(string fileName)
    {
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);

        yield return new WaitForEndOfFrame();

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        var bytes = texture.EncodeToPNG();
        Destroy(texture);
        System.IO.File.WriteAllBytes(fileName, bytes);

        Debug.Log("ScreenShot: " + fileName);
    }
}