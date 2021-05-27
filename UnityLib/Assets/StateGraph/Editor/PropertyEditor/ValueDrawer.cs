using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public abstract class ValueDrawer<T> : TypeDrawer<T> where T : struct
    {
        protected T Value;

        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            EditorGUI.BeginChangeCheck();
            if (content != null)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(content);
                    DoDraw(val, context);
                }
            }
            else
            {
                DoDraw(val, context);
            }
            return EditorGUI.EndChangeCheck();
        }

        public abstract void DoDraw(object val, StateGraph context);

        public override object GetValue()
        {
            return Value;
        }
    }

}