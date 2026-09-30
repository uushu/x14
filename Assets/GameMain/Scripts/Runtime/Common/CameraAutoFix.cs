using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class CameraAutoFix : MonoBehaviour
    {
        public Camera gameCamera;
        
        // 自动旋转允许的方向
        public void Awake()
        {
            ScreenOrientation designAutoRotation = Screen.orientation;

            float aspect = gameCamera.aspect;

            float designOrthographicSize = 6.4f;

            float designHeight = 1280f;
            float designWidth = 720f;

            float designAspect = designWidth / designHeight;

            float widthOrthographicSize = designOrthographicSize * designAspect;
            Debug.Log($"测试:{designAutoRotation}");
            switch (designAutoRotation)
            {
                case ScreenOrientation.Portrait:
                    if (aspect < designAspect)
                    {
                        gameCamera.orthographicSize = widthOrthographicSize / aspect;
                    }
                    if (aspect > designAspect)
                    {
                        gameCamera.orthographicSize = designOrthographicSize;
                    }
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    break;
                case ScreenOrientation.AutoRotation:
                case ScreenOrientation.LandscapeLeft:
                case ScreenOrientation.LandscapeRight:
                default:
                    {
                        Debug.Log($"测试2:{aspect}:{designAspect}");
                        if (aspect < designAspect)
                        {
                            gameCamera.orthographicSize = widthOrthographicSize / aspect;
                        }
                        if (aspect > designAspect)
                        {
                            gameCamera.orthographicSize = designOrthographicSize;
                        }
                    }
                    break;
                }
        }
    }
}