using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ModCache
{
    private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

    public static Sprite Get(Dictionary<string, PicPath> pathes, string key)
    {
        if (spriteCache.ContainsKey(key))
        {
            return spriteCache[key];
        }

        PicPath keyPath = pathes.GetValueOrDefault(key, null);
        if (keyPath == null)
        {
            Debug.Log("경로를 알 수 없는 이미지: " + key);
            spriteCache[key] = null;
            textureCache[key] = null;
            return null;
        }

        Texture2D tex = Resources.Load<Texture2D>(keyPath.path);

        Sprite sprite = null;
        if (tex == null)
            Debug.Log("이미지 없음. 경로: " + "Mods/Images/" + key);
        else
        {
                sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f)
                );
        }

        spriteCache[key] = sprite;
        textureCache[key] = tex;

        return sprite;
    }

    public static void Release(string path)
    {
        if (spriteCache.TryGetValue(path, out var sprite))
        {
            Object.Destroy(sprite);
            spriteCache.Remove(path);
        }

        if (textureCache.TryGetValue(path, out var tex))
        {
            Object.Destroy(tex);
            textureCache.Remove(path);
        }
    }

    public static void ReleaseAll()
    {
        foreach (var sprite in spriteCache.Values)
            Object.Destroy(sprite);

        foreach (var tex in textureCache.Values)
            Object.Destroy(tex);

        spriteCache.Clear();
        textureCache.Clear();
    }
}