using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameMain
{
    public class GridItemPool
    {
        GameObject mPrefabObj;
        string mPrefabName;
        int mInitCreateCount = 1;
        List<LoopGridViewItem> mTmpPooledItemList = new List<LoopGridViewItem>();
        List<LoopGridViewItem> mPooledItemList = new List<LoopGridViewItem>();
        static int mCurItemIdCount = 0;
        RectTransform mItemParent = null;

        public GridItemPool()
        {

        }

        public void Init<T>(GameObject prefabObj, int createCount, RectTransform parent) where T : LoopGridViewItem
        {
            mPrefabObj = prefabObj;

            mPrefabName = mPrefabObj.name;

            mInitCreateCount = createCount;
            mItemParent = parent;

            mPrefabObj.SetActive(false);

            for (int i = 0; i < mInitCreateCount; ++i)
            {
                T tViewItem = CreateItem<T>();

                RecycleItemReal(tViewItem);
            }
        }

        public T GetItem<T>() where T : LoopGridViewItem
        {
            mCurItemIdCount++;
            T tItem = null;
            if (mTmpPooledItemList.Count > 0)
            {
                int count = mTmpPooledItemList.Count;
                tItem = mTmpPooledItemList[count - 1] as T;
                mTmpPooledItemList.RemoveAt(count - 1);
                tItem.gameObject.SetActive(true);
            }
            else
            {
                int count = mPooledItemList.Count;
                if (count == 0)
                {
                    tItem = CreateItem<T>();
                }
                else
                {
                    tItem = mPooledItemList[count - 1] as T;
                    mPooledItemList.RemoveAt(count - 1);
                    tItem.gameObject.SetActive(true);
                }
            }
            tItem.ItemId = mCurItemIdCount;
            tItem.name = $"{typeof(T).Name}_{mCurItemIdCount}";
            return tItem;
        }

        public void DestroyAllItem()
        {
            ClearTmpRecycledItem();
            int count = mPooledItemList.Count;
            for (int i = 0; i < count; ++i)
            {
                GameObject.DestroyImmediate(mPooledItemList[i].gameObject);
            }
            mPooledItemList.Clear();
        }

        public T CreateItem<T>() where T : LoopGridViewItem
        {
            GameObject go = GameObject.Instantiate<GameObject>(mPrefabObj, Vector3.zero, Quaternion.identity, mItemParent);
            go.SetActive(true);
            RectTransform rf = go.GetComponent<RectTransform>();
            rf.localScale = Vector3.one;
            rf.anchoredPosition3D = Vector3.zero;
            rf.localEulerAngles = Vector3.zero;
            T tViewItem = go.GetComponent<T>();
            tViewItem.ItemPrefabName = mPrefabName;
            tViewItem.InitializeLoopItem();
            return tViewItem;
        }

        void RecycleItemReal<T>(T item) where T : LoopGridViewItem
        {
            item.gameObject.SetActive(false);

            mPooledItemList.Add(item);
        }

        public void RecycleItem(LoopGridViewItem item)
        {
            item.PrevItem = null;
            item.NextItem = null;
            mTmpPooledItemList.Add(item);
        }

        public void ClearTmpRecycledItem()
        {
            int count = mTmpPooledItemList.Count;
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; ++i)
            {
                RecycleItemReal(mTmpPooledItemList[i]);
            }
            mTmpPooledItemList.Clear();
        }
    }
}
