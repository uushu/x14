using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockReliveWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRestart;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRelive;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnClose;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI ReliveTimesText;

        public Action OnCloseReliveCallBack { get; set; }

        public Action OnRestartCallBack { get; set; }

        public Action OnReliveCallBack { get; set; }

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);
            ReliveTimesText.text = parameters.ToString();
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
                }

            }, LookAdsType.花了个花复活);
        }
    }
}