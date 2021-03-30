using SharpRecast;
using UnityEngine;

public class RecatGridTest : MonoBehaviour
{
    public Voxelization voxel;
    public Heightfield heightfield;
    public RecastGridData gridData;
    public Transform ValidPoint;

    public float Size = 1;
    public bool ShowVoxel = true;
    public bool ShowHeightfield = false;
    public bool ShowGrid = false;


    [ContextMenu("重置")]
    public void Clear()
    {
        voxel = null;
    }

    [ContextMenu("测试")]
    public void Test()
    {
        var bounds = RecastBuilder.GetBoundsFromGameObject(gameObject);
        //bounds.Expand(2.0f);
        bounds = RecastBuilder.AlignBounds(bounds);
        bounds.Expand(new Vector3(0, 1.6f, 0));
        voxel = RecastBuilder.Creat(bounds, Size);
        RecastBuilder.AddGameObject(voxel, gameObject);
        Vector3 min = voxel.Bounds.min;

        heightfield = RecastBuilder.VoxelToHeightfield(voxel);
        var bitMask = RecastBuilder.VoxelizationWalkableFilter(voxel);
        if (ValidPoint)
        {
            var pos = Vector3Int.CeilToInt((ValidPoint.position - min) / voxel.CellSize);
            Vector2Int pt = new Vector2Int(pos.x, pos.z);
            Vector2Int size = new Vector2Int(voxel.Size.x, voxel.Size.z);
            var validMask = RecastBuilder.CalcClosePoint(bitMask, size, pt);
            gridData = new RecastGridData
            {
                OriginPoint = new Vector3(min.x, min.z),
                Width = voxel.Size.x,
                Length = voxel.Size.z,
                Mask = new SerializBitArray(validMask)
            };
        }
        else
        {
            gridData = RecastBuilder.VoxelizationToGrid(voxel);
        }
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 matrix4 = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (ShowVoxel && voxel != null)
        {
            RecastGizmos.DrawVoxel(voxel);
            Gizmos.DrawWireCube(voxel.Bounds.center, voxel.Bounds.size);
        }
        if (ShowHeightfield && heightfield != null)
        {
            RecastGizmos.DrawHeightfield(heightfield);
        }
        if (ShowGrid && gridData != null)
        {
            RecastGizmos.DrawGridData(gridData);
        }
        Gizmos.matrix = matrix4;
    }
}
