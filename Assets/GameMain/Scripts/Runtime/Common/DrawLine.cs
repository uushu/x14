using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class DrawLine : MonoBehaviour
    {
        public List<Transform> points;

        private void OnDrawGizmos()
        {
            if (points.Count <= 0) return;
            
            Gizmos.color = Color.red;
            
            for (int i = 0; i < points.Count - 1; i++)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
            
            Gizmos.DrawLine(points[points.Count - 1].position, points[0].position);
        }
    }
}