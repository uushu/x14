using cfg;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class GM_AddBagProp : ABaseComponent
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TMP_Dropdown mBagPropDropDown;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnSub;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Button mBtnAdd;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TMP_InputField mInputField;

        private void Start()
        {
            var dataList = ConfigMgr.Instance.Tables.BagPropCategory.DataList;

            var bagPropName = new List<string>();

            for (int i = 0; i < dataList.Count; i++)
            {
                bagPropName.Add($"{dataList[i].Name}");
            }

            mBagPropDropDown.ClearOptions();
            mBagPropDropDown.AddOptions(bagPropName);
            mBagPropDropDown.value = bagPropName.Count - 1;

            mBagPropDropDown.onValueChanged.AddListener(OnValueChange);

            mBtnAdd.onClick.AddListener(() =>
            {
                if (int.TryParse(mInputField.text, out var value))
                {
                    GmAddBagProp(value);
                }
            });

            mBtnSub.onClick.AddListener(() =>
            {
                if (int.TryParse(mInputField.text, out var value))
                {
                    GmRemoveBagProp(value);
                }
            });
        }

        private void OnValueChange(int index)
        {
            var dataList = ConfigMgr.Instance.Tables.BagPropCategory.DataList;
            Debugger.Log($"值改变:{dataList[index].Name}");
        }

        private void GmAddBagProp(int value)
        {
            var selectValue = mBagPropDropDown.value;
            List<BagPropConfig> dataList = ConfigMgr.Instance.Tables.BagPropCategory.DataList;
            BagMgr.Instance.AddBagProp(dataList[selectValue].Id, value);
            UIWindowTool.ShowPromptBox($"道具:{dataList[selectValue].Name}添加成功");
        }

        private void GmRemoveBagProp(int value)
        {
            var selectValue = mBagPropDropDown.value;
            List<BagPropConfig> dataList = ConfigMgr.Instance.Tables.BagPropCategory.DataList;
            BagMgr.Instance.RemoveBagProp(dataList[selectValue].Id, value);
            UIWindowTool.ShowPromptBox($"道具:{dataList[selectValue].Name}添加成功");
        }
    }
}