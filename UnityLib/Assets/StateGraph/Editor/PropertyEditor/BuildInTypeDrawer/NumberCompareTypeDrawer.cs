using UnityEditor;

namespace PropertyEditor
{
    public class NumberCompareTypeDrawer : ValueDrawer<NumberCompareType>
    {
        private static string[] symbols = new string[] { "<", "<=", "=", ">=", ">"};
        public override void DoDraw(object val, StateGraph context)
        {
            Value = (NumberCompareType)val;
            Value = (NumberCompareType)EditorGUILayout.Popup((int)Value, symbols);
        }
    }


}