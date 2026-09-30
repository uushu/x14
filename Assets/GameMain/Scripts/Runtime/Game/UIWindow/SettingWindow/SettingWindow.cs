using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using GameMain.Garden;
using TMPro;
using UnityEngine;

public class SettingWindow : ABaseWindow
{
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomToggle mUICustomToggle_Music;

		[Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomToggle mUICustomToggle_Sound;

		[Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomToggle mUICustomToggle_Shake;
        
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnClose;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mGmBtn;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TextMeshProUGUI mTextUUid;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton copyBtn;
    
        public Action OnCloseSettingWindowClick {  get; set; }

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            CheckShowUpdate();
            mUICustomToggle_Music.OnToggleClickCallback = OnMusicToggleClick;
            mUICustomToggle_Sound.OnToggleClickCallback = OnSoundToggleClick;
            mUICustomToggle_Shake.OnToggleClickCallback = OnShakeToggleClick;
            
            mBtnClose.AddListener(OnClseWindowClick);

            mGmBtn.gameObject.SetActive(GameEntrance.Instance.openGmWindow);
            if (!GameEntrance.Instance.openGmWindow)
            {
                mGmBtn.gameObject.SetActive(GameEntrance.IsShowGm(PlatformMgr.UserId));
            }
            
            mGmBtn.AddListener(() =>
            {
                UIWindowMgr.Instance.OpenWindowAsync<GmWindow>();
            });
            mTextUUid.text = $"UUID:{PlatformMgr.UserId}";
            copyBtn.AddListener(() =>
            {
                PlatformMgr.Instance.GetCopyToClipboard(mTextUUid.text, (result) =>
                {
                    UIWindowTool.ShowPromptBox(result ? "复制成功" : "复制失败");
                });
            });
        }

        private void OnMusicToggleClick(bool isOn)
        {
            AudioMgr.Instance.MusicOn = !isOn;
            CheckShowUpdate();
        }

        private void OnSoundToggleClick(bool isOn)
        {
            AudioMgr.Instance.SoundOn = !isOn;
            CheckShowUpdate();
        }

        private void OnShakeToggleClick(bool isOn)
        {
            AudioMgr.Instance.ShakeIsOn = !isOn;
            CheckShowUpdate();
        }

        public void CheckShowUpdate()
        {
            mUICustomToggle_Music.SetToggle(AudioMgr.Instance.MusicOn);
            mUICustomToggle_Sound.SetToggle(AudioMgr.Instance.SoundOn);
            mUICustomToggle_Shake.SetToggle(AudioMgr.Instance.ShakeIsOn);
        }

     

        private void OnClseWindowClick()
        {
            CloseSelfWindow();
            OnCloseSettingWindowClick?.Invoke();
        }

    
#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            var musicComponent = transform.Find("WindowRoot/imgBg/UICustomToggle-Music");
            if (musicComponent != null)
            {
                mUICustomToggle_Music = musicComponent.GetComponent<GameMain.UICustomToggle>();
            }

            var soundComponent = transform.Find("WindowRoot/imgBg/UICustomToggle-Sound");
            if (soundComponent != null)
            {
                mUICustomToggle_Sound = soundComponent.GetComponent<GameMain.UICustomToggle>();
            }

            var shakeComponent = transform.Find("WindowRoot/imgBg/UICustomToggle-Shake");
            if (shakeComponent != null)
            {
                mUICustomToggle_Shake = shakeComponent.GetComponent<GameMain.UICustomToggle>();
            }
            
            var closeComponent = transform.Find("WindowRoot/imgBg/btnClose");
            if (closeComponent != null)
            {
                mBtnClose = closeComponent.GetComponent<GameMain.UICustomButton>();
            }
        }

#endif
    }
