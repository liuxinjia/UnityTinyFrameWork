// using System.Runtime.InteropServices;
// using UnityEngine;
// using UnityEditor;
// using System.Collections.Generic;
// using System.Reflection;
// using System.Text.RegularExpressions;

// using System.IO;

// namespace Cr7SundTools
// {
//     public partial class AnalyzeResourcesWindow : EditorWindow
//     {
//         private static readonly string[] Contents = { "Sprite", "Texture", "Model", "Atlas" };
//         private static readonly string[] BuildPlatforms = { BuildTargetGroup.Android.ToString() };
//         private static readonly string[] RealBuildPlatforms = { BuildTargetGroup.Android.ToString(), BuildTargetGroup.iOS.ToString() };
//         private List<FoldOutData> foldOutDatas = new List<FoldOutData>();
//         private Dictionary<string, AssetImporter> importerDict = new Dictionary<string, AssetImporter>();

//         System.Reflection.MethodInfo getStorageMemorySizeLongMethodInfo = null;
//         System.Reflection.MethodInfo getPreviewTextureMethodInfo = null;
//         System.Reflection.MethodInfo setPlatformSettingMethodInfo = null;
//         System.Reflection.MethodInfo getPlatformSettingMethodInfo = null;


//         [MenuItem("Tools/Optimize/AnalyzeResourcesWindow %#&A")]
//         private static void ShowWindow()
//         {
//             var window = GetWindow<AnalyzeResourcesWindow>();
//             window.titleContent = new GUIContent("AnalyzeResourcesWindow");
//             window.Show();
//         }

//         private void OnEnable()
//         {
//             var actionDict = new Dictionary<string, List<string>>(Contents.Length);
//             foreach (var content in Contents)
//             {
//                 var actions = new List<string>();
//                 actionDict.Add(content, actions);
//             }

//             var methods = typeof(AnalyzeResourcesWindow).GetMethods(BindingFlags.Public | BindingFlags.Instance);

//             foreach (var method in methods)
//             {
//                 foreach (var content in Contents)
//                 {
//                     if (Regex.IsMatch(method.Name, content))
//                     {
//                         actionDict[content].Add(method.Name);
//                     }
//                 }
//             }

//             foreach (var content in Contents)
//                 foldOutDatas.Add(new FoldOutData(content, actionDict[content].ToArray()));

//             InitEditorReflection();
//         }

//         private void InitEditorReflection() 
//         {
//             System.Type type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
//             getStorageMemorySizeLongMethodInfo = type.GetMethod("GetStorageMemorySizeLong", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//             getPreviewTextureMethodInfo = typeof(UnityEditor.U2D.SpriteAtlasExtensions).GetMethod("GetPreviewTextures", BindingFlags.NonPublic | BindingFlags.Static);
//             setPlatformSettingMethodInfo = typeof(UnityEditor.U2D.SpriteAtlasExtensions).GetMethod("SetPlatformSettings", BindingFlags.Public | BindingFlags.Static);
//             getPlatformSettingMethodInfo = typeof(UnityEditor.U2D.SpriteAtlasExtensions).GetMethod("GetPlatformSettings", BindingFlags.Public | BindingFlags.Static);
//         }

//         private void OnGUI()
//         {
//             EditorGUILayout.BeginVertical();
//             {
//                 for (int i = 0; i < foldOutDatas.Count; i++)
//                 {
//                     FoldOutData item = foldOutDatas[i];
//                     item.showPos = EditorGUILayout.Foldout(item.showPos, item.content);

//                     if (item.showPos)
//                     {
//                         foreach (var actionName in item.actioNames)
//                         {
//                             if (GUILayout.Button(actionName))
//                             {
//                                 InvokeFoldOutAction(actionName);
//                             }
//                         }
//                     }
//                 }
//             }

//             GUILayout.Space(15);
//             if (GUILayout.Button("Reimport All Assets")) OnLostFocus();
//             EditorGUILayout.EndVertical();
//         }

//         private void OnLostFocus()
//         {
//             // foreach (var item in importerDict) item.Value.SaveAndReimport();
//             if (importerDict.Count > 0) Debug.Log($"Reimports All Assets - {importerDict.Count}");
//             importerDict.Clear();
//         }

//         private void OnDisable()
//         {
//             importerDict.Clear();
//         }


//         #region  HelperClass

//         private void ExportData2Excel(List<string> values, string tableName = "sprite", string excelName = "SpriteInfo")
//         {
//             if (values.Count < 2) return;
//             //drawtreasure 1 1 10 100
//             string path = Application.dataPath + $"/{excelName}.xlsx";

//             Excel excel = ExcelHelper.LoadExcel(path);
//             if (excel == null)
//             {
//                 excel = ExcelHelper.CreateExcel(path);
//                 excel.Tables[0].TableName = tableName;
//             }

//             int tableIndex = excel.Tables.Count; //deafult tables length is one when CreateNewExcel 
//             for (int j = 0; j < excel.Tables.Count; j++)
//             {
//                 ExcelTable item = excel.Tables[j];
//                 if (item.TableName == tableName)
//                 {
//                     tableIndex = j;
//                     break;
//                 }
//             }

//             if (tableIndex == excel.Tables.Count)
//                 excel.AddTable(tableName);
//             ExcelTable excelTable = excel.Tables[tableIndex];


//             int startRow = 1;
//             excelTable.SetValue(startRow, 1, "ID");
//             excelTable.SetValue(startRow, 2, "资源名字");
//             excelTable.SetValue(startRow, 3, "资源路径");

//             for (int colIndex = 0; colIndex < values.Count; colIndex++)
//             {
//                 var titles = values[colIndex].Split(',');
//                 if (colIndex == 0)
//                 {
//                     for (int i = 0; i < titles.Length; i++)
//                     {
//                         excelTable.SetValue(colIndex + startRow, i + 4, titles[i]);
//                     }
//                 }
//                 else
//                 {
//                     excelTable.SetValue(colIndex + startRow, 1, colIndex.ToString());
//                     for (int i = 0; i < titles.Length; i++)
//                     {
//                         if (i == 0)
//                         {
//                             var fileNames = titles[i].Split('/');
//                             var fileName = fileNames[fileNames.Length - 1].Split('.');
//                             excelTable.SetValue(colIndex + startRow, i + 2, fileName[0]);
//                             excelTable.SetValue(colIndex + startRow, i + 3, titles[i]);
//                         }
//                         else
//                         {
//                             excelTable.SetValue(colIndex + startRow, 3 + i, titles[i]);
//                         }
//                     }
//                 }
//             }


//             ExcelHelper.SaveExcel(excel, path);

//         }

//         private void InvokeFoldOutAction(string actionName)
//         {
//             System.Reflection.MethodInfo methodInfo = typeof(AnalyzeResourcesWindow).GetMethod(actionName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
//             methodInfo.Invoke(this, null);
//         }

//         private long GetInspectorMemory(UnityEngine.Object obj)
//         {
//             if (obj == null)
//             {
//                 Debug.Log("obj is empty, unable to get the hard disk size");
//                 return 0;
//             }
//             Sprite sprite = obj as Sprite;
//             Texture texture = null;
//             if (sprite != null)
//             {
//                 texture = sprite.texture;
//             }
//             if (texture == null)
//             {
//                 texture = obj as Texture;
//             }
//             if (texture != null)
//             {
//                 // Debug.Log("Memory usage:" + EditorUtility.FormatBytes(UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(texture)));
//                 // Debug.Log("Hard disk occupation:" + EditorUtility.FormatBytes((int)GetStorageMemorySizeMethodInfo.Invoke(null, new object[] { texture })));
//                 return (long)getStorageMemorySizeLongMethodInfo.Invoke(null, new object[] { texture });
//             }
//             return 0;
//         }

//         private void GroupAssetToFolder(string[] assetPath, string destPath)
//         {
//             if (!Directory.Exists(destPath))
//             {
//                 Directory.CreateDirectory(destPath);
//             }

//             foreach (var path in assetPath)
//             {
//                 AssetDatabase.MoveAsset(path, destPath + path.Substring(path.LastIndexOf('/')));
//             }
//         }

//         #endregion
//     }


//     public class FoldOutData
//     {
//         public bool showPos = true;
//         public string content;
//         public List<string> actioNames;

//         public FoldOutData(string content, string[] actioName)
//         {
//             this.content = content;
//             this.actioNames = new List<string>(actioName);
//         }

//     }

//     public static class ExtensionClass
//     {
//         #region Cache Reflection
// #if UNITY_2018_4_13
// #endif
//         private static MethodInfo GetSourceTextureInformationMethodInfo;

//         #endregion



//         public static UnityEditor.AssetImporters.SourceTextureInformation GetSourceTextureInformation(this TextureImporter importer)
//         {
//             if (GetSourceTextureInformationMethodInfo == null) GetSourceTextureInformationMethodInfo = typeof(TextureImporter).GetMethod("GetSourceTextureInformation", BindingFlags.Instance | BindingFlags.NonPublic);
//             return GetSourceTextureInformationMethodInfo.Invoke(importer, null) as UnityEditor.AssetImporters.SourceTextureInformation;
//         }

//         public static void getSourceTextureWidthAndHeight(this TextureImporter importer, out int width, out int height)
//         {
// #if UNITY_2021_2_OR_NEWER
//             importer.GetSourceTextureWidthAndHeight(out width, out int height);
// #else
//             var info = importer.GetSourceTextureInformation();
//             if (info.width == -1)
//                 throw new System.InvalidOperationException("The texture has not yet finished importing. This most likely means this method was called in an AssetPostprocessor.OnPreprocessAsset callback.");
//             width = info.width;
//             height = info.height;
// #endif
//         }
//     }



// }
