using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameMain.FlowerLine
{
    public class FlowerLineDropStartPoints : MonoBehaviour
    {
        [LabelText("六边形格子")]
        public UIHexagonGrids hexagonGrids;

        [LabelText("掉落的点")]
        public List<RectTransform> dropPoints;

        private List<Vector2Int> _startDropPoints;
        public List<Vector2Int> GetStartDropPoints
        {
            get
            {
                if (_startDropPoints == null)
                {
                    _startDropPoints = new();

                    foreach (var point in dropPoints)
                    {
                        var gridXY = hexagonGrids.LocalPositionToGridXY(point.transform.localPosition);
                        _startDropPoints.Add(gridXY);
                    }
                }

                return _startDropPoints;
            }
        }

#if UNITY_EDITOR

        [Button("刷新")]
        private void OnValidate()
        {
            dropPoints = transform.GetComponentsInChildren<RectTransform>(false).ToList();
            dropPoints.RemoveAt(0);

            foreach (var dropPoint in dropPoints)
            {
                var gridXY = hexagonGrids.LocalPositionToGridXY(dropPoint.transform.localPosition);
                var localPoint = hexagonGrids.GridXYToLocalPosition(gridXY);
                dropPoint.transform.localPosition = localPoint;
            }
        }
#endif
    }
}