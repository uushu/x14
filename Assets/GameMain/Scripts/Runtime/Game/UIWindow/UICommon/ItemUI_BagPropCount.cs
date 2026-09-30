using System;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public class ItemUI_BagPropCount : ABaseComponent
    {
        public BagPropId bagPropId;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mImgPropIcon;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTxtPropCount;

        private readonly EventGroup _eventGroup = new();
        
        private void Start()
        {
            _eventGroup.AddListener<GameEventDefine.GetBagProp>(ProcessMessage);
            _eventGroup.AddListener<GameEventDefine.UseBagProp>(ProcessMessage);

            RefreshBagPropUI();
        }

        private void OnDestroy()
        {
            _eventGroup.RemoveAllListener();
        }

        private void RefreshBagPropUI()
        {
            var bagPropData = BagMgr.Instance.TryGetBagProp(bagPropId);

            if (bagPropData == null)
            {
                bagPropData = new ItemData_BagProp(bagPropId, 0);
            }

            ResourceMgr.Instance.SetImageSprite(mImgPropIcon, bagPropData.Config.Sprite);
            mTxtPropCount.text = bagPropData.PropCountString;
        }

        private void ProcessMessage(IEventMessage obj)
        {
            RefreshBagPropUI();
        }
    }
}