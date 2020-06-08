using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class Localization
{
    /// <summary>
    /// 每一行数据
    /// </summary>
    static string line;

    /// <summary>
    /// 是否加载过数据
    /// </summary>
    static bool isLoadFile = false;

    /// <summary>
    /// unity内置api  获取assets文件夹路径
    /// </summary>
    static string path = Application.dataPath;

    /// <summary>
    /// 缓存.txt中的数据字典   主要用这个字典操作
    /// </summary>
    public static Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

    /// <summary>
    /// 设置当前语言
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static string lastLanguage = null;
    static public string SetLanguage(string languagetype)
    {
        //if (languagetype == lastLanguage)
        //{
        //    return lastLanguage;
        //}
        LoadDictionary();
        List<string> lan = dictionary["KEY"];
        for (int i = 0; i < lan.Count; i++)
        {

            if (languagetype==lan[i])
            {
                PlayerPrefs.SetInt("language", i);
                lastLanguage = languagetype;
                LocalizationText.SetLanguage();
                return lan[i];
            }
        }
        Debug.LogError("there is no this language:" + languagetype);
        return null;
    }

    /// <summary>
    /// 输入key，获取当前存储的语言文字，如果上一次没有存因为是int类型所以默认为0，汉字Chinese
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }
        LoadDictionary();
        List<string> languages = dictionary[key];
        int index = PlayerPrefs.GetInt("language");
        if (index < languages.Count)
        {
            if (index < 0)
            {
                index = 0;
            }
            return languages[index];
        }
        Debug.LogError("Which key = " + key + " this line have untranslated words???");
        return null;
    }


    /// <summary>
    /// 加载读取Localization.txt文件
    /// </summary>
    public static void LoadDictionary()
    {
        if (isLoadFile && Application.isPlaying)
        {
            return;
        }
        else
        {
            dictionary.Clear();
            // 读取Localization.csv文件的路径     可自行修改对应路径
          
            TextAsset textAsset = Resources.Load("Localization") as TextAsset;
            string alltext = textAsset.text;
            string[] line = alltext.Split(new char[2] { '\r', '\n' });
            foreach (var item in line)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                string[] content = item.Split(',');   //按逗号隔开
                dictionary.Add(content[0], GetLanguages(content));
            }
            //StreamReader read = File.OpenText(path + @"/Scripts/Localization/Localization.txt");
            //while (!string.IsNullOrEmpty(line = read.ReadLine()))
            //{
            //string[] content = line.Split(',');   //按逗号隔开
            //dictionary.Add(content[0], GetLanguages(content));
            //}
            isLoadFile = true;
        }
    }


    /// <summary>
    /// 从加载的每一行数据中选出语言
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    static List<string> GetLanguages(string[] content)
    {
        List<string> strList = new List<string>();
        for (int i = 1; i < content.Length; i++)
        {
            if (!string.IsNullOrEmpty(content[i]))
            {
                strList.Add(content[i]);
            }
        }
        return strList;
    }
}
