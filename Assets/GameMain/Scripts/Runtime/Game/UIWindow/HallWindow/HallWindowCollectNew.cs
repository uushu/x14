using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;

public class HallWindowCollectNew : MonoBehaviour
{
    private EventGroup _eventGroup = new();
    void Start()
    {
        _eventGroup.AddListener<GameEventDefine.CollectFlowerChange>(ProcessMessage);
        UpdateUI();
    }

    private void ProcessMessage(IEventMessage obj)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        var data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_CollectFragments>();
        if (data != null)
        {
            gameObject.SetActive(data.NewGet.Count>0);
        }
    }

    private void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }
}
