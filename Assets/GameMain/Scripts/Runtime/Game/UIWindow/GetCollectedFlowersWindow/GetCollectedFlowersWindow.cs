using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetCollectedFlowersWindow : ABaseWindow
{
   public TextMeshProUGUI Name;

   public Image ResImage;
   
   public Transform AnimGO;
   
   public Action<string> OnClose;

   private string ResName;
   protected async override void OnShow(object parameters)
   {
      AudioMgr.Instance.PlayAudioOneShot(AudioName.GetAwardAudio);
      var FlowerAtlasId= (int)parameters;
      var flowerAtlas =
         ConfigMgr.Instance.Tables.FlowerAtlasCategory.GetOrDefault(FlowerAtlasId);
      if (flowerAtlas != null)
      {
         Name.text = flowerAtlas.FlowerAtlasName;
         ResourceMgr.Instance.SetImageSprite(ResImage, flowerAtlas.FlowerSpriteNoMask);
         ResName = flowerAtlas.FlowerSpriteNoMask;
      }
      AnimGO.transform.localPosition = new Vector3(-300f, 243f, 0);
      DOTween.Sequence().Append(
         AnimGO.DOLocalMove(new Vector3(240,-300,0), 1.5f)
      ).AppendInterval(2f).SetLoops(-1, LoopType.Restart);
      
   }

   protected override void OnHide()
   {
      OnClose?.Invoke(ResName);
   }
}
