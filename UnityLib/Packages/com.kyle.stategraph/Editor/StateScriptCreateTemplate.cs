using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string fileNameWithOutExtension = Path.GetFileNameWithoutExtension(pathName);
            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append("#if UNITY_EDITOR").AppendLine();
            stringBuilder.Append($"[TypeIdentify(\"{System.Guid.NewGuid()}\")]").AppendLine();
            //stringBuilder.Append("#endif").AppendLine();
            stringBuilder.Append($"public class {fileNameWithOutExtension} : IStateNode").AppendLine();
            stringBuilder.Append("{").AppendLine();
            stringBuilder.Append("}").AppendLine();
            File.WriteAllText(pathName, stringBuilder.ToString(), Encoding.UTF8);
            AssetDatabase.ImportAsset(pathName);
            var o = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

    }
    [MenuItem("Assets/Create/StateScript", false, 81)]
    static void Create()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<CreateStateScriptAction>(),
            GetSelectedPathOrFallBack() + "/NewStateNode.cs",
            EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
            ""//"Assets/StateGraph/Editor/Common/Template/NewStateNode.txt"
            );
    }
}
