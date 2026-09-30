using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class TemplateBtn : MonoBehaviour
{
    [LabelText("禁止点击音效")]
    public bool muteClickAudio;
    [LabelText("按钮点击音效")]
    [ShowIf("ClickAudio")]
    public AudioClipName clipName;
    
    public BtnExtension clickBtn;
    public BtnExtension unClickBtn;

    /// <summary>
    /// 设置是否显示点击
    /// </summary>
    public void SetShowClick(bool isShowClick)
    {
        clickBtn.gameObject.SetActive(isShowClick);
        unClickBtn.gameObject.SetActive(!isShowClick);
    }
    
    /// <summary>
    /// 设置单击
    /// </summary>
    public void SetClick(Action clickBtnAction = null,Action unClickBtnAction = null)
    {
        var temp = new Button.ButtonClickedEvent();
        temp.AddListener(() =>
        {
            if (muteClickAudio == false)
            {
                if (string.IsNullOrEmpty(clipName.audioName))
                {
                    clipName.audioName = AudioName.UIClickDefault.AudioName;
                    clipName.audioVolume = AudioName.UIClickDefault.AudioVolume;
                }
                    
                AudioMgr.Instance.PlayAudioOneShot(clipName.audioName, clipName.audioVolume);
            }
            
            clickBtnAction?.Invoke();
        });
        clickBtn.onClick = temp;
        var temp2 = new Button.ButtonClickedEvent();
        temp2.AddListener(() =>
        {
            unClickBtnAction?.Invoke();
        });
        unClickBtn.onClick = temp2;
    }
    /// <summary>
    /// 设置双击
    /// </summary>
    public void SetDoubleClick(Action clickBtnAction = null,Action unClickBtnAction = null)
    {
        var temp = new Button.ButtonClickedEvent();
        temp.AddListener(() =>
        {
            clickBtnAction?.Invoke();
        });
        clickBtn.OnDoubleClick = temp;
        var temp2 = new Button.ButtonClickedEvent();
        temp2.AddListener(() =>
        {
            unClickBtnAction?.Invoke();
        });
        unClickBtn.OnDoubleClick = temp2;
    }
    /// <summary>
    /// 设置长按
    /// </summary>
    public void SetPressClick(Action longPressBtnAction = null,Action unLongPressBtnAction = null)
    {
        var temp = new Button.ButtonClickedEvent();
        temp.AddListener(() =>
        {
            longPressBtnAction?.Invoke();
        });
        clickBtn.OnLongPress = temp;
        var temp2 = new Button.ButtonClickedEvent();
        temp2.AddListener(() =>
        {
            unLongPressBtnAction?.Invoke();
        });
        unClickBtn.OnLongPress = temp2;
    }
    
#if UNITY_EDITOR
    private bool ClickAudio => muteClickAudio == false;
#endif
    
}
