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
        var dpName = type.GetCustomAttribute<DisaplayNameAttribute>();
        typeName = dpName != null ? dpName.Name : type.Name;
    }
    public static GUIStyle backGrountStyle;
    public void OnInspectorGUI()
    {
        if (backGrountStyle == null)
        {
            backGrountStyle = new GUIStyle("OL box NoExpand");
            backGrountStyle.padding = new RectOffset(0, 0, 0, 0);
        }
        using (new GUILayout.VerticalScope(backGrountStyle))
        {
            using(new GUILayout.HorizontalScope("ContentToolbar"))
            {
                foldout = EditorGUILayout.Foldout(foldout, $"{typeName} - {Node.Name}", true);
                if (!foldout)
                    return;

            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Node.Data.GetType().Name, EditorStyles.centeredGreyMiniLabel);
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
                    drawer.Draw(null, Node.Data, Node.Graph);
                }
            }
        }
    }
}
