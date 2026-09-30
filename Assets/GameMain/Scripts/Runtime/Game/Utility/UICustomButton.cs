using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMain
{
    [RequireComponent(typeof(Button))]
    public class UICustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Button targetButton;

        [LabelText("禁止点击音效")]
        public bool muteClickAudio;

        [LabelText("按钮点击音效")]
        [ShowIf("ClickAudio")]
        public AudioClipName clipName;

        [LabelText("连续点击的间隔")]
        public float continuousClickInterval = 0.2f;
        [LabelText("点击动画")]
        public bool needAni = true;
        
        private Action _callback;

        private float _lastClickSuccessTime;

        private Vector3 startScale;
        private void Awake()
        {
             startScale = transform.localScale;
        }

        public async void SetGray(bool isGray)
        {
            var imgComponent = targetButton.GetComponent<Image>();

            if (imgComponent != null)
            {
                if (isGray)
                {
                    var grayMaterial = await ResourceMgr.Instance.LoadAssetAsync<Material>("UIGray_Static");
                    imgComponent.material = grayMaterial;
                }
                else
                {
                    imgComponent.material = null;
                }
            }
        }

        public void SetInteractable(bool interactable)
        {
            targetButton.interactable = interactable;
            SetGray(!interactable);
        }

        public void AddListener(System.Action onClick)
        {
            if (targetButton == null)
            {
                Debugger.LogError("目标按钮=NULL");
                return;
            }

            _callback = onClick;
            targetButton.onClick.RemoveAllListeners();
            targetButton.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var realtimeSinceStartup = Time.realtimeSinceStartup;
            //点击太快的判断
            if (realtimeSinceStartup - _lastClickSuccessTime < continuousClickInterval) return;

            if (muteClickAudio == false)
            {
                if (string.IsNullOrEmpty(clipName.audioName))
                {
                    clipName.audioName = AudioName.UIClickDefault.AudioName;
                    clipName.audioVolume = AudioName.UIClickDefault.AudioVolume;
                }

                AudioMgr.Instance.PlayAudioOneShot(clipName.audioName, clipName.audioVolume);
            }

            _lastClickSuccessTime = realtimeSinceStartup;
            _callback?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (DOTween.IsTweening(transform))
            { 
                transform.DOKill();
            }
            
            if (needAni)
            {
                transform.DOScale(startScale * 0.93f, 0.2f);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (DOTween.IsTweening(transform))
            {
                transform.DOKill();
            }

            if (needAni)
            {
                transform.DOScale(startScale, 0.2f);
            }
        }

#if UNITY_EDITOR

        private bool ClickAudio => muteClickAudio == false;

        private void OnValidate()
        {
            targetButton = GetComponent<Button>();
            targetButton.transition = Selectable.Transition.None;
        }
#endif
    }
}