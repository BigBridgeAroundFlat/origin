//  NineSlicedProvider.cs
//  http://kan-kikuchi.hatenablog.com/entry/NineSlicedProvider
//
//  Created by kan.kikuchi on 2017.02.15.

using UnityEngine;

namespace Common.Other
{
    /// <summary>
    /// SpriteRendererを9スライスためのクラス
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class NineSlicedProvider : MonoBehaviour
    {

        //対象のレンダラーとマテリアル
        private SpriteRenderer _spriteRenderer;
        private Material _material = null;

        //=================================================================================
        //初期化　
        //=================================================================================

        private void Awake()
        {
            Adjust();
        }

        private void OnEnable()
        {
            Adjust();
        }

        //=================================================================================
        //更新
        //=================================================================================

        private void Update()
        {
            Adjust();
        }

        //9スライスに合うように調整する
        private void Adjust()
        {
            //SpriteRendererを取得していなければ取得し、マテリアルを設定
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                _spriteRenderer.sharedMaterial = new Material(Shader.Find("Rickshao/NineSlicedShader"));
                _material = _spriteRenderer.sharedMaterial;
            }

            //Spriteが設定されていなければ処理しない
            if (_spriteRenderer.sprite == null)
            {
                return;
            }

            //調整処理
            float width = _spriteRenderer.sprite.rect.width;
            float borderLeft = _spriteRenderer.sprite.border.x;
            float borderBottom = _spriteRenderer.sprite.border.y;
            float borderRight = _spriteRenderer.sprite.border.z;
            float borderTop = _spriteRenderer.sprite.border.w;

            float left = borderLeft / width;
            float bottom = borderBottom / width;
            float right = borderRight / width;
            float top = borderTop / width;

            _material.SetFloat("top", top);
            _material.SetFloat("bottom", bottom);
            _material.SetFloat("right", right);
            _material.SetFloat("left", left);

            _material.SetFloat("sx", this.transform.localScale.x);
            _material.SetFloat("sy", this.transform.localScale.y);
        }
    }
}