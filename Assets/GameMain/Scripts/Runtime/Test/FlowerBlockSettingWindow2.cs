using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using GameMain.FlowerBlock;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class FlowerBlockSettingWindow2 : ABaseWindow
{
    [FoldoutGroup("组件")] public GameMain.UICustomToggle musicToggle;
    [FoldoutGroup("组件")] public GameMain.UICustomToggle soundToggle;
    [FoldoutGroup("组件")] public GameMain.UICustomToggle shakeToggle;
    [FoldoutGroup("组件")] public GameMain.UICustomButton closeBtn;
    [FoldoutGroup("组件")] public GameMain.UICustomButton menuBtn;
    [FoldoutGroup("组件")] public GameMain.UICustomButton resetBtn;
    [FoldoutGroup("组件")] public GameMain.UICustomButton nextBtn;
    
    
   public Action OnCloseSettingWindowClick { get; set; }
   public Action OnBackHallClickCallBack { get; set; }
   public Action OnRestartClickCallBack { get; set; }
   

   

   protected override void OnShow(object paremeters)
   {
      base.OnShow(paremeters);
      CheckShowUpdate();
      
      musicToggle.OnToggleClickCallback = OnMusicToggleClick;
      soundToggle.OnToggleClickCallback = OnSoundToggleClick;
      shakeToggle.OnToggleClickCallback = OnShakeToggleClick;
      closeBtn.AddListener(OnCloseWindowClick);
      menuBtn.AddListener(OnMenuWindowClick);
      resetBtn.AddListener(OnResetWindowClick);
      
      
      var blockWindow=UIWindowMgr.Instance.GetWindow<FlowerBlockWindow>()!=null;
      if (blockWindow)
      {
         nextBtn.AddListener(() =>
         {
            CloseSelfWindow();
            UIWindowMgr.Instance.GetWindow<FlowerBlockWindow>().AutoNextLevel();
         });
         nextBtn.gameObject.SetActive(GameEntrance.IsShowGm(PlatformMgr.UserId));
      }
      else
      {
         nextBtn.gameObject.SetActive(false);
      }

     

   }

   

   private void OnResetWindowClick()
   {
      CloseSelfWindow();
      OnRestartClickCallBack?.Invoke();
   }

   private void OnMenuWindowClick()
   {
      CloseSelfWindow();
      OnBackHallClickCallBack?.Invoke();//触发回到主界面的事件
      
   }

   private void OnCloseWindowClick()
   {
      CloseSelfWindow();
      OnCloseSettingWindowClick?.Invoke();
   }

   private void OnShakeToggleClick(bool isOn)
   {
      AudioMgr.Instance.ShakeIsOn = !isOn;
      CheckShowUpdate();
   }

   private void OnSoundToggleClick(bool isOn)
   {
      AudioMgr.Instance.SoundOn = !isOn;
      CheckShowUpdate();
   }

   private void OnMusicToggleClick(bool isOn)
   {
      AudioMgr.Instance.MusicOn = !isOn;
      CheckShowUpdate();
   }

   private void CheckShowUpdate()
   {
      musicToggle.SetToggle(AudioMgr.Instance.MusicOn);
      soundToggle.SetToggle(AudioMgr.Instance.SoundOn);
      shakeToggle.SetToggle(AudioMgr.Instance.ShakeIsOn);
   }
   
   
   
#if UNITY_EDITOR   
   public void OnValidate()
      {
            base.OnValidate();

            var musicComponent = transform.Find("WindowRoot/imgBg/UICustomToggle-Music");
            if (musicComponent != null)
            {
               musicToggle = musicComponent.GetComponent<GameMain.UICustomToggle>();
            }

            var soundComponent = transform.Find("WindowRoot/imgBg/UICustomToggle-Sound");
            if (soundComponent != null)
            {
                soundToggle = soundComponent.GetComponent<GameMain.UICustomToggle>();
            }

            var shakeComponent = transform.Find("WindowRoot/imgBg/UICustomToggle-Shake");
            if (shakeComponent != null)
            {
                shakeToggle = shakeComponent.GetComponent<GameMain.UICustomToggle>();
            }
            
            var closeComponent = transform.Find("WindowRoot/imgBg/btnClose");
            if (closeComponent != null)
            {
                closeBtn = closeComponent.GetComponent<GameMain.UICustomButton>();
            }
            
            var backComponent = transform.Find("WindowRoot/imgBg/btnBack");
            if (backComponent != null)
            {
               menuBtn= backComponent.GetComponent<GameMain.UICustomButton>();
            }
            
            var resetComponent=transform.Find("WindowRoot/imgBg/btnReset");
            if (resetComponent != null)
            {
                resetBtn = resetComponent.GetComponent<GameMain.UICustomButton>();
            }
      }
#endif
}
