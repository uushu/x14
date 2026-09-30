using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class PoolsMgr : AMonoManager<PoolsMgr>
    {

        public Transform poolRoot;

        protected override void OnEnter()
        {
            base.OnEnter();

        }

        protected override void OnExit()
        {
            base.OnExit();

        }
    }
}