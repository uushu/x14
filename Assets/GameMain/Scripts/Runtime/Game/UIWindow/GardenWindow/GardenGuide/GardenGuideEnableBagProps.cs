using GameMain.BTree;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.Garden
{
    public class GardenGuideEnableBagProps : ABaseActionNode
    {
        private bool _isEnable;

        public GardenGuideEnableBagProps(bool isEnable)
        {
            _isEnable = isEnable;
        }

        protected override void OnStart()
        {
            var gardenWindow = UIWindowMgr.Instance.GetWindow<GardenWindow>();
            gardenWindow.mBtnRefresh.customButton.targetButton.interactable = _isEnable;
            gardenWindow.mBtnRemove.customButton.targetButton.interactable = _isEnable;
            gardenWindow.mBtnRotate.customButton.targetButton.interactable = _isEnable;
            SetFinish(true);
        }

        protected override void OnCancel()
        {
            throw new System.NotImplementedException();
        }

    }
}