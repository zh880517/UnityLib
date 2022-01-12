using System.Text;

namespace CodeGenerator
{
    public class CSharpWriter
    {
        private int tabCount = 0;

        private string lineBreak = "\n";

        public StringBuilder Stream { get; } = new StringBuilder();

        public struct Scop : System.IDisposable
        {
            private readonly CSharpWriter writer;
            public Scop(CSharpWriter writer)
            {
                writer.BeginScope();
                this.writer = writer;
            }

            public void Dispose()
            {
                writer.EndScope();
            }
        }

        /// <param name="winBreakLine">换行符选择，如果生成的文件需要编辑，则选择true，vs里面新加行换行符是Windows默认换行符</param>
        public CSharpWriter(bool winBreakLine = false)
        {
            if (winBreakLine)
                lineBreak = "\r\n";
        }

        public CSharpWriter BeginScope()
        {
            NewLine();
            Stream.Append('{');
            tabCount += 1;
            NewLine();
            return this;
        }

        public CSharpWriter EndScope()
        {
            tabCount -= 1;
            NewLine();
            Stream.Append('}');
            NewLine();
            return this;
        }

        public CSharpWriter EmptyScop(bool emptyLine = true)
        {
            NewLine();
            Stream.Append('{');
            NewLine();
            Stream.Append('}');
            NewLine();
            if (emptyLine)
                NewLine();
            return this;
        }

        public CSharpWriter EmptyLine()
        {
            Stream.Append(lineBreak);
            return this;
        }

        public CSharpWriter NewLine()
        {
            Stream.Append(lineBreak).Append(' ', tabCount*4);
            return this;
        }

        public CSharpWriter Write(string val)
        {
            Stream.Append(val);
            return this;
        }

        public override string ToString()
        {
            return Stream.ToString();
        }
    }

}
