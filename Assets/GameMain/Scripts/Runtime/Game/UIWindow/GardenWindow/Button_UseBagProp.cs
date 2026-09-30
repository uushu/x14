using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public class Button_UseBagProp : MonoBehaviour
    {
        public UICustomButton customButton;

        public BagPropId propId;

        public Image imgCoungBg;

        public Image imgAd;

        public Image imgFree;

        public TextMeshProUGUI textCount;

        [LabelText("是否免费使用道具")]
        public bool isFreeUseBagProp;

        private EventGroup _eventGroup;

        private Action _onUseBagPropSuccessCallBack;

        private Func<bool> _useBagPropCondition;

        private LookAdsType _lookAdType;

        private void Start()
        {
            customButton.AddListener(OnUseClick);
            _eventGroup = new();
            _eventGroup.AddListener<GameEventDefine.GetBagProp>(ProcessMessage);
        }

        private void OnDestroy()
        {
            _eventGroup?.RemoveAllListener();
        }

        public void SetUseBagPropCallback(bool isFreeUse, Func<bool> useBagPropCondition, Action onUseBagPropSuccessCallBack, LookAdsType lookAdsType)
        {
            isFreeUseBagProp = isFreeUse;
            _useBagPropCondition = useBagPropCondition;
            _onUseBagPropSuccessCallBack = onUseBagPropSuccessCallBack;
            _lookAdType = lookAdsType;

            RefreshBagProp();
        }

        private void ProcessMessage(IEventMessage message)
        {
            RefreshBagProp();
        }

        private void RefreshBagProp()
        {
            imgFree.gameObject.SetActive(false);
            textCount.gameObject.SetActive(false);
            imgAd.gameObject.SetActive(false);
            imgCoungBg.enabled = true;

            var bagPropCount = BagMgr.Instance.GetBagPropCount(propId);

            if (isFreeUseBagProp)
            {
                imgFree.gameObject.SetActive(true);
                imgCoungBg.enabled = false;
            }
            else if (bagPropCount > 0)
            {
                textCount.text = bagPropCount.ToString();
                textCount.gameObject.SetActive(true);
            }
            else
            {
                imgAd.gameObject.SetActive(true);
            }
        }

        private void OnUseClick()
        {
            if (_useBagPropCondition != null && _useBagPropCondition.Invoke() == false) return;

            if (isFreeUseBagProp)
            {
                _onUseBagPropSuccessCallBack?.Invoke();
                return;
            }

            var bagPropCount = BagMgr.Instance.GetBagPropCount(propId);

            if (bagPropCount > 0)
            {
                BagMgr.Instance.RemoveBagProp(propId, 1);
                _onUseBagPropSuccessCallBack?.Invoke();
                GameEventDefine.SendUseInGameBagProp(propId, 1, false);
                RefreshBagProp();
            }
            else
            {
                PlatformMgr.Instance.LookAd((result) => {

                    if (result)
                    {
                        _onUseBagPropSuccessCallBack?.Invoke();
                        GameEventDefine.SendUseInGameBagProp(propId, 1, true);
                    }

                }, _lookAdType);
            }
        }
    }
}