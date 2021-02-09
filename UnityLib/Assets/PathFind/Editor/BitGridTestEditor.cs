using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BitGridTest))]
public class BitGridTestEditor : Editor
{
    private void OnSceneGUI()
    {
        var test = target as BitGridTest;
        if (test.Grid == null)
            return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown)
        {
            Vector3 pos = GetWorldPosition(e.mousePosition);
            if (pos.x < 0 || pos.y < 0)
                return;

            Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(pos.x/test.Grid.Scale), Mathf.FloorToInt(pos.z / test.Grid.Scale));
            if (gridPos.x >= test.Grid.Width || gridPos.y >= test.Grid.Height)
                return;
            bool reBuild = false;
            if (e.button == 0)
            {
                test.StartPos = gridPos;
                reBuild = true;
            }
            else if (e.button == 1)
            {
                test.EndPos = gridPos;
                reBuild = true;
            }
            if (reBuild)
            {
                System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                test.TestPath();
                Debug.LogFormat("寻路耗时 : {0}", stopwatch.Elapsed);
            }
        }
    }

    Vector3 GetWorldPosition(Vector2 mousePos)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        Vector3 point = new Vector3(0, 0);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0.55f, 0f));

        if (groundPlane.Raycast(ray, out float dist))
        {
            point = ray.GetPoint(dist);
        }

        return point;
    }
}
