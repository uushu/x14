using cfg;
using Cysharp.Threading.Tasks;
using Luban;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public class FlowerWaitingArea : ILogicBase
    {
        [LabelText("游戏入口")]
        public GardenGameRoot flowerRoot;

        [LabelText("旋转的按钮")]
        public List<Image> rotateButtons;

        [LabelText("鲜花初始的形状缩放")]
        public float flowerShapeInitScale = 0.8f;

        [LabelText("等待区列表")]
        public List<FlowerShape> waitingFlowerShape;

        public bool WaitAreaIsEmpty
        {
            get
            {
                for (int i = 0; i < waitingFlowerShape.Count; i++)
                {
                    if (waitingFlowerShape[i].gameObject.activeSelf)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsGameFailed
        {
            get
            {
                for (int i = 0; i < waitingFlowerShape.Count; i++)
                {
                    if (waitingFlowerShape[i].gameObject.activeSelf == false) continue;

                    if (waitingFlowerShape[i].isCanPutToMap)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsGeneratorFlowers { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < waitingFlowerShape.Count; i++)
            {
                waitingFlowerShape[i].gameObject.SetActive(false);
                waitingFlowerShape[i].Initialize();
            }

            flowerRoot.flowerGridsMap.OnPlaceToMapBeforeCallBack += OnFlowerPlaceToMapBefore;
            flowerRoot.flowerGridsMap.OnPlaceToMapAfterCallBack += OnFlowerPlaceToMapAfter;
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            flowerRoot.flowerGridsMap.OnPlaceToMapBeforeCallBack -= OnFlowerPlaceToMapBefore;
            flowerRoot.flowerGridsMap.OnPlaceToMapAfterCallBack -= OnFlowerPlaceToMapAfter;
        }

        public override void OnStart()
        {
            base.OnStart();

            IsGeneratorFlowers = false;
            HideAllRotateButton();
        }

        public override void OnSuccess()
        {
            base.OnSuccess();

            SetFlowerForbidenDrag();
        }

        private void OnFlowerPlaceToMapBefore(FlowerRemoveResult result)
        {
            HideAllRotateButton();

            if (WaitAreaIsEmpty == false && !result.isReliveRemove)
            {
                RefreshShapeEnable();
            }
        }

        private void OnFlowerPlaceToMapAfter()
        {
            //是否刷新的等待区
            if (WaitAreaIsEmpty)
            {
                if (GardenGameRoot.Instance.currentStatus != EStatus.Success)
                {
                    AutoRefreshWaitingShape(IGardenRefreshRule.LeastCountCanPutInMap);
                }
            }
        }

        private void PushAllToPool()
        {
            for (int i = 0; i < waitingFlowerShape.Count; i++)
            {
                var flowerShap = waitingFlowerShape[i];
                flowerShap.PushAllToPool();
            }
        }

        //等待区消耗完自动刷新等待区
        public async void AutoRefreshWaitingShape(int leastEnsurtPutInMapCount)
        {
            if (IsGeneratorFlowers)
            {
                Debugger.LogError("正在生成鲜花");
                return;
            }

            //权重随机
            flowerRoot.GardenRefreshRule.RefreshFlowerInfo(waitingFlowerShape.Count, leastEnsurtPutInMapCount, out var listInfos1);
            await GeneratorWaitingShape(listInfos1);
            GardenRefreshFlowerInfoList.Release(listInfos1);
        }

        private async UniTask GeneratorWaitingShape(List<GardenRefreshFlowerInfo> listInfos)
        {
            if (waitingFlowerShape.Count != listInfos.Count)
            {
                Debugger.LogError($"刷新等待区失败 Count != listInfos.Count:{listInfos.Count}");
                return;
            }

            IsGeneratorFlowers = true;
            for (int i = 0; i < waitingFlowerShape.Count; i++)
            {
                var gridType = listInfos[i].GridTypeId;
                var mainFlowerId = listInfos[i].MainFlowerId;

                var flowerShap = waitingFlowerShape[i];
                flowerShap.gameObject.SetActive(true);
                flowerShap.transform.localPosition = Vector3.zero;
                flowerShap.transform.localScale = Vector3.one * flowerShapeInitScale;
                await flowerShap.GeneratorFlowerRuntime(gridType, mainFlowerId, listInfos[i].FlowersId);
            }

            RefreshShapeEnable();
            //等动画播放完成
            await UniTask.WaitForSeconds(1f);
            IsGeneratorFlowers = false;
        }

        //使用道具刷新等待区的鲜花,必须可以放置在等待区
        public void UseBagPropRefresh()
        {
            if (IsGeneratorFlowers) return;

            PushAllToPool();
            AutoRefreshWaitingShape(waitingFlowerShape.Count);
        }

        public async void ReliveRefresh()
        {
            PushAllToPool();

            //权重随机
            flowerRoot.GardenRefreshRule.ReliveRefreshFlowerInfo(waitingFlowerShape.Count, out var listInfos1);
            await GeneratorWaitingShape(listInfos1);
            GardenRefreshFlowerInfoList.Release(listInfos1);
        }

        //格子是否可以放在地图上
        public void RefreshShapeEnable()
        {
            if (GardenGameRoot.Instance.currentStatus == EStatus.Success)
            {
                return;
            }

            for (int i = 0; i < waitingFlowerShape.Count; i++)
            {
                var flowerShap = waitingFlowerShape[i];

                if (flowerShap.gameObject.activeSelf == false) continue;
                flowerShap.RefreshFlowerEnable();
            }
        }

        //游戏通关设置花朵的alpha
        public void SetFlowerForbidenDrag()
        {
            for (int i = 0; i < waitingFlowerShape.Count; i++)
            {
                var flowerShap = waitingFlowerShape[i];
                if (flowerShap.gameObject.activeSelf == false) continue;
                flowerShap.ForbidenDrag = true;
            }
        }

        public void ShowRotateButton()
        {
            for (int i = 0; i < rotateButtons.Count; i++)
            {
                if (waitingFlowerShape[i].gameObject.activeSelf == false)
                {
                    rotateButtons[i].gameObject.SetActive(false);
                    continue;
                }

                if (waitingFlowerShape[i].ActiveFlowers.Count <= 1)
                {
                    rotateButtons[i].gameObject.SetActive(false);
                    continue;
                }

                rotateButtons[i].gameObject.SetActive(true);
            }
        }

        private void HideAllRotateButton()
        {
            for (int i = 0; i < rotateButtons.Count; i++)
            {
                rotateButtons[i].gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            waitingFlowerShape = transform.GetComponentsInChildren<FlowerShape>().ToList();
        }

        [FoldoutGroup("编辑器")]
        [Button("随机生成花朵")]
        private void RandomGenerator()
        {
            var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenFlowerGridTypeCategory.bytes");
            var category = new GardenFlowerGridTypeCategory(new ByteBuf(bytes.bytes));

            var bytes2 = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenFlowerTypeCategory.bytes");
            var category2 = new GardenFlowerTypeCategory(new ByteBuf(bytes2.bytes));

            for (int i = 0; i < waitingFlowerShape.Count; i++)
            {
                var randomGrid = UtilityTool.RandomNumber(category.DataList);
                var randomFlower = UtilityTool.RandomNumber(category2.DataList);

                var flowerShape = waitingFlowerShape[i];
                flowerShape.transform.localPosition = Vector3.zero;
                flowerShape.mainFlowerId = randomFlower.Id;
                flowerShape.gridTypeId = randomGrid.Id;
                flowerShape.GeneratorFlowerEditor();
            }
        }
#endif
    }
}