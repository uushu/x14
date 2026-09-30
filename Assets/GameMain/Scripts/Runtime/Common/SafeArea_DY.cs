using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class SafeArea_DY : SafeArea_Base
    {
        protected override void OnStart()
        {
#if UNITY_DOUYIN_GAME

            var sysInfo = StarkSDKSpace.StarkSDK.API.GetSystemInfo();

            TTSystemInfo window = TT.GetSystemInfo();
            ScreenWidth = (float)sysInfo.screenWidth * (float)window.pixelRatio;
            ScreenHeight = (float)sysInfo.screenHeight * (float)window.pixelRatio;

            var safeArea = sysInfo.safeArea;

            Debug.Log($"抖音屏幕安全区域: bottom={safeArea.bottom} height={safeArea.height} left={safeArea.left} right={safeArea.right} top={safeArea.top} width={safeArea.width}");

            LastSafeArea = new Rect((float)safeArea.left, (float)(ScreenHeight - safeArea.bottom), (float)safeArea.width, (float)safeArea.height);
#else
            ScreenWidth = Screen.width;
             ScreenHeight = Screen.height;
#endif
        }
    }
}
