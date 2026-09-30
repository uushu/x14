using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;

namespace GameMain
{
    public class ItemUI_BagProp : ABaseComponent
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgPropBg;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTxtPropCount;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgPropIcon;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTextPropName;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image debrisImage;

        public List<Sprite> bgList = new List<Sprite>();

        public UICustomButton clickBtn;
        public bool canClick;

        private ItemData_BagProp propData;
        public ItemData_BagProp PropData => propData;

        public void RefreshBagPropUI(ItemData_BagProp _propData)
        {
            propData = _propData;
            RectTransform rect = mImgPropIcon.GetComponent<RectTransform>();
            mTxtPropCount.text = $"x{_propData.PropCountString}";
            ResourceMgr.Instance.SetImageSprite(mImgPropIcon, _propData.Config.Sprite);
            if (mTextPropName != null)
            {
                mTextPropName.text = _propData.Config.Name;
            }

            
            if (clickBtn!=null)
            {
                if (canClick)
                {
                    clickBtn.targetButton.enabled = true;
                    mImgPropBg.raycastTarget = true;
                    clickBtn?.AddListener(OnClick);
                }
                else
                {
                    clickBtn.targetButton.enabled = false;
                    mImgPropBg.raycastTarget = false;
                    clickBtn?.AddListener(null);
                }
            }
           
        }
        public void OnClick()
        {
        }
    }
}