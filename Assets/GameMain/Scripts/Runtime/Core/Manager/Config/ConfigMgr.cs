using cfg;
using Cysharp.Threading.Tasks;
using Luban;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class ConfigMgr : AMonoManager<ConfigMgr>
    {
        public override int Priority => 2;

        private Tables _tables;
        public Tables Tables => _tables;

        private readonly Dictionary<string, TextAsset> _bytesAssets = new Dictionary<string, TextAsset>();

        public async UniTask LoadAllConfigs()
        {
            LoadAssetsByTagOperation<TextAsset> loadExcel = new LoadAssetsByTagOperation<TextAsset>("LubanData");

            YooAsset.YooAssets.StartOperation(loadExcel);

            await loadExcel.ToUniTask();

            foreach (var config in loadExcel.AssetObjects)
            {
                Debugger.Log($"加载配置文件:{config.name}", Debugger.ELogColor.Orange);
                _bytesAssets.Add(config.name.ToLower(), config);
            }

            _tables = new Tables(LoadByteBuf);
        }

        private ByteBuf LoadByteBuf(string fileName)
        {

            if (_bytesAssets.TryGetValue(fileName, out var buff))
            {
                return new ByteBuf(buff.bytes);
            }

            Debugger.LogError($"不存在的配置文件:{fileName}");
            return null;
        }

        protected override void OnEnter()
        {
           
        }

        protected override void OnUpdateInternal(float deltaTime) { }

        protected override void OnExit()
        {
            _tables = null;
        }
    }
}