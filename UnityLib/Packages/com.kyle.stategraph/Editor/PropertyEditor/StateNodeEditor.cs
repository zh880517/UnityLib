using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class StateNodeEditor
{
    private PropertyEditor.IDrawer drawer;
    private bool foldout = true;
    private string typeName;
    private StateNode Node;
    public StateNodeEditor(StateNode node)
    {
        Type type = node.Data.GetType();
        Node = node;
        drawer = PropertyEditor.DrawerCollector.CreateDrawer(type);
        var dpName = type.GetCustomAttribute<DisplayNameAttribute>();
        typeName = dpName != null ? dpName.Name : type.Name;
    }
    public static GUIStyle backGrountStyle = new GUIStyle("OL box NoExpand") { padding = new RectOffset(0, 0, 0, 0) };
    public void OnInspectorGUI()
    {
        using (new GUILayout.VerticalScope(backGrountStyle))
        {
            using(new GUILayout.HorizontalScope("ContentToolbar"))
            {
                foldout = EditorGUILayout.Foldout(foldout, $"{typeName} - {Node.Name}", true);
                if (!foldout)
                    return;

            }
            using (new GUILayout.VerticalScope(PropertyEditor.ClassTypeDrawer.ContentStyle))
            {
                EditorGUILayout.SelectableLabel(Node.Data.GetType().Name, EditorStyles.centeredGreyMiniLabel, GUILayout.Height(20));
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("名字");
                    EditorGUI.BeginChangeCheck();
                    string name = EditorGUILayout.TextField(Node.Name);
                    if (EditorGUI.EndChangeCheck())
                    {
                        PropertyEditor.DrawerCollector.OnPropertyModify(Node.Graph);
                        Node.Name = name;
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("描述");
                    EditorGUI.BeginChangeCheck();
                    string commit = EditorGUILayout.TextArea(Node.Comments);
                    if (EditorGUI.EndChangeCheck())
                    {
                        PropertyEditor.DrawerCollector.OnPropertyModify(Node.Graph);
                        Node.Comments = commit;
                    }
                }
                drawer.Draw(null, Node.Data, Node.Graph);
            }
        }
    }
}
