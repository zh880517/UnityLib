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
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            text = Regex.Replace(text, "#ClassName#", fileNameWithOutExtension);
            text = Regex.Replace(text, "#GUID#", System.Guid.NewGuid().ToString());
            UTF8Encoding uTF8Encoding = new UTF8Encoding(true, false);
            StreamWriter streamWriter = new StreamWriter(pathName, false, uTF8Encoding);
            streamWriter.Write(text);
            streamWriter.Close();
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
            "Assets/StateGraph/Editor/Template/NewStateNode.txt"
            );
    }
}
