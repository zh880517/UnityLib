namespace ViewECS
{
    public static class ComponentClear
    {
        public static void Clear<T>(T component) where T : IViewComponent
        {
            if (component is IClearable clear)
            {
                clear.Clear();
            }
        }
    }
}
