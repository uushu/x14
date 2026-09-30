using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static HyzRankManager;

namespace GameMain
{
    public static class SaveToServer
    {
        private static string SaveUrl
        {
            get
            {
#if UNITY_EDITOR
                return "http://192.168.0.201:8589/mini/checkpoint/save";
#else
                return "https://gamesupport.huanyz.com/mini/checkpoint/save";
#endif
            }
        }

        private static string GetUrl
        {
            get
            {
#if UNITY_EDITOR
                return "http://192.168.0.201:8589/mini/checkpoint/get";
#else

                return "https://gamesupport.huanyz.com/mini/checkpoint/get";
#endif
            }
        }

        [System.Serializable]
        public class DataCenterRequest : HyzProjectSetting.PlatformPostData
        {
            [System.Serializable]
            public class SaveDataWithUid
            {
                public string uuid;

                public string data;
            }

            public SaveDataWithUid data;

            public DataCenterRequest(string dataCenter, string uuid)
            {
                data = new SaveDataWithUid()
                {
                    uuid = uuid,
                    data = dataCenter
                };

                channel = $"{HyzProjectSetting.ProjectTag}_{PlatformMgr.Instance.ePlatform}";
                clientId = HyzProjectSetting.ClientId;
                platform = $"{PlatformMgr.Instance.ePlatform}";
                salt = "huanyz";
                sign = GetSign();
            }

            private string GetSign()
            {
                var str = "";
                SortedDictionary<string, string> signDict = new SortedDictionary<string, string>();
                signDict.Add(nameof(data.uuid), data.uuid);
                signDict.Add(nameof(data.data), data.data);

                foreach (var keyV in signDict)
                {
                    if (keyV.Value == null || string.IsNullOrEmpty(keyV.Value))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(str))
                    {
                        str += $"{keyV.Key}={keyV.Value}";
                    }
                    else
                    {
                        str += $"&{keyV.Key}={keyV.Value}";
                    }
                }

                var tempStr = $"{clientId}{str}{version}{salt}{HyzProjectSetting.ClientSecret}";
                var result = HyzProjectSetting.SignTool.CreateSign(tempStr);

                return result;
            }
        }

        [System.Serializable]
        public class DataCenterResponse : HyzProjectSetting.SuccedResponseBase
        {
            [System.Serializable]
            public class SaveDataWithOutUid
            {
                public string data;

                public long time;
            }

            public string data;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void AddListener()
        {
            UniEvent.AddListener<PlatformEventDefine.GetOpenIdSuccess>(Processmessage);
            UniEvent.AddListener<PlatformEventDefine.SaveDataToServer>(Processmessage);
        }

        private static void Processmessage(IEventMessage message)
        {
            if (message is PlatformEventDefine.GetOpenIdSuccess getOpenId)
            {
                SaveLocalSaveToServer().Forget();
            }
            else if (message is PlatformEventDefine.SaveDataToServer saveToServer)
            {
                InternalSaveToServer(LocalSaveMgr.Instance.DataCenter).Forget();
            }
        }

        public static async UniTask SaveLocalSaveToServer()
        {
            Debugger.Log($"openId={PlatformMgr.Instance.OpenId}");
            var requestData = new DataCenterRequest(null, PlatformMgr.Instance.OpenId);
            var postJson = JsonConvert.SerializeObject(requestData);

            (var result, var serverSaveJson) = await UtilityTool.RequestPostData(GetUrl, postJson, null);
            if (result == false) return;

            var response = JsonConvert.DeserializeObject<DataCenterResponse>(serverSaveJson);

            if (result && response != null && response.data != null && string.IsNullOrEmpty(response.data) == false)
            {
                var responseSaveData = JsonConvert.DeserializeObject<DataCenterResponse.SaveDataWithOutUid>(response.data);

                if (string.IsNullOrEmpty(responseSaveData.data) == false)
                {
                    var dataCenterResponse = JsonConvert.DeserializeObject<DataCenter>(responseSaveData.data);

                    if (dataCenterResponse.lastSaveToServerTimestamp > LocalSaveMgr.Instance.DataCenter.lastSaveToServerTimestamp)
                    {
                        //替换本地的保存的数据
                        Debugger.LogError("玩家清空了本地数据,本地数据替换为服务器数据");
                        LocalSaveMgr.Instance.ReplaceLocalSaveData(dataCenterResponse);
                    }
                    else
                    {
                        if (dataCenterResponse.lastDirtyTimeStamp < LocalSaveMgr.Instance.DataCenter.lastDirtyTimeStamp)
                        {
                            //需要保存数据到服务器
                            await InternalSaveToServer(LocalSaveMgr.Instance.DataCenter);
                            Debugger.Log("本地数据改变,保存数据到服务器");
                        }
                        else
                        {
                            Debugger.Log("数据没变化");
                        }
                    }
                    return;
                }
            }

            //第一次保存数据
            var dataCenter = LocalSaveMgr.Instance.DataCenter;
            await InternalSaveToServer(dataCenter);
            Debugger.LogError($"第一次保存数据到服务器:{PlatformMgr.Instance.OpenId}");
        }

        private static async UniTask InternalSaveToServer(DataCenter datacenter)
        {
            var openId = PlatformMgr.Instance.OpenId;
            datacenter.lastSaveToServerTimestamp = TimeTool.GetNowTimeStamp();
            var dataCenterJson = JsonConvert.SerializeObject(datacenter, Formatting.None);
            var wrapperData = new DataCenterRequest(dataCenterJson, openId);

            var saveToServerJson = JsonConvert.SerializeObject(wrapperData, Formatting.None);
            var result = await UtilityTool.RequestPostData(SaveUrl, saveToServerJson, null);

            Debugger.Log($"保存本地数据到服务器成功:{result.Item1} {result.Item2}");
        }

        public static async UniTask ClearLocalSave()
        {
            //替换本地的保存的数据
            var dataCenter = new DataCenter();
            await InternalSaveToServer(dataCenter);
            LocalSaveMgr.Instance.ReplaceLocalSaveData(dataCenter);
        }

        public static void ClearLocalSave2()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
              
                EditorUtility.DisplayDialog("清理本地数据", "请先运行编辑器,在清理本地数据!", "确定");
                return;
            }
#endif
            ClearLocalSave().Forget();
        }
    }
}