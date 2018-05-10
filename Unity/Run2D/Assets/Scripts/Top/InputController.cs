using TouchScript.Gestures;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<TapGesture>().Tapped += OnTapped;
    }
    private void OnDisable()
    {
        GetComponent<TapGesture>().Tapped -= OnTapped;
    }

    // フリックジェスチャーが成功すると呼ばれるメソッド
    private void OnTapped(object sender, System.EventArgs e)
    {
        Debug.Log("タップ");
    }
}