using System.IO;
using UnityEditor;
using System.Collections.Generic;

public abstract class StateCreatWizard<T> : ScriptableWizard  where T :StateGraph
{
    public string Name;
    public T CopyFrom;
    private string[] _directorys;

    public string[] Directorys
    {
        get
        {
            if (_directorys == null)
            {
                string saveDir = GetSaveDirectory();
                var dirs = Directory.GetDirectories(saveDir, "*", SearchOption.AllDirectories);
                List<string> formatDirs = new List<string>() { "无" };
                foreach (var dir in dirs)
                {
                    formatDirs.Add(dir.Replace(saveDir, "").Replace("\\", "/"));
                }
                _directorys = formatDirs.ToArray();
            }
            return _directorys;
        }
    }
    [UnityEngine.HideInInspector]
    public int SelectDirectory;
    private string lastCheckName;
    protected abstract string GetSaveDirectory();
    /// <summary>
    /// 检查文件名是否合法
    /// </summary>
    /// <returns>错误信息，没有错误返回null</returns>
    protected abstract string NameCheck();

    protected override bool DrawWizardGUI()
    {
        bool modify = base.DrawWizardGUI();
        EditorGUI.BeginChangeCheck();
        SelectDirectory = EditorGUILayout.Popup("目录", SelectDirectory, Directorys);
        if (EditorGUI.EndChangeCheck())
            modify = true;
        return modify;
    }

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
        string subPath = Name;
        if (SelectDirectory > 0 && SelectDirectory < Directorys.Length)
        {
            subPath = $"{Directorys[SelectDirectory]}/{Name}";
        }
        string path = string.Format("{0}/{1}.asset", GetSaveDirectory(), subPath).Replace('\\', '/').Replace("//", "/");
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
            string subPath = Name;
            if (SelectDirectory > 0 && SelectDirectory < Directorys.Length)
            {
                subPath = $"{Directorys[SelectDirectory]}/{Name}";
            }
            string path = string.Format("{0}/{1}.asset", GetSaveDirectory(), subPath).Replace('\\', '/').Replace("//", "/");
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
