using System;
using System.Collections;
using System.Collections.Generic;

namespace GameMain.Garden
{
    public class GardenSettingWindow : ABaseWindow
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomToggle mUICustomToggle_Music;

		[Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomToggle mUICustomToggle_Sound;

		[Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomToggle mUICustomToggle_Shake;

		[Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnHall;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnRestart;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnClose;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public GameMain.UICustomButton mBtnNextLevel;

        public Action OnBackHallClickCallBack { get; set; }

        public Action OnRestartClickCallBack { get; set; }

        public Action OnCloseSettingWindowClick {  get; set; }

        protected override void OnShow(object parameters)
        {
            base.OnShow(parameters);

            CheckShowUpdate();
            mUICustomToggle_Music.OnToggleClickCallback = OnMusicToggleClick;
            mUICustomToggle_Sound.OnToggleClickCallback = OnSoundToggleClick;
            mUICustomToggle_Shake.OnToggleClickCallback = OnShakeToggleClick;

            mBtnHall.AddListener(OnHallClick);
            mBtnRestart.AddListener(OnRestart);
            mBtnClose.AddListener(OnClseWindowClick);

            mBtnNextLevel.AddListener(() =>
            {
                UIWindowMgr.Instance.GetWindow<GardenWindow>().AutoNextLevel();
            });
            
            mBtnNextLevel.gameObject.SetActive(GameEntrance.IsShowGm(PlatformMgr.UserId));
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

        private void OnHallClick()
        {
            CloseSelfWindow();
            OnBackHallClickCallBack?.Invoke();
        }

        private void OnClseWindowClick()
        {
            CloseSelfWindow();
            OnCloseSettingWindowClick?.Invoke();
        }

        private void OnRestart()
        {
            CloseSelfWindow();
            OnRestartClickCallBack?.Invoke();
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

            var hallComponent = transform.Find("WindowRoot/imgBg/btnHall");
            if (hallComponent != null)
            {
                mBtnHall = hallComponent.GetComponent<GameMain.UICustomButton>();
            }

            var restartComponent = transform.Find("WindowRoot/imgBg/btnRestart");
            if (restartComponent != null)
            {
                mBtnRestart = restartComponent.GetComponent<GameMain.UICustomButton>();
            }

            var closeComponent = transform.Find("WindowRoot/imgBg/btnClose");
            if (closeComponent != null)
            {
                mBtnClose = closeComponent.GetComponent<GameMain.UICustomButton>();
            }
        }

#endif
    }
}