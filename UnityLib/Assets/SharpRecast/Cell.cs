using System.Collections.Generic;

namespace SharpRecast
{
    public class Cell
    {
        public List<Span> Spans { get; private set; } = new List<Span>();
        public int Height { get; private set; }
        public Cell(int height)
        {
            Height = height;
        }
        public void AddSpan(Span span)
        {
            if (span.Min > span.Max)
            {
                int tmp = span.Min;
                span.Min = span.Max;
                span.Max = tmp;
            }
            RecastMath.Clamp(span.Min, 0, Height);
            RecastMath.Clamp(span.Max, 0, Height);
            for (int i = 0; i < Spans.Count; ++i)
            {
                Span cur = Spans[i];
                if (cur.Min > span.Max)
                {
                    Spans.Insert(i, span);
                    return;
                }
                else if (cur.Max >= span.Min)
                {
                    if (cur.Min < span.Min)
                        span.Min = cur.Min;

                    if (cur.Max > span.Max)
                    {
                        span.Max = cur.Max;
                    }
                    Spans.RemoveAt(i);
                    i--;
                }
            }
            Spans.Add(span);
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