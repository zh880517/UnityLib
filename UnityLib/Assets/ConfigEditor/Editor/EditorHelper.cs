using UnityEngine;

public static class EditorHelper
{
    public static void RegisterCompleteObjectUndo(Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCompleteObjectUndo(obj, "modify property");
        UnityEditor.EditorUtility.SetDirty(obj);
#endif
    }
    public static void RegisterCreatedObjectUndo(Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(obj, "modify property");
#endif
    }

    public static void DestroyObjectImmediate(Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(obj);
#endif
    }
}
