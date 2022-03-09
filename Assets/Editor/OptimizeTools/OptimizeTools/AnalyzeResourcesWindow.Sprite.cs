namespace Cr7SundTools
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public partial class AnalyzeResourcesWindow
    {
        string[] searchInFolders = new[] {
                 "Assets/" };

        public void AnalyzeSprites()
        {
            AnalyzeSpriteSize();
            AnalyzeSpriteFormat();
            AnalyzeSpriteOtherSettings();
        }

        /// <summary>
        /// 1. 默认ETC, 不上4的倍数直接变RGB32, 而且没开启ETC2 Fallback, override 都没开启
        /// 只考虑Large/Small 文件加， 其他配置打进图集
        /// </summary>
        public void AnalyzeSpriteFormat()
        {

            var textureGUIDs = AssetDatabase.FindAssets("t:texture2D", searchInFolders);

            var invalidTextures = new HashSet<string>();
            for (int i = 0; i < textureGUIDs.Length; i++)
            {
                string guid = textureGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + textureGUIDs.Length, path, (float)i / textureGUIDs.Length);

                bool modified = false;
                TextureImporter importer;
                if (!importerDict.ContainsKey(path))
                    importer = TextureImporter.GetAtPath(path) as TextureImporter;
                else
                    importer = importerDict[path] as TextureImporter;

                if (importer == null) continue;
                bool haveAlpha = importer.DoesSourceTextureHaveAlpha();

                var textureSettings = new List<TextureImporterPlatformSettings>(BuildPlatforms.Length + 1);
                // textureSettings.Add(importer.GetDefaultPlatformTextureSettings());
                foreach (var platform in BuildPlatforms)
                {
                    textureSettings.Add(importer.GetPlatformTextureSettings(platform));
                }


                // settings instance will be immediately change, you can see in inspector
                foreach (var textureSetting in textureSettings)
                {
                    var textureFormat = textureSetting.format;
                    // importer.textureType != TextureImporterType.Default &&  
                    if (importer.textureType != TextureImporterType.Sprite) continue;
                    // if (importer.textureShape != TextureImporterShape.Texture2D) continue;
                    if (!textureFormat.ToString().Contains("ASTC"))
                    {
                        string invalidTextureInfo = string.Format("{0},{1},", path, textureFormat);
                        if (!invalidTextures.Contains(invalidTextureInfo)) invalidTextures.Add(invalidTextureInfo);
                        modified = true;
                        textureSetting.format = textureFormat;
                        importer.SetPlatformTextureSettings(textureSetting);
                    }
                }


                // #if UNITY_ANDROID
                // #elif UNITY_IOS
                // #else
                // #endif
                if (modified)
                    if (!importerDict.ContainsKey(path)) importerDict.Add(path, importer);

            }
            EditorUtility.ClearProgressBar();

            List<string> logInfos = new List<string>(invalidTextures);
            logInfos.Insert(0, "图片格式");
            ExportSpiteData2Excell(logInfos, "图片格式不对");

        }

        public void AnalyzeSpriteSize()
        {
            var textureGUIDs = AssetDatabase.FindAssets("t:texture2D");
            string atlas = "Atlas";
            var beyondSizeTextures = new HashSet<string>();
            var invalidTextures = new HashSet<string>();
            for (int i = 0; i < textureGUIDs.Length; i++)
            {
                string guid = textureGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + textureGUIDs.Length, path, (float)i / textureGUIDs.Length);

                bool modified = false;
                TextureImporter importer;
                if (!importerDict.ContainsKey(path))
                    importer = TextureImporter.GetAtPath(path) as TextureImporter;
                else
                    importer = importerDict[path] as TextureImporter;

                if (importer == null) continue;

                var textureSettings = new List<TextureImporterPlatformSettings>(BuildPlatforms.Length + 1);
                // textureSettings.Add(importer.GetDefaultPlatformTextureSettings());

                foreach (var platform in BuildPlatforms)
                {
                    textureSettings.Add(importer.GetPlatformTextureSettings(platform));
                }

                // settings instance will be immediately change, you can see in inspector
                foreach (var textureSetting in textureSettings)
                {
                    var textureFormat = textureSetting.format;
                    if (importer.textureType != TextureImporterType.Sprite) continue;

                    importer.getSourceTextureWidthAndHeight(out int width, out int height);
                    // if (textureSetting.maxTextureSize > 1024)
                    {

                        if (width > 1024 || height > 1024)
                        {
                            modified = true;
                            textureSetting.maxTextureSize = 1024;
                            importer.SetPlatformTextureSettings(textureSetting);
                            string textureDebugInfo = string.Format("{0},{1}x{2},", path, width, height);
                            if (!beyondSizeTextures.Contains(textureDebugInfo)) beyondSizeTextures.Add(textureDebugInfo);
                        }
                    }
                    if (!Regex.IsMatch(path, atlas))
                    {
                        importer.getSourceTextureWidthAndHeight(out int w, out int h);
                        if (!Mathf.IsPowerOfTwo(w) || !Mathf.IsPowerOfTwo(h))
                        {
                            if (!invalidTextures.Contains(path)) invalidTextures.Add(path);
                        }
                    }

                }

                if (modified)
                    if (!importerDict.ContainsKey(path)) importerDict.Add(path, importer);
            }
            EditorUtility.ClearProgressBar();


            List<string> logInfos = new List<string>(beyondSizeTextures);
            logInfos.Insert(0, "图片尺寸");
            ExportSpiteData2Excell(logInfos, "尺寸过大");
        }

        public void AnalyzeSpriteOtherSettings()
        {

            var textureGUIDs = AssetDatabase.FindAssets("t:texture2D", searchInFolders);

            var invalidTextures = new HashSet<string>();
            for (int i = 0; i < textureGUIDs.Length; i++)
            {
                string guid = textureGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + textureGUIDs.Length, path, (float)i / textureGUIDs.Length);

                bool modified = false;
                TextureImporter importer;
                if (!importerDict.ContainsKey(path))
                    importer = TextureImporter.GetAtPath(path) as TextureImporter;
                else
                    importer = importerDict[path] as TextureImporter;

                if (importer == null) continue;

                var textureSettings = new List<TextureImporterPlatformSettings>(BuildPlatforms.Length + 1);
                // textureSettings.Add(importer.GetDefaultPlatformTextureSettings());

                foreach (var platform in BuildPlatforms)
                {
                    textureSettings.Add(importer.GetPlatformTextureSettings(platform));
                }

                string debugInfo = path + ",";
                // settings instance will be immediately change, you can see in inspector
                foreach (var textureSetting in textureSettings)
                {
                    var textureFormat = textureSetting.format;
                    if (importer.textureType != TextureImporterType.Sprite) continue;

                    // Overide platforms
                    if (!textureSetting.overridden) { textureSetting.overridden = true; modified = true; debugInfo += "false,"; }
                    else { debugInfo += "true,"; }
                }

                //Read-write access
                if (importer.isReadable) { importer.isReadable = false; modified = true; debugInfo += "true,"; }
                else { debugInfo += "false,"; }
                //Mip-Map enabled
                if (importer.mipmapEnabled) { importer.mipmapEnabled = false; modified = true; debugInfo += "true,"; }
                else { debugInfo += "false,"; }

                if (modified)
                {
                    if (!invalidTextures.Contains(debugInfo)) invalidTextures.Add(debugInfo);
                    if (!importerDict.ContainsKey(path)) importerDict.Add(path, importer);
                }
            }
            EditorUtility.ClearProgressBar();

            List<string> debugInfos = new List<string>(invalidTextures);
            debugInfos.Insert(0, "（Android）平台是否重写, 开启读写, MipMap");
            ExportSpiteData2Excell(debugInfos, "其他设置");

        }

        public void AnalyzeSpriteTransparency()
        {
            //alpha chanel
            EditorGUILayout.ObjectField(Selection.activeGameObject, typeof(Texture), false);

        }


        
        private void ExportSpiteData2Excell(List<string> values, string tableName = "effect")
        {
            ExportData2Excel(values, tableName, "SpriteInfo");
        }
    }
}