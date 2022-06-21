// namespace Cr7SundTools
// {
//     using UnityEngine;
//     using UnityEditor;
//     using System.Collections.Generic;
//     using System.Text.RegularExpressions;
//     using System.Text;

//     public partial class AnalyzeResourcesWindow
//     {

//         void AnalyzeModel()
//         {
//             var textureGUIDs = AssetDatabase.FindAssets("t:model", new[] { "Assets/" });

//             foreach (var guid in textureGUIDs)
//             {
//                 var path = AssetDatabase.GUIDToAssetPath(guid);
//                 var asset = AssetDatabase.LoadMainAssetAtPath(path);
//             }
//         }

//         public void AnalyzeModels()
//         {
//             string path = Application.dataPath + $"/ModelInfo.xlsx";
//             System.IO.File.Delete(path);
//             AnalyzeModelReadable();

//             Application.OpenURL(path);
//         }

//         internal void AnalyzeModelAnimations()
//         {
//             this.ShowNotification(new GUIContent("不做先"));
//         }

//         public void AnalyzeModelFormat()
//         {
//             var modelGUIDs = AssetDatabase.FindAssets("t:Model");
//             var petPaths = new List<string>();
//             for (int i = 0; i < modelGUIDs.Length; i++)
//             {
//                 string guid = modelGUIDs[i];
//                 string path = AssetDatabase.GUIDToAssetPath(guid);

//                 if (!path.ToLower().EndsWith(".fbx"))
//                 {
//                     var str = path.Substring(path.LastIndexOf('.') + 1);
//                     petPaths.Add($"{path},{str}");
//                 }
//             }

//             var titleNames = new List<string>();
//             titleNames.Add("格式");
//             ExportModelData2Excel(petPaths, "不同格式的模型", titleNames);
//         }

//         internal class ModelOutputInfo
//         {
//             public string name;
//             public List<string> resultPaths;
//             public bool isImport;
//             public Regex regex;

//             public ModelOutputInfo(string keyPath, bool import = true)
//             {
//                 if (!string.IsNullOrEmpty(keyPath))
//                 {
//                     var strs = keyPath.Split('/');
//                     this.name = strs[strs.Length - 1];
//                     if (string.IsNullOrEmpty(this.name)) Debug.LogError($"Please Delete the last / in {keyPath}");
//                     this.regex = new Regex(keyPath);
//                 }
//                 else
//                 {
//                     this.name = "Others";
//                 }
//                 this.isImport = import;
//                 resultPaths = new List<string>();
//             }
//         }

//         public void AnalyzeModelReadable()
//         {
//             var petOutputInfo = new ModelOutputInfo("Assets/Art/Character/Pet");
//             var monsterOutputInfo = new ModelOutputInfo("Assets/Art/Character/NewMonster");
//             var wingOutputInfo = new ModelOutputInfo("Assets/Art/Character/Wings");
//             var mountOutputInfo = new ModelOutputInfo("Assets/Art/Character/ZuoQi");
//             var characterOutputInfo = new ModelOutputInfo("Assets/Art/Character/PC", false);
//             var effectOutputInfo = new ModelOutputInfo("Assets/Art/Effect", false);
//             var sceneResourceOutputInfo = new ModelOutputInfo("Assets/Art/Scene resources", false);
//             var otherOutputInfo = new ModelOutputInfo("", false);

//             var outputInfos = new List<ModelOutputInfo>();
//             outputInfos.Add(petOutputInfo);
//             outputInfos.Add(monsterOutputInfo);
//             outputInfos.Add(wingOutputInfo);
//             outputInfos.Add(mountOutputInfo);
//             outputInfos.Add(characterOutputInfo);
//             outputInfos.Add(effectOutputInfo);
//             outputInfos.Add(sceneResourceOutputInfo);
//             outputInfos.Add(otherOutputInfo);

//             var modelGUIDs = AssetDatabase.FindAssets("t:Model");
//             var excludeRegexs = new[] { new Regex("CinemachineExamples"), new Regex("Cinema Suite"), new Regex("Plugins") };
//             AssetDatabase.StartAssetEditing();
//             for (int i = 0; i < modelGUIDs.Length; i++)
//             {
//                 string guid = modelGUIDs[i];
//                 string path = AssetDatabase.GUIDToAssetPath(guid);

//                 if (true)
//                 {
//                     EditorUtility.DisplayProgressBar("reimport Model " + i + "/" + modelGUIDs.Length, path, (float)i / modelGUIDs.Length);
//                     ModelImporter modelImporter = ModelImporter.GetAtPath(path) as ModelImporter;

//                     bool isReadable = modelImporter.isReadable;
//                     if (isReadable)
//                     {
//                         bool isSkip = false;

//                         foreach (var excludeRegex in excludeRegexs)
//                         {
//                             if (excludeRegex.IsMatch(path)) { isSkip = true; break; }
//                         }
//                         if (isSkip) continue;

//                         if (modelImporter.addCollider) Debug.Log(path);

//                         foreach (var outputInfo in outputInfos)
//                         {
//                             if (outputInfo.regex != null && outputInfo.regex.IsMatch(path))
//                             {
//                                 outputInfo.resultPaths.Add($"{path},");
//                                 isSkip = true;
//                                 if (outputInfo.isImport) { modelImporter.isReadable = false; modelImporter.SaveAndReimport(); }
//                                 break;
//                             }
//                         }

//                         if (!isSkip) otherOutputInfo.resultPaths.Add(path);
//                     }
//                 }
//             }
//             EditorUtility.ClearProgressBar();

//             AssetDatabase.StopAssetEditing();

//             var titleNames = new List<string>();
//             foreach (var outputInfo in outputInfos)
//             {
//                 Debug.Log(outputInfo.name);
//                 ExportModelData2Excel(outputInfo.resultPaths, outputInfo.name, titleNames);
//             }


//             Debug.Log("finish");
//         }


//         internal void ExportModelData2Excel(List<string> values, string tableName = "effect", List<string> titles = null)
//         {
//             var sb = new StringBuilder();
//             foreach (var item in titles)
//             {
//                 sb.Append($"{item},");
//             }
//             values.Insert(0, sb.ToString());

//             ExportData2Excel(values, tableName, "ModelInfo");
//         }



//     }
// }