using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace GameMain
{
    public class LocalSaveMgr : AMonoManager<LocalSaveMgr>
    {
        private DataCenter _dataCenter;
        public DataCenter DataCenter
        {
            get
            {
                if (_dataCenter == null)
                {
                    string saveData = PlayerPrefs.GetString(Constant.PLAYER_LOCAL_SAVE_DATA);

                    if (string.IsNullOrEmpty(saveData) == false)
                    {
                        _dataCenter = JsonConvert.DeserializeObject<DataCenter>(saveData);
                        _dataCenter.LoadLocalDataed();
                    }
                    else
                    {
                        _dataCenter = new DataCenter();
                    }

                    Debugger.Log($"加载本地数据:{saveData}", Debugger.ELogColor.Green);
                }

                return _dataCenter;
            }

            set => _dataCenter = value;
        }

        protected override void OnEnter()
        {
            base.OnEnter();

        }

        protected override void OnExit()
        {
            base.OnExit();
            
#if UNITY_EDITOR
            PlatformEventDefine.SendSaveDataToServer();
#endif
            SaveLocalData(true);
        }

        private float intervalTime = 10f;
        private float time;
        protected override void OnUpdateInternal(float deltaTime)
        {
            time += Time.unscaledDeltaTime;
            if (time >= intervalTime)
            {
                time = 0;
                if (_dataCenter != null)
                {
                    SaveLocalData(true);
                }
            }
            
            if (_dataCenter != null && _dataCenter.DirtyKey.Count > 0)
            {
                SaveLocalData(false);
            }
        }

        public void SaveLocalData(bool force)
        {
            if (_dataCenter != null)
            {
                _dataCenter.SaveDirtyData(force);
                var saveData = JsonConvert.SerializeObject(_dataCenter, Formatting.None);
                _dataCenter.lastDirtyTimeStamp = TimeTool.GetNowTimeStamp();

                PlayerPrefs.SetString(Constant.PLAYER_LOCAL_SAVE_DATA, saveData);
#if UNITY_DOUYIN_GAME
                TT.PlayerPrefs.Save();
#else
                PlayerPrefs.Save();
#endif

#if UNITY_EDITOR

                string saveLocalFile = $"{Application.persistentDataPath}/{Constant.PLAYER_LOCAL_SAVE_DATA}";

                if (Directory.Exists(saveLocalFile) == false)
                {
                    Directory.CreateDirectory(saveLocalFile);
                }

                string filePath = $"{saveLocalFile}/PLAYER_LOCAL_SAVE_DATA.json";

                if (File.Exists(filePath) == false)
                {
                    File.Create(filePath).Close();
                }

                File.WriteAllText(filePath, saveData);
               // Debugger.Log($"保存本地数据:{filePath}", Debugger.ELogColor.Gray);
#endif
            }
        }

        public void ReplaceLocalSaveData(DataCenter newDataCenter)
        {
            if (newDataCenter == null) return;

            _dataCenter = newDataCenter;
            SaveLocalData(true);

            Debugger.Log($"替换本地数据:{_dataCenter}", Debugger.ELogColor.Green);
        }
    }
}
