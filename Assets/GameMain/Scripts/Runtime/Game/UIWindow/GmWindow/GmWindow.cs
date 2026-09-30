using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMain
{
    public class GmWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Toggle mAdToggle;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mClearBtn;
        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            mAdToggle.onValueChanged.AddListener(OnToggleChange);

            mAdToggle.isOn = !GameEntrance.Instance.platformMgr.skipLookAd;
            mClearBtn.onClick.AddListener(OnClearClick);
        }

        private async void OnClearClick()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            await SaveToServer.ClearLocalSave();

            GameEntrance.Instance.UnRegisterAfter(GameEntrance.Instance.gamePlayMgr);
            GameEntrance.Instance.RegisterAfter(GameEntrance.Instance.gamePlayMgr);

            UIWindowTool.FirstEnterGame().Forget();
        }

        private void OnToggleChange(bool toggle)
        {
            GameEntrance.Instance.platformMgr.skipLookAd = !toggle;
        }

    }
}