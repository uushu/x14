using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyBar : MonoBehaviour
{
    public Image CurrencyImage;
    public UICustomButton addBtn;
    public TextMeshProUGUI CurrencyText;

    public int ItemId = 1;
    
    private readonly EventGroup _eventGroup = new();
    private void Awake()
    {
        addBtn.AddListener(OnAddClick);
    }

    void Start()
    {
        _eventGroup.AddListener<GameEventDefine.GetBagProp>(ProcessMessage);
        _eventGroup.AddListener<GameEventDefine.UseBagProp>(ProcessMessage);
        RefreshBagPropUI();
    }
    private void ProcessMessage(IEventMessage obj)
    {
        RefreshBagPropUI();
    }
    private void RefreshBagPropUI()
    {
        var bagPropData = BagMgr.Instance.TryGetBagProp(ItemId);

        if (bagPropData == null)
        {
            bagPropData = new ItemData_BagProp(ItemId, 0);
        }
        ResourceMgr.Instance.SetImageSprite(CurrencyImage, bagPropData.Config.Sprite);
        CurrencyText.text = bagPropData.PropCountString;
    }
    private void OnDestroy()
    {
        _eventGroup.RemoveAllListener();
    }

    private void OnAddClick()
    {
        if (ItemId==GameTool.ItemGoldId)
        {
            UIWindowMgr.Instance.OpenWindowAsync<GoldGetWindow>();
            //UIWindowMgr.Instance.OpenWindowAsync<GoldGetWindow2>();
        }
    }
    
 
}