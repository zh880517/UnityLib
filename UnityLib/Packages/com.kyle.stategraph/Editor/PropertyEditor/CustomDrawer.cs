using UnityEditor;
using UnityEngine;

namespace PropertyEditor
{
    public abstract class CustomDrawerBase : IDrawer
    {

        public bool Draw(GUIContent content, object val, StateGraph context)
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
        protected abstract void DoDraw(object val, StateGraph context);

        public abstract object GetValue();

        public abstract void SetAttribute(PropertyCustomDrawerAttribute attribute);
    }

    public abstract class CustomDrawer<T> : CustomDrawerBase where T : PropertyCustomDrawerAttribute
    {
        protected T Attrbute;
        public override void SetAttribute(PropertyCustomDrawerAttribute attribute)
        {
            Attrbute = attribute as T;
        }
    }
}