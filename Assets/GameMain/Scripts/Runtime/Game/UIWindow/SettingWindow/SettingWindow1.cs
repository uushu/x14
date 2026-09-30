using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;

using GameMain.Garden;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingWindow1 : ABaseWindow
{
   [FoldoutGroup("组件")] public GameMain.UICustomToggle musicToggle;
   [FoldoutGroup("组件")] public GameMain.UICustomToggle soundToggle;
   [FoldoutGroup("组件")] public GameMain.UICustomToggle shakeToggle;
   [FoldoutGroup("组件")] public GameMain.UICustomButton closeBtn;
   [FoldoutGroup("组件")] public GameMain.UICustomButton idCopyBtn;
   [FoldoutGroup("组件")] public GameMain.UICustomButton menuBtn;
   [FoldoutGroup("组件")] public TextMeshProUGUI idText;
   [FoldoutGroup("组件")] public Toggle gmToggle;
   
   
   public Action OnCloseSettingWindowClick { get; set; }
   public Action onBackToMainSettingWindowClick { get; set; }

   private void Awake()
   {
      gmToggle.isOn = false;
   }


   protected override void OnShow(object paremeters)
   {
      base.OnShow(paremeters);
      CheckShowUpdate();
      
      musicToggle.OnToggleClickCallback = OnMusicToggleClick;
      soundToggle.OnToggleClickCallback = OnSoundToggleClick;
      shakeToggle.OnToggleClickCallback = OnShakeToggleClick;
      closeBtn.AddListener(OnCloseWindowClick);
      menuBtn.AddListener(OnMenuWindowClick);
      
      //GM的显示控制
      gmToggle.gameObject.SetActive(GameEntrance.Instance.openGmWindow);
      if (!GameEntrance.Instance.openGmWindow)
      {
         gmToggle.gameObject.SetActive(GameEntrance.IsShowGm(PlatformMgr.UserId));
         
      }
      
      //GM的事件监听
      gmToggle.onValueChanged.AddListener(OnGmToggleClick);

      idText.text = $"UUID:{PlatformMgr.UserId}";
      idCopyBtn.AddListener(() =>
      {
         PlatformMgr.Instance.GetCopyToClipboard(idText.text, (result) =>
         {
            UIWindowTool.ShowPromptBox(result? "复制成功" : "复制失败");
         });
      });

   }

   private void OnMenuWindowClick()
   {
      CloseSelfWindow();
      onBackToMainSettingWindowClick?.Invoke();//触发回到主界面的事件
      
   }

   private void OnGmToggleClick(bool isOn)
   {
      if (isOn)
      {
         UIWindowMgr.Instance.OpenWindowAsync<GmWindow>();
      }
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

      }
#endif
}


