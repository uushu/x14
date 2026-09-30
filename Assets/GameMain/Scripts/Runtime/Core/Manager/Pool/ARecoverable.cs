using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public abstract class ARecoverable : MonoBehaviour
    {
        public string Location { get; set; }

        public virtual void OnPushToPool() { }
        public virtual void OnGetFromPool() { }
    }
}