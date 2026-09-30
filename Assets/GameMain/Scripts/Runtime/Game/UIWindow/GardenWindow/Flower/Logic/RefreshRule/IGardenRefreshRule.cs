using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace GameMain.Garden
{
    public class GardenRefreshFlowerInfo
    {
        //格子的类型
        public int GridTypeId;

        //鲜花的主Id
        public int MainFlowerId;

        //鲜花列表
        public List<int> FlowersId;

        public GardenRefreshFlowerInfo()
        {
            FlowersId = new();
        }

        public void Clear()
        {
            FlowersId.Clear();
        }

        private static Queue<GardenRefreshFlowerInfo> _pool = new();

        public static GardenRefreshFlowerInfo Get()
        {
            GardenRefreshFlowerInfo info = default;

            if (_pool.Count > 0)
            {
                info = _pool.Dequeue();
            }

            info = new GardenRefreshFlowerInfo();
            // Debugger.Log($"Get GardenRefreshFlowerInfo:{info.GetHashCode()}");
            return info;
        }

        public static void Release(GardenRefreshFlowerInfo info)
        {
            // Debugger.Log($"Release GardenRefreshFlowerInfo:{info.GetHashCode()}");
            info.FlowersId.Clear();
            _pool.Enqueue(info);
        }
    }

    public class GardenRefreshFlowerInfoList
    {
        public static List<GardenRefreshFlowerInfo> Get(int listCount)
        {
            var list = ListPool<GardenRefreshFlowerInfo>.Get();

            for (int i = 0; i < listCount; i++)
            {
                var flowerInfo = GardenRefreshFlowerInfo.Get();
                list.Add(flowerInfo);
            }

            Debugger.Log($"Get GardenRefreshFlowerInfo:{list.GetHashCode()}");
            return list;
        }

        public static void Release(List<GardenRefreshFlowerInfo> listInfos)
        {
            Debugger.Log($"Release GardenRefreshFlowerInfo:{listInfos.GetHashCode()}");

            for (int i = 0; i < listInfos.Count; i++)
            {
                GardenRefreshFlowerInfo.Release(listInfos[i]);
            }
            listInfos.Clear();
            ListPool<GardenRefreshFlowerInfo>.Release(listInfos);
        }
    }

    public interface IGardenRefreshRule
    {
        //最少能放入地图的格子数 
        public const int LeastCountCanPutInMap = 2;

        //key=形状Id value=权重
        public SortedDictionary<int, int> GridTypeCacheDict { get; }

        public List<List<int>> InitMapGridType { get; }

        void Initialize(int levelId);
        void RefreshFlowerInfo(int maxCount, int leastCount, out List<GardenRefreshFlowerInfo> flowerInfos);
        int RefreshFlowerId();
        void FillGridFlowers(int gridMaxCount, int mainFlowerId, ref List<int> gridFlowers);
        bool RefreshBuffterFly(out int butterFlyId);
        void InsureLeastFlowerCanPlace(int leastCount, out List<GardenRefreshFlowerInfo> listInfos);
        void ReliveRefreshFlowerInfo(int refreshCount, out List<GardenRefreshFlowerInfo> flowerInfos);
    }
}