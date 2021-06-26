using UnityEngine;

namespace PropertyEditor
{
    public interface IDrawer
    {
        bool Draw(GUIContent content, object val, StateGraph context);
        object GetValue();
    }
}