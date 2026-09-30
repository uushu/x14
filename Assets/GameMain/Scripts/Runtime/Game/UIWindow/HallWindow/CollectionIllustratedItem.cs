using System.Collections;
using System.Collections.Generic;
using cfg;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionIllustratedItem : EnhancedScrollerCellView
{
    public List<CollectionIllustratedItemItem> listItem;

    public void SetData(List<FlowerAtlasConfig> data)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].gameObject.SetActive(i < data.Count);
            if (i < data.Count)
            {
                listItem[i].SetData(data[i]);
            }
        }
    }
    
}
