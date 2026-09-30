using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using GameMain;
using UnityEngine;

public class ShopPrefab : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView CellView;
    private List<int> list = new List<int>();

    void Start()
    {
    }

    private void DoPlayScale()
    {
        scroller.transform.localScale = Vector3.zero;
        scroller.transform.DOScale(Vector3.one, 0.2f);
    }

    public void Init()
    {
        list.Clear();
        foreach (var item in ConfigMgr.Instance.Tables.ShopCategory.DataList)
        {
            if (!list.Contains(item.Id))
            {
                list.Add(item.Id);
            }
        }

        scroller.Delegate = this;
        scroller.ReloadData();
        DoPlayScale();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return list.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        var item = CellView.GetComponent<ShopItem>();
        var config = ConfigMgr.Instance.Tables.ShopCategory.Get(list[dataIndex]);
        if (config != null)
        {
            if (config.ShopType == 1)
            {
                return item.LookAdNode.GetComponent<RectTransform>().rect.height;
            }

            if (config.ShopType == 2)
            {
                return item.BuyNode.GetComponent<RectTransform>().rect.height;
            }
        }
        return CellView.GetComponent<RectTransform>().rect.height;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ShopItem cellView = scroller.GetCellView(CellView) as ShopItem;
        cellView.gameObject.SetActive(true);
        cellView.name = "Cell " + dataIndex.ToString();
        cellView.SetData(list[dataIndex]);
        return cellView;
    }
}