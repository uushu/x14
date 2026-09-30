using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using GameMain;
using UnityEngine;

public class CollectionPrefab : MonoBehaviour, ISelectBtnHandle, IEnhancedScrollerDelegate
{
    public List<SelectBtn> SelectList;

    public Transform ThemeNode;
    public Transform FlowersNode;
    public Transform IllustratedNode;

    public EnhancedScroller Scroller;

    public EnhancedScrollerCellView CellView;

    private int selectIndex = -1;

    private CollectionIllustratedPrefab IllustratedPrefab;
    private CollectionThemePrefab ThemePrefab;

    private List<List<FlowerCollectConfig>> CollectionFlowers = new();
    
    private void Awake()
    {
       // Scroller.transform.localScale=new Vector3(0.1f,0.1f,0.1f);
    }

    private void OnDisable()
    {
        selectIndex = -1;
    }
    
    private void DoPlayScale()
    {
        Scroller.transform.localScale=Vector3.zero;
        Scroller.transform.DOScale(Vector3.one, 0.2f);
    }

    public async  void Init(int index = 0)
    {
        for (int i = 0; i < SelectList.Count; i++)
        {
            SelectList[i].Init(this, i);
        }
        selectIndex = index;
        UpdateSelectShow();
        CollectionFlowers.Clear();
        var dataList = ConfigMgr.Instance.Tables.FlowerCollectCategory.DataList;
        for (int i = 0; i < dataList.Count; i += 3)
        {
            List<FlowerCollectConfig> list = new List<FlowerCollectConfig>();
            for (int j = 0; j < 3 && i + j < dataList.Count; j++)
            {
                list.Add(dataList[i + j]);
            }

            CollectionFlowers.Add(list);
        }
        Scroller.Delegate = this;
        Scroller.ReloadData();
        DoPlayScale();
    }
    
    private async Task<T> LoadCollectionPrefab<T>(string name, Transform tf)
    {
        var go = await ResourceMgr.Instance.LoadAssetAsync<GameObject>(name);
        var instance = Instantiate(go, tf);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localScale = Vector3.one;
        var Prefab = instance.GetComponent<T>();
        return Prefab;
    }

    public async void OnSelectBtnClick(int index)
    {
        if (selectIndex == index) return;
        selectIndex = index;
        UpdateSelectShow();
        if (index == 1)
        {
            if (IllustratedPrefab == null)
            {
                IllustratedPrefab =
                    await LoadCollectionPrefab<CollectionIllustratedPrefab>("CollectionIllustratedPrefab",
                        IllustratedNode);
            }

            IllustratedPrefab.Init();
        }

        if (index == 2)
        {
            if (ThemePrefab == null)
            {
                ThemePrefab = await LoadCollectionPrefab<CollectionThemePrefab>("CollectionThemePrefab", ThemeNode);
            }

            ThemePrefab.Init();
        }
        if (index == 0)
        {
            Init();
        }
    }

    public void UpdateSelectShow()
    {
        for (int i = 0; i < SelectList.Count; i++)
        {
            SelectList[i].SetSelect(i == selectIndex);
        }
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return CollectionFlowers.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return CellView.GetComponent<RectTransform>().rect.height;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        CollectionFlowerItem cellView = scroller.GetCellView(CellView) as CollectionFlowerItem;
        cellView.gameObject.SetActive(true);
        cellView.name = "Cell " + dataIndex.ToString();
        cellView.SetData(CollectionFlowers[dataIndex]);
        return cellView;
    }

    private void OnDestroy()
    {
        if (IllustratedPrefab != null)
        {
            Destroy(IllustratedPrefab.gameObject);
        }

        IllustratedPrefab = null;
        if (ThemePrefab != null)
        {
            Destroy(ThemePrefab.gameObject);
        }

        ThemePrefab = null;
    }
}