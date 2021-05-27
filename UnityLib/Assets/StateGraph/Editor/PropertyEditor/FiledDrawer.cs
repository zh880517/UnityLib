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

        public FiledDrawer(FieldInfo info, IDrawer drawer)
        {
            Info = info;
            Drawer = drawer;
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
            if (Drawer == null || ValidMethod != null && !(bool)ValidMethod.Invoke(data, null))
                return false;
            if (Drawer.Draw(Content, Info.GetValue(data), context))
            {
                DrawerCollector.OnPropertyModify(context);
                Info.SetValue(data, Drawer.GetValue());
                return true;
            }
            return false;
        }

        public static FiledDrawer Create(FieldInfo info)
        {
            if (info.IsStatic || !info.IsPublic || info.GetCustomAttribute<HideInInspector>() != null)
                return null;
            var custom = info.GetCustomAttribute<PropertyCustomDrawerAttribute>();
            IDrawer drawer = null;
            if (custom != null)
                drawer = DrawerCollector.CreateDrawer(custom.GetType(), custom);
            if (drawer == null)
                drawer = DrawerCollector.CreateDrawer(info.FieldType);
            if (drawer == null)
                return null;
            return new FiledDrawer(info, drawer);
        }
    }

}