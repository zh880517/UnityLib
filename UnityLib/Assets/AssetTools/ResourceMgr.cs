using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceMgr
{
    public static void LoadPrefab(string dir, string name, Action<GameObject> func)
    {

    }

    public static IEnumerator LoadPrefab(string dir, string name)
    {
        yield break;
    }


    public static void LoadUIPrefab(string dir, string name, Action<GameObject> func)
    {

    }

    public static IEnumerator LoadUIPrefab(string dir, string name)
    {
        yield break;
    }

    public static void LoadAtlasSprite(string dir, string name, Action<Sprite> func)
    {

    }

    public static void LoadAloneSprite(string dir, string name, Action<Sprite> func)
    {

    }
}
