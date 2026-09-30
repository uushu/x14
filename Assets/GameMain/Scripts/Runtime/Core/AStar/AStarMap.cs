using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class AStarMap
    {
        [Flags]
        public enum EStarNode
        {
            [LabelText("未定义")]
            None = 1 << 0,
            [LabelText("可行走")]
            Walkable = 1 << 1,
            [LabelText("墙壁")]
            Obstacle = 1 << 2,
            [LabelText("技能槽位")]
            SkillUnit = 1 << 3,
        }

        //斜向消耗
        private const int INCLINED_PATH_COST = 14;
        //直线消耗
        private const int STRAIGHT_PATH_COST = 10;

        public Vector2Int MapSize { get; private set; }

        public AStarNode[,] AllNodes { get; private set; }

        public AStarMap(Vector2Int mapSize)
        {
            MapSize = mapSize;

            InitializeMap();
        }

        private void InitializeMap()
        {
            AllNodes = new AStarNode[MapSize.x, MapSize.y];

            for (int y = 0; y < MapSize.y; y++)
            {
                for (int x = 0; x < MapSize.x; x++)
                {
                    AllNodes[x, y] = new(new Vector2Int(x, y), EStarNode.Walkable);
                    AllNodes[x, y].ResetValue();
                }
            }
        }

        private void ResetValue()
        {
            for (int j = 0; j < MapSize.y; j++)
            {
                for (int i = 0; i < MapSize.x; i++)
                {
                    AllNodes[i, j].ResetValue();
                }
            }
        }

        public AStarNode GetAStarNode(Vector2Int nodeIndex)
        {
            if (nodeIndex.x < 0 || nodeIndex.x >= MapSize.x)
            {
                Debugger.LogError($"下标越界:{nodeIndex}");
                return null;
            }

            if (nodeIndex.y < 0 || nodeIndex.y >= MapSize.y)
            {
                Debugger.LogError($"下标越界:{nodeIndex}");
                return null;
            }

            return AllNodes[nodeIndex.x, nodeIndex.y];
        }

        public bool AStarNodeIsValid(Vector2Int nodeIndex)
        {
            if (nodeIndex.x < 0 || nodeIndex.x >= MapSize.x)
            {
                return false;
            }

            if (nodeIndex.y < 0 || nodeIndex.y >= MapSize.y)
            {
                return false;
            }

            return true;
        }

        public AStarNode GetAStarNode(int index)
        {
            return GetAStarNode(new Vector2Int(index % MapSize.x, index / MapSize.x));
        }

        private int CalcNodeDistance(AStarNode node1, AStarNode node2)
        {
            var offsetX = Mathf.Abs(node1.MapIndex.x - node2.MapIndex.x);
            var offsetY = Mathf.Abs(node1.MapIndex.y - node2.MapIndex.y);

            var straightOffset = Mathf.Abs(offsetX - offsetY);
            var maxOffset = offsetX > offsetY ? offsetX : offsetY;

            var inclinedOffset = maxOffset - straightOffset;

            return straightOffset * STRAIGHT_PATH_COST + inclinedOffset * INCLINED_PATH_COST;
        }

        private void GetNeighborNodeList(AStarNode node, List<Vector2Int> forceWalkable, ref List<AStarNode> result)
        {
            result.Clear();

            Vector2Int[] neighborIndexs = new Vector2Int[]
            {
                new Vector2Int(-1, 0),new Vector2Int(0, -1),
                new Vector2Int(1, 0), new Vector2Int(0, 1),
                new Vector2Int(-1, 1), new Vector2Int(-1, -1),
                new Vector2Int(1, -1), new Vector2Int(1, 1),
            };

            for (int i = 0; i < neighborIndexs.Length; i++)
            {
                var neighborIndex = neighborIndexs[i] + node.MapIndex;

                if (neighborIndex.x >= MapSize.x || neighborIndex.x < 0)
                {
                    continue;
                }

                if (neighborIndex.y >= MapSize.y || neighborIndex.y < 0)
                {
                    continue;
                }

                if (GetAStarNode(neighborIndex).CanWalkable == false &&
                    (forceWalkable == null || forceWalkable.Contains(neighborIndex) == false))
                {
                    continue;
                }

                if (i >= 4)
                {
                    var node1 = GetAStarNode(neighborIndex - new Vector2Int(neighborIndexs[i].x, 0));
                    var node2 = GetAStarNode(neighborIndex - new Vector2Int(0, neighborIndexs[i].y));

                    //斜角只要有阻挡物 就不可以移动
                    if ((node1.CanWalkable == false || node2.CanWalkable == false))
                    {
                        continue;
                    }
                }

                var neighborNode = GetAStarNode(neighborIndex);
                if (neighborNode != null)
                {
                    result.Add(neighborNode);
                }
            }
        }

        public bool SearAStarPath(Vector2Int startGrid, Vector2Int finishGrid, List<Vector2Int> forceWalkable, ref List<AStarNode> result)
        {
            var startNode = GetAStarNode(startGrid);
            var finishNode = GetAStarNode(finishGrid);

            return FindAStarPath(startNode, finishNode, forceWalkable, ref result);
        }

        public bool FindAStarPath(AStarNode startNode, AStarNode endNode, List<Vector2Int> forceWalkable, ref List<AStarNode> result)
        {
            result.Clear();
            var openList = new List<AStarNode>() { startNode };
            var closeList = new List<AStarNode>();
            var nearstNode = startNode;
            float minDistance = float.MaxValue;

            ResetValue();

            AStarNode currentNode = null;
            while (openList.Count > 0)
            {
                if (currentNode == null)
                {
                    currentNode = openList[0];
                    openList.RemoveAt(0);
                }
                else
                {
                    openList.Sort((a, b) =>
                    {
                        if (a.FCost > b.FCost)
                        {
                            return 1;
                        }
                        else if (a.FCost == b.FCost)
                        {
                            return b.HCost - a.HCost;
                        }
                        else
                        {
                            return -1;
                        }
                    });

                    currentNode = openList[0];
                    openList.RemoveAt(0);
                }

                if (currentNode == endNode)
                {
                    //Debug.Log("A*找到了路径");
                    NodeToPath(currentNode, ref result);
                    return true;
                }

                closeList.Add(currentNode);

                var neighborNodeList = UnityEngine.Pool.ListPool<AStarNode>.Get();
                GetNeighborNodeList(currentNode, forceWalkable, ref neighborNodeList);

                for (int i = 0; i < neighborNodeList.Count; i++)
                {
                    var currentNeighbor = neighborNodeList[i];

                    if (closeList.Contains(currentNeighbor))
                    {
                        continue;
                    }

                    int currentNeighborGcost = currentNode.GCost + CalcNodeDistance(currentNode, currentNeighbor);
                    int currentNeighborHcost = CalcNodeDistance(currentNeighbor, endNode);
                    int currentNeighborFcost = currentNeighborGcost + currentNeighborHcost;

                    if (openList.Contains(currentNeighbor) == false)
                    {
                        currentNeighbor.GCost = currentNeighborGcost;
                        currentNeighbor.HCost = currentNeighborHcost;
                        currentNeighbor.ParentNode = currentNode;
                        openList.Add(currentNeighbor);

                        var distance = currentNeighborHcost;
                        if (minDistance > distance)
                        {
                            nearstNode = currentNeighbor;
                            minDistance = distance;
                        }
                    }
                    else
                    {
                        if (currentNeighborGcost < currentNeighbor.GCost)
                        {
                            currentNeighbor.GCost = currentNeighborGcost;
                            currentNeighbor.HCost = currentNeighborHcost;
                            currentNeighbor.ParentNode = currentNode;
                        }
                    }
                }

                UnityEngine.Pool.ListPool<AStarNode>.Release(neighborNodeList);
            }

            Debug.Log($"未找到路径 startNode={startNode} -> endNode:{endNode} nearstNode：{nearstNode} 使用最短部分路径");
            NodeToPath(nearstNode, ref result);
            return false;
        }

        private void NodeToPath(AStarNode target, ref List<AStarNode> result)
        {
            var currentParentNode = target;
            while (currentParentNode != null)
            {
                result.Add(currentParentNode);
                currentParentNode = currentParentNode.ParentNode;
            }
            result.Reverse();
        }
    }
}