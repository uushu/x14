using GameMain.Garden;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.FlowerBlock
{
    public partial class FlowerBlockMap : ILogicBase
    {
        [LabelText("地图格子")]
        public UICustomGrids uicustomGrids;

        [LabelText("拖拽的背景图")]
        public RectTransform dragImage;

        [LabelText("预制体")]
        public ItemUI_FlowerBlock flowerBlockPrefab;

        [LabelText("鲜花的根节点")]
        public Transform flowerRoot;

        public override void Initialize()
        {
            base.Initialize();
            HideDragImage();

            Initialize_Data();
            Initialize_Event();
            Initialize_Generator();
        }

        public override void UnInitialize()
        {
            base.UnInitialize();

            UnInitialize_Data();
            UnInitialize_Event();
            UnInitialize_Generator();
        }

#if UNITY_EDITOR

        [Button("生成地图")]
        private void GeneratorMap()
        {
            uicustomGrids.GeneratorGrids((gridXY, gridInstance) =>
            {
                var y = gridXY.y;

                var  gridIndex = uicustomGrids.GridXY2Index(gridXY);

                if ((gridIndex % 2 + y % 2) == 1)
                {
                    gridInstance.transform.GetComponent<Image>().color = Color.gray;
                }

            }, null);

            GetComponent<RectTransform>().sizeDelta = new Vector2(uicustomGrids.Width, uicustomGrids.Heigth);
        }

        [Button("生成等待区格子")]
        private void GeneratorBlock()
        {
            GeneratorByRule(uicustomGrids.rowCount);
        }
#endif
    }
}