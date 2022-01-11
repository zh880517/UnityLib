using System;
using System.Collections.Generic;
using System.Reflection;

namespace LiteECS.Editor
{
    public class ComponentGenerator
    {
        private readonly List<Type> componentTypes = new List<Type>();
        private ECSContextConfig context;

        public ComponentGenerator(ECSContextConfig config)
        {
            componentTypes.AddRange(config.ComponentTypes);
            context = config;
        }

        public void Gen(GeneratorFolder folder)
        {
            folder.AddFile($"{context.Name}Components", GenComponentsFile());
            //自动System生成
            foreach (var type in componentTypes)
            {
                var cleanup = type.GetCustomAttribute<CleanupAttribute>();
                if (cleanup == null)
                    continue;
                CodeWriter writer = new CodeWriter();
                string className = $"{type.Name}CleanupSystem";
                writer.Write($"using TComponent = {type.FullName};").NewLine();
                writer.Write($"public class {className} : LiteECS.ICleanupSystem");
                using(new CodeWriter.Scop(writer))
                {
                    if (cleanup.Mode == CleanupMode.DestroyEntity)
                    {
                        GenDestroySystem(type, writer, className);
                    }
                    else
                    {
                        GenRemoveSystem(type, writer, className);
                    }
                }
                folder.AddFile(className, writer.ToString());
            }
        }

        private void GenDestroySystem(Type type, CodeWriter writer, string className)
        {
            writer.Write($"private readonly LiteECS.Group<TComponent> group;").NewLine();
            writer.Write($"public {className}({context.Name}Context context)");
            using(new CodeWriter.Scop(writer))
            {
                writer.Write($"group = context.CreatGroup<TComponent>();");
            }
            writer.Write($"public void OnCleanup()");
            using (new CodeWriter.Scop(writer)) 
            {
                writer.Write($"if (group.Count > 0)");
                using (new CodeWriter.Scop(writer))
                {
                    writer.Write($"while (group.TryGet(out LiteECS.Entity entity, out _))");
                    using (new CodeWriter.Scop(writer)) 
                    {
                        writer.Write("entity.Destroy();");
                    }
                    writer.Write("group.Reset();");
                }
            }
        }

        private void GenRemoveSystem(Type type, CodeWriter writer, string className)
        {
            writer.Write($"private readonly {className}Context context;").NewLine();
            writer.Write($"public {className}({context.Name}Context context)");
            using (new CodeWriter.Scop(writer))
            {
                writer.Write("this.context = context;");
            }
            writer.Write($"public void OnCleanup()");
            using (new CodeWriter.Scop(writer))
            {
                writer.Write($"context.RemoveAll<TComponent>();");
            }
        }

        private string GenComponentsFile()
        {
            CodeWriter writer = new CodeWriter();
            writer.Write($"public static partial class {context.Name}Components");
            using(new CodeWriter.Scop(writer))
            {
                writer.Write($"static {context.Name}Components()");
                using (new CodeWriter.Scop(writer))
                {
                    writer.Write($"OnContextCreat = DoContentInit;").NewLine();
                    writer.Write($"ComponentCount = {componentTypes.Count};").NewLine();
                    writer.Write("InitComponentsIdentity();");
                }
                writer.Write($"static void InitComponentsIdentity()");
                using(new CodeWriter.Scop(writer))
                {
                    for (int i=0; i<componentTypes.Count; ++i)
                    {
                        var type = componentTypes[i];
                        if (typeof(IUnique).IsAssignableFrom(type))
                        {
                            writer.Write($"LiteECS.ComponentIdentity<{type.FullName}>.Unique = true;").NewLine();
                        }
                        writer.Write($"LiteECS.ComponentIdentity<{type.FullName}>.Id = {i};");
                        if (i < componentTypes.Count - 1)
                        {
                            writer.NewLine();
                        }
                    }
                }
                writer.Write($"static void DoContentInit({context.Name}Context context)");
                using (new CodeWriter.Scop(writer))
                {
                    for (int i = 0; i < componentTypes.Count; ++i)
                    {
                        var type = componentTypes[i];
                    
                        if (typeof(LiteECS.IUnique).IsAssignableFrom(type))
                        {
                            writer.Write($"context.InitComponentCollector<{type.FullName}>();");
                        }
                        else
                        {
                            writer.Write($"context.InitUniqueComponentCollector<{type.FullName}>();");
                        }
                        if (i < componentTypes.Count - 1)
                        {
                            writer.NewLine();
                        }
                    }
                }
            }
            return writer.ToString();
        }



    }


}
