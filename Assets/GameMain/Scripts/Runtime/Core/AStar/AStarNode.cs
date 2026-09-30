using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class AStarNode
    {
        //起点消耗
        public int GCost { get; set; }

        //目标点消耗
        public int HCost { get; set; }

        public int FCost => GCost + HCost;

        public Vector2Int MapIndex { get; private set; }

        public AStarNode ParentNode { get; set; }

        public AStarMap.EStarNode NodeType { get; set; }

        public bool CanWalkable => (NodeType & AStarMap.EStarNode.Walkable) == AStarMap.EStarNode.Walkable;

        public bool HasSkill => (NodeType & AStarMap.EStarNode.SkillUnit) == AStarMap.EStarNode.SkillUnit;

        public AStarNode(Vector2Int mapIndex, AStarMap.EStarNode nodeType)
        {
            MapIndex = mapIndex;
            NodeType = nodeType;
        }

        public void ResetValue()
        {
            GCost = int.MaxValue;
            HCost = 0;
            ParentNode = null;
        }

        public override string ToString()
        {
            return $"A*Node {MapIndex} {NodeType}";
        }
    }
}