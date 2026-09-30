using System.Collections;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using GameMain;
using UnityEngine;

public class CollectionIllustratedPrefab : MonoBehaviour, IEnhancedScrollerDelegate
{
    public EnhancedScroller Scroller;

    public EnhancedScrollerCellView CellView;

    private List<List<FlowerAtlasConfig>> IllustratedList = new();

    void Start()
    {
    }

    private void DoPlayScale()
    {
        Scroller.transform.localScale = Vector3.zero;
        Scroller.transform.DOScale(Vector3.one, 0.2f);
    }

    public async void Init()
    {
        IllustratedList.Clear();
        var dataList = ConfigMgr.Instance.Tables.FlowerAtlasCategory.DataList;
        for (int i = 0; i < dataList.Count; i += 2)
        {
            List<FlowerAtlasConfig> list = new List<FlowerAtlasConfig>();
            for (int j = 0; j < 2 && i + j < dataList.Count; j++)
            {
                list.Add(dataList[i + j]);
            }

            IllustratedList.Add(list);
        }
        Scroller.Delegate = this;
        Scroller.ReloadData();
        Scroller.transform.localScale = Vector3.zero;
        //await UniTask.WaitForSeconds(0.2f);
        DoPlayScale();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return IllustratedList.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return CellView.GetComponent<RectTransform>().rect.height;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        CollectionIllustratedItem cellView = scroller.GetCellView(CellView) as CollectionIllustratedItem;
        cellView.gameObject.SetActive(true);
        cellView.name = "Cell " + dataIndex.ToString();
        cellView.SetData(IllustratedList[dataIndex]);
        return cellView;
    }
}