using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Luban;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public class FlowerShape : MonoBehaviour
    {
        [LabelText("鲜花格子")]
        public UICustomGrids customGrids;

        [LabelText("格子形状")]
        public GardenGridTypeId gridTypeId;

        [LabelText("鲜花类型")]
        public GardenFlowerId mainFlowerId;

        [LabelText("点击Y偏移")]
        public float clickYOffset;

        //等待区状态的格子的行列间隔
        private Vector2Int _waitStateRowColSpace = new Vector2Int(-30, -30);
        private float _scaleMin = 0.8f;
        private Vector2 _offset;
        private RectTransform _parentRect;
        public RectTransform ParentRect
        {
            get
            {
                if (_parentRect == null)
                {
                    _parentRect = transform.parent.GetComponent<RectTransform>();
                }
                return _parentRect;
            }
        }

        public GardenFlowerGridTypeConfig GridTypeConfig { get; private set; }

        public GardenFlowerTypeConfig MainFlowerTypeConfig { get; private set; }

        private FlowerWaitingArea _waitingArea => GardenGameRoot.Instance.waitingArea;

        private FlowerGridsMap _gridMap => GardenGameRoot.Instance.flowerGridsMap;

        [LabelText("显示的鲜花")]
        public List<Garden_FlowerSingle> ActiveFlowers { get; set; }

        private List<Vector2Int> _gridsXY = new();
        public List<Vector2Int> GridsXY
        {
            get
            {
                customGrids.GetGridXY(ref _gridsXY);
               return _gridsXY;
            }
        }

        [LabelText("是否可以放置地图")]
        public bool isCanPutToMap;

        [LabelText("禁止拖拽")]
        private bool _forbidenDrag;
        public bool ForbidenDrag
        {
            get => _forbidenDrag;
            set
            {
                _forbidenDrag = value;
                SetFlowerAlpha(_forbidenDrag ? 0.6f : 1f);
            }
        }

        private bool _isClickDown = false;

        public bool IsCanDrag => isCanPutToMap && GardenGameRoot.Instance.currentStatus == EStatus.Gaming && ForbidenDrag == false;

        public void Initialize()
        {
            customGrids.generatorRoot.gameObject.DestroyAllChildren();
            ActiveFlowers = new();
            _forbidenDrag = false;
        }

        public void PushAllToPool()
        {
            if (ActiveFlowers.Count > 0)
            {
                foreach (var flower in ActiveFlowers)
                {
                    GardenGameRoot.Instance.flowerPool.PushToFlowerPool(flower);
                }

                ActiveFlowers.Clear();
            }
        }

        public bool RuntimeIsInMap(ref List<Vector2Int> runtimeGrids)
        {
            runtimeGrids.Clear();

            //判断鲜花运行时的格子是否超出了地图的范围
            foreach (var flower in ActiveFlowers)
            {
                //左下角
                var flowerXY = _gridMap.customGrids.TransformToGridXY(flower.transform);

                if (_gridMap.customGrids.GridIndexIsValid(flowerXY) == false || runtimeGrids.Contains(flowerXY))
                {
                    runtimeGrids.Clear();
                    return false;
                }

                runtimeGrids.Add(flowerXY);
            }

            return true;
        }

        public bool GetRuntimeGridXY(ref List<Vector2Int> runtimeGridXY)
        {
            if (RuntimeIsInMap(ref runtimeGridXY) == false)
            {
                return false;
            }

            return true;
        }

        public void GetRuntimeGridXY(Vector2Int leftUpGridXY, ref List<Vector2Int> runtimeGridXY)
        {
            runtimeGridXY.Clear();

           customGrids.GetGridXY(ref runtimeGridXY);

            for (var i = 0; i < runtimeGridXY.Count; i++)
            {
                runtimeGridXY[i] += leftUpGridXY;
            }
        }

        public async UniTask GeneratorFlowerRuntime(int gridTypeId, int mainFlowerId, List<int> flowersId)
        {
            _forbidenDrag = false;

            this.gridTypeId = gridTypeId;
            this.mainFlowerId = mainFlowerId;

            _scaleMin = transform.localScale.x;

            var gridTypeConfig = ConfigMgr.Instance.Tables.GardenFlowerGridTypeCategory.GetOrDefault(gridTypeId);
            var flowerTypeConfig = ConfigMgr.Instance.Tables.GardenFlowerTypeCategory.GetOrDefault(mainFlowerId);

            ActiveFlowers.Clear();

            await GeneratorFlower(
                gridTypeConfig,
                flowerTypeConfig,
                flowersId,
                async (flowerId) =>
            {
                var flowerTypeConfig = ConfigMgr.Instance.Tables.GardenFlowerTypeCategory.GetOrDefault(flowerId);
                var gridInstance = await GardenGameRoot.Instance.flowerPool.multiPools.GetEntityAsync(flowerTypeConfig.GardenSpinePrefab);

                var flower = gridInstance.GetComponent<Garden_FlowerSingle>();
                flower.Initialize_Flower(flowerTypeConfig, false);
                ActiveFlowers.Add(flower);
                DelayShow(flower, ActiveFlowers.Count * 0.1f);

                return gridInstance.gameObject;
            });
        }

        private async void DelayShow(Garden_FlowerSingle flowerSingle, float delayTime)
        {
            flowerSingle.gameObject.SetActive(false);
            await UniTask.WaitForSeconds(delayTime);
            flowerSingle.gameObject.SetActive(true);
            flowerSingle.Flower_PlayDefaultAnimation();
        }

        private async UniTask GeneratorFlower(
            GardenFlowerGridTypeConfig gridTypeConfig,
            GardenFlowerTypeConfig mainFlowerTypeConfig,
            List<int> flowersId,
            Func<int, UniTask<GameObject>> onCreateFlower)
        {
            GridTypeConfig = gridTypeConfig;
            MainFlowerTypeConfig = mainFlowerTypeConfig;

            var dataList = gridTypeConfig.GridData;
            customGrids.rowSpace = _waitStateRowColSpace.y;
            customGrids.colSpace = _waitStateRowColSpace.x;

            await customGrids.GeneratorGrids(gridTypeConfig.GridData, gridTypeConfig.Row, gridTypeConfig.Col, flowersId, onCreateFlower);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GardenGameRoot.Instance.dragingFlower != null) return;
            if (IsCanDrag == false) return;
            _isClickDown = true;
            GardenGameRoot.Instance.dragingFlower = this;
            PlatformMgr.Instance.Shake();

            transform.localPosition = Vector3.zero;
            var downLocalPosition = RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRect, eventData.position, UIWindowMgr.Instance.uiCamera, out var downPoint);
            _offset = new Vector2(transform.localPosition.x, transform.localPosition.y) - downPoint;
            customGrids.RefreshRowColSpace(0, 0);

            if (DOTween.IsTweening(transform)) transform.DOKill();
            transform.localScale = Vector3.one;

            AudioMgr.Instance.PlayAudioOneShot(AudioName.FlowerDownAudio);
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isClickDown == false) return;
            _isClickDown = false;
            GardenGameRoot.Instance.dragingFlower = null;

            if (IsCanDrag == false || _waitingArea.flowerRoot.flowerGridsMap.OnPointerUp_TryEliminateFlowers(this) == false)
            {
                var sequence = DOTween.Sequence();
                customGrids.RefreshRowColSpace(_waitStateRowColSpace.y, _waitStateRowColSpace.x);
                var t1 = transform.DOScale(Vector3.one * _scaleMin, 0.3f);
                var t2 = transform.DOLocalMove(Vector3.zero, 0.3f);
                sequence.Append(t1);
                sequence.Join(t2);
                sequence.SetTarget(transform);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isClickDown == false) return;
            if (IsCanDrag == false) return;
            if (GardenGameRoot.Instance.dragingFlower != this) return;

            var downLocalPosition = RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRect, eventData.position, UIWindowMgr.Instance.uiCamera, out var downPoint);
            transform.localPosition = _offset + downPoint + new Vector2(0, clickYOffset);

            _waitingArea.flowerRoot.flowerGridsMap.OnDrag_FlowerShape(this);
        }

        public void RefreshFlowerEnable()
        {
            isCanPutToMap = GardenGameRoot.Instance.flowerGridsMap.FlowerCanPlaceInAny(this);
            foreach (var flowerUI in ActiveFlowers)
            {
                flowerUI.SetGray(isCanPutToMap == false);
            }
        }

        private void SetFlowerAlpha(float alpha)
        {
            foreach (var flowerUI in ActiveFlowers)
            {
                flowerUI.SetSpineAlpha(alpha);
            }
        }

        public void OnGridClick(PointerEventData eventData)
        {
            AudioMgr.Instance.PlayAudioOneShot(AudioName.RotatePropAudio);

            //旋转
            RotateSelf90();
        }

        private Vector3 RotatePoint(Vector3 pivot, Vector3 point, Quaternion rotation)
        {
            // 计算旋转中心点与待旋转点之间的向量
            var dir = point - pivot;

            // 计算旋转矩阵
            var rotationMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

            // 将向量dir与旋转矩阵相乘，得到旋转后的向量
            Vector3 rotatedDir = rotationMatrix * dir;

            // 将旋转后的向量与旋转中心点的坐标相加，得到旋转后的点的坐标
            var rotatedPoint = pivot + rotatedDir;

            // 更新旋转后的点的位置
            return rotatedPoint;
        }

        [Obsolete]
        private void RotateSelf90_Obsolete()
        {
            customGrids.ClockwiseRotate90(true);
            RefreshFlowerEnable();
        }

        private void RotateSelf90()
        {
            var rotateEffect = GardenGameRoot.Instance.flowerPool.rotateEffectPool.GetFromPoolSync().Item1;
            rotateEffect.transform.SetParent(transform);
            rotateEffect.transform.localScale = Vector3.one;
            rotateEffect.transform.localPosition = Vector3.zero;

            DelayPushToPool(rotateEffect, 1.5f);

            customGrids.RoateSelf90(true, 0.3f, () =>
            {
                RefreshFlowerEnable();
            });
        }

        private async void DelayPushToPool(GameObject rotateEffect, float delayTime)
        {
            await UniTask.WaitForSeconds(delayTime);
            GardenGameRoot.Instance.flowerPool.rotateEffectPool.PushToPool(rotateEffect);
        }

#if UNITY_EDITOR

        private const string flowerFloder = "Assets/GameMain/AssetBundle/Prefab/FlowerPrefab/";

        [FoldoutGroup("编辑器")]
        [Button("生成鲜花-编辑器")]
        public void GeneratorFlowerEditor()
        {
            var bytes = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenFlowerGridTypeCategory.bytes");
            var category = new GardenFlowerGridTypeCategory(new ByteBuf(bytes.bytes));
            var typeConfig = category.GetOrDefault(gridTypeId);

            customGrids.generatorRoot.gameObject.DestroyAllChildren();

            if (typeConfig == null)
            {
                Debug.LogError($"不存在的鲜花格子类型 gridTypeId:{gridTypeId}");
                return;
            }

            var bytes2 = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/AssetBundle/LubanData/Bytes/GardenFlowerTypeCategory.bytes");
            var category2 = new GardenFlowerTypeCategory(new ByteBuf(bytes2.bytes));
            var flowerConfig = category2.GetOrDefault(mainFlowerId);

            if (typeConfig == null)
            {
                Debug.LogError($"不存在的鲜花类型 flowerId:{mainFlowerId}");
                return;
            }

            customGrids.rowCount = typeConfig.Row;
            customGrids.colCount = typeConfig.Col;

            var gridXY = customGrids.ListToGridXY(typeConfig.GridData, typeConfig.Row, typeConfig.Col);
            customGrids.GeneratorGrids(null, gridXY);
        }

        [FoldoutGroup("编辑器")]
        [Button("测试是否可以放置格子在地图上")]
        private void TestIsCanPutInMapAny()
        {
            GardenGameRoot.Instance.flowerGridsMap.FlowerCanPlaceInAny(this);
        }

        [FoldoutGroup("编辑器")]
        [Button("测试旋转-顺时针-90")]
        private void TestRotate()
        {
            //customGrids.ClockwiseRotate90(true);
            RotateSelf90();
        }

        [FoldoutGroup("编辑器")]
        [Button("测试旋转-逆时针-90")]
        private void TestRotate2()
        {
            customGrids.ClockwiseRotate90(false);
        }
#endif
    }
}