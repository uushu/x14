using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UICustomButton))]
public class SelectBtn : MonoBehaviour
{
   private UICustomButton btn;
   public TextMeshProUGUI text;
   public bool isSelect;
   public Image bg;
   public Sprite[] bgSprites;//0:未选择 1:选择

   public Material[] textOutLine;//0:未选择 1:选择

   [LabelText("选中显示,不选中隐藏")]
   public List<GameObject> selectShow;
   [LabelText("选中隐藏,不选中显示")]
   public List<GameObject> selectHide;

   private ISelectBtnHandle handle;
   private int indexId;
   private void Awake()
   {
      btn = GetComponent<UICustomButton>();
      btn.AddListener(OnClick);
   }

   public void Init(ISelectBtnHandle _handle,int _indexId)
   {
      handle = _handle;
      indexId = _indexId;
   }
   
   private void OnClick()
   {
      if(isSelect)return;
      //SetSelect(!isSelect);
      handle?.OnSelectBtnClick(indexId);
   }

   public void SetSelect(bool select)
   {
      isSelect = select;
      
      bg.sprite = bgSprites[isSelect ? 1 : 0];
      
      for (int i = 0; i < selectShow.Count; i++)
      {
         selectShow[i]?.SetActive(select);
      }

      for (int i = 0; i < selectHide.Count; i++)
      {
         selectHide[i]?.SetActive(!select); 
      }

      if (text!=null)
      {
         ColorUtility.TryParseHtmlString(isSelect ? "#FFFFFF" : "#CCB36C",out var color);
         text.color = color;
         text.fontMaterial = textOutLine[isSelect ? 1 : 0];
      }
   }
}

public interface ISelectBtnHandle
{
   void OnSelectBtnClick(int index);
}