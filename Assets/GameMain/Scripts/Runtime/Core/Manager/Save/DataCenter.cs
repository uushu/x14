using System.Collections.Generic;
using cfg;
using Newtonsoft.Json;
using UnityEngine;

namespace GameMain
{
    [System.Serializable]
    public class LocalSaveItem
    {
        public string saveItemName;

        public string saveItemJson;

        public LocalSaveItem(string saveName, ASaveData saveJson)
        {
            saveItemName = saveName;
            saveItemData = saveJson;
        }

        [System.NonSerialized]
        public ASaveData saveItemData;

        public void SerializeObject()
        {
            if (saveItemData != null)
            {
                saveItemJson = JsonConvert.SerializeObject(saveItemData);
            }
        }

        public void DeserializeObject<T>() where T : ASaveData, new()
        {
            if (string.IsNullOrEmpty(saveItemJson) == false)
            {
                saveItemData = JsonConvert.DeserializeObject<T>(saveItemJson);
                saveItemData.OnAfterDeserialize();
            }
        }
    }

    [System.Serializable]
    public class DataCenter
    {
        [JsonIgnore]
        private GlobalConfigCategory GlobalConfig => ConfigMgr.Instance.Tables.GlobalConfigCategory;

        //数据变化 分块的刷新保存数据
        public List<LocalSaveItem> localSaves = new();

        //最后一次保存服务器的时间
        public long lastSaveToServerTimestamp;

        //最后一次数据改变的时间
        public long lastDirtyTimeStamp;

        public DataCenter() { }

        [JsonIgnore]
        public HashSet<string> DirtyKey { get; set; } = new();

        public T GetLocalSave<T>(bool isDirty = false) where T : ASaveData, new()
        {
            var saveTypeKey = typeof(T).Name;

            if (isDirty) DirtyKey.Add(saveTypeKey);

            for (int i = 0; i < localSaves.Count; i++)
            {
                var saveItem = localSaves[i];

                if (saveItem.saveItemName.Equals(saveTypeKey))
                {
                    if (saveItem.saveItemData == null)
                    {
                        saveItem.DeserializeObject<T>();
                    }

                    if (saveItem.saveItemData == null)
                    {
                        Debug.LogError("数据错误 saveItemData=null");
                    }

                    if (saveItem.saveItemData is T t)
                    {
                        return t;
                    }
                    else
                    {
                        Debug.LogError($"数据错误 saveItemName:{saveItem.saveItemName} is not {typeof(T)}");
                    }

                    localSaves.RemoveAt(i);
                    break;
                }
            }

            var newSave = new T();
            var newCell = new LocalSaveItem(saveTypeKey, newSave);
            localSaves.Add(newCell);
            newSave.SetDefaultData(GlobalConfig);
            Debugger.Log($"添加本地数据:{newCell.saveItemName}", Debugger.ELogColor.Orange);
            return newSave;
        }

        public void SaveDirtyData(bool force)
        {
            if (force)
            {
                foreach (var localSave in localSaves)
                {
                    localSave.SerializeObject();
                }
            }
            else if (DirtyKey.Count > 0)
            {
                foreach (var dirtyKey in DirtyKey)
                {
                    foreach (var localSave in localSaves)
                    {
                        if (localSave.saveItemName.Equals(dirtyKey))
                        {
                            localSave.SerializeObject();
                            break;
                        }
                    }
                }

                DirtyKey.Clear();
            }
        }

        internal void LoadLocalDataed()
        {
            DirtyKey = new();
        }
    }
}