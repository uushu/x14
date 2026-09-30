using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class StartGame
    {
        public static async UniTask LoadAssets()
        {
            var handler = YooAsset.YooAssets.LoadAssetAsync<GameObject>("GameEntrance");
            await handler.ToUniTask();
            var instance = GameObject.Instantiate(handler.AssetObject as GameObject);
            var entrance = instance.GetComponent<GameEntrance>();
            entrance.Initialize();
        }
    }
}