using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockSuccessWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnNextLevel;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnMainPage;

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            int levelId = (int)parameters;

            mBtnNextLevel.AddListener(OnNextLevelClick);
            mBtnMainPage.AddListener(OnMainPageClick);
        }

        private void OnMainPageClick()
        {
            UIWindowTool.ChangeToHallScene(false).Forget();
        }

        private void OnNextLevelClick()
        {
            CloseSelfWindow();
            UIWindowMgr.Instance.GetWindow<FlowerBlockWindow>().ManualStarGame();
        }
    }
}