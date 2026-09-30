using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public class TiledMask : Image
    {
        [LabelText("行数")]
        public int rowCount = 1;

        [LabelText("列数")]
        public int colCount = 1;

        [LabelText("进度值")]
        public float progress;

        [LabelText("翻转显示Y")]
        public bool flipShowY;

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            if (overrideSprite == null) return;

            var totalCount = rowCount * colCount;
            var maxShowProgress = totalCount * progress;

            int currentProgress = 0;
            for (int j = 0; j < rowCount; j++)
            {
                for (int i = 0; i < colCount; i++)
                {
                    var currentStartIndex = 0;

                    if (flipShowY)
                    {
                        currentStartIndex = i + (rowCount - 1 - j) * colCount;
                    }
                    else
                    {
                        currentStartIndex = i + j * colCount;
                    }

                    if (currentProgress >= maxShowProgress)
                    {
                        currentStartIndex *= 4;

                        for (int x = currentStartIndex; x < currentStartIndex + 4; x++)
                        {
                            ClearColor(toFill, x);
                        }
                    }

                    currentProgress++;
                }
            }
        }

        private void ClearColor(VertexHelper toFill, int index)
        {
            UIVertex vertex = default;
            toFill.PopulateUIVertex(ref vertex, index);
            vertex.color = Color.clear;
            toFill.SetUIVertex(vertex, index);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (overrideSprite == null) return;

            var spriteSize = overrideSprite.rect.size;
            var width = spriteSize.x * colCount;
            var height = spriteSize.y * rowCount;
            rectTransform.sizeDelta = new Vector2(width, height);
        }
#endif
    }
}