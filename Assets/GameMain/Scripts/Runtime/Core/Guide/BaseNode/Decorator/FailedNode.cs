using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public class FailedNode : ABaseDecoratorNode
    {
        protected override void OnChildFinish(ABaseNode childNode, bool success)
        {
            SetFinish(false);
        }
    }
}