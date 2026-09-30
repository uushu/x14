using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;
using UnityEngine.UI;

public class SettingBtn : MonoBehaviour
{
    public UICustomButton Btn;

    public Image HeadImage; 
    // Start is called before the first frame update
    void Start()
    {
        if (!string.IsNullOrEmpty(PlatformMgr.IconUrl))
        {
            StartCoroutine(HeadImage.SetHttpUrl(PlatformMgr.IconUrl));
        }
        Btn.AddListener(() =>
        {
            UIWindowMgr.Instance.OpenWindowAsync<SettingWindow>();
            //UIWindowMgr.Instance.OpenWindowAsync<SettingWindow1>();
        });
    }
}
