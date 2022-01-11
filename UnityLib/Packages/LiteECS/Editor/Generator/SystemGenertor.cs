namespace LiteECS.Editor
{
    public enum ECSSystemGenerateType
    {
        Initialize,
        Execute,
        Cleanup,
        TearDown,
        GroupExecute,
        ReactiveExecute,
    }

    public static class SystemGenertor
    {
        public static string Gen(string className, string context, ECSSystemGenerateType type, string componentName = null)
        {
            CodeWriter writer = new CodeWriter(true);
            if (type < ECSSystemGenerateType.GroupExecute)
            {
                writer.Write($"public class {className} : LiteECS.I{type}System");
                using (new CodeWriter.Scop(writer))
                {
                    writer.Write($"{context}Context context;").NewLine();
                    writer.Write($"public {className}({context}Context context)");
                    using (new CodeWriter.Scop(writer))
                    {
                        writer.Write("this.context = context;");
                    }
                    writer.Write($"public void On{type}()");
                    writer.EmptyScop();
                }
            }
            else if (type == ECSSystemGenerateType.GroupExecute)
            {
                GenGroupExecuteSystem(writer, className, context, componentName);
            }
            else if (type == ECSSystemGenerateType.ReactiveExecute)
            {
                GenReactiveExecuteSystem(writer, className, context, componentName);
            }
            return writer.ToString();
        }

        public static void GenGroupExecuteSystem(CodeWriter writer, string className, string context, string componentName)
        {
            if (string.IsNullOrEmpty(componentName))
            {
                componentName = $"I{context}Component";
            }
            writer.Write($"using TComponent = {componentName};").NewLine();
            writer.Write($"public class {className} : LiteECS.GroupExecuteSystem<{context}Entity, TComponent>");
            using (new CodeWriter.Scop(writer))
            {
                writer.Write($" private {context}Context Context => context as {context}Context;").NewLine();
                writer.Write($"public {className}({context}Context context):base(context)");
                writer.EmptyScop();
                writer.Write($"protected override void OnExecuteEntity({context}Entity entity, TComponent component)");
                writer.EmptyScop();
            }
        }

        public static void GenReactiveExecuteSystem(CodeWriter writer, string className, string context, string componentName)
        {
            if (string.IsNullOrEmpty(componentName))
            {
                componentName = $"I{context}Component";
            }
            writer.Write($"using TComponent = {componentName};").NewLine();
            writer.Write($"public class {className} : LiteECS.ReactiveExecuteSystem<{context}Entity, TComponent>");
            using (new CodeWriter.Scop(writer))
            {
                writer.Write($" private {context}Context Context => context as {context}Context;").NewLine();
                writer.Write($"public {className}({context}Context context):base(context)");
                writer.EmptyScop();
                writer.Write($"protected override void OnExecuteEntity({context}Entity entity, TComponent component)");
                writer.EmptyScop();
            }
        }

    }
}