using Sirenix.OdinInspector;
using cfg;
using UnityEngine.UI;

namespace GameMain
{
    public enum EMiniGameType
    {
        [LabelText("俄罗斯花园")]
        GardenFlower,
        [LabelText("花了个花")]
        BlockFlower,
        [LabelText("花朵连线")]
        LineFlower,
    }

    public abstract class AMiniGameBase : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Image bigBg;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTxtLevel;

        [LabelText("已经复活次数")]
        public int alreadyReliveCount;

        public abstract int MaxReliveCount { get; }

        public bool IsCanRelive => alreadyReliveCount < MaxReliveCount;

        public int LastReliveCount => MaxReliveCount - alreadyReliveCount;

        protected void RefreshLevelText(string levelStr)
        {
            if (mTxtLevel != null)
            {
                mTxtLevel.text = levelStr;
            }
        }

        protected virtual void StartGame()
        {
            ThemeSkinConfig temp = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>().GetUseTheme();
            if (temp != null)
            {
                if(bigBg) ResourceMgr.Instance.SetImageSprite(bigBg,temp.GameBgName);
            }

            alreadyReliveCount = 0;
        }

        public virtual void TrySetNextLevel() { }
    }
}