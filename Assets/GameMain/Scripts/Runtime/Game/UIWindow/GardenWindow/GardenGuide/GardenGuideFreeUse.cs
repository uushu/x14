using GameMain.BTree;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.Garden
{
    public class GardenGuideFreeUse : ABaseActionNode
    {
        protected override async void OnStart()
        {
            var guideWindow = await UIWindowMgr.Instance.OpenWindowAsync<GuideWindow>();

            guideWindow.ShowFreeUsePopDialog(() =>
            {
                SetFinish(true);
            });
        }

        protected override void OnCancel()
        {
        }
    }
}