namespace LiteECS.Editor
{
    public enum ECSSystemGenerateType
    {
        Initialize,
        Execute,
        Cleanup,
        TearDown,
        //分割
        GroupExecute,
        Reactive,
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
            else if (type == ECSSystemGenerateType.Reactive)
            {
                GenReactiveSystem(writer, className, context, componentName);
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
                writer.Write($"{context}Context context;").NewLine();
                writer.Write($"public {className}({context}Context context):base(context)");
                using (new CodeWriter.Scop(writer))
                {
                    writer.Write("this.context = context;");
                }
                writer.Write($"protected override void OnExecuteEntity({context}Entity entity, TComponent component)");
                writer.EmptyScop();
            }
        }

        public static void GenReactiveSystem(CodeWriter writer, string className, string context, string componentName)
        {
            if (string.IsNullOrEmpty(componentName))
            {
                componentName = $"I{context}Component";
            }
            writer.Write("using System.Collections.Generic;").NewLine();
            writer.Write($"public class {className} : LiteECS.ReactiveSystem<{context}Entity, {componentName}>");
            using (new CodeWriter.Scop(writer))
            {
                writer.Write($"{context}Context context;").NewLine();
                writer.Write($"public {className}({context}Context context):base(context, LiteECS.ComponentEvent.OnAddOrModify)");
                using (new CodeWriter.Scop(writer))
                {
                    writer.Write("this.context = context;");
                }

                writer.NewLine();

                writer.Write($"protected override void OnExecuteEntitis(List<{context}Entity> entities)");
                using (new CodeWriter.Scop(writer))
                {
                    writer.Write("for (int i=0; i< entities.Count; ++i)");
                    using (new CodeWriter.Scop(writer))
                    {
                        writer.Write("var entity = entities[i];");
                    }
                }
            }
        }
    }


}