using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockFailedWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnMainPage;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRestart;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnShare;

        private bool _isEndlessMode;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);
            _isEndlessMode = (bool)parameters;

            mBtnMainPage.AddListener(OnMainPageClick);
            mBtnRestart.AddListener(OnRestartClick);
            mBtnShare.AddListener(OnShareClick);
        }

        private void OnShareClick()
        {
          PlatformMgr.Instance.ShowShare("快来挑战！！！！", (flag) =>
          {
              
          });
        }

        private void OnRestartClick()
        {
            CloseSelfWindow();

            if (_isEndlessMode == false)
            {
                var blockWindow = UIWindowMgr.Instance.GetWindow<FlowerBlockWindow>();
                blockWindow.ManualStarGame();
            }
            else
            {
                var blockEndlessWindow = UIWindowMgr.Instance.GetWindow<FlowerBlockEndlessWindow>();
                blockEndlessWindow.ManualStarGame();
            }
        }

        private void OnMainPageClick()
        {
            UIWindowTool.ChangeToHallScene(false).Forget();
        }
    }
}