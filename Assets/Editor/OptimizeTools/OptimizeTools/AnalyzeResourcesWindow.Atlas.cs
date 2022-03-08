namespace Cr7SundTools
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

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
            var textureGUIDs = AssetDatabase.FindAssets("t:texture2D");
            var beyondSizeTextures = new HashSet<string>();
            var invalidTextures = new HashSet<string>();
            for (int i = 0; i < textureGUIDs.Length; i++)
            {
                string guid = textureGUIDs[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);

                EditorUtility.DisplayProgressBar("reimport texture " + i + "/" + textureGUIDs.Length, path, (float)i / textureGUIDs.Length);

    }
            EditorUtility.ClearProgressBar();


            List<string> logInfos = new List<string>(beyondSizeTextures);
            logInfos.Insert(0, "图片尺寸");
            ExportAtlasData2Excell(logInfos, "尺寸过大");
        }




        
        private void ExportAtlasData2Excell(List<string> values, string tableName = "effect")
        {
            ExportData2Excel(values, tableName, "AtlasInfo");
        }
    }
}