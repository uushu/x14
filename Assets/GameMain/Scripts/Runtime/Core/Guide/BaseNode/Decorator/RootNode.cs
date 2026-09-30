using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameMain.BTree
{
    public class RootNode  : ABaseDecoratorNode
    {
        public Action<bool> OnFinishCallBack { get; set; }

        protected override void OnChildFinish(ABaseNode childNode, bool success)
        {
            SetFinish(success);

            OnFinishCallBack?.Invoke(success);
        }
    }
}
