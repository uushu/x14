using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public class UICustomSlider : MonoBehaviour
    {
        public RectTransform sliderParent;

        public Image imgSlider;

        public Image imgHandler;

        public TextMeshProUGUI txtCurrent;

        public TextMeshProUGUI txtMax;

        public bool isShowPercent;

        [OnValueChanged("Refersh")] [Range(0, 1)]
        public float progress;

        private float _currentValue;
        public float CurrentValue => _currentValue;

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        public float DeltaLength => MaxValue - MinValue;

        public void SetProgress(float newProgress)
        {
            imgSlider.fillAmount = newProgress;

            if (imgHandler != null)
            {
                imgHandler.transform.localPosition = new Vector3(sliderParent.sizeDelta.x * (newProgress - 0.5f), 0, 0);
            }
        }

        public void SetProgressPercent(float currentValue, float fromValue, float toValue)
        {
            isShowPercent = true;
            txtCurrent.text = $"{imgSlider.fillAmount * 100}%";
            _currentValue = currentValue;
            MinValue = fromValue;
            MaxValue = toValue;

            SetProgress(currentValue / DeltaLength);
        }

        public void SetProgress(float currentValue, float fromValue, float toValue)
        {
            _currentValue = currentValue;
            MinValue = fromValue;
            MaxValue = toValue;
            var newProgress = currentValue / DeltaLength;

            txtCurrent.text = currentValue.ToString();
            if (txtMax != null) txtMax.text = toValue.ToString();

            SetProgress(newProgress);
        }

        public void SetProgressTween(float value, float minValue, float maxValue, float tweenTime, Action<float> onUpdate, Action onComplete)
        {
            TryKillTween();

             MinValue = minValue;
            MaxValue = maxValue;
            if (txtMax != null) txtMax.text = maxValue.ToString();

            float tweenValue = _currentValue;
            _currentValue = Mathf.Min(value, DeltaLength);
         
            DOTween.To(() => tweenValue, (value) =>
            {
                tweenValue = value;
                var newProgress = tweenValue / DeltaLength;
                SetProgress(newProgress);
                var newValue = tweenValue + minValue;
                txtCurrent.text = UtilityTool.ShortNumberToString(newValue);
                onUpdate?.Invoke(newValue);

            }, _currentValue, tweenTime).OnComplete(() => onComplete?.Invoke()).SetTarget(transform).SetEase(Ease.Linear);
        }

        public void TryKillTween()
        {
            if (DOTween.IsTweening(transform)) transform.DOKill();
        }

#if UNITY_EDITOR

        private void Refersh(float newProgress)
        {
            SetProgress(newProgress);
        }
#endif
    }
}