using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LocalizationExtension : Editor
{
    [MenuItem("GameObject/UI/Localization_Text", false)]
    static public void AddText(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Text");
        LocalizationText txt = go.AddComponent<LocalizationText>();
        PlaceUIElementRoot(go, menuCommand);
    }

    // 以下是创建Text为Canvas的子物体，添加EventSystem，设置Canvas该有的组件的等
    private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (parent == null || parent.GetComponentInParent<Canvas>() == null)
        {
            parent = GetOrCreateCanvasGameObject();
        }
        string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
        element.name = uniqueName;
        Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
        Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
        GameObjectUtility.SetParentAndAlign(element, parent);
        Selection.activeGameObject = element;
    }

    static public GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;
        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;
        canvas = UnityEngine.Object.FindObjectOfType(typeof(Canvas)) as Canvas;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;
        return CreateNewUI();
    }

    static public GameObject CreateNewUI()
    {
        var root = new GameObject("Canvas");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
        CreateEventSystem(false, null);
        return root;
    }
    private static void CreateEventSystem(bool select, GameObject parent)
    {
        var esys = UnityEngine.Object.FindObjectOfType<EventSystem>();
        if (esys == null)
        {
            var eventSystem = new GameObject("EventSystem");
            GameObjectUtility.SetParentAndAlign(eventSystem, parent);
            esys = eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
        }
        if (select && esys != null)
        {
            Selection.activeGameObject = esys.gameObject;
        }
    }
}


/// <summary>
/// 这里重写添加KeyString属性必须要规范到serializedproperty KeyString然后重写OnEnable，在OnInspectorGUI里显示，
/// 否则在预制体预览界面才会出现此bug
/// 用DelayedTextField是要按回车或者焦点移动才会完成，防止一直持续刷
/// 处理SerializedProperty类型的字段需要用以下两句包裹起来，否则会出问题，
///  serializedObject.Update();
///  serializedObject.ApplyModifiedProperties();
/// 详情参考UGUI源码：https://bitbucket.org/Unity-Technologies/ui/downloads/?tab=downloads
/// </summary>
[CustomEditor(typeof(LocalizationText), true)]
class LocalizationTextEditor : UnityEditor.UI.TextEditor
{
   
    private SerializedProperty KeyString;
    bool isShowPreview = false;
    string headLine;
    GUIStyle style;
    protected override void OnEnable()
    {
        base.OnEnable();
        KeyString = serializedObject.FindProperty("KeyString");
        style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = 20;
        style.alignment = TextAnchor.LowerLeft;
    }
    public override void OnInspectorGUI()
    {
        LocalizationText component = (LocalizationText)target;
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.ObjectField("Font Style", component.StyleFont(), typeof(UIFont), true);
        EditorGUILayout.DelayedTextField(KeyString);
        serializedObject.ApplyModifiedProperties();
        component.font = component.StyleFont().CurrentFont;
        string key = component.KeyString;
        if (!string.IsNullOrEmpty(key))
        {
            Localization.LoadDictionary();
            if (!Localization.dictionary.ContainsKey(key))
            {
                return;
            }
            List<string> languages = Localization.dictionary["KEY"];     // 获取有几种语言
            List<string> value = Localization.dictionary[key];
            //绘制预览样式                
            GUILayout.BeginHorizontal();
            if (!GUILayout.Toggle(true, headLine, "dragtab", GUILayout.MinWidth(20f)))
            {               
                isShowPreview = !isShowPreview;
            }
            GUILayout.EndHorizontal();
            if (isShowPreview)
            {
                headLine = "\u25BC Preview";
                for (int i = 0; i < value.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(languages[i]);
                    GUILayout.Space(15);
                    GUILayout.TextField(value[i], style);
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                headLine = "\u25BA Preview";
            }
        }
    }

}
