using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class StateScriptCreateTemplate
{
    public static string GetSelectedPathOrFallBack()
    {
        string path = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    class CreateStateScriptAction : EndNameEditAction
    {
        public System.Func<string, string> GenFileContent;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string fileNameWithOutExtension = Path.GetFileNameWithoutExtension(pathName);
            string content = GenFileContent(fileNameWithOutExtension);
            /*
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append("#if UNITY_EDITOR").AppendLine();
            stringBuilder.Append($"[TypeIdentify(\"{System.Guid.NewGuid()}\")]").AppendLine();
            //stringBuilder.Append("#endif").AppendLine();
            stringBuilder.Append($"public class {fileNameWithOutExtension} : IStateNode").AppendLine();
            stringBuilder.Append("{").AppendLine();
            stringBuilder.Append("}").AppendLine();
            stringBuilder.ToString();
            */
            File.WriteAllText(pathName, content, Encoding.UTF8);
            AssetDatabase.ImportAsset(pathName);
            var o = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            ProjectWindowUtil.ShowCreatedAsset(o);
            AssetDatabase.OpenAsset(o);
        }

    }

    public static string GenClassCode(string className, string nameSpace, string baseType)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"namespace {nameSpace}").AppendLine();
        stringBuilder.Append("{").AppendLine();
        stringBuilder.Append($"    [TypeIdentify(\"{System.Guid.NewGuid()}\")]").AppendLine();
        stringBuilder.Append($"    public class {className} : {baseType}").AppendLine();
        stringBuilder.Append("    {").AppendLine();
        stringBuilder.Append("    }").AppendLine();
        stringBuilder.Append("}").AppendLine();
        return stringBuilder.ToString();
    }

    public static void CreateStateGraphTypeClass(string nameSpace, string baseType)
    {
        CreateStateGraphTypeClass((className) => GenClassCode(className, nameSpace, baseType));
    }

    public static void CreateStateGraphTypeClass(System.Func<string, string> genContentFunc)
    {
        var action = ScriptableObject.CreateInstance<CreateStateScriptAction>();
        action.GenFileContent = genContentFunc;

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            action,
            GetSelectedPathOrFallBack() + "/NewStateNode.cs",
            EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
            ""
            );
    }
}
