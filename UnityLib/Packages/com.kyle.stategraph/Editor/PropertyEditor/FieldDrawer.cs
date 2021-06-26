using System.Reflection;
using UnityEngine;
namespace PropertyEditor
{
    public class FieldDrawer
    {
        public FieldInfo Info;
        public IDrawer Drawer;

        private readonly GUIContent Content;
        private readonly MethodInfo ValidMethod;

        public FieldDrawer(FieldInfo info, IDrawer drawer)
        {
            Info = info;
            Drawer = drawer;
            var display = info.GetCustomAttribute<DisplayNameAttribute>();
            var toolTip = info.GetCustomAttribute<TooltipAttribute>();
            Content = new GUIContent(display == null ? info.Name : display.Name, toolTip != null ? toolTip.tooltip : "");
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

        public static FieldDrawer Create(FieldInfo info)
        {
            IDrawer drawer = DrawerCollector.CreateDrawer(info);
            if (drawer == null)
                return null;
            return new FieldDrawer(info, drawer);
        }
    }

}