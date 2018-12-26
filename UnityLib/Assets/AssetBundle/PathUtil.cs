using System.IO;
using UnityEngine;

public static class PathUtil
{
    //资源WWW加载目录
    private static string streamAssertUrl;
    public static string StreamAssertUrl
    {
        get
        {
            if (streamAssertUrl == null)
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        streamAssertUrl = "jar:file://" + Application.dataPath + "!/assets/";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        streamAssertUrl = "file:///" + Application.dataPath + "/Raw/";
                        break;
                    default:
                        streamAssertUrl = "file://" + Application.streamingAssetsPath + "/";
                        break;
                }
            }
            return streamAssertUrl;
        }
    }

    //资源直接读取路径
    private static string streamAssertPath;
    public static string StreamAssertPath
    {
        get
        {
            if (streamAssertPath == null)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    streamAssertPath = Application.dataPath + "!assets/";
                }
                else
                {
                    streamAssertPath = Application.streamingAssetsPath + "/";
                }
            }
            return streamAssertPath;
        }
    }

    //热更新文件存放目录
    private static string hotPatchPath;
    public static string HotPatchPath
    {
        get
        {
            if (hotPatchPath == null)
            {
                hotPatchPath = Application.persistentDataPath + "/patch/";
            }
            return hotPatchPath;
        }
    }

    //热更新文件Url路径
    private static string hotPatchUrl;
    public static string HotPatchUrl
    {
        get
        {
            if (hotPatchUrl == null)
            {
                hotPatchUrl = "file:///" + HotPatchPath;
            }
            return hotPatchUrl;
        }
    }

    public static string GetStreamAssertFileUrl(string name)
    {
        if (File.Exists(HotPatchPath + name))
            return HotPatchUrl + name;
        return StreamAssertUrl + name;
    }

    public static string GetStreamAssertFilePath(string name)
    {
        string hotPatchPath = HotPatchPath + name;
        if (File.Exists(hotPatchPath))
            return hotPatchPath;
        return StreamAssertPath + name;
    }
    
}
