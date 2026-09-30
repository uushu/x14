using GameMain.BTree;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenGuideUseRefresh : ABaseActionNode
    {
        protected override async void OnStart()
        {
            var guideWindow = await UIWindowMgr.Instance.OpenWindowAsync<GuideWindow>();

            var gardenWindow = UIWindowMgr.Instance.GetWindow<GardenWindow>();
            var rect = gardenWindow.mBtnRefresh.GetComponent<RectTransform>();
            guideWindow.ShowUseRefreshPopDialog(rect, () =>
            {
                SetFinish(true);
            });
        }

        protected override void OnCancel()
        {
        }
    }
}