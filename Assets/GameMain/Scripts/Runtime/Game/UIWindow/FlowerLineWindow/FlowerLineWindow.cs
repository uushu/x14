using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using cfg;
using UnityEngine.UI;

namespace GameMain.FlowerLine
{
    public class FlowerLineWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Image bg;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnSetting;

#if UNITY_EDITOR
        [LabelText("测试连线关卡Id")]
        public LineLevelId mLevelId;
#endif

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            mBtnSetting.AddListener(() =>
            {
                UIWindowTool.ChangeToHallScene(false).Forget();
            });

            AudioMgr.Instance.PlayBgm(AudioName.BgmGarden);
            FlowerLineGameRoot.Instance.Initialize();

            StartGame();
        }

        protected override void OnHide()
        {
            base.OnHide();
            FlowerLineGameRoot.Instance.UnInitialize();
        }

        public void StartGame()
        {
            ThemeSkinConfig temp = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>().GetUseTheme();
            if (temp != null)
            {
                if(bg) ResourceMgr.Instance.SetImageSprite(bg,temp.GameBgName);
            }
            
            int levelId = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerBlock>().levelId;

#if UNITY_EDITOR
            if (mLevelId > 0)
            {
                levelId = mLevelId;
            }
#endif

            FlowerLineGameRoot.Instance.StartGame(levelId);
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            FlowerLineGameRoot.Instance.OnUpdate(deltaTime);
        }
    }
}