using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace GameMain
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPromptBox : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        
        public TextMeshProUGUI txtContent;

        [LabelText("延迟隐藏")]
        public float delayHideTime;

        [LabelText("缩放时间")]
        public float scaleTime;
        
        [LabelText("淡出时间")]
        public float fadeOutTime;
        
        public void ShowPromptBox(string content)
        {
            txtContent.text = content;
            gameObject.RefreshAllContentSizeFitter();

            canvasGroup.alpha = 1;
            transform.localScale = Vector3.zero;
            
            Sequence seq = DOTween.Sequence();

            var t1 = transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.OutBack);
            seq.Append(t1);

            seq.AppendInterval(delayHideTime);
            
            var t2 = canvasGroup.DOFade(0, fadeOutTime);
            seq.Append(t2);

            seq.OnComplete(() =>
            {
                GameObject.Destroy(gameObject);
            });
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
#endif
    }
}