using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.BTree
{
    public class TestBT : MonoBehaviour
    {
        private BehaviourTree _tree;

        // Start is called before the first frame update 编码 utf-8
        void Start()
        {
            var rootNode = new RootNode();
            _tree = new BehaviourTree(rootNode);

            var sequence = new SequenceNode();
            rootNode.AddChild(sequence);

            var delayNode = new DelayNode(1);
            sequence.AddChild(delayNode);

            var logNode = new LogNode();
            logNode.logLevel = LogNode.ELogLevel.Info;
            logNode.logString = "测试打印Info";
            sequence.AddChild(logNode);

            delayNode = new DelayNode(2);
            sequence.AddChild(delayNode);

            logNode = new LogNode();
            logNode.logLevel = LogNode.ELogLevel.Warning;
            logNode.logString = "测试打印Warning";
            sequence.AddChild(logNode);

            delayNode = new DelayNode(3);
            sequence.AddChild(delayNode);

            logNode = new LogNode();
            logNode.logLevel = LogNode.ELogLevel.Error;
            logNode.logString = "测试打印Error";
            sequence.AddChild(logNode);

            _tree.OnFinishedEvent += OnTreeFinish;
            _tree.Start();
        }

        [Button("重新开始行为树")]
        public void RestartTree()
        {
            _tree.Restart();
        }

        private void OnDestroy()
        {
            _tree.OnFinishedEvent -= OnTreeFinish;
        }

        private void OnTreeFinish(bool result)
        {
            Debug.Log($"行为树完成 result:{result}");
        }
    }
}