using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class FullScreen : MonoBehaviour
    {
        private void Start()
        {
            var safeArea = UIWindowMgr.Instance.GetComponent<SafeArea_Base>();
            
            float designHeight = 1334;
            float designWidth = 750;
            
            float logWidth = Mathf.Log(safeArea.ScreenWidth / designWidth, 2);
            float logHeight = Mathf.Log(safeArea.ScreenHeight / designHeight, 2);
            float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, 0);
            var scaleFactor = Mathf.Pow(2, logWeightedAverage);

            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}