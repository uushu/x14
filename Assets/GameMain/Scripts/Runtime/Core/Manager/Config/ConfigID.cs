using cfg;
using Luban;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
#if UNITY_EDITOR
    public static class AssetLoader
    {
        public static List<Sprite> LoadAllSprite(string foldPath)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Sprite", new[] {
                foldPath
            });

            var result = new List<Sprite>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                result.Add(asset);
            }
            return result;
        }

        public static Sprite LoadSprite(string foldPath, string spriteName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Sprite", new[] {
                foldPath
            });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (asset.name == spriteName)
                {
                    return asset;
                }
            }

            Debug.LogError($"编辑器加载图片失败:foldPath={foldPath} spriteName={spriteName}");
            return null;
        }

        public static GameObject LoadGameObject(string foldPath, string objName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Prefab", new[] {
                foldPath
            });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (asset.name == objName)
                {
                    return asset;
                }
            }

            Debug.LogError($"编辑器加载预制体失败:foldPath={foldPath} objName={objName}");
            return null;
        }

        public static GameObject LoadWindowAssetByName(string assetName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Prefab", new[] {
                "Assets/GameMain/GameRes/Prefab/Window"
            });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (asset.name == assetName)
                {
                    return asset;
                }
            }
            return null;
        }

        public static List<AudioClip> LoadAudioAssets()
        {
            string[] guids = AssetDatabase.FindAssets($"t:AudioClip", new[] {
                "Assets/GameMain/AssetBundle/Audio"
            });

            var list = new List<AudioClip>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                list.Add(asset);
            }
            return list;
        }

        public static string LoadScriptAssets(string scriptName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Script", new[] {
                "Assets/GameMain/Scripts/Runtime/Game/UI"
            });

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (path.EndsWith($"{scriptName}.cs"))
                {
                    return path;
                }
            }

            Debug.LogError($"不存在的脚本:{scriptName}");
            return null;
        }

        public static T GetCategory_Editor<T>() where T : class
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/GameMain/AssetBundle/LubanData/Bytes/{typeof(T).Name}.bytes");
            var instance = Activator.CreateInstance(typeof(T), new ByteBuf(bytes.bytes)) as T;
            return instance;
        }
    }
#endif

    [System.Serializable]
    public class AudioClipName
    {
        [ValueDropdown("GetIdList")]
        [LabelText("音乐Clip")]
        public string audioName;

        [LabelText("音乐大小")]
        [Range(0, 1)]
        public float audioVolume = 1;

        public static implicit operator string(AudioClipName audioClip)
        {
            return audioClip.audioName;
        }

        public static implicit operator AudioClipName(string name)
        {
            return new AudioClipName() {
                audioName = name
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var audioList = AssetLoader.LoadAudioAssets();

            var list = new List<string>();

            list.Add("");

            foreach (var item in audioList)
            {
                list.Add(item.name);
            }

            foreach (var audioClip in audioList)
            {
                list.Add($"{audioClip.name}");
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    [LabelText("按钮路径")]
    public class WindowButtonPath
    {
        [ValueDropdown("GetIdList")]
        [LabelText("窗口名")]
        [OnValueChanged("OnWindowNameChange")]
        public string windowName;

#if UNITY_EDITOR
        [LabelText("预制体")]
        public GameObject windowGo;
#endif

        [LabelText("按钮路径")]
        [ValueDropdown("GetButtonList")]
        [OnValueChanged("OnPathChange")]
        public string buttonPath;

        public static implicit operator string(WindowButtonPath windowButtonPath)
        {
            return windowButtonPath.windowName;
        }

        public static implicit operator WindowButtonPath(string windowName)
        {
            return new WindowButtonPath() {
                windowName = windowName
            };
        }

        public override string ToString()
        {
            return $"windowName={windowName} goPath={buttonPath}";
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var types = TypeCache.GetTypesDerivedFrom<ABaseWindow>();

            var list = new ValueDropdownList<string>();

            foreach (var window in types)
            {
                list.Add(new ValueDropdownItem<string>(window.Name, window.Name));
            }

            return list;
        }

        private void OnWindowNameChange(string name)
        {
            windowGo = AssetLoader.LoadWindowAssetByName(windowName);
        }

        private void OnPathChange()
        {
            Debug.Log($"按钮路径={buttonPath}");
        }

        private IEnumerable GetButtonList()
        {
            var pathList = new List<string>();

            if (windowGo == null) return pathList;

            windowGo.GetAllTypePath<Button>(windowGo.transform, ref pathList);

            return pathList;
        }
#endif
    }

    [System.Serializable]
    [LabelText("窗口路径")]
    public class WindowName
    {
        [ValueDropdown("GetIdList")]
        [LabelText("窗口名")]
        [OnValueChanged("OnWindowNameChange")]
        public string windowName;

#if UNITY_EDITOR
        [LabelText("预制体")]
        public GameObject windowGo;
#endif

        public static implicit operator string(WindowName windowButtonPath)
        {
            return windowButtonPath.windowName;
        }

        public static implicit operator WindowName(string windowName)
        {
            return new WindowName() {
                windowName = windowName
            };
        }

        public override string ToString()
        {
            return $"windowName={windowName}";
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var types = TypeCache.GetTypesDerivedFrom<ABaseWindow>();

            var list = new ValueDropdownList<string>();

            foreach (var window in types)
            {
                list.Add(new ValueDropdownItem<string>(window.Name, window.Name));
            }

            return list;
        }

        private void OnWindowNameChange(string name)
        {
            windowGo = AssetLoader.LoadWindowAssetByName(windowName);
        }

        private IEnumerable GetButtonList()
        {
            var list = new List<string>();

            if (windowGo == null) return list;

            windowGo.GetAllTypePath<Button>(windowGo.transform, ref list);

            return list;
        }
#endif
    }

    [System.Serializable]
    [LabelText("红点路径")]
    public class RedPointPath
    {
        [ValueDropdown("GetIdList")]
        [LabelText("红点路径")]
        public string redPointPath;

        public static implicit operator string(RedPointPath mini)
        {
            return mini.redPointPath;
        }

        public static implicit operator RedPointPath(string name)
        {
            return new RedPointPath() {
                redPointPath = name
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var fields = typeof(RedDotPath).GetFields();

            var list = new ValueDropdownList<string>();

            foreach (var field in fields)
            {
                list.Add(new ValueDropdownItem<string>((string)field.GetValue(null), (string)field.GetValue(null)));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class SpineAnimationName
    {
        [LabelText("Spine动画组件")]
        public SkeletonGraphic spineAnimation;

        [LabelText("动画名")]
        [ValueDropdown("GetSpineAnimation")]
        public string animationName;

        [LabelText("循环次数")]
        public int loopCount;

        public void PlaySpineAnimation()
        {
            if (spineAnimation == null || string.IsNullOrEmpty(animationName) || animationName.Equals("<None>"))
            {
                return;
            }

            spineAnimation.AnimationState.SetAnimation(0, animationName, loopCount <= 0);
        }

#if UNITY_EDITOR
        private List<string> GetSpineAnimation()
        {
            var list = new List<string>();

            if (spineAnimation == null) return list;

            list.Add("<None>");

            foreach (var ani in spineAnimation.SkeletonData.Animations)
            {
                list.Add(ani.Name);
            }

            return list;
        }
 #endif
    }

    [LabelText("背包道具类型")]
    [System.Serializable]
    public class BagPropId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("道具Id")]
        public int bagPropId;

        public static implicit operator int(BagPropId boxItemId)
        {
            return boxItemId.bagPropId;
        }

        public static implicit operator BagPropId(int bagPropId)
        {
            return new BagPropId() {
                bagPropId = bagPropId
            };
        }

        public override string ToString()
        {
            return $"bagPropId={bagPropId}";
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/BagPropCategory.bytes");
            var category = new BagPropCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            foreach (var config in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{config.Remark}({config.Id})", config.Id));
            }

            return list;
        }
#endif
    }

    [LabelText("背包道具类型")]
    [System.Serializable]
    public class BagPropIdCount
    {
        [ValueDropdown("GetIdList")]
        [LabelText("道具Id")]
        public int bagPropId;

        [LabelText("道具数量")]
        public float bagPropCount;

        public override string ToString()
        {
            return $"bagPropId={bagPropId} bagPropCount={bagPropCount}";
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/GameRes/LubanData/Bytes/BagPropCategory.bytes");
            var category = new BagPropCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            foreach (var config in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{config.Name}({config.Id})", config.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class GardenLevelId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("普通关卡Id")]
        public int levelId;

        public static implicit operator int(GardenLevelId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator GardenLevelId(int levelId)
        {
            return new GardenLevelId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenLevelCategory.bytes");
            var category = new GardenLevelCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);

            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.LevelName}({levelConfig.Id})", levelConfig.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class GardenEndlessLevelId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("无尽关卡Id")]
        public int levelId;

        public static implicit operator int(GardenEndlessLevelId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator GardenEndlessLevelId(int levelId)
        {
            return new GardenEndlessLevelId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenEndlessLevelCategory.bytes");
            var category = new GardenEndlessLevelCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);

            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.Id}", levelConfig.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class GardenFlowerId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("鲜花")]
        public int levelId;

        public static implicit operator int(GardenFlowerId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator GardenFlowerId(int levelId)
        {
            return new GardenFlowerId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenFlowerTypeCategory.bytes");
            var category = new GardenFlowerTypeCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);

            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.FlowerName}({levelConfig.Id})", levelConfig.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class BlockFlowerId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("鲜花")]
        public int levelId;

        public static implicit operator int(BlockFlowerId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator BlockFlowerId(int levelId)
        {
            return new BlockFlowerId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/BlockFlowerTypeCategory.bytes");
            var category = new BlockFlowerTypeCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);

            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.FlowerName}({levelConfig.Id})", levelConfig.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class GardenGridTypeId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("格子类型")]
        public int levelId;

        public static implicit operator int(GardenGridTypeId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator GardenGridTypeId(int levelId)
        {
            return new GardenGridTypeId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenFlowerGridTypeCategory.bytes");
            var category = new GardenFlowerGridTypeCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);
            
            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.GridName}({levelConfig.Id})", levelConfig.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class BlockLevelId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("普通关卡Id")]
        public int levelId;

        public static implicit operator int(BlockLevelId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator BlockLevelId(int levelId)
        {
            return new BlockLevelId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/BlockLevelCategory.bytes");
            var category = new BlockLevelCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);

            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.LevelName}({levelConfig.Id})", levelConfig.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class BlockGridTypeId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("格子类型")]
        public int levelId;

        public static implicit operator int(BlockGridTypeId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator BlockGridTypeId(int levelId)
        {
            return new BlockGridTypeId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/BlockGridTypeCategory.bytes");
            var category = new BlockGridTypeCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);

            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.GridName}({levelConfig.Id})", levelConfig.Id));
            }

            return list;
        }
#endif
    }

    [System.Serializable]
    public class LineLevelId
    {
        [ValueDropdown("GetIdList")]
        [LabelText("普通关卡Id")]
        public int levelId;

        public static implicit operator int(LineLevelId levelId)
        {
            return levelId.levelId;
        }

        public static implicit operator LineLevelId(int levelId)
        {
            return new LineLevelId()
            {
                levelId = levelId
            };
        }

#if UNITY_EDITOR

        private IEnumerable GetIdList()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/LineLevelCategory.bytes");
            var category = new LineLevelCategory(new ByteBuf(bytes.bytes));

            var list = new ValueDropdownList<int>();

            list.Add("None", -1);

            foreach (var levelConfig in category.DataList)
            {
                list.Add(new ValueDropdownItem<int>($"{levelConfig.LevelName}({levelConfig.Id})", levelConfig.Id));
            }

            return list;
        }
#endif
    }
}