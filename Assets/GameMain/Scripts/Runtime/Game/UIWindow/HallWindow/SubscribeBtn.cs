using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;

public class SubscribeBtn : MonoBehaviour
{
    public GameMain.UICustomButton Btn;
    
    protected readonly EventGroup _eventGroup = new EventGroup();
    private void Awake()
    {
        _eventGroup.AddListener<GameEventDefine.SubscribeSucess>(ProcessMessage);
    }
    private void Start()
    { 
        Btn.AddListener(OnClick);
#if UNITY_DOUYIN_GAME
        gameObject.SetActive(false);
        if (PlatformMgr.Instance.Platform is PlatformDy platformDy)
        {
            platformDy.CheckFeedSubscribePowerStatus((data) =>
            {
                if ((bool)data["success"])
                {
                    gameObject.SetActive(false);
                }
                
            }, (error,errorStr) =>
            {
                //UIWindowMgr.Instance.ShowPromptBox(errorStr);
            });
        }
        
#else
         gameObject.SetActive(false);
#endif
       
    }

    private void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }

    private void ProcessMessage(IEventMessage obj)
    {
        if (obj is GameEventDefine.SubscribeSucess )
        {
            gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        UIWindowMgr.Instance.OpenWindowAsync<SubscribeWindow>();
    }
}
