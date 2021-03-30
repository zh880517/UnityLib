using System.Collections.Generic;
using UnityEngine;

namespace SharpRecast
{
    public class Cell
    {
        public struct Span
        {
            public int Min;
            public int Max;
        }

        public List<Span> Spans { get; private set; } = new List<Span>();
        public int Height { get; private set; }
        public Cell(int height)
        {
            Height = height;
        }

        public void Combin(int walkableHeight)
        {
            for (int i = 1; i < Spans.Count; ++i)
            {
                Span pre = Spans[i - 1];
                Span cur = Spans[i];
                if (cur.Min - pre.Max < walkableHeight)
                {
                    pre.Max = cur.Max;
                    Spans[i - 1] = pre;
                    Spans.RemoveAt(i);
                    --i;
                }
            }
        }
    }
}