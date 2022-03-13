namespace Cr7SundTools
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine.U2D;
    using UnityEditor.U2D;
    using System.Reflection;
    using System.Text;
    using System.IO;

    public partial class AnalyzeResourcesWindow
    {

        public void AnalyzeAtlass()
        {
            string path = Application.dataPath + $"/AtlasInfo.xlsx";
            System.IO.File.Delete(path);
            AnalyzeAtlasSize();

            Application.OpenURL(path);
        }


        public void AnalyzeAtlasSize()
        {
            var atlasGUIDs = AssetDatabase.FindAssets("t:spriteatlas");
            var beyondSizeTextures = new HashSet<string>();

            var packAtlass = new List<SpriteAtlas>();
            for (int i = 0; i < atlasGUIDs.Length; i++)
            {
                string guid = atlasGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                packAtlass.Add(spriteAtlas);
            }

            SpriteAtlas[] spriteAtlases = packAtlass.ToArray();
            SpriteAtlasUtility.PackAtlases(spriteAtlases, EditorUserBuildSettings.activeBuildTarget);
            // SpriteAtlasUtility.PackAllAtlases(BuildTarget.StandaloneWindows);

            for (int i = 0; i < atlasGUIDs.Length; i++)
            {
                string guid = atlasGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + atlasGUIDs.Length, path, (float)i / atlasGUIDs.Length);

                var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                SerializedObject so = new SerializedObject(spriteAtlas);
                SerializedProperty sp = so.FindProperty("m_EditorData.packables");

                foreach (var platFormName in BuildPlatforms)
                {
                    StringBuilder sb = new StringBuilder();
                    var setting = spriteAtlas.GetPlatformSettings(platFormName);

                    var previewTextures = getPreviewTextureMethodInfo.Invoke(null, new[] { spriteAtlas }) as Texture2D[];
                    int width = 0, height = 0;

                    foreach (var previewTexture in previewTextures)
                    {
                        width += previewTexture.width;
                        height += previewTexture.height;
                    }

                    if (width > 2048 || height > 2048)
                    {
                        sb.Append($"{path},{width},{height},{previewTextures.Length},{setting.maxTextureSize},");

                        var invaildSprites = GetSpriteAtlasSprites(spriteAtlas);
                        var invaildSb = new StringBuilder();
                        foreach (var sprite in invaildSprites)
                        {
                            var texture = sprite.texture;
                            if (IsMaxTexture(texture))
                            {
                                invaildSb.Append($"{texture.name} ({texture.width}*{texture.height}) + ");
                            }
                        }
                        if (invaildSb.Length > 0) sb.Append($"{invaildSb.ToString()},");

                        beyondSizeTextures.Add(sb.ToString());
                    }

                    // setting.maxTextureSize = 2
                    // spriteAtlas.SetPlatformSettings(setting);
                }
            }
            EditorUtility.ClearProgressBar();

            List<string> logInfos = new List<string>(beyondSizeTextures);
            logInfos.Insert(0, "宽,高,图片数量,图集最大尺寸, 过大图片");
            ExportAtlasData2Excell(logInfos, "尺寸过大");
        }

        public void AnalyzeAtlasFormats()
        {
            var atlasGUIDs = AssetDatabase.FindAssets("t:spriteatlas");
            var invalidTextures = new HashSet<string>();
            var defaultSettings = new List<TextureImporterPlatformSettings>();
            for (int i = 0; i < atlasGUIDs.Length; i++)
            {
                string guid = atlasGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + atlasGUIDs.Length, path, (float)i / atlasGUIDs.Length);

                var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                SerializedObject so = new SerializedObject(spriteAtlas);
                foreach (var platFormName in BuildPlatforms)
                {
                    var setting = spriteAtlas.GetPlatformSettings(platFormName);
                    // setting.maxTextureSize = 2
                    // spriteAtlas.SetPlatformSettings(setting);
                    if (!setting.format.ToString().Contains("ASTC"))
                        invalidTextures.Add($"{path},{setting.format},{spriteAtlas.spriteCount},");
                }
            }
            EditorUtility.ClearProgressBar();

            List<string> logInfos = new List<string>(invalidTextures);
            logInfos.Insert(0, "图集格式");
            ExportAtlasData2Excell(logInfos, "图集格式");
        }


        public void AnalyzeAtlasComparisson()
        {
            // const string folderPath = "Assets/bundles/Atlas/large/UI";
            // string atlasName = folderPath.Substring(folderPath.LastIndexOf('/'));
            // string atlasPath = folderPath + atlasName + ".spriteatlas";

            var atlasGUIDs = AssetDatabase.FindAssets("t:spriteatlas");
            var defaultSettings = new List<TextureImporterPlatformSettings>();

            AssetDatabase.StartAssetEditing();
            for (int w = 0; w < atlasGUIDs.Length; w++)
            {
                string atlasGUID = atlasGUIDs[w];
                var atlasPath = AssetDatabase.GUIDToAssetPath(atlasGUID);
                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

                EditorUtility.DisplayProgressBar("reimport texture " + w + "/" + atlasGUIDs.Length, atlasPath, (float)w / atlasGUIDs.Length);

                // Selection.activeObject = spriteAtlas; // Something go wrong if you don't select the sprite (inspector ) '

                // SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);

                var atlasSprites = GetSpriteAtlasSprites(spriteAtlas);
                var previewTextures = getPreviewTextureMethodInfo.Invoke(null, new[] { spriteAtlas }) as Texture2D[];

                long atlasSize = 0;
                if (previewTextures.Length < 1)
                {
                    Debug.LogError($"Has no preview textures: {atlasPath}");
                    return;
                }
                for (var i = 0; i < previewTextures.Length; i++)
                {
                    atlasSize += GetInspectorMemory(previewTextures[i]);
                }
                var textureFormat = previewTextures[0].format; // only currently support current buildPlform

                foreach (var platformName in BuildPlatforms)
                {
                    long totalSpriteSize = 0;

                    for (int i = 0; i < atlasSprites.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("check sprites " + i + "/" + atlasSprites.Count, atlasSprites[i].name, (float)i / atlasSprites.Count);

                        var sprite = atlasSprites[i];
                        var path = AssetDatabase.GetAssetPath(sprite);

                        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (sprite.texture.format != textureFormat)
                        {
                            Debug.LogError(sprite.texture.name);
                            Debug.LogError($"Please make sure the both atals and sprites have the same format! AtalsFormat:{textureFormat} {sprite.texture.format} AtalsName: {atlasPath} ");
                            KeepSameTextureFormatWithAtals(atlasPath);
                            break;
                        }
                        totalSpriteSize += GetInspectorMemory(sprite);
                    }

                    float v = totalSpriteSize / (float)atlasSize;
                    if(v >=1 )
                    Debug.Log($" {spriteAtlas.name}-{platformName}: {totalSpriteSize} / {atlasSize} = {v }");
                }

                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.StopAssetEditing();
            System.GC.Collect();
        }

        private void KeepSameTextureFormatWithAtals(string atlasPath)
        {

            {
                SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
                // SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);

                var atlasSprites = GetSpriteAtlasSprites(spriteAtlas);
                var previewTextures = getPreviewTextureMethodInfo.Invoke(null, new[] { spriteAtlas }) as Texture2D[];

                if (previewTextures.Length < 1)
                {
                    Debug.LogError($"Has no preview textures: {atlasPath}");
                    return;
                }
                var textureFormat = previewTextures[0].format; // only currently support current buildPlform

                var invalidTextures = new HashSet<string>();
                foreach (var platformName in BuildPlatforms)
                {
                    for (int i = 0; i < atlasSprites.Count; i++)
                    {
                        EditorUtility.DisplayProgressBar("check sprites " + i + "/" + atlasSprites.Count, atlasSprites[i].name, (float)i / atlasSprites.Count);

                        var sprite = atlasSprites[i];
                        var path = AssetDatabase.GetAssetPath(sprite);

                        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (sprite.texture.format != textureFormat)
                        {
                            var textureSetting = textureImporter.GetPlatformTextureSettings(platformName);
                            textureSetting.overridden = true;
                            textureSetting.format = (TextureImporterFormat)textureFormat;
                            textureImporter.SetPlatformTextureSettings(textureSetting);
                            textureImporter.SaveAndReimport();
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
                Debug.Log($"Finish spriteAtlas");
            }
        }


        private List<Sprite> GetSpriteAtlasSprites(SpriteAtlas spriteAtlas)
        {
            var resulstSprites = new List<Sprite>();
            SerializedObject so = new SerializedObject(spriteAtlas);
            SerializedProperty sp = so.FindProperty("m_EditorData.packables");
            if (sp != null)
            {
                for (var j = 0; j < sp.arraySize; j++)
                {
                    var po = sp.GetArrayElementAtIndex(j);
                    if (po != null)
                    {
                        int instanceID = po.objectReferenceValue.GetInstanceID();
                        string assetPath = AssetDatabase.GetAssetPath(instanceID);

                        if (ProjectWindowUtil.IsFolder(instanceID))
                        {
                            var guids = AssetDatabase.FindAssets("t:Sprite", new[] { assetPath });
                            for (int k = 0; k < guids.Length; k++)
                            {
                                resulstSprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[k])));
                            }
                        }
                        else if (po.objectReferenceValue.GetType() == typeof(Texture2D))
                        {
                            resulstSprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(assetPath));
                        }
                        else if (po.objectReferenceValue.GetType() == typeof(Sprite))
                        {
                            //SpriteAtlas don not include texture
                            Debug.LogWarning($"SpriteAtlas can not pack texture: {assetPath}");
                        }
                    }
                }


                foreach (var sprite in resulstSprites)
                {
                    // var atlasSprite = spriteAtlas.GetSprite(sprite.name); // Don't fukcing user it in editor, it will cost a long long time
                }

                //Valid Method
                // Sprite[] sprites = resulstSprites.ToArray();
                // if (spriteAtlas.GetSprites(sprites) > 0)
                // {
                //     Debug.Log("Succesfully get sprites from atlas");
                // }

            }
            return resulstSprites;
        }

        [System.Obsolete("Can add folder automatically")]
        private SpriteAtlas CreateAtlas(string atlasName, string atlasPath)
        {
            TextureImporterFormat[] textureFormats = new[] { TextureImporterFormat.ASTC_RGBA_8x8, TextureImporterFormat.ASTC_RGBA_8x8 };

            SpriteAtlas spriteAtlas = new SpriteAtlas();
            spriteAtlas.name = atlasName;

            for (int i = 0; i < RealBuildPlatforms.Length; i++)
            {
                string platformName = RealBuildPlatforms[i];
                var texImportSetting = getPlatformSettingMethodInfo.Invoke(null, new object[] { spriteAtlas, platformName }) as TextureImporterPlatformSettings;
                texImportSetting.overridden = true;
                texImportSetting.maxTextureSize = 2048;
                texImportSetting.format = textureFormats[i];
                setPlatformSettingMethodInfo.Invoke(null, new object[] { spriteAtlas, texImportSetting });
            }
            AssetDatabase.CreateAsset(spriteAtlas, atlasPath);
            AssetDatabase.Refresh();
            return spriteAtlas;
        }

        private static bool IsMaxTexture(Texture2D texture)
        {
            return texture.width > 256 && texture.height > 256;
        }


        private void ExportAtlasData2Excell(List<string> values, string tableName = "atlas")
        {
            ExportData2Excel(values, tableName, "AtlasInfo");
        }
    }
}