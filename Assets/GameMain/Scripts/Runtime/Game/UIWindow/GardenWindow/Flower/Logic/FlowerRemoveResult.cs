using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    //鲜花消除的结果
    public class FlowerRemoveResult
    {
        [LabelText("本次放置的格子")]
        public List<Garden_FlowerSingle> putInMapFlowers = new();

        [LabelText("放置在屏幕上的坐标")]
        public Vector2 putInMapScreenPoint;

        [LabelText("横排")]
        public List<int> rowList = new();

        [LabelText("竖排")]
        public List<int> colList = new();

        [LabelText("是否复活消除")]
        public bool isReliveRemove;
        
        [LabelText("移除的鲜花")]
        public Dictionary<int, List<Garden_FlowerSingle>> removeFlowers = new();

        public int TotalRemoveCount => rowList.Count + colList.Count;

        public bool HasRowColRemove => TotalRemoveCount > 0;

        public void Clear()
        {
            putInMapFlowers.Clear();
            colList.Clear();
            rowList.Clear();
            removeFlowers.Clear();
            isReliveRemove = false;
        }

        public override string ToString()
        {
            return $"rowList={rowList.Count} colList={colList.Count}";
        }
    }
}