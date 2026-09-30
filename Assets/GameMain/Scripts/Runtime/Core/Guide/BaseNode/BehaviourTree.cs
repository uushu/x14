using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public class BehaviourTree
    {
        public RootNode RootNode { get; private set; }

        public event Action<bool> OnFinishedEvent
        {
            add
            {
                if (RootNode == null)
                {
                    Debug.LogError("RootNode = NULL");
                    return;
                }

                RootNode.OnFinishCallBack += value;
            }
            remove
            {
                if (RootNode == null)
                {
                    Debug.LogError("RootNode = NULL");
                    return;
                }

                RootNode.OnFinishCallBack -= value;
            }
        }

        public Action OnCanceled { get; set; }

        public BehaviourTree(RootNode rootNode)
        {
            RootNode = rootNode;
        }

        public void Start()
        {
            if (RootNode == null)
            {
                throw new Exception("行为树运行失败,根节点为空");
            }

            if (RootNode.CurrentStatus == Status.Runing)
            {
                Debug.LogError("行为树已经运行");
                return;
            }

            RootNode.Start();
        }

        public void Cancel()
        {
            if (RootNode.CurrentStatus != Status.Runing)
            {
                Debug.LogError($"行为树未运行 CurrentStatus={RootNode.CurrentStatus}");
                return;
            }

            RootNode.Cancel();
            OnCanceled?.Invoke();
        }

        public void Restart()
        {
            Cancel();
            RootNode.Start();
        }
    }
}