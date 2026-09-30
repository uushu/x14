using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameMain.BTree
{
    public class SelectorNode : ABaseComposeNode
    {
        private int _currentChildIndex = -1;

        protected override void OnStart()
        {
            _currentChildIndex = -1;

        }

        private void StartNextChild()
        {
            _currentChildIndex++;
            if (_currentChildIndex >= Children.Count)
            {
                SetFinish(false);
                return;
            }

            var currentChild = Children[_currentChildIndex];
            currentChild.Start();
        }

        protected override void OnCancel()
        {
            var currentChild = Children[_currentChildIndex];
            currentChild?.Cancel();
        }

        protected override void OnChildFinish(ABaseNode childNode, bool success)
        {
            if (success)
            {
                SetFinish(true);
            }
            else
            { 
                StartNextChild();
            }
        }
    }
}
