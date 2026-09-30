using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

namespace GameMain
{
    public class ResourceMgr : AMonoManager<ResourceMgr>
    {
        public override int Priority => 1;

        private Dictionary<string, AssetHandle> _assetCacheDict;

        protected override void OnEnter()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            _assetCacheDict = new();
        }

        protected override void OnExit()
        {
            UnloadUnusedAssets();
            _assetCacheDict = null;
        }

        public void UnloadUnusedAssets()
        {
            _assetCacheDict.Clear();

            YooAsset.YooAssets.GetPackage(GameConstant.DefaultPackage).UnloadUnusedAssetsAsync();
            System.GC.Collect();
        }

        public async UniTask<GameObject> InstantiateGameObject(string location, Transform parent, bool instantiateInWorldSpace)
        {
            var prefab = await LoadAssetAsync<GameObject>(location);

            GameObject instance = null;

            if (parent != null)
            {
                instance = GameObject.Instantiate(prefab, parent, instantiateInWorldSpace);
            }
            else
            {
                instance = GameObject.Instantiate(prefab);
            }

            return instance;
        }

        public async UniTask<TObject> LoadAssetAsync<TObject>(string location, System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update) where TObject : Object
        {
            if (_assetCacheDict.TryGetValue(location, out var asset))
            {
                if (asset != null && asset.AssetObject is TObject tAsset)
                {
                    return tAsset;
                }
                else
                {
                    Debugger.LogError($"资源缓存=NULL location:{location}");
                }
            }

            Debugger.Log($"开始加载资源:{location}", Debugger.ELogColor.Orange);

            var handler = YooAsset.YooAssets.LoadAssetAsync<TObject>(location);
            await handler.ToUniTask(progress, timing);

            Debugger.Log($"加载资源成功:{location}", Debugger.ELogColor.Orange);

            if (handler.AssetObject is TObject obj)
            {
                _assetCacheDict[location] = handler;
                return obj;
            }

            Debugger.LogError($"类型转换失败 location:{location} 资源类型:{handler.AssetObject.GetType()} 加载类型:{typeof(TObject)}");
            return null;
        }

        public async UniTask<TObject> LoadSubAssetAsync<TObject>(string location, string assetName, System.IProgress<float> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update) where TObject : Object
        {
            if (_assetCacheDict.TryGetValue(location, out var asset))
            {
                if (asset != null && asset.AssetObject is TObject tAsset)
                {
                    return tAsset;
                }
                else
                {
                    Debugger.LogError($"资源缓存=NULL location:{location}");
                }
            }

            Debugger.Log($"加载资源:{location}", Debugger.ELogColor.Orange);

            var handler = YooAsset.YooAssets.LoadSubAssetsAsync<TObject>(location);
            
            await handler.ToUniTask(progress, timing);

            var subAsset = handler.GetSubAssetObject<TObject>(location);

            if (subAsset == null)
            {
                Debugger.LogError($"不存在的资源类型 location:{location} assetName:{assetName} TObject:{typeof(TObject)}");
            }

            return subAsset;
        }

        public async UniTask<YooAsset.SceneHandle> LoadSceneAsync(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, LocalPhysicsMode physicsMode = LocalPhysicsMode.None, bool suspendLoad = false, uint priority = 100)
        {
            Debugger.Log($"YooAssets 场景加载 ---{location}---", Debugger.ELogColor.Orange);

            var handler = YooAsset.YooAssets.LoadSceneAsync(location, sceneMode, physicsMode, suspendLoad, priority);

            await handler.ToUniTask();

            return handler;
        }

        public async UniTask<Sprite> LoadSprite(string location)
        {
            return await LoadAssetAsync<Sprite>(location);
        }

        public void SetImageSprite(Image image, string spriteLocation, bool setNative)
        {
            SetImageSprite(image, spriteLocation, (target) =>
            {
                if (setNative)
                {
                    target?.SetNativeSize();
                }
            });
        }

        public async void SetImageSprite(Image image, string spriteLocation, System.Action<Image> success = null)
        {
            if (image == null)
            {
                Debugger.LogError("参数错误 Image = NUll");
                return;
            }

            if (string.IsNullOrEmpty(spriteLocation))
            {
                Debugger.LogError("参数错误 spriteLocation = NUll");
                return;
            }

            if (image.sprite != null && image.sprite.name.Equals(spriteLocation))
            {
                return;
            }

            var sprite = await LoadSprite(spriteLocation);

            if (image != null)
            {
                image.sprite = sprite;
                success?.Invoke(image);
            }
        }

        public async UniTask<AudioClip> LoadAudioClip(string location)
        {
            return await LoadAssetAsync<AudioClip>(location);
        }
    }
}