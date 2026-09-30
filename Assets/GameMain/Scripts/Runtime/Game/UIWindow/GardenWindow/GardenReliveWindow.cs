using System;
using System.Collections.Generic;
using TMPro;

namespace GameMain.Garden
{
    public class GardenReliveWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRestart;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRelive;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnClose;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI remainingTimes;
        
        public Action OnCloseReliveCallBack {  get; set; }

        public Action OnRestartCallBack {get; set; }

        public Action OnReliveCallBack {  get; set; }

        public BagPropId mReliveBagPropId;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);
            remainingTimes.text = ((int)parameters).ToString();

            AudioMgr.Instance.PlayAudioOneShot(AudioName.GardenReliveAudio);

            mBtnRestart.AddListener(OnRestartClick);
            mBtnRelive.AddListener(OnReliveClick);
            mBtnClose.AddListener(OnCloseReliveClick);
        }

        private void OnCloseReliveClick()
        {
            CloseSelfWindow();
            OnCloseReliveCallBack?.Invoke();
        }

        private void OnRestartClick()
        {
            CloseSelfWindow();
            OnRestartCallBack?.Invoke();
        }

        private void OnReliveClick()
        {
            PlatformMgr.Instance.LookAd((result) =>
            {
                if (result)
                {
                    CloseSelfWindow();
                    OnReliveCallBack?.Invoke();
                    GameEventDefine.SendUseInGameBagProp(mReliveBagPropId, 1, true);
                }

            }, LookAdsType.俄罗斯花园复活);
        }
    }
}