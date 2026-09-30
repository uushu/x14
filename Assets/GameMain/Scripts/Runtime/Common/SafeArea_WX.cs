using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class SafeArea_WX : SafeArea_Base
    {
        private void Start()
        {
        }
        
        protected override void OnStart()
        {
#if UNITY_WECHAT_GAME

            WeChatWASM.WX.GetSystemInfo(new WeChatWASM.GetSystemInfoOption()
            {
                success = (result) =>
                {
                    ScreenWidth = (float)result.screenWidth * (float)result.pixelRatio;
                    ScreenHeight = (float)result.screenHeight * (float)result.pixelRatio;
                    var safeArea = result.safeArea;
                    Debug.Log($"微信屏幕安全区域: bottom={safeArea.bottom} height={safeArea.height} left={safeArea.left} right={safeArea.right} top={safeArea.top} width={safeArea.width}");
                    LastSafeArea = new Rect((float)safeArea.left, (float)(ScreenHeight - safeArea.bottom), (float)safeArea.width, (float)safeArea.height);
                }
            });
#else
            ScreenHeight = Screen.width;
            ScreenHeight = Screen.height;
#endif
        }
    }
}
