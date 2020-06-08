using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

[AddComponentMenu("UI/Localization_Text")]
public class LocalizationText : Text
{
    /// <summary>
    /// 文本的key
    /// </summary>
    [SerializeField]
    [FormerlySerializedAs("KeyString")]
    public string KeyString;
    

    /// <summary>
    /// 自定义字体，方便后期替换
    /// if可以注释掉，然后自己手动更换字体的prefab
    /// </summary>
    private static UIFont _styleFont = null;
    public UIFont StyleFont()
    {
        if (_styleFont == null)
        {
            GameObject _font = Resources.Load("FontStyle") as GameObject;
            _styleFont = _font.GetComponent<UIFont>();
        }
        return _styleFont;
    }

    /// <summary>
    /// 文本的value
    /// </summary>
    public override string text
    {
        get
        {
            if (!string.IsNullOrEmpty(KeyString))
            {
                m_Text = Localization.Get(KeyString);
            }
            return m_Text;
        }
        set
        {
            if (String.IsNullOrEmpty(value))
            {
                if (String.IsNullOrEmpty(m_Text))
                    return;
                m_Text = "";
                SetVerticesDirty();
            }
            else if (m_Text != value)
            {
                m_Text = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
    }

    /// <summary>
    /// 初始化  并监听是否修改语言
    /// 一开始去除接受点击事件，因为游戏中大部分文字都是不需要点击事件的
    /// 修改字体
    /// </summary>
    public delegate void SetLanEventHandler();
    public static SetLanEventHandler SetLanguage;
    protected override void OnEnable()
    {
        base.OnEnable();
        raycastTarget = false;
        supportRichText = false;
        SetLanguage += OnLocalize;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        SetLanguage -= OnLocalize;
    }

    /// <summary>
    /// 重新本地化，用于游戏内切换语言时调用
    /// </summary>
    public void OnLocalize()
    {
       
        if (!string.IsNullOrEmpty(KeyString))
        {
            text = Localization.Get(KeyString);
        }
    }


}
