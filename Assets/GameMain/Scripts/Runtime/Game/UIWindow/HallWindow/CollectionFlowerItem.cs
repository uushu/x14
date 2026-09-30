using System.Collections;
using System.Collections.Generic;
using cfg;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public class CollectionFlowerItem : EnhancedScrollerCellView
{
    public List<CollectionFlowerItemItem> listItem;
    public void SetData(List<FlowerCollectConfig> data)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].gameObject.SetActive(i<data.Count);
            if (i<data.Count)
            {
                listItem[i].SetData(data[i]);
            }
        }
    }
}
