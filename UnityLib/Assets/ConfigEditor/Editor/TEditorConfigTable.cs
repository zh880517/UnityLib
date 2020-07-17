using System.Collections.Generic;

public class TEditorConfigTable<T> : EditorConfigTableBase where T : EditroConfigUnitBase
{
    public List<T> Units = new List<T>();
}
