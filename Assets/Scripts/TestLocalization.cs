using System;
using UnityEngine;
using UnityEngine.UI;

public class TestLocalization : MonoBehaviour
{
    LocalizationText mText1;
    Button mEnglishBtn;
    Button mChineseBtn;
    void Start()
    {
        
        mText1 = transform.Find("Text1").GetComponent<LocalizationText>();
        //初始化语言种类
        Localization.SetLanguage("Chinese");
        // 这里填写自定义文字中的key，切换中英文才能生效
        mText1.KeyString = "text2";
        // 下面这种方式虽然也可以赋值但是不能实时切换语言，不推荐使用
        //mText1.text = Localization.Get("text2");
    }

    /// <summary>
    /// 切换中文
    /// </summary>
    public void OnClickChineseBtn()
    {
        Localization.SetLanguage("Chinese");
    }
    /// <summary>
    /// 切换英文
    /// </summary>
    public void OnClickEnglishBtn()
    {
        Localization.SetLanguage("English");
    }
}
