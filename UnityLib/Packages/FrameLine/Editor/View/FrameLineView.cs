using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace FrameLine
{
    public enum FrameClipHitPartType
    {
        Normal,
        LeftCtrl,
        RightCtrl,
    }
    public struct FrameClipHitResult
    {
        public Vector2 ClickPos;
        public FrameClipRef Clip;
        public FrameClipHitPartType HitPart;
        public int Frame;
    }
    public partial class FrameLineView : ScriptableObject, ISerializationCallbackReceiver
    {

        public FrameLineAsset Asset;
        public EditorWindow Window;
        public List<FrameClipRef> SelectedClips = new List<FrameClipRef>();
        public int CurrentFrame;
        public Vector2 ScrollPos;
        public int FrameCount => Asset.FrameCount;

        //滚动区域看见信息
        public int VisableFrameStart { get; private set; }
        public int VisableFrameEnd { get; private set; }
        public int VisableTrackStart { get; private set; }
        public int VisableTrackEnd { get; private set; }

        public bool IsSlecected(FrameClipRef clipRef)
        {
            return SelectedClips.Contains(clipRef);
        }

        public FrameClipHitResult HitTest(Vector2 point)
        {
            int hitFrame = FrameUtil.PosToFrame(point.x);
            FrameClipHitResult result = new FrameClipHitResult() { Frame = hitFrame, ClickPos = point };

            if (hitFrame < FrameCount && hitFrame >= 0)
            {
                int hitTrackIndex = Mathf.FloorToInt(point.y / ViewStyles.TrackHeight);
                int preTrackCount = 0;
                for (int i=0; i<Asset.Tracks.Count; ++i)
                {
                    var track = Asset.Tracks[i];
                    int hitSubIndex = hitTrackIndex - preTrackCount;
                    if (hitSubIndex < 0)
                        break;
                    int subTrakCount = track.Foldout ? track.Count : 1;
                    preTrackCount += subTrakCount;
                    if (hitSubIndex >= subTrakCount)
                        continue;
                    foreach (var clipRef in track.Clips)
                    {
                        var clip = clipRef.Clip;
                        if (clip.StartFrame >= hitFrame)
                        {
                            int endFrame = clip.StartFrame + clip.Length - 1;
                            if (clip.Length <= 0 || endFrame >= FrameCount)
                                endFrame = FrameCount - 1;

                            if (endFrame >= hitFrame)
                            {
                                float frameOffset = point.x % ViewStyles.FrameWidth;
                                if (frameOffset <= ViewStyles.ClipCtrlWidth && clip.StartFrame == hitFrame)
                                {
                                    result.HitPart = FrameClipHitPartType.LeftCtrl;
                                }
                                else if (endFrame == hitFrame && (frameOffset >= (ViewStyles.FrameWidth - ViewStyles.ClipCtrlWidth)))
                                {
                                    result.HitPart = FrameClipHitPartType.RightCtrl;
                                }
                                break;
                            }
                        }
                    }
                    if (result.Clip)
                        break;
                }
            }
            return result;
        }

        protected virtual void DrawToolBar()
        {
        }

        public virtual void RegistUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(Asset, name);
            Undo.RegisterCompleteObjectUndo(this, name);
            Undo.RegisterCompleteObjectUndo(Window, name);
            EditorUtility.SetDirty(Asset);
        }

        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
            //反序列化处理
            for (int i = 0; i < SelectedClips.Count; ++i)
            {
                var clipRef = SelectedClips[i];
                clipRef.Clip = Asset.Find(clipRef.ID);
            }
        }
    }
}