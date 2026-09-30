using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;

public class MainTaskNode : MonoBehaviour
{
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public GameMain.UICustomButton MainTaskBtn;
    
    [Sirenix.OdinInspector.FoldoutGroup("组件")]
    public GameObject OkFlag;

    public DoShakeRotation DoShake;
    
    private EventGroup _eventGroup = new();
    void Start()
    {
        _eventGroup.AddListener<GameEventDefine.AchievementProgressChange>(ProcessMessage);
        MainTaskBtn.AddListener(OnTaskClick);
        updateView();
    }

    private void ProcessMessage(IEventMessage obj)
    {
        updateView();
    }

    private void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }
    private void updateView()
    {
        OkFlag.SetActive(AchievementMgr.Instance.IsMainTaskFinish());
        if (OkFlag.activeSelf)
        {
            DoShake.PlayAnim();
        }
        else
        {
            DoShake.StopAnim();
        }
    }

    private void OnTaskClick()
    {
        UIWindowMgr.Instance.OpenWindowAsync<AchievementWindow>();   
    }
   
}
