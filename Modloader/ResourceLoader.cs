using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    [HideInInspector]
    public ModDatabase database; // shared

    void LoadFromTextAsset<TData, TItem>(
    TextAsset file,
    Dictionary<string, TItem> databaseDict,
    string logPrefix
)
    where TData : ModDataBase<TItem>
    where TItem : IHasID
    {
        try
        {
            TData modData = JsonConvert.DeserializeObject<TData>(file.text);

            if (modData == null)
            {
                Debug.LogError($"JSON 파싱 실패: {file.name}");
                return;
            }

            if (modData.items == null)
            {
                Debug.LogError($"items null: {file.name}");
                return;
            }

            foreach (var item in modData.items)
            {
                if (item == null)
                {
                    Debug.LogError($"item null: {file.name}");
                    continue;
                }

                if (string.IsNullOrEmpty(item.id))
                {
                    Debug.LogError($"ID 없음: {file.name}");
                    continue;
                }

                if (databaseDict == null)
                {
                    Debug.LogError("databaseDict null");
                    return;
                }

                databaseDict[item.id] = item;
                Debug.Log($"{logPrefix} 로드: {item.id}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"파일 로드 실패: {file.name}\n{e}");
        }
    }

    public void LoadMods()
    {
        TextAsset[] allFiles = Resources.LoadAll<TextAsset>("");

        foreach (var file in allFiles)
        {
            string lowerName = file.name.ToLower();

            if (lowerName.EndsWith("_picpath"))
            {
                LoadFromTextAsset<Data_picpath, PicPath>(
                    file, database.picpathDatabase, "이미지 경로"
                );
            }
        }
    }
}