using PropertyEditor;
using System.Collections.Generic;

public class StateGraphPropertyEditor
{
    private StateGraph Graph;
    
    public List<FieldDrawer> Fields = new List<FieldDrawer>();

    public StateGraphPropertyEditor(StateGraph graph)
    {
        Graph = graph;
        var type = graph.GetType();
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var fieldDrawer = FieldDrawer.Create(field);
            if (fieldDrawer != null)
            {
                if (field.DeclaringType != type)
                {
                    Fields.Insert(0, fieldDrawer);
                }
                else
                {
                    Fields.Add(fieldDrawer);
                }
            }
        }
    }

    public void Draw()
    {
        foreach (var field in Fields)
        {
            field.Draw(Graph, Graph);
        }
    }
}
