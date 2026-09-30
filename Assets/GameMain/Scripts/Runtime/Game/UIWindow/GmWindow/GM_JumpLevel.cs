using GameMain.Garden;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class GM_JumpLevel : ABaseComponent
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnJump;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TMP_Dropdown mLevelDropDown;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnAdd_1;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnAdd_10;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnAdd_100;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnSub_1;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnSub_10;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnSub_100;

        private void Start()
        {
            var dataList = ConfigMgr.Instance.Tables.GardenLevelCategory.DataList;

            var levelNames = new List<string>();

            int selectIndex = 0;
            var selectLevel = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().levelId;
            for (int i = 0; i < dataList.Count; i++)
            {
                levelNames.Add($"{dataList[i].LevelName}");

                if (dataList[i].Id == selectLevel)
                { 
                    selectIndex = i;
                }
            }

            mLevelDropDown.ClearOptions();
            mLevelDropDown.AddOptions(levelNames);
            mLevelDropDown.value = selectIndex;
            mLevelDropDown.onValueChanged.AddListener(OnValueChange);

            mBtnJump.onClick.AddListener(() =>
            {
                var levelId = ConfigMgr.Instance.Tables.GardenLevelCategory.DataList[mLevelDropDown.value].Id;
                LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_FlowerGarden>().SetNextLevelId(levelId);

                var gardenWindow = UIWindowMgr.Instance.GetWindow<GardenWindow>();
                if (gardenWindow != null)
                {
                    gardenWindow.ManualStarGame();
                }
                else
                {
                    UIWindowTool.ShowPromptBox("跳关成功!!!");
                }
            });

            mBtnAdd_1.onClick.AddListener(() =>
            {
                OnAddLevel(1);
            });

            mBtnAdd_10.onClick.AddListener(() =>
            {
                OnAddLevel(10);
            });

            mBtnAdd_100.onClick.AddListener(() =>
            {
                OnAddLevel(100);
            });

            mBtnSub_1.onClick.AddListener(() =>
            {
                OnAddLevel(-1);
            });

            mBtnSub_10.onClick.AddListener(() =>
            {
                OnAddLevel(-10);
            });

            mBtnSub_100.onClick.AddListener(() =>
            {
                OnAddLevel(-100);
            });
        }

        private void OnAddLevel(int addLevel)
        {
            var dataList = ConfigMgr.Instance.Tables.GardenLevelCategory.DataList;
            var targetLevel = mLevelDropDown.value + addLevel;
            targetLevel = Mathf.Clamp(targetLevel, 0, dataList.Count - 1);

            mLevelDropDown.value = targetLevel;
        }

        private void OnValueChange(int levelIndex)
        {
            Debugger.Log($"选择关卡Id:{ConfigMgr.Instance.Tables.GardenLevelCategory.DataList[levelIndex].Id}");
        }
    }
}