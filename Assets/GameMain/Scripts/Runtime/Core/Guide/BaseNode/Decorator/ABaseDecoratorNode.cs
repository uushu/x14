using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public class ABaseDecoratorNode : ABaseNode
    {
        public ABaseNode Child { get; set; }

        public override void AddChild(ABaseNode childNode)
        {
            if (childNode == null)
            {
                Debug.LogError("AddChild childNode=null");
                return;
            }

            if (childNode.ParentNode != null)
            {
                childNode.ParentNode.RemoveChild(childNode);
            }

            childNode.ParentNode = this;
            Child = childNode;
        }

        public override void ForeachChild(Action<ABaseNode> action)
        {
            action?.Invoke(Child);
        }

        public override void RemoveAllChildren()
        {
            RemoveChild(Child);
        }

        public override void RemoveChild(ABaseNode childNode)
        {
            if (childNode == null)
            {
                Debug.LogError("RemoveChild childNode=null");
                return;
            }

            childNode.ParentNode = null;
            Child = null;
        }

        protected override void OnCancel()
        {
            Child.Cancel();
        }

        protected override void OnChildFinish(ABaseNode childNode, bool success)
        {
            SetFinish(success);
        }

        protected override void OnStart()
        {
            Child.Start();
        }
    }
}
