using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameMain
{
    [System.Serializable]
    public class ComponentItem
    {
        [HorizontalGroup("组件")]
        public Object component;

        [HorizontalGroup("组件")]
        [Button("复制组件")]
        public void CopyComponentReference()
        {
            GUIUtility.systemCopyBuffer = GetCopyComponent();
        }

        public string GetCopyComponent()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\t\t[Sirenix.OdinInspector.FoldoutGroup(\"组件\")]");
            sb.AppendLine($"\t\tpublic {component.GetType()} m{UtilityTool.ToUpperFirst(component.name)};");

            return sb.ToString();
        }

        [Button("复制")]
        [ShowIf("CanCopyCode")]
        [HorizontalGroup("C2")]
        private void CopyCode1()
        {
            if (component is Button button)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"\t\tm{UtilityTool.ToUpperFirst(component.name)}.AddButtonClickListener({FunctionName()});");

                GUIUtility.systemCopyBuffer = sb.ToString();
            }
        }

        private string FunctionName()
        {
            return $"On{component.name.Replace("btn", "")}Click";
        }

        [Button("复制方法")]
        [ShowIf("CanCopyCode")]
        [HorizontalGroup("C2")]
        private void CopyCode2()
        {
            if (component is Button button)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"\t\t private void {FunctionName()}()");
                sb.AppendLine("\t\t{");

                sb.AppendLine();
                sb.AppendLine("\t\t}");

                GUIUtility.systemCopyBuffer = sb.ToString();
            }
        }

        private bool CanCopyCode()
        {
            return false;
        }
    }

    public abstract class ABaseComponent : MonoBehaviour
    {
#if UNITY_EDITOR
        
        [LabelText("UI组件列表")]
        public List<ComponentItem> uiProperties;

        protected Transform GetRoot()
        {
            var root = transform;

            while (root.parent != null)
            {
                if (root.parent.name.Contains("Canvas"))
                {
                    break;
                }

                root = root.parent;
            }

            return root;
        }

        private bool ShowGenerateReferenceButton()
        {
            if (uiProperties.Count <= 0)
            {
                return false;
            }

            foreach (var property in uiProperties)
            {
                if (HasProperty(property.component) == false)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasProperty(Object component)
        {
            if (component == null) return false;
            
            var window = gameObject.GetComponent<ABaseComponent>();

            var fileInfos = window.GetType().GetFields();

            foreach (var item in fileInfos)
            {
                if (item.Name.ToLower().Contains(component.name.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        //[Button("生成-组件引用")]
        [ShowIf("ShowGenerateReferenceButton")]
        public void GenerateReference()
        {
            string className = $"{GetType().Name}";

            string filePath = AssetLoader.LoadScriptAssets(className);

            var fileContent = File.ReadAllText(filePath);

            string modifyResult = AddComponentReference(fileContent);

            File.WriteAllText(filePath, modifyResult);

            AddComponentReference();
        }

        private string AddComponentReference(string fileContent)
        {
            var pattern = @"(\b(public|private|protected|internal)\b\s+)?\bclass\b\s+\w+\s*(\s*:\s*\w+\s*)?\{";
            
            MatchCollection matches = Regex.Matches(fileContent, pattern);
            
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                CopyComponentReference();

                fileContent = fileContent.Insert(match.Index + match.Length, $"\r{GUIUtility.systemCopyBuffer}");
            }

            return fileContent;
        }

        [Button("复制-组件引用")]
        private void CopyComponentReference()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < uiProperties.Count; i++)
            {
                if (HasProperty(uiProperties[i].component))
                {
                    continue;
                }

                if (i < uiProperties.Count - 1)
                {
                    sb.AppendLine(uiProperties[i].GetCopyComponent());
                }
                else
                {
                    sb.Append(uiProperties[i].GetCopyComponent());
                }
            }

            string reference = sb.ToString();

            GUIUtility.systemCopyBuffer = reference;
        }

        [Button("添加-组件引用")]
        [ShowIf("ComponentReferenceHasEmpty")]
        [InfoBox("组件有空引用", InfoMessageType.Error)]
        public void AddComponentReference()
        {
            var window = gameObject.GetComponent<ABaseComponent>();

            foreach (var item in uiProperties)
            {
                FieldInfo fieldInfo = window.GetType().GetField($"m{UtilityTool.ToUpperFirst(item.component.name)}");

                if (fieldInfo == null) continue;

                fieldInfo.SetValue(window, item.component);
            }
        }

        [HorizontalGroup("属性")]
        [Button("恢复属性")]
        public void RecoverProperties()
        {
            uiProperties.Clear();

            var window = gameObject.GetComponent<ABaseComponent>();

            var fileInfos = window.GetType().GetFields();

            foreach (var item in fileInfos)
            {
                if (item.Name.StartsWith("m") && (item.GetValue(window) is Object obj))
                {
                    uiProperties.Add(new ComponentItem() { component = obj});
                }
            }
        }

        [HorizontalGroup("属性")]
        [Button("清空属性")]
        public void ClearProperties()
        {
            uiProperties.Clear();
        }

        private bool ComponentReferenceHasEmpty()
        {
            var window = gameObject.GetComponent<ABaseComponent>();

            var fileInfos = window.GetType().GetFields();

            foreach (var item in fileInfos)
            {
                if (item.Name.StartsWith("m") && (item.GetValue(window) == null))
                {
                    return true;
                }
            }

            return false;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AllScriptsReloaded()
        {
            Debug.Log($"脚本编译完成:{DateTime.Now}");
        }
#endif
    }
}