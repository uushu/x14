using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public enum Status
    { 
        Free,
        Runing,
        Success,
        Failed,
    }

    public abstract class ABaseNode
    {
        public Status CurrentStatus { get; protected set; }

        public ABaseNode ParentNode { get; set; }

        public abstract void AddChild(ABaseNode childNode);

        public abstract void RemoveChild(ABaseNode childNode);

        public abstract void RemoveAllChildren();

        public abstract void ForeachChild(Action<ABaseNode> action);

        public void Start()
        {
            if (CurrentStatus == Status.Runing)
            {
                Debug.LogError($"当前节点已经运行 CurrentStatus:{CurrentStatus}");
                return;
            }

            Debugger.Log($"开始运行节点:{this}");
            CurrentStatus = Status.Runing;
            OnStart();
        }

        public void Cancel()
        {
            if (CurrentStatus != Status.Runing)
            {
                Debug.LogError($"当前节点不是已经运行 CurrentStatus:{CurrentStatus}");
                return;
            }

            Debugger.Log($"取消运行节点:{this}");
            CurrentStatus = Status.Free;
            OnCancel();
        }

        protected virtual void SetFinish(bool success)
        {
            Debugger.Log($"完成运行节点:{this} success={success}");

            CurrentStatus = success ? Status.Success : Status.Failed;
            ParentNode?.ChildFinish(this, success);
        }

        protected void ChildFinish(ABaseNode childNode, bool success)
        {
            OnChildFinish(childNode, success);
        }

        protected abstract void OnStart();
        protected abstract void OnCancel();
        protected abstract void OnChildFinish(ABaseNode childNode, bool success);

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}