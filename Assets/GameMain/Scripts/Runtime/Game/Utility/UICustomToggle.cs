using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    [RequireComponent(typeof(UICustomButton))]
    public class UICustomToggle : MonoBehaviour
    {
        public GameObject imgOnGo;

        public GameObject imgOffGo;

        public UICustomButton btnToggle;
        
        public Action<bool> OnToggleClickCallback { get; set; }
        public Action<bool> OnToggleChangeCallback { get; set; }

        [LabelText("是否激活")]
        [OnValueChanged("OnToggleChange")]
        public bool isOn;

        private void Start()
        {
            btnToggle.AddListener(OnToggleClick);
        }

        private void OnToggleClick()
        {
            RefreshToggle();
            OnToggleClickCallback?.Invoke(isOn);
        }

        private void RefreshToggle()
        {
            if (isOn)
            {
                imgOnGo.gameObject.SetActive(true);
                imgOffGo.gameObject.SetActive(false);
            }
            else
            {
                imgOnGo.gameObject.SetActive(false);
                imgOffGo.gameObject.SetActive(true);
            }
        }

        public void SetToggle(bool isOn)
        {
            this.isOn = isOn;
            OnToggleChangeCallback?.Invoke(isOn);
            RefreshToggle();
        }

#if UNITY_EDITOR

        private void OnToggleChange(bool ison)
        {
            RefreshToggle();
        }

        private void OnValidate()
        {
            btnToggle = transform.GetComponent<UICustomButton>();
        }
#endif
    }
}