using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace SharpRecast
{
    public static class RecastBuilder
    {
        private static readonly Vector3[] BoxVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
        };

        private static readonly int[] BoxTriangles = new int[]
        {
            0, 1, 2,
            1, 2, 3,

            4, 5, 6,
            5, 6, 7,

            0, 3, 5,
            3, 5, 4,

            3, 2, 6,
            2, 6, 5,

            1, 2, 7,
            1, 7, 6,

            1, 0, 4,
            0, 4, 7,
        };

        public static Bounds GetBoundsFromGameObject(GameObject gameObject)
        {
            Bounds bounds = new Bounds();
            var renderers = gameObject.GetComponentsInChildren<Renderer>(false);
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }

        public static Bounds AlignBounds(Bounds b)
        {
            Vector3 min = b.min;
            Vector3 max = b.max;
            b.min = Vec3Floor(min);
            b.max = Vec3Ceil(max);
            return b;
        }

        public static Vector3 Vec3Floor(Vector3 v)
        {
            return new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
        }

        public static Vector3 Vec3Ceil(Vector3 v)
        {
            return new Vector3(Mathf.Ceil(v.x), Mathf.Ceil(v.y), Mathf.Ceil(v.z));
        }

        public static Voxelization Creat(Bounds b, float size)
        {
            return new Voxelization(b, size);
        }

        public static void AddMesh(Voxelization voxel, Vector3[] vertices, int[] triangles, Matrix4x4 matrix)
        {
            int count = triangles.Length / 3;
            for (int i=0; i<count; i++)
            {
                Vector3 v1 = vertices[triangles[i*3]];
                Vector3 v2 = vertices[triangles[i*3 + 1]];
                Vector3 v3 = vertices[triangles[i*3 + 2]];
                voxel.VoxelTriangle(matrix.MultiplyPoint(v1), matrix.MultiplyPoint(v2), matrix.MultiplyPoint(v3));
            }
        }

        public static void AddBox(Voxelization voxel, Vector3 center, Vector3 size, Matrix4x4 matrix)
        {
            matrix = Matrix4x4.TRS(center, Quaternion.identity, size * 0.5f) * matrix;
            AddMesh(voxel, BoxVertices, BoxTriangles, matrix);
        }

        public static void AddMeshByRender(Voxelization voxel, MeshRenderer renderer)
        {
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
                return;
            if (!renderer.bounds.Intersects(voxel.Bounds))
                return;
            var mesh = meshFilter.sharedMesh;
            AddMesh(voxel, mesh.vertices, mesh.triangles, meshFilter.transform.localToWorldMatrix);
        }

        public static void AddGameObject(Voxelization voxel, GameObject gameObject, int layerMask = -1)
        {
            var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var render in renderers)
            {
                if (( (1<<render.gameObject.layer) & layerMask) != 0)
                {
                    AddMeshByRender(voxel, render);
                }
            }
            var boxCollider = gameObject.GetComponentsInChildren<BoxCollider>();
            foreach (var box in boxCollider)
            {
                if (box.isTrigger || ((1 << box.gameObject.layer) & layerMask) != 0)
                    continue;
                AddBox(voxel, box.center, box.size, box.transform.localToWorldMatrix);
            }
        }

        public static Heightfield VoxelToHeightfield(Voxelization voxel)
        {
            Heightfield heightfield = new Heightfield(voxel.Bounds.min, voxel.Size.x, voxel.Size.z, voxel.CellSize);
            Cell cell = new Cell(voxel.Size.y);
            for (int x=0; x<voxel.Size.x; ++x)
            {
                for (int z = 0; z < voxel.Size.z; ++z)
                {
                    int min = -1;
                    int count = 0;
                    for (int y = 0; y < voxel.Size.y; ++y)
                    {
                        bool mask = voxel.Voxel.Get(x, y, z);
                        if (mask)
                        {
                            count++;
                            if (min == -1)
                            {
                                min = y;
                                count = 1;
                            }
                        }
                        else
                        {
                            if (min != -1)
                            {
                                cell.Spans.Add(new Cell.Span { Min = min, Max = min + count });
                                min = -1;
                                count = 0;
                            }
                        }
                    }
                    if (cell.Spans.Count > 0)
                    {
                        int cellIndex = z * voxel.Size.x + x;
                        heightfield.Cells[cellIndex] = cell;
                        cell = new Cell(voxel.Size.y);
                    }
                }
            }
            return heightfield;
        }

        public static BitArray VoxelizationWalkableFilter(Voxelization voxel)
        {
            BitArray bitArray = new BitArray(voxel.Size.x * voxel.Size.z);
            int bgHeight = (int)voxel.Bounds.min.y;
            if (bgHeight < 0)
                bgHeight = -bgHeight;
            for (int x = 0; x < voxel.Size.x; ++x)
            {
                for (int z = 0; z < voxel.Size.x; ++z)
                {
                    if (voxel.Check(x, bgHeight, z) && !voxel.Check(x, bgHeight+1, z))
                    {
                        bitArray.Set(z * voxel.Size.x + x, true);
                    }
                }
            }
            return bitArray;
        }

        private static readonly Vector2Int[] dirs = new Vector2Int[]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        };

        public static BitArray CalcClosePoint(BitArray source, Vector2Int size, Vector2Int validPos)
        {
            if (!source.Get(validPos.y * size.x + validPos.x))
                return null;
            BitArray dest = new BitArray(source.Length);
            Queue<Vector2Int> points = new Queue<Vector2Int>(size.x*size.y);
            points.Enqueue(validPos);
            while (points.Count > 0)
            {
                Vector2Int curr = points.Dequeue();
                for (int i=0; i< dirs.Length; ++i)
                {
                    var pt = curr + dirs[i];
                    if (pt.x >= 0 && pt.x < size.x && pt.y >= 0 && pt.y < size.y)
                    {
                        int idx = pt.y * size.x + pt.x;
                        if (source.Get(idx) && !dest.Get(idx))
                        {
                            dest.Set(idx, true);
                            points.Enqueue(pt);
                        }
                    }
                }
            }
            return dest;
        }

        public static RecastGridData VoxelizationToGrid(Voxelization voxel)
        {
            Vector3 min = voxel.Bounds.min;
            RecastGridData gridData = new RecastGridData
            {
                OriginPoint = new Vector3(min.x, min.z),
                Width = voxel.Size.x,
                Length = voxel.Size.z,
                Mask = new SerializBitArray(voxel.Size.x * voxel.Size.z)
            };
            int bgHeight = (int)voxel.Bounds.min.y;
            if (bgHeight < 0)
                bgHeight = -bgHeight;
            for (int x = 0; x < voxel.Size.x; ++x)
            {
                for (int z = 0; z < voxel.Size.x; ++z)
                {
                    if (voxel.Check(x, bgHeight, z) && !voxel.Check(x, bgHeight + 1, z))
                    {
                        gridData.Mask.Set(z * voxel.Size.x + x, true);
                    }
                }
            }
            return gridData;
        }
    }

}
