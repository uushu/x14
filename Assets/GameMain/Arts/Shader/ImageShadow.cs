using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public class ImageShadow : MonoBehaviour
    {
        // Start is called before the first frame update 编码 utf-8
        void Start()
        {
            // 获取纹理并传递到shader
            var shadowMat = transform.GetChild(0).GetComponent<SpriteRenderer>().material;
            var playerTex = GetComponent<SpriteRenderer>()?.sprite?.texture;
            shadowMat.SetTexture("_PlayerTex", playerTex);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}