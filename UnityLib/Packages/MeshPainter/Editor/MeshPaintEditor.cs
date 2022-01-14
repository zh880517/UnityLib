using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MeshPainter
{
    [System.Serializable]
    public class BrushData
    {
        public int Size = 16;
        public float Stronger = 0.5f;
        public int OrthographicSize = 2;
    }

    public class MeshPaintEditor : EditorWindow
    {
        public MeshRenderer Selected;
        public MeshCollider EditorCollider;
        public Material EditorMaterial;
        public MeshPainterShader CurrentShader;
        public bool IsEditing;
        public int BrushSelect;
        public int ChannelSelect;
        public List<MeshRenderer> EditorRecoder = new List<MeshRenderer>();

        public BrushData Brush = new BrushData();
        private Texture2D[] brushTextures;
        public Texture2D[] ChannelTexture = new Texture2D[4];


        private void OnSelectionChange()
        {
        
            if (Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject) )
            {
                if (Selection.activeGameObject.GetComponent<MeshFilter>())
                {
                    var select = Selection.activeGameObject.GetComponent<MeshRenderer>();
                    SetTarget(select);
                }
                else
                {
                    SetTarget(null);
                }
            }
            Repaint();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            OnSelectionChange();
        }
        private void OnFocus()
        {
            if (EditorMaterial)
            {
                CurrentShader = MeshPainterConfig.Instance.FindShader(EditorMaterial.shader);
            }
            UpdateBrush();
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;

            if (EditorCollider)
            {
                DestroyImmediate(EditorCollider.gameObject);
            }
        }

        private void SetTarget(MeshRenderer target)
        {
            if (Selected != target)
            {
                Selected = target;
                IsEditing = false;
                ChannelSelect = 0;
                EditorMaterial = Selected ? Selected.sharedMaterial : null;
            }
            if (!Selected)
            {
                EditorMaterial = null;
            }
            if (EditorMaterial)
            {
                CurrentShader = MeshPainterConfig.Instance.FindShader(EditorMaterial.shader);
            }
            if (!Selected && EditorCollider)
            {
                EditorMaterial = null;
            }
        }

        private bool CheckCollider()
        {
            if (Selected == null || CurrentShader == null)
                return false;
            if (EditorCollider == null)
            {
                var go = new GameObject("编辑器用，勿动");
                go.hideFlags = HideFlags.DontSave;
                EditorCollider = go.AddComponent<MeshCollider>();
            }
            EditorCollider.transform.position = Selected.transform.position;
            EditorCollider.transform.localScale = Selected.transform.lossyScale;
            EditorCollider.transform.rotation = Selected.transform.rotation;
            var meshFilter = Selected.GetComponent<MeshFilter>();
            EditorCollider.sharedMesh = meshFilter?.sharedMesh;
            EditorCollider.isTrigger = false;
            return true;
        }

        private void UpdateBrush()
        {
            do
            {
                if (brushTextures == null)
                    break;
                if (brushTextures.Length != MeshPainterConfig.Instance.Brushs.Count)
                    break;
                return;
            } while (false);
            brushTextures = MeshPainterConfig.Instance.Brushs.Where(it => it).ToArray();
        }

        public Vector2 scorllPos;
        void OnGUI()
        {
            using(var scroll = new GUILayout.ScrollViewScope(scorllPos, false, true))
            {
                DrawBrush();
                DrawEditorInfo();
                scorllPos = scroll.scrollPosition;
            }
        }

        private void DrawBrush()
        {
            using(new GUILayout.VerticalScope("Box"))
            {
                GUILayout.Label("画刷设置", EditorStyles.boldLabel);
                Brush.Size = EditorGUILayout.IntSlider("尺寸", Brush.Size, 1, 50);
                Brush.Stronger = EditorGUILayout.Slider("强度", Brush.Stronger, 0, 1);
                Brush.OrthographicSize = EditorGUILayout.IntSlider("正交尺寸", Brush.OrthographicSize, 2, 10);
                UpdateBrush();
                if (brushTextures.Length == 0)
                {
                    GUILayout.Label("无有效画刷，打开Windosw/MeshPainter，配置画刷");
                    return;
                }
                float width = position.width;
                int row = Mathf.FloorToInt(width / 40);
                int line = Mathf.CeilToInt(brushTextures.Length / (float)row);
                BrushSelect = GUILayout.SelectionGrid(BrushSelect, brushTextures, row, "gridlist", GUILayout.Height(line * 40), GUILayout.Width(40 * row));

            }
        }

        private void DrawEditorButton()
        {
            EditorGUI.BeginChangeCheck();
            var color = GUI.backgroundColor;
            if (IsEditing)
                GUI.backgroundColor = Color.green;
            IsEditing = GUILayout.Toggle(IsEditing, EditorGUIUtility.IconContent("EditCollider"), GUI.skin.GetStyle("Button"), GUILayout.Width(35), GUILayout.Height(25));
            if (IsEditing)
                GUILayout.Label("Drawing");
            GUI.backgroundColor = color;
            if (EditorGUI.EndChangeCheck() && IsEditing)
            {
                Tools.current = Tool.None;
            }
        }

        void DrawEditorInfo()
        {
            if (Selected == null)
            {
                GUILayout.Label("选择需要编辑器的物件，需要MeshRenderer");
                return;
            }
            if (EditorMaterial == null)
            {
                GUILayout.Label("选择物件无有效的材质球，无法编辑");
                return;
            }
            EditorGUI.BeginChangeCheck();
            var select = EditorGUILayout.ObjectField(Selected, typeof(MeshRenderer), true) as MeshRenderer;
            if (EditorGUI.EndChangeCheck())
            {
                SetTarget(select);
            }
            using (new GUILayout.HorizontalScope())
            {
                if (CurrentShader != null)
                {
                    DrawEditorButton();
                }
                else
                {
                    GUILayout.Label("shader不支持，无法编辑");
                    return;
                }
            }
            int channleCount = System.Math.Min(4, CurrentShader.ChannleName.Count);
            for (int i=0; i< channleCount; ++ i)
            {
                ChannelTexture[i] = EditorMaterial.GetTexture(CurrentShader.ChannleName[i]) as Texture2D;
            }
            float width = position.width;
            int row = Mathf.FloorToInt(width / 90);
            int line = Mathf.CeilToInt(channleCount / (float)row);

            ChannelSelect = GUILayout.SelectionGrid(ChannelSelect, ChannelTexture, row, "gridlist", GUILayout.Height(line*100), GUILayout.Width(90*row));

            GUILayout.Label("控制贴图需要设置为可读写");
            GUILayout.Label("控制贴图的格式需要设置为非压缩");
            GUILayout.Label("控制贴图", EditorStyles.boldLabel);
            Texture2D controlTexture = EditorMaterial.GetTexture(CurrentShader.ControlName) as Texture2D;
            if (controlTexture)
            {
                GUILayout.Box(controlTexture);
            }
            else
            {
                GUILayout.Label("无控制贴图");
            }

            DrawCreateControlTexture();
        }

        private int creatTextureSize = 128;
        private Color creatTextureColor = new Color(1, 0, 0, 0);
        static readonly int[] SelectSize = new int[] { 64, 128, 256, 512, 1024 };
        static readonly string[] SelectSizeShow = new string[] { "64", "128", "256", "512", "1024" };
        private void DrawCreateControlTexture()
        {
            GUILayout.Label("创建控制贴图", EditorStyles.boldLabel);
            using(new GUILayout.HorizontalScope())
            {
                creatTextureSize = EditorGUILayout.IntPopup(creatTextureSize, SelectSizeShow,SelectSize);
                creatTextureColor = EditorGUILayout.ColorField(creatTextureColor);
                if (GUILayout.Button("创建"))
                {
                    string filePath = EditorUtility.SaveFilePanel("保存为", "Assets", "channelControl.png", "png");
                    Texture2D texture = new Texture2D(creatTextureSize, creatTextureSize, TextureFormat.RGBA32, false);
                    for (int i=0; i<creatTextureSize; ++i)
                    {
                        for (int j = 0; j < creatTextureSize; ++j)
                        {
                            texture.SetPixel(j, i, creatTextureColor);
                        }
                    }
                    System.IO.File.WriteAllBytes(filePath, texture.EncodeToPNG());
                    AssetDatabase.Refresh();
                }
            }

        }


        private bool isPainting;
        void OnSceneGUI(SceneView view)
        {
            if (CurrentShader != null)
            {
                Handles.BeginGUI();
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        DrawEditorButton();
                        GUILayout.Space(20);
                    }
                    GUILayout.Space(30);
                }
                Handles.EndGUI();
            }
            if (Tools.current != Tool.None)
                return;
            if (!IsEditing || !CheckCollider())
                return;
            Event e = Event.current;
            HandleUtility.AddDefaultControl(0);
            Ray terrain = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (!EditorCollider.Raycast(terrain, out var hit, 1000))
                return;

            float orthographicSize = (Brush.Size * Selected.transform.localScale.x) * (EditorCollider.sharedMesh.bounds.size.x / (Brush.OrthographicSize * 100));//The orthogonal size of the brush on the model
            MeshPaintUtil.DrawPoint(hit.point, hit.normal, orthographicSize);
            if (e.alt == false && e.control == false && e.shift == false && e.button == 0)
            {
                if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown)
                {
                    Texture2D brush = brushTextures[BrushSelect];
                    if (!brush)
                        return;
                    Texture2D controlTexture = EditorMaterial.GetTexture(CurrentShader.ControlName) as Texture2D;
                    if (!controlTexture)
                        return;
                    isPainting = true;
                    MeshPaintUtil.PaintMaskTexture(controlTexture, hit.textureCoord, 
                        ChannelSelect, 
                        brush, Brush.Size, Brush.OrthographicSize, Brush.Stronger);
                    Repaint();
                }
            }
            if (e.type == EventType.MouseUp && isPainting)
            {
                isPainting = false;
                Texture2D controlTexture = EditorMaterial.GetTexture(CurrentShader.ControlName) as Texture2D;
                if (!controlTexture)
                    return;
                MeshPaintUtil.SaveTexture(controlTexture);
            }

        }
    }

}

