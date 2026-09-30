using System.Collections;
using System.Collections.Generic;
using GameMain.RedDot;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain
{
    public class UIRedDotListener : MonoBehaviour
    {
        [LabelText("红点")]
        [InfoBox("添加红点组件", InfoMessageType.Error, "RedGoIsNull")]
        public GameObject redDotGo;

        [LabelText("红点激活显示")]
        public List<GameObject> showObjList;
        
        [LabelText("红点激活隐藏")]
        public List<GameObject> hideObjList;
        
        [ValueDropdown("GetRedDotPath", ExpandAllMenuItems = false)]
        [OnValueChanged("OnValueChange")]
        [LabelText("红点路径")]
        public string redDotPath;
        
        [ReadOnly]
        [LabelText("运行时路径")]
        public string runtimeRedDotPath;
        
        private void Start()
        {
            if (redDotPath.Contains("{0}") == false)
            {
                //不是可变路径
                AddListener(redDotPath);
            }
        }

        private void OnDestroy()
        {
            RemoveListener();
        }

        public void AddListener(string path)
        {
            RemoveListener();
            
            runtimeRedDotPath = path;
            
            var redDotNode = RedDotMgr.Instance.AddListener(runtimeRedDotPath, OnRedDotValueChange);
            
            OnRedDotValueChange(redDotNode);
        }

        public void RemoveListener()
        {
            if (string.IsNullOrEmpty(runtimeRedDotPath))
            {
                return;
            }
            
            RedDotMgr.Instance.RemoveListener(runtimeRedDotPath, OnRedDotValueChange);
        }

        private void OnRedDotValueChange(RedDotTreeNode redDotNode)
        {
            if (redDotNode != null && redDotNode.Value > 0)
            {
                redDotGo.SetActive(true);

                ShowOrHideGameObject(true);
            }
            else
            {
                redDotGo.SetActive(false);
                
                ShowOrHideGameObject(false);
            }
        }

        private void ShowOrHideGameObject(bool isActive)
        {
            foreach (var item in showObjList)
            {
                item.SetActive(isActive);
            }
            
            foreach (var item in hideObjList)
            {
                item.SetActive(!isActive);
            }
        }

#if UNITY_EDITOR

        private bool RedGoIsNull => redDotGo == null;
        
        private void OnValueChange(string newPath)
        {
            Debug.Log($"红点路径改变 newPath:{newPath}");
        }

        private IEnumerable GetRedDotPath()
        {
            var fields = typeof(RedDotPath).GetFields();

            var list = new List<string>();

            foreach (var field in fields)
            {
                string value = (string)field.GetValue(null);

                var list2 = value.Split('/');

                var tempList = new List<string>();
                
                for (int i = 0; i < list2.Length; i++)
                {
                    tempList.Add(list2[i]);

                    list.Add(string.Join('/', tempList));
                }
            }

            return list;
        }

#endif
    }
}
