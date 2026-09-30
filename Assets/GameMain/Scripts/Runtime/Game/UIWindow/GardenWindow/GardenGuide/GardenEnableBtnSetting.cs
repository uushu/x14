using GameMain.BTree;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.Garden
{
    public class GardenEnableBtnSetting : ABaseActionNode
    {
        public bool IsEnable;

        public GardenEnableBtnSetting(bool isEnalbe)
        {
            IsEnable = isEnalbe;
        }

        protected override void OnStart()
        {
           var gardenWindow = UIWindowMgr.Instance.GetWindow<GardenWindow>();
            
            if(gardenWindow != null)
            {
                gardenWindow.mBtnSetting.SetInteractable(IsEnable);
            }

            SetFinish(true);
        }

        protected override void OnCancel()
        {
        }
    }
}