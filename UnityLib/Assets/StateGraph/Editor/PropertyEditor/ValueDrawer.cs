namespace PropertyEditor
{
    public abstract class ValueDrawer<T> : TypeDrawer<T> where T : struct
    {
        protected T Value;

        public override object GetValue()
        {
            return Value;
        }
    }

}