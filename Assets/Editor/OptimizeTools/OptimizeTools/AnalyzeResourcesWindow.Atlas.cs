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

    public partial class AnalyzeResourcesWindow
    {
        public void AnalyzeAtlass()
        {
            string path = Application.dataPath + $"/AtlasInfo.xlsx";
            System.IO.File.Delete(path);
            AnalyzeAtlasSize();

            Application.OpenURL(path);
        }


        public async void AnalyzeAtlasSize()
        {
            var textureGUIDs = AssetDatabase.FindAssets("t:spriteatlas");
            var beyondSizeTextures = new HashSet<string>();
            var defaultSettings = new List<TextureImporterPlatformSettings>();
            var getPreviewTextureMethodInfo = typeof(SpriteAtlasExtensions).GetMethod("GetPreviewTextures", BindingFlags.NonPublic | BindingFlags.Static);

            var packAtlass = new List<SpriteAtlas>();
            for (int i = 0; i < textureGUIDs.Length; i++)
            {
                string guid = textureGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            }
            SpriteAtlas[] spriteAtlases = packAtlass.ToArray();
            // SpriteAtlasUtility.PackAtlases(spriteAtlases, EditorUserBuildSettings.activeBuildTarget);


            for (int i = 0; i < textureGUIDs.Length; i++)
            {
                string guid = textureGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + textureGUIDs.Length, path, (float)i / textureGUIDs.Length);

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
                        var invaildTextures = new List<Texture2D>();
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
                                        var guids = AssetDatabase.FindAssets("t:texture", new[] { assetPath });
                                        for (int k = 0; k < guids.Length; k++)
                                        {
                                            invaildTextures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[k])));
                                        }
                                    }
                                    else if (po.objectReferenceValue.GetType() == typeof(Sprite)
                                    || po.objectReferenceValue.GetType() == typeof(Texture2D))
                                    {
                                        invaildTextures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath));
                                    }

                                }
                            }
                        }

                        sb.Append($"{path},{width},{height},{previewTextures.Length},{setting.maxTextureSize},");

                        var invaildSb = new StringBuilder();
                        foreach (var texture in invaildTextures)
                        {
                            if (texture.width > 256 && texture.height > 256)
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
            var textureGUIDs = AssetDatabase.FindAssets("t:spriteatlas");
            var invalidTextures = new HashSet<string>();
            var defaultSettings = new List<TextureImporterPlatformSettings>();
            for (int i = 0; i < textureGUIDs.Length; i++)
            {
                string guid = textureGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + textureGUIDs.Length, path, (float)i / textureGUIDs.Length);

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




        private void ExportAtlasData2Excell(List<string> values, string tableName = "atlas")
        {
            ExportData2Excel(values, tableName, "AtlasInfo");
        }
    }
}