using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace GameMain.AOT
{
    public class UILoadingWindow : MonoBehaviour
    {
        [FoldoutGroup("组件")]
        public Slider sliderBar;
        [FoldoutGroup("组件")]
        public TextMeshProUGUI text;
        [FoldoutGroup("组件")]
        public GameObject loadingObj;

        private Action startAction;
        void Start()
        {
            sliderBar.gameObject.SetActive(false);
            loadingObj.gameObject.SetActive(false);
            sliderBar.value = 0;
        }
        /// <summary>
        /// 设置开始回调
        /// </summary>
        public void SetStartAction(Action callBack)
        {
            startAction = callBack;
        }
        /// <summary>
        /// 显示进度条完成
        /// </summary>
        private float targetValue;
        public void ShowBarValue(float value)
        {
            targetValue = value;
        }

        
        public void Update()
        {
            if (sliderBar.value < targetValue)
            {
                sliderBar.value += Time.deltaTime;
                text.text = (sliderBar.value * 100).ToString("0");
                if (sliderBar.value >= 1f)
                {
                    sliderBar.value = 1;
                    text.text = "100";
                    GameObject.Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void OnClickStartBtn()
        {
            sliderBar.gameObject.SetActive(true);
            loadingObj.gameObject.SetActive(true);
            startAction?.Invoke();
        }
        
    }
}