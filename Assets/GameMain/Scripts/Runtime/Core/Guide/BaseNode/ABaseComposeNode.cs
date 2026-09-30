using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public abstract class ABaseComposeNode : ABaseNode
    {
        public List<ABaseNode> Children = new();

        public override void AddChild(ABaseNode childNode)
        {
            if (childNode == null)
            {
                Debug.LogError("AddChild 参数:childNode=null");
                return;
            }

            if (Children.Contains(childNode))
            {
                Debug.LogError($"{childNode}已经加入集合");
                return;
            }

            if(childNode.ParentNode != null)
            {
                childNode.ParentNode.RemoveChild(childNode);
            }

            childNode.ParentNode = this;
            Children.Add(childNode);
        }

        public override void RemoveChild(ABaseNode childNode)
        {
            if (childNode == null)
            {
                Debug.LogError("AddChild 参数:childNode=null");
                return;
            }

            childNode.ParentNode = null;
            Children.Remove(childNode);
        }

        public override void RemoveAllChildren()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                RemoveChild(Children[i]);
            }
        }

        public override void ForeachChild(Action<ABaseNode> action)
        {
            foreach (var child in Children)
            {
                action?.Invoke(child);
            }
        }
    }
}