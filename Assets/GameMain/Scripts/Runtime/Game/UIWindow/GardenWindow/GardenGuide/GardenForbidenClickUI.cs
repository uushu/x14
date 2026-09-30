using GameMain.BTree;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.Garden
{
    public class GardenForbidenClickUI : ABaseActionNode
    {
        public bool isForbidenClickUI;

        public GardenForbidenClickUI(bool isForbiden)
        {
            isForbidenClickUI = isForbiden;
        }

        protected override async void OnStart()
        {
            if (isForbidenClickUI)
            {
                var guideWindow = await UIWindowMgr.Instance.OpenWindowAsync<GuideWindow>();
                guideWindow.ShowForbidenInputUI(true);
            }
            else
            {
               await UIWindowMgr.Instance.CloseWindow<GuideWindow>(false);
            }
            SetFinish(true);
        }


        protected override void OnCancel()
        {
        }

    }
}