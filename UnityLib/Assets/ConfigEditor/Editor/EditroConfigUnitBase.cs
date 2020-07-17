using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class EditorConfigUnitRef
{
    public EditroConfigUnitBase Unit;
    public int Count;
}

public class EditroConfigUnitBase : ScriptableObject
{
    [SerializeField]
    [HideInInspector]
    private List<EditroConfigUnitBase> beRefs = new List<EditroConfigUnitBase>();
    [SerializeField]
    [HideInInspector]
    private List<EditorConfigUnitRef> refUnits = new List<EditorConfigUnitRef>();
    public List<EditroConfigUnitBase> BeRefs => beRefs;
    public List<EditorConfigUnitRef> RefUnits => refUnits;

    public void AddRef(EditroConfigUnitBase target)
    {
        EditorHelper.RegisterCompleteObjectUndo(this);
        var refObj = RefUnits.Find(obj => obj.Unit == target);
        if (refObj == null)
        {
            refObj = new EditorConfigUnitRef() { Unit = target };
            RefUnits.Add(refObj);
        }
        refObj.Count++;
        if (refObj.Count == 1)
        {
            EditorHelper.RegisterCompleteObjectUndo(target);
            target.BeRefs.Add(this);
        }
    }

    public void RemoveRef(EditroConfigUnitBase target)
    {
        var refObj = RefUnits.Find(obj => obj.Unit == target);
        if (refObj != null)
        {
            EditorHelper.RegisterCompleteObjectUndo(this);
            refObj.Count--;
            if (refObj.Count == 0)
            {
                RefUnits.Remove(refObj);
                EditorHelper.RegisterCompleteObjectUndo(target);
                target.BeRefs.Remove(this);
            }
        }
    }
}
