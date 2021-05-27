using UnityEngine;

namespace PropertyEditor
{
    public abstract class CustomDrawerBase : IDrawer
    {
        public abstract bool Draw(GUIContent content, object val, StateGraph context);

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