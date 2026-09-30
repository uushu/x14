using System;
using System.Collections.Generic;
using System.Text;

namespace GameMain.BTree
{
    public class SequenceNode : ABaseComposeNode
    {
        private int _currentRunningChildIndex = -1;

        private void StartNextChild()
        {
            _currentRunningChildIndex++;
            if (_currentRunningChildIndex >= Children.Count)
            {
                SetFinish(true);
                return;
            }

            var currentNode = Children[_currentRunningChildIndex];
            currentNode.Start();
        }

        protected override void OnStart()
        {
            _currentRunningChildIndex = -1;
            StartNextChild();
        }

        protected override void OnCancel()
        {
            var currentChild = Children[_currentRunningChildIndex];
            currentChild.Cancel();
        }

        protected override void OnChildFinish(ABaseNode childNode, bool success)
        {
            if (success)
            {
                StartNextChild();
            }
            else
            {
                SetFinish(false);
            }
        }

    }
}
