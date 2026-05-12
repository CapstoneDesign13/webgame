using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class ModDatabase : ScriptableObject
{
    public static ModDatabase Instance { get; private set; }

    public Dictionary<string, PicPath> picpathDatabase = new Dictionary<string, PicPath>();

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("ModDatabase instance already exists!");
        }
    }

    public Sprite GetPic(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        return ModCache.Get(picpathDatabase, id);
    }
}