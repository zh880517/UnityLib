using UnityEditor.Compilation;

public static class MonoScriptUtils
{
    public static System.Reflection.Assembly GetScriptAssembly(string scriptPath)
    {
        string assemblyName = CompilationPipeline.GetAssemblyNameFromScriptPath(scriptPath);
        if (!string.IsNullOrEmpty(assemblyName))
        {
            //去掉文件名后缀
            assemblyName = assemblyName.Substring(0, assemblyName.Length - 4);
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return assembly;
                }
            }
        }
        return null;
    }
}
