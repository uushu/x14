using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.Garden
{
    public class GardenLongTimeNotOperation : AGameLongTimeNotOperation
    {
        protected override bool _executeUpdate => GardenGameRoot.Instance.currentStatus == EStatus.Gaming;

        public override void Initialize()
        {
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapAfterCallBack += PlaceToMapAfterCallBack;
        }

        private void PlaceToMapAfterCallBack()
        {
            ResetTimer();
        }

        public override void UnInitialize()
        {
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapAfterCallBack -= PlaceToMapAfterCallBack;
        }
    }
}