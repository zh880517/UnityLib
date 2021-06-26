using UnityEngine;

namespace PropertyEditor
{
    public class VariableCompareDrawer : ValueDrawer<VariableCompare>
    {
        private ReadVariableDrawer left = new ReadVariableDrawer();
        private ReadVariableDrawer right = new ReadVariableDrawer();
        private IDrawer middle = DrawerCollector.CreateDrawer(typeof(NumberCompareType));
        protected override void DoDraw(object val, StateGraph context)
        {
            bool modify = false;
            using (new GUILayout.HorizontalScope())
            {
                modify |= left.Draw(null, Value.Left, context);
                modify |= middle.Draw(null, Value.CompareType, context);
                if (Value.CompareType < NumberCompareType.Probability)
                    modify |= right.Draw(null, Value.Right, context);
            }
            Value.Left = left.TValue();
            Value.CompareType = (NumberCompareType)middle.GetValue();
            Value.Right = right.TValue();
            if (modify)
                GUI.changed = true;
        }
    }
}