using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public interface IBlockPassGameRule
    {
        public Action OnGameSuccessCallBack { get; set; }

        public Action OnGameFailedCallBack { get; set; }

        bool CollectButterflyAnimationIsComplete();
    }
}