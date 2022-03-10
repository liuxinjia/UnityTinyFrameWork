using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Cr7Sund.Editor.Excels;
using System;

namespace Cr7SundTools
{
    public partial class AnalyzeResourcesWindow : EditorWindow
    {
        private static readonly string[] Contents = { "Sprite", "Texture", "Model","Atlas" };
        private static readonly string[] BuildPlatforms = { BuildTargetGroup.Android.ToString() };
        private List<FoldOutData> foldOutDatas = new List<FoldOutData>();
        private Dictionary<string, AssetImporter> importerDict = new Dictionary<string, AssetImporter>();


        [MenuItem("Tools/Optimize/AnalyzeResourcesWindow %#&A")]
        private static void ShowWindow()
        {
            var window = GetWindow<AnalyzeResourcesWindow>();
            window.titleContent = new GUIContent("AnalyzeResourcesWindow");
            window.Show();
        }

        private void OnEnable()
        {
            var actionDict = new Dictionary<string, List<string>>(Contents.Length);
            foreach (var content in Contents)
            {
                var actions = new List<string>();
                actionDict.Add(content, actions);
            }

            var methods = typeof(AnalyzeResourcesWindow).GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                foreach (var content in Contents)
                {
                    if (Regex.IsMatch(method.Name, content))
                    {
                        actionDict[content].Add(method.Name);
                    }
                }
            }

            foreach (var content in Contents)
                foldOutDatas.Add(new FoldOutData(content, actionDict[content].ToArray()));

        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                for (int i = 0; i < foldOutDatas.Count; i++)
                {
                    FoldOutData item = foldOutDatas[i];
                    item.showPos = EditorGUILayout.Foldout(item.showPos, item.content);

                    if (item.showPos)
                    {
                        foreach (var actionName in item.actioNames)
                        {
                            if (GUILayout.Button(actionName))
                            {
                                InvokeFoldOutAction(actionName);
                            }
                        }
                    }
                }
            }

            GUILayout.Space(15);
            if (GUILayout.Button("Reimport All Assets")) OnLostFocus();
            EditorGUILayout.EndVertical();
        }

        private void OnLostFocus()
        {
            // foreach (var item in importerDict) item.Value.SaveAndReimport();
            if (importerDict.Count > 0) Debug.Log($"Reimports All Assets - {importerDict.Count}");
            importerDict.Clear();
        }

        private void OnDisable()
        {
            importerDict.Clear();
        }


        #region  HelperClass

        private void ExportData2Excel(List<string> values, string tableName = "sprite", string excelName = "SpriteInfo")
        {
            if (values.Count < 2) return;
            //drawtreasure 1 1 10 100
            string path = EditorUtil.GetProjectAbsolutePath($"{excelName}.xlsx");
            var excel = new ExcelWriter(path);
            var excelTable = excel.CreateTable(tableName);

            List<(string header, Type type)> headers = new List<(string header, Type)>();

            for (int colIndex = 0; colIndex < values.Count; colIndex++)
            {
                var titles = values[colIndex].Split(',');
                if (colIndex == 0)
                {
                    headers.Add(("资源名字", typeof(string)));
                    headers.Add(("资源路径", typeof(string)));

                    for (int i = 0; i < titles.Length; i++)
                    {
                        headers.Add((titles[i], typeof(string)));
                    }
                    excelTable.InitHeaders(headers);
                }
                else
                {
                    for (int i = 0; i < titles.Length-1; i++)
                    {
                        if (i == 0)
                        {
                            var fileNames = titles[i].Split('/');
                            var fileName = fileNames[fileNames.Length - 1].Split('.');
                            excelTable.SetValue(colIndex - 1, i, fileName[0]);
                            excelTable.SetValue(colIndex-1, i + 1, titles[i]);
                        }
                        else
                        {
                            excelTable.SetValue(colIndex - 1, 1 + i, titles[i]);
                        }
                    }
                }
            }


            excel.SaveExcels();

        }

        private void InvokeFoldOutAction(string actionName)
        {
            System.Reflection.MethodInfo methodInfo = typeof(AnalyzeResourcesWindow).GetMethod(actionName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            methodInfo.Invoke(this, null);
        }
        #endregion
    }


    public class FoldOutData
    {
        public bool showPos = true;
        public string content;
        public List<string> actioNames;

        public FoldOutData(string content, string[] actioName)
        {
            this.content = content;
            this.actioNames = new List<string>(actioName);
        }

    }

    public static class ExtensionClass
    {
        #region Cache Reflection
#if UNITY_2018_4_13
#endif
        private static MethodInfo GetSourceTextureInformationMethodInfo;

        #endregion



        public static UnityEditor.AssetImporters.SourceTextureInformation GetSourceTextureInformation(this TextureImporter importer)
        {
            if (GetSourceTextureInformationMethodInfo == null) GetSourceTextureInformationMethodInfo = typeof(TextureImporter).GetMethod("GetSourceTextureInformation", BindingFlags.Instance | BindingFlags.NonPublic);
            return GetSourceTextureInformationMethodInfo.Invoke(importer, null) as UnityEditor.AssetImporters.SourceTextureInformation;
        }

        public static void getSourceTextureWidthAndHeight(this TextureImporter importer, out int width, out int height)
        {
#if UNITY_2021_2_OR_NEWER
            importer.GetSourceTextureWidthAndHeight(out width, out height);
#else
            var info = importer.GetSourceTextureInformation();
            if (info.width == -1)
                throw new System.InvalidOperationException("The texture has not yet finished importing. This most likely means this method was called in an AssetPostprocessor.OnPreprocessAsset callback.");
            width = info.width;
            height = info.height;
#endif
        }
    }



}
