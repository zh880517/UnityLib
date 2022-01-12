
using CodeGenerator;

namespace LiteECS.Editor
{
    public static class ContextGenertor
    {

        public static string Gen(string name, string upLevelContext)
        {
            CSharpWriter writer = new CSharpWriter(true);
            writer.Write($"public interface I{name}Component : LiteECS.IComponent");
            writer.EmptyScop();

            writer.Write($"public class {name}Entity : LiteECS.EntityT<I{name}Component>");
            using (new CSharpWriter.Scop(writer))
            {
                writer.Write($"public {name}Entity(LiteECS.Context context, int id) : base(context, id)");
                writer.EmptyScop(false);
            }

            writer.Write($"public static partial class {name}Components");
            using (new CSharpWriter.Scop(writer))
            {
                writer.Write($"public static System.Action<{name}Context> OnContextCreat;").NewLine();
                writer.Write($"public static int ComponentCount {{ get; private set; }}").NewLine();
            }

            writer.Write($"public class {name}Context : LiteECS.ContextT<{name}Entity>");
            using (new CSharpWriter.Scop(writer))
            {
                if (!string.IsNullOrEmpty(upLevelContext))
                {
                    writer.Write($"public {upLevelContext}Context {upLevelContext}{{ get; private set; }}").NewLine();
                }
                //Ctor
                writer.Write($"protected {name}Context(int componentTypeCount) : base(componentTypeCount, CreatFunc)");
                writer.EmptyScop();

                //CreatFunc
                writer.Write($"private static {name}Entity CreatFunc(LiteECS.Context context, int id)");
                using (new CSharpWriter.Scop(writer))
                {
                    writer.Write($"return new {name}Entity(context, id);");
                }

                //Creat
                writer.Write($"public static {name}Context Creat()");
                using (new CSharpWriter.Scop(writer))
                {
                    writer.Write($"var contxt = new {name}Context({name}Components.ComponentCount);").NewLine();
                    writer.Write($"{name}Components.OnContextCreat(contxt);").NewLine();
                    writer.Write("return contxt;");
                }
                if (!string.IsNullOrEmpty(upLevelContext))
                {
                    writer.Write($"public void Set{upLevelContext}( {upLevelContext}Context {upLevelContext.ToLower()} )");
                    using (new CSharpWriter.Scop(writer))
                    {
                        writer.Write($"{upLevelContext} = {upLevelContext.ToLower()};");
                    }
                }
            }


            return writer.ToString();
        }
    }
}