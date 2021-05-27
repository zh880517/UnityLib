using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class ListDrawer : IDrawer
    {
        private Type ElementType;
        private Type Type;
        public List<IDrawer> Drawers = new List<IDrawer>();
        private bool foldout = true;
        private static readonly GUIContent empty = new GUIContent();
        public static readonly GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
        public ListDrawer(Type type)
        {
            Type = type;
            ElementType = type.GenericTypeArguments[0];
        }

        public bool Draw(GUIContent content, object val, StateGraph context)
        {
            using (new GUILayout.HorizontalScope("RL Header"))
            {
                foldout = EditorGUILayout.Foldout(foldout, content ?? empty);
                if (!foldout)
                    return false;
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(iconToolbarPlusMore, "RL FooterButton"))
                {
                    DrawerCollector.OnPropertyModify(context);
                    var list = val as IList;
                    list.Add(CreatNew());
                }
            }
            return Draw(val, context);
        }

        private bool Draw(object val, StateGraph context)
        {
            if (val == null)
            {
                return true;
            }
            IList list = val as IList;
            while (Drawers.Count < list.Count)
            {
                Drawers.Add(DrawerCollector.CreateDrawer(ElementType));
            }
            for (int i = 0; i < list.Count; ++i)
            {
                using (new GUILayout.HorizontalScope("Box"))
                {
                    var drawer = Drawers[i];
                    if (GUILayout.Button("", "RL DragHandle"))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("删除"), false, (obj) =>
                        {
                            int idx = (int)obj;
                            DrawerCollector.OnPropertyModify(context);
                            GUI.FocusControl(null);
                            Drawers.RemoveAt(idx);
                            list.RemoveAt(idx);
                        }, i);
                        if (i > 0)
                        {
                            menu.AddItem(new GUIContent("上移"), false, (obj) =>
                            {
                                int idx = (int)obj;
                                DrawerCollector.OnPropertyModify(context);
                                GUI.FocusControl(null);
                                var old = list[idx];
                                list[idx] = list[idx - 1];
                                list[idx - 1] = old;
                                var oldDrawer = Drawers[idx];
                                Drawers[idx] = Drawers[idx - 1];
                                Drawers[idx - 1] = oldDrawer;
                            }, i);
                        }
                        if (i < list.Count - 1)
                        {
                            menu.AddItem(new GUIContent("下移"), false, (obj) =>
                            {
                                int idx = (int)obj;
                                DrawerCollector.OnPropertyModify(context);
                                GUI.FocusControl(null);
                                var old = list[idx];
                                list[idx] = list[idx + 1];
                                list[idx + 1] = old;
                                var oldDrawer = Drawers[idx];
                                Drawers[idx] = Drawers[idx + 1];
                                Drawers[idx + 1] = oldDrawer;
                            }, i);
                        }
                        menu.ShowAsContext();
                    }
                    using (new GUILayout.VerticalScope())
                    {
                        if (drawer.Draw(null, list[i], context))
                        {
                            DrawerCollector.OnPropertyModify(context);
                            list[i] = drawer.GetValue();
                        }

                    }
                }
            }
            return false;
        }

        private object CreatNew()
        {
            if (ElementType == typeof(string))
                return string.Empty;
            return Activator.CreateInstance(ElementType);
        }

        public object GetValue()
        {
            return Activator.CreateInstance(Type);
        }
    }

}