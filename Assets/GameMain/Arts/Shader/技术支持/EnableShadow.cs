using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class EnableShadow : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Renderer>().receiveShadows = true;
    }
}
