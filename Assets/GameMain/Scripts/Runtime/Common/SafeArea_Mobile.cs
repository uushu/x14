using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class SafeArea_Mobile : SafeArea_Base
    {
        protected override void OnStart()
        {
            var safeArea = Screen.safeArea;

            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;

            Debug.Log($"移动端屏幕安全区域: bottom={safeArea.yMax} height={safeArea.height} left={safeArea.xMin} right={safeArea.xMax} top={safeArea.yMin} width={safeArea.width}");

            LastSafeArea = safeArea;
        }
    }
}