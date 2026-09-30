using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using GameMain;
using UnityEngine;

public class CollectionThemePrefab : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller enScroller;
    public EnhancedScrollerCellView cellViewPrefab;

    private List<CollectionThemeItemSaveData> data = new List<CollectionThemeItemSaveData>();

    private float oldPress;

    private void DoPlayScale()
    {
        enScroller.transform.localScale = Vector3.zero;
        enScroller.transform.DOScale(Vector3.one, 0.2f);
    }

    public async void Init()
    {
        //enScroller.transform.localScale =  Vector3.zero;
        oldPress = 0;
        UpdateShow();
        enScroller.transform.localScale =  Vector3.zero;
        //await UniTask.WaitForSeconds(0.2f);
        DoPlayScale();
    }

    public void UpdateShow(bool holdPos = false)
    {
        data = LocalSaveMgr.Instance.DataCenter.GetLocalSave<Save_Main>().allThemeItem.Dictionary.Values.ToList();
        enScroller.Delegate = this;
        if (holdPos)
        {
            enScroller.ReloadData(1 - enScroller.ScrollRect.verticalScrollbar.value);
        }
        else
        {
            enScroller.ReloadData();
           // enScroller;
        }
        
        //刷新显示
        foreach (Transform item in enScroller.Container)
        {
            CollectionThemeItem temp = item.GetComponent<CollectionThemeItem>();
            if (temp != null && item.gameObject.activeSelf)
            {
                temp.UpdateShow();
            }
        }
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellViewPrefab.GetComponent<RectTransform>().rect.height;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        CollectionThemeItem cellView = scroller.GetCellView(cellViewPrefab) as CollectionThemeItem;
        cellView.gameObject.SetActive(true);
        cellView.name = "Cell " + dataIndex.ToString();
        cellView.SetData(this, data[dataIndex]);
        return cellView;
    }
}