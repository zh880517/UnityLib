using System.IO;
using UnityEditor;

public abstract class StateCreatWizard<T> : ScriptableWizard  where T :StateGraph
{
    public string Name;
    public T CopyFrom;
    private string lastCheckName;
    protected abstract string GetSaveDirectory();
    /// <summary>
    /// 检查文件名是否合法
    /// </summary>
    /// <returns>错误信息，没有错误返回null</returns>
    protected abstract string NameCheck();

    void OnWizardUpdate()
    {
        if (lastCheckName != Name)
        {
            CheckName();
            lastCheckName = Name;
        }
        isValid = !string.IsNullOrWhiteSpace(Name) && string.IsNullOrEmpty(errorString);
    }

    void CheckName()
    {

        foreach (var ch in Name)
        {
            if (ch == '_' || char.IsDigit(ch) || char.IsLetter(ch))
                continue;
            errorString = string.Format("名字中含有非法字符 {0}", ch);
            return;
        }

        string path = string.Format("{0}/{1}.asset", GetSaveDirectory(), Name).Replace('\\', '/').Replace("//", "/");
        if (File.Exists(path))
        {
            errorString = string.Format("文件已存在 {0}", path);
            return;
        }
        errorString = NameCheck();
    }

    public void OnWizardCreate()
    {
        do
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                EditorUtility.DisplayDialog("错误", string.Format("名字不能为空"), "确定");
                break;
            }
            if (!string.IsNullOrWhiteSpace(errorString))
            {
                EditorUtility.DisplayDialog("错误", string.Format("名字不符合要求:\n{0}", errorString), "确定");
                break;
            }
            string path = string.Format("{0}/{1}.asset", GetSaveDirectory(), Name).Replace('\\', '/').Replace("//", "/");
            T graph;
            if (CopyFrom != null)
            {
                graph = Instantiate(CopyFrom);
                AssetDatabase.CreateAsset(graph, path);
            }
            else
            {
                graph = StateGraph.LoadOrCreat<T>(path);
            }
            AssetDatabase.OpenAsset(graph);
            return;
        } while (false);
    }

}
