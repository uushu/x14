using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GameMain.Editor
{
    public static class ToolEditor
    {
        [MenuItem("GameObject/引导/获取UI路径")]
        public static void CreateCustomObject()
        {
            var selectObj = Selection.activeGameObject;
            if (selectObj == null)
            {
                Debug.LogError("未选中物体");
                return;
            }

            var go = selectObj.gameObject;
            var pathList = new List<string>();

            while (go != null)
            {
                pathList.Add(go.name);

                if (go.transform.GetComponent<ABaseWindow>() != null)
                {
                    break;
                }

                if (go.transform.parent == null)
                {
                    break;
                }

                go = go.transform.parent.gameObject;
            }

            pathList.Reverse();
            var path = string.Join("/", pathList);
            Debug.Log($"路径: <color=yellow>{path}</color>");
            GUIUtility.systemCopyBuffer = path;
        }

        [MenuItem("工具/X14/生成配表-微信(bytes)")]
        public static void ExcelToUnit_Wx()
        {
            ExecuteBat("gen_byte_x14_wx.bat", $"{Application.dataPath}/../../../excels/DataTables/");
        }
        
        [MenuItem("工具/X14/生成配表-抖音(bytes)")]
        public static void ExcelToUnity_Dy()
        {
            ExecuteBat("gen_byte_x14_dy.bat", $"{Application.dataPath}/../../../excels/DataTables/");
        }

        [MenuItem("工具/X15/生成配表-微信(bytes)")]
        public static void ExcelToUnityX15_WX()
        {
            ExecuteBat("gen_byte_x15_wx.bat", $"{Application.dataPath}/../../../excels/DataTables/");
        }

        [MenuItem("工具/X15/生成配表-抖音(bytes)")]
        public static void ExcelToUnityX15_Dy()
        {
            ExecuteBat("gen_byte_x15_dy.bat", $"{Application.dataPath}/../../../excels/DataTables/");
        }

        [MenuItem("工具/X16/生成配表-微信(bytes)")]
        public static void ExcelToUnityX16_WX()
        {
            ExecuteBat("gen_byte_x16_wx.bat", $"{Application.dataPath}/../../../excels/DataTables/");
        }

        [MenuItem("工具/X16/生成配表-抖音(bytes)")]
        public static void ExcelToUnityX16_Dy()
        {
            ExecuteBat("gen_byte_x16_dy.bat", $"{Application.dataPath}/../../../excels/DataTables/");
        }

        private static void ExecuteBat(string batFile, string workDir)
        {
            string path = FormatPath(workDir + batFile);

            if (System.IO.File.Exists(path) == false)
            {
                Debug.LogError($"不存在的bat文件:{path}");
                return;
            }

            try
            {
                System.Diagnostics.Process process = null;
                
                process = new System.Diagnostics.Process();

                process.StartInfo.WorkingDirectory = workDir;

                process.StartInfo.FileName = batFile;

                process.Start();

                process.WaitForExit();

                process.Close();

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ex:{ex.ToString()}");
            }
        }

        private static string FormatPath(string path)
        {
            path = path.Replace("//", "\\");

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                path = path.Replace('\\', '/');
            }

            return path;
        }

        [MenuItem("工具/打开 PersistentDataPath")]
        public static void OpenPersistentDataPath()
        {
            Application.OpenURL(Application.persistentDataPath);
        }

        [MenuItem("工具/打开 DataPath")]
        public static void OpenDataPath()
        {
            Application.OpenURL(Application.dataPath);
        }
        
        [MenuItem("工具/清空本地玩家数据")]
        public static void ClearLocalSaveData()
        {
            SaveToServer.ClearLocalSave2();

            PlayerPrefs.DeleteAll();

            string persistentPath = $"{Application.persistentDataPath}/{Constant.PLAYER_LOCAL_SAVE_DATA}";

            if (Directory.Exists(persistentPath))
            {
                Debug.Log($"删除文件成功:{persistentPath}");
                Directory.Delete(persistentPath, true);
            }

            if (Application.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                EditorUtility.DisplayDialog("清理本地数据", "清空本地数据成功!", "确定");
            }
        }

        [MenuItem("工具/打开Excel目录")]
        public static void OpenExcelFolder()
        {
            Application.OpenURL($"{Application.dataPath}/../../../excels/DataTables/配置表");
        }

        [MenuItem("工具/打开美术目录")]
        public static void OpenArtsFolder()
        {
            Application.OpenURL($"{Application.dataPath}/../../../美术");
        }

        [MenuItem("工具/打开策划目录")]
        public static void OpenPlanFolder()
        {
            Application.OpenURL($"{Application.dataPath}/../../../策划");
        }
        
        [MenuItem("工具/设置微信权限信息", priority = 100001)]
        public static void SetWeiXin()
        {
            string path = Path.GetFullPath("..\\", Application.dataPath);
            string targetPath = Path.Combine(path, "WeiXinBuild\\minigame\\game.json");
        
            StreamReader streamReader = new StreamReader(targetPath);
            string jsonStr = streamReader.ReadToEnd();
                streamReader.Close();
            if (!jsonStr.Contains("获取昵称和头像用于展示"))
            {
                jsonStr = jsonStr.Replace("\"desktop\"",
                    "\"desktop\",\n  \"permission\":{\n\"scope.userFuzzyLocation\":{\"desc\":\"你的位置信息将用于小游戏位置接口的效果展示\"},\n\"scope.userInfo\":{\"desc\":\"获取昵称和头像用于展示\"}\n}");
            }
            else
            {
                Debug.LogError($"已经设置过了微信权限");
            }
   
            StreamWriter streamWriter = new StreamWriter(targetPath);
            streamWriter.Write(jsonStr);
            streamWriter.Close();

            Debug.LogError($"设置微信权限信息完成:{targetPath}");
        }

        [MenuItem("工具/移除缺失脚本", priority = 100002)]
        public static void RemoveMissScript()
        {
            if (Selection.gameObjects == null)
            {
                Debug.LogError("请选择对象");
                return;
            }
            
            m_goCount = 0;
            m_missingCount = 0;
            foreach (var go in Selection.gameObjects)
            {
                search(go);
            }
            Debug.LogError($"移除缺失脚本完成:{m_goCount}个对象，移除{m_missingCount}个缺失脚本");
        }
        
        static int m_goCount;
        static int m_missingCount;
        static void search(GameObject go)
        {
            m_goCount++;
            m_missingCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            foreach (Transform child in go.transform)
                search(child.gameObject);
        }

        [MenuItem("工具/测试", priority = 100003)]
        public static void TestScript()
        {
            DateTime date1 = new DateTime(2008, 4, 10, 6, 30, 0);
            Debug.LogError(date1.ToString("G",DateTimeFormatInfo.CurrentInfo));
        }
        
        public class PostBuild : AssetPostprocessor
        {
            [PostProcessBuild(100)] //确保这个方法在打包后执行
            public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
            {
#if UNITY_WECHAT_GAME
                //Debug.LogError($"设置微信权限信息");
                //SetWeiXin();
#endif
                Debug.LogError($"打包结束");
            }
        }
    }
}