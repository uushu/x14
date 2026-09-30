using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using Spine.Unity;
using UnityEngine;
using Animation = Spine.Animation;
using AnimationState = Spine.AnimationState;

public class DanceCatScript : MonoBehaviour
{
   public SkeletonGraphic spine;

   public void Init(string danceName)
   {
      //初始化SkeletonAnimation
      spine.Initialize(true);
      
      //spine.AnimationState.SetEmptyAnimation(0, 0);
      //spine.AnimationState.SetAnimation(0, danceName, true);
      RandomCyclePlay();
   }

   private AnimationState.TrackEntryDelegate complete;
   /// <summary>
   /// 随机循环播放
   /// </summary>
   public void RandomCyclePlay()
   {
      Animation[] allAni = spine.SkeletonData.Animations.ToArray();
      Animation temp =  allAni[Random.Range(0, allAni.Length)];
      //Debug.LogError($"随机播放{temp.Name}");
      spine.AnimationState.SetAnimation(0,  temp.Name, false);
      complete = delegate
      {
         spine.AnimationState.Complete -= complete;
         complete = null;
         RandomCyclePlay();
      };
      spine.AnimationState.Complete += complete;
   }
}
