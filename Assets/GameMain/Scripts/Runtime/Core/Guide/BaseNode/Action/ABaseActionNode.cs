using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public abstract class ABaseActionNode : ABaseNode
    {
        public sealed override void AddChild(ABaseNode childNode)
        {
            throw new System.NotImplementedException();
        }

        public sealed override void RemoveAllChildren()
        {
            throw new System.NotImplementedException();
        }

        public sealed override void RemoveChild(ABaseNode childNode)
        {
            throw new System.NotImplementedException();
        }

        public sealed override void ForeachChild(Action<ABaseNode> action)
        {
            throw new NotImplementedException();
        }


        protected sealed override void OnChildFinish(ABaseNode childNode, bool success)
        {
            throw new System.NotImplementedException();
        }
    }
}