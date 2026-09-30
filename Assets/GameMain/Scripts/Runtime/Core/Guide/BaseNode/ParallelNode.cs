using System;
using System.Collections.Generic;
using System.Linq;

namespace GameMain.BTree
{
    public class ParallelNode : ABaseComposeNode
    {
        public enum EParallelCondition
        {
            FirstSuccess,
            FirstFailed,
        }

        public EParallelCondition Condition { get; set; }

        private int _successCount;
        private int _failedCount;

        protected override void OnStart()
        {
            _successCount = 0;
            _failedCount = 0;
            foreach (var child in Children)
            {
                child.Start();
            }
        }

        protected override void OnCancel()
        {
            foreach (var child in Children)
            { 
                child.Cancel();
            }
        }

        protected override void OnChildFinish(ABaseNode childNode, bool success)
        {
            if (success)
            {
                _successCount++;
            }
            else
            { 
                _failedCount++;
            }

            switch (Condition)
            {
                case EParallelCondition.FirstSuccess:

                    if (_successCount > 0)
                    {
                        Cancel();
                        SetFinish(true);
                    }
                    else if (_failedCount == Children.Count)
                    {
                        Cancel();
                        SetFinish(false);
                    }

                    break;
                case EParallelCondition.FirstFailed:

                    if (_failedCount > 0)
                    {
                        Cancel();
                        SetFinish(true);
                    }
                    else if (_successCount == Children.Count)
                    {
                        Cancel();
                        SetFinish(false);
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
