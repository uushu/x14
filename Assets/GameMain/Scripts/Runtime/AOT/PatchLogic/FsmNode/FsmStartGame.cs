using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YooAsset;

namespace GameMain.AOT
{
    internal class FsmStartGame : IStateNode
    {
        private PatchOperation _owner;

/// <summary>
/// 实现状态节点接口的创建方法
/// </summary>
/// <param name="machine">状态机实例</param>
        void IStateNode.OnCreate(StateMachine machine)
        {
    // 将状态机的拥有者转换为PatchOperation类型并赋值给_owner字段
            _owner = machine.Owner as PatchOperation;
        }
        void IStateNode.OnEnter()
        {
            YooAssets.SetDefaultPackage(YooAssets.GetPackage(Init.DefaultPackage));
            LoadAsset();
        }

        private async void LoadAsset()
        {
          //  UserEventDefine.UserProgressUpdateValue.SendEventMessage(0.5f);
            await StartGame.LoadAssets();
            PatchEventDefine.PatchStepsChange.SendEventMessage("开始游戏！");
            _owner.SetFinish();
        }

       /* private IEnumerator LoadDll()
        {
            UserEventDefine.UserProgressUpdateValue.SendEventMessage(0.5f);

            // 先补充元数据
            List<string> aotDllList = new()
            {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll", // 如果使用了Linq，需要这个
                "UniTask.dll",
                "AOT.dll",
                "Luban.Runtime.dll",
                "YooAsset.dll",
                "YooAsset.MiniGame.dll",
            };

            foreach (var aotDllName in aotDllList)
            {
                var handler = YooAsset.YooAssets.LoadAssetAsync<TextAsset>(aotDllName);
                yield return handler;

                byte[] dllBytes = (handler.AssetObject as TextAsset).bytes;

                var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HybridCLR.HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");

                handler.Release();
            }

            var gameMainDll = "GameMain";
            // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
            var handler2 = YooAsset.YooAssets.LoadAssetAsync<TextAsset>($"{gameMainDll}.dll");
            yield return handler2;
            System.Reflection.Assembly hotUpdateAss = System.Reflection.Assembly.Load((handler2.AssetObject as TextAsset).bytes);
            handler2.Release();
#else
            // Editor下无需加载，直接查找获得HotUpdate程序集
            System.Reflection.Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == gameMainDll);
#endif

            System.Type type = hotUpdateAss.GetType("GameMain.StartGame");
            // type.GetMethod("LoadAssets").Invoke(null, null);


            PatchEventDefine.PatchStepsChange.SendEventMessage("开始游戏！");
            _owner.SetFinish();
        }
       */

        void IStateNode.OnUpdate()
        {
        }
        void IStateNode.OnExit()
        {
        }
    }
}