using UnityEditor;
using UnityEngine;

public class EditorStyleDisplayWindow : EditorWindow 
{
    Vector2 scrollPosition = new Vector2(0,0);
    string search = "";
    [MenuItem(("Tools/EditorStyleDisplay"))]
    public static void Init()
    {
        EditorStyleDisplayWindow window=(EditorStyleDisplayWindow)EditorWindow.GetWindow(typeof(EditorStyleDisplayWindow));
        window.titleContent = new GUIContent ("EditorStyleViewer");
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.Label("Click a Sample to copy its Name to your Clipboard","MiniBoldLabel");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Search:");
        search = EditorGUILayout.TextField(search);
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach(GUIStyle style in GUI.skin.customStyles){
            if(style.name.ToLower().Contains(search.ToLower())){
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                GUILayout.Space(7);
                if(GUILayout.Button(style.name,style)){
                    EditorGUIUtility.systemCopyBuffer = "\""+style.name+"\"";
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel("\""+style.name+"\"");
                GUILayout.EndHorizontal();
                GUILayout.Space(11);
            }
        }
        GUILayout.EndScrollView();
    }
}