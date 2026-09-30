using GameMain.BTree;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.Garden
{
    public class GardenGuideActiveBagProps : ABaseActionNode
    {
        private bool _isEnable;

        public GardenGuideActiveBagProps(bool isEnable)
        {
            _isEnable = isEnable;
        }

        protected override void OnStart()
        {
            var gardenWindow = UIWindowMgr.Instance.GetWindow<GardenWindow>();
            gardenWindow.mBtnRefresh.gameObject.SetActive(_isEnable);
            gardenWindow.mBtnRemove.gameObject.SetActive(_isEnable);
            gardenWindow.mBtnRotate.gameObject.SetActive(_isEnable);
            SetFinish(true);
        }

        protected override void OnCancel()
        {
            throw new System.NotImplementedException();
        }

    }
}