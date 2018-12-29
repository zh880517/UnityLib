using System.IO;
using UnityEngine;

public static class FileUtil
{
    public static string ReadString(string filePath, bool onlyHotPatch = false)
    {
        string hotPatchPath = PathUtil.HotPatchPath + filePath;
        if (File.Exists(hotPatchPath))
            return File.ReadAllText(hotPatchPath);

        if (onlyHotPatch)
            return null;

        int index = filePath.LastIndexOf('.');
        if (index > 0)
            filePath = filePath.Substring(0, index);
        var txtAsset = Resources.Load<TextAsset>(filePath);
        return txtAsset != null ? txtAsset.text : null;
    }

    public static byte[] ReadBytes(string filePath, bool onlyHotPatch = false)
    {
        string hotPatchPath = PathUtil.HotPatchPath + filePath;
        if (File.Exists(hotPatchPath))
            return File.ReadAllBytes(hotPatchPath);

        if (onlyHotPatch)
            return null;

        int index = filePath.LastIndexOf('.');
        if (index > 0)
            filePath = filePath.Substring(0, index);
        var txtAsset = Resources.Load<TextAsset>(filePath);
        return txtAsset != null ? txtAsset.bytes : null;
    }

    public static void DeleteFile(string filePath)
    {
        string hotPatchPath = PathUtil.HotPatchPath + filePath;
        if (File.Exists(hotPatchPath))
            File.Delete(hotPatchPath);
    }

    public static string FindHotPathFilePath(string filePath)
    {
        string hotPatchPath = PathUtil.HotPatchPath + filePath;
        if (File.Exists(hotPatchPath))
            return hotPatchPath;
        return null;
    }
}
