using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMain;
using UnityEngine;

public class HallToggleNode : MonoBehaviour
{
    private List<UICustomButton> btns;
    public RectTransform selectImage;

    private void Awake()
    {
        btns = transform.GetComponentsInChildren<UICustomButton>().ToList();
        for (int i = 0; i < btns.Count; i++)
        {
            var index = i;
            btns[i].AddListener(() => { OnClick(index); });
        }
    }

    public void InitAction(int SelectIndex, Action<int> action)
    {
        OnClickAction = action;
        if (SelectIndex >= btns.Count)
        {
            SelectIndex = 0;
        }

        OnClick(SelectIndex);
    }

    public void SetSelectIndex(int index)
    {
        OnClick(index);
    }

    private int CurSelectIndex;

    public Action<int> OnClickAction;

    public void OnClick(int index)
    {
        if (CurSelectIndex != index)
        {
            var targetPos = btns[index].gameObject.transform.localPosition;
            selectImage.transform.DOLocalMoveX(targetPos.x, 0.2f);
            CurSelectIndex = index;
            OnClickAction?.Invoke(index);
        }

        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].transform.GetChild(0).DOScale(i == index ? Vector3.one * 1.35f : Vector3.one, 0.2f);
        }
    }
}