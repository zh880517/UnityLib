using System.Reflection;
using UnityEngine;
namespace PropertyEditor
{
    public class FiledDrawer
    {
        public FieldInfo Info;
        public IDrawer Drawer;

        private readonly GUIContent Content;
        private readonly MethodInfo ValidMethod;

        public FiledDrawer(FieldInfo info)
        {
            Info = info;
            var custom = info.GetCustomAttribute<PropertyCustomDrawerAttribute>();
            if (custom != null)
                Drawer = DrawerCollector.CreateDrawer(custom.GetType());
            if (Drawer != null)
                Drawer = DrawerCollector.CreateDrawer(info.FieldType);
            var display = info.GetCustomAttribute<DisaplayNameAttribute>();
            Content = new GUIContent(display == null ? info.Name : display.Name);
            var validFunc = info.GetCustomAttribute<ValidFuncAttribute>();
            if (validFunc != null)
            {
                ValidMethod = info.DeclaringType.GetMethod(validFunc.Name);
            }
        }

        public bool Draw(object data, StateGraph context)
        {
            if (!(bool)ValidMethod.Invoke(data, null))
                return true;
            if (Drawer.Draw(Content, Info.GetValue(data), context))
            {
                DrawerCollector.OnPropertyModify(context);
                Info.SetValue(data, Drawer.GetValue());
                return true;
            }
            return false;
        }
    }

}