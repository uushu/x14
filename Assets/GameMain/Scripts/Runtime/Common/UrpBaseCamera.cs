using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.Universal;

namespace GameMain
{
    public class UrpBaseCamera : MonoBehaviour
    {
        private void Awake()
        {
            var baseCamera = GetComponent<Camera>();

          //  UniversalAdditionalCameraData data = baseCamera.GetUniversalAdditionalCameraData();
          //  data.cameraStack.Add(UIWindowMgr.Instance.uiCamera);
        }
    }
}