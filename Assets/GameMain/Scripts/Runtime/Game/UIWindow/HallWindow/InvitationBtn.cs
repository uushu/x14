using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;

public class InvitationBtn : MonoBehaviour
{
    public UICustomButton btn;
    void Start()
    {
        btn.AddListener(() =>
        {
            UIWindowMgr.Instance.OpenWindow<InvitationWindow>();
        });
    }

 
}
