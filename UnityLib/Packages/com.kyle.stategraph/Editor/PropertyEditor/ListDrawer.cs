using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace PropertyEditor
{
    public class ListDrawer : IDrawer
    {
        private Type ElementType;
        private FieldInfo Field;
        public List<IDrawer> Drawers = new List<IDrawer>();
        private bool foldout = true;
        private int selectIndex = -1;

        private static readonly GUIContent empty = new GUIContent();
        public static readonly GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus", "Choose to add to list");
        public static readonly GUIContent iconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
        public ListDrawer(FieldInfo field)
        {
            Field = field;
            ElementType = Field.FieldType.GenericTypeArguments[0];
        }

        public bool Draw(GUIContent content, object val, StateGraph context)
        {
            using (new GUILayout.HorizontalScope("RL Header"))
            {
                foldout = EditorGUILayout.Foldout(foldout, content ?? empty, true);
                if (!foldout)
                    return false;
            }
            bool modify = false;
            using(new GUILayout.VerticalScope("RL Background"))
            {
                modify = Draw(val, context);
            }
            int btnWidth = 25;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                IList list = val as IList;
                using (new GUILayout.HorizontalScope("RL Footer"))
                {
                    using (new EditorGUI.DisabledScope(selectIndex <= 0))
                    {
                        if (GUILayout.Button("▲", "RL FooterButton", GUILayout.Width(btnWidth)))
                        {
                            int idx = selectIndex;
                            DrawerCollector.OnPropertyModify(context);
                            GUI.FocusControl(null);
                            var old = list[idx];
                            list[idx] = list[idx - 1];
                            list[idx - 1] = old;
                            var oldDrawer = Drawers[idx];
                            Drawers[idx] = Drawers[idx - 1];
                            Drawers[idx - 1] = oldDrawer;
                            selectIndex--;
                        }
                    }
                    using (new EditorGUI.DisabledScope(selectIndex >= list.Count - 1))
                    {
                        if (GUILayout.Button("▼", "RL FooterButton", GUILayout.Width(btnWidth)))
                        {
                            int idx = selectIndex;
                            DrawerCollector.OnPropertyModify(context);
                            GUI.FocusControl(null);
                            var old = list[idx];
                            list[idx] = list[idx + 1];
                            list[idx + 1] = old;
                            var oldDrawer = Drawers[idx];
                            Drawers[idx] = Drawers[idx + 1];
                            Drawers[idx + 1] = oldDrawer;
                            selectIndex++;
                        }
                    }
                    if (GUILayout.Button(iconToolbarPlusMore, "RL FooterButton", GUILayout.Width(btnWidth)))
                    {
                        DrawerCollector.OnPropertyModify(context);
                        list.Add(CreatNew());
                        selectIndex = list.Count - 1;
                    }
                    using (new EditorGUI.DisabledScope(selectIndex < 0))
                    {
                        if (GUILayout.Button(iconToolbarMinus, "RL FooterButton", GUILayout.Width(btnWidth)))
                        {
                            DrawerCollector.OnPropertyModify(context);
                            list.RemoveAt(selectIndex);
                            Drawers.RemoveAt(selectIndex);
                            if (selectIndex >= list.Count)
                            {
                                selectIndex--;
                            }
                        }
                    }
                }
                GUILayout.Space(10);
            }
            return modify;
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
                Drawers.Add(DrawerCollector.CreatContainerDrawer(Field, ElementType));
            }
            for (int i = 0; i < list.Count; ++i)
            {
                string style = selectIndex == i ? "MeTransitionSelect" : "RL Element";
                using (new GUILayout.HorizontalScope(style))
                {
                    var drawer = Drawers[i];
                    if (GUILayout.Button("", "RL DragHandle", GUILayout.ExpandHeight(true)))
                    {
                        selectIndex = i;
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
                GUILayout.Space(4);
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
            return Activator.CreateInstance(Field.FieldType);
        }
    }

}