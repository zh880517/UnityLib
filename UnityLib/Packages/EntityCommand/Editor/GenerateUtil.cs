using CodeGenerator;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace EntityCommand.Editor
{
    public static class GenerateUtil
    {
        public static string CommandShortName(Type type)
        {
            string name = type.Name;
            if (name.EndsWith("Command"))
                name = name.Substring(0, name.Length - 7);
            return name;
        }

        private static string ToParamList(Type type)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (fields.Length == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];
                sb.Append(", ");
                sb.Append($"{TypeUtil.TypeToName(field.FieldType)} new{field.Name}");
            }
            return sb.ToString();
        }
        public static void GenerateContextExtern(Type type, CSharpWriter writer)
        {
            string shortName = CommandShortName(type);
            writer.Write($"Add{shortName}(this EntityCommand.ICommandContext context, long id{ToParamList(type)})");
            using (new CSharpWriter.Scop(writer))
            {
                writer.Write($"if(context.IsValid(id)");
                using (new CSharpWriter.Scop(writer))
                {
                    writer.Write($"var cmd = context.NewCommand<{TypeUtil.TypeToName(type)}>();");
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        writer.Write($"cmd.{field.Name} = new{field.Name};").NewLine();
                    }
                    writer.Write($"context.AddCommand(id, cmd)");
                }
            }
        }

        public static void GenerateExterns(IEnumerable<Type> types, string className, string filePath)
        {
            CSharpWriter writer = new CSharpWriter();
            writer.Write($"public static {className}");
            using (new CSharpWriter.Scop(writer))
            {
                foreach (var type in types)
                {
                    GenerateContextExtern(type, writer);
                }
            }
            FileUtil.WriteFileWithCreateFolder(filePath, writer.ToString());
        }

    }
}