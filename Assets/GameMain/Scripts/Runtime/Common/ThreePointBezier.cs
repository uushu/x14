using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain
{
    public class ThreePointBezier : MonoBehaviour
    {
        public void TestMove(Vector3 middle, Vector3 target)
        {
            StartCoroutine(DoMove(transform.position, middle, target));
        }

        private IEnumerator DoMove(Vector3 startPosition, Vector3 middle, Vector3 target)
        {
            for (float i = 0; i < 1; i += Time.deltaTime)
            {
                var p1 = Vector3.Lerp(startPosition, middle, i);
                var p2 = Vector3.Lerp(middle, target, i);
                var p3 = Vector3.Lerp(p1, p2, i);
                yield return StartCoroutine(MoveTarget(p3));
            }

            GameObject.Destroy(gameObject);
        }

        private IEnumerator MoveTarget(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                var direction = targetPosition - transform.position;
                transform.up = direction;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);

                yield return null;
            }
        }
    }
}