using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public abstract class ValueDrawer<T> : TypeDrawer<T> where T : struct
    {
        protected T Value;

        public override bool Draw(GUIContent content, object val, StateGraph context)
        {
            Value = (T)val;
            EditorGUI.BeginChangeCheck();
            using (new GUILayout.HorizontalScope())
            {
                if(content != null)
                    GUILayout.Label(content);
                DoDraw(val, context);
            }
            return EditorGUI.EndChangeCheck();
        }

        public T TValue()
        {
            return Value;
        }

        protected abstract void DoDraw(object val, StateGraph context);

        public override object GetValue()
        {
            return Value;
        }
    }

}