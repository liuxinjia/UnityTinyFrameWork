using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;
 
public class SpriteAtlasConverter
{
    private static bool GetTagIfTexture(string Path, ref Dictionary<string, List<string>> dict)
    {
        Object[] data = AssetDatabase.LoadAllAssetsAtPath(Path);
        foreach (Object o in data)
        {
            Texture2D s = o as Texture2D;
            if (s != null)
            {
                TextureImporter ti= AssetImporter.GetAtPath(Path) as TextureImporter;
                List<string> spritesWithTag;
                if (!dict.ContainsKey(Path))
                {
                    spritesWithTag = new List<string>();
                    dict[ti.spritePackingTag] = spritesWithTag;
                }
                else
                {
                    spritesWithTag = dict[ti.spritePackingTag];
                }
                spritesWithTag.Add(Path);
                return true;
            }
        }
        return false;
    }
 
    private static Texture2D GetTexture(string Path)
    {
        Object[] data = AssetDatabase.LoadAllAssetsAtPath(Path);
        foreach (Object o in data)
        {
            Texture2D s = (Texture2D)o;
            if (s != null)
                return s;
        }
        return null;
    }
 
    private static bool CacheSpriteAtlasSprites(string Path, ref SortedSet<string> SpritesInAtlas)
    {
        Object[] data = AssetDatabase.LoadAllAssetsAtPath(Path);
        foreach (Object o in data)
        {
            SpriteAtlas sa = o as SpriteAtlas;
            if (sa != null)
            {
                Sprite[] sprites = new Sprite[sa.spriteCount];
                sa.GetSprites(sprites);
 
                foreach (Sprite sprite in sprites)
                {
                    SpritesInAtlas.Add(AssetDatabase.GetAssetPath(sprite));
                }
            }
            return true;
        }
        return false;
    }
 
    [MenuItem("Assets/Create SpriteAtlas for selected Sprites.")]
    public static void CreateAtlasForSelectedSprites()
    {
        SpriteAtlas sa = new SpriteAtlas();
        AssetDatabase.CreateAsset(sa, "Assets/sample.spriteatlas");
        foreach (var obj in Selection.objects)
        {
            Object o = obj as Sprite;
            if (o != null)
                SpriteAtlasExtensions.Add(sa, new Object[] { o });
        }
        AssetDatabase.SaveAssets();
    }
 
    [MenuItem("Assets/SpriteAtlas Migrate")]
    public static void SpriteAtlasMigrator()
    {
        List<string> TexturesList = new List<string>();
        List<string> SpriteAtlasList = new List<string>();
        Dictionary<string, List<string>> spriteTagMap = new Dictionary<string, List<string>>();
        SortedSet<string> SpritesInAtlas = new SortedSet<string>();
 
        foreach (string s in AssetDatabase.GetAllAssetPaths())
        {
            if (s.StartsWith("Packages") || s.StartsWith("ProjectSettings") || s.Contains("scene"))
                continue;
            bool hasSprite = GetTagIfTexture(s, ref spriteTagMap);
            if (hasSprite)
            {
                TexturesList.Add(s);
            }
            else if (s.Contains("spriteatlas"))
            {
                bool hasSpriteAtlas = CacheSpriteAtlasSprites(s, ref SpritesInAtlas);
                if (hasSpriteAtlas)
                    SpriteAtlasList.Add(s);
            }
        }
 
        foreach (KeyValuePair<string, List<string>> tag in spriteTagMap)
        {
            bool found = SpriteAtlasList.Contains(tag.Key);
            if (!found)
            {
                string atlasPath = "Assets/" + tag.Key + ".spriteatlas";
                SpriteAtlas sa = new SpriteAtlas();
                AssetDatabase.CreateAsset(sa, atlasPath);
                sa.name = tag.Key;
                List<string> ss = tag.Value;
                foreach (string s in ss)
                {
                    Object o = GetTexture(s);
                    SpriteAtlasExtensions.Add(sa, new Object[] { o });
                }
                AssetDatabase.SaveAssets();
            }
        }
 
    }
}