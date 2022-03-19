
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

public class OptimizeAnim
{

    private const string ImportAnimPath = "ImportAnimations";

    [MenuItem("Tools/Optimize/Fbx/DoAnimOptimize %l")]
    public static void DoFbxAnimOptimize()
    {
        EditorUtil.Instance.ClearConsoleLog();

        var files = AssetDatabase.GetAllAssetPaths();
        List<string> pathList = new List<string>();
        foreach (var path in files)
        {
            if (path.ToLower().EndsWith("fbx"))
            {
                pathList.Add(path);
            }
        }


        for (int i = 0; i < pathList.Count; i++)
        {
            bool isModified = false;
            bool hasAnim = false;
            string path = pathList[i];

            var cancel = EditorUtility.DisplayCancelableProgressBar("parsing fbx " + i + "/" + pathList.Count, path, (float)i / pathList.Count);
            if (cancel)
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);


            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            string modelName = go.name;
            var newClipFileInfoDict = new Dictionary<string, AnimFileInfo>();
            var oldClipFileInfoDict = new Dictionary<string, AnimFileInfo>();

            foreach (var obj in objs)
            {
                if (obj is AnimationClip clip)
                {
                    // isModified = OptimizeAnimationClip(path, clip);
                    hasAnim = true;

                    string prefixFolder = clip.name;

                    string directoryName = string.Empty;
                    if (!string.IsNullOrEmpty(modelName))
                    {
                        directoryName = $"{path.Substring(0, path.Length - modelName.Length - ".fbx".Length)}{ImportAnimPath}";

                        directoryName.Replace('@', '_');

                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                    }
                    var destAnimPath = $"{directoryName}/{modelName}.anim";
                    var newClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(destAnimPath);
                    if (newClip != null) AssetDatabase.DeleteAsset(destAnimPath);
                    newClip = OptimizeAndCreateAnimationClip(destAnimPath, clip, false, false);

                    var tmpPath = destAnimPath.Replace(modelName, modelName + 1);
                    var tmpClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(tmpPath);
                    if (tmpClip != null) AssetDatabase.DeleteAsset(tmpPath);
                    tmpClip = OptimizeAndCreateAnimationClip(tmpPath, clip, false, false);

                    LogOptimizedInfo(tmpClip, newClip, tmpPath, destAnimPath);
                    AssetDatabase.DeleteAsset(tmpPath);
                }
            }
            if (hasAnim)
            {
                // AssetDatabase.DeleteAsset(path);
            }

            if (isModified)
            {
                AssetDatabase.ImportAsset(path);
            }
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.Refresh();
        Debug.LogError($"Easy!!! Easy !!! Finish Successfully ");
    }

    private static AnimationClip OptimizeAndCreateAnimationClip(string path, AnimationClip clip, bool excludeScale = true, bool lowerPrecison = true)
    {
        var newClip = new AnimationClip();

        AssetDatabase.CreateAsset(newClip, path);
        try
        {
            foreach (var curveBinding in AnimationUtility.GetCurveBindings(clip))
            {
                // 1. 去除动画文件的Scale 信息
                if (excludeScale)
                {
                    string name = curveBinding.propertyName.ToLower();
                    if (name.Contains("scale"))
                    {
                        AnimationUtility.SetEditorCurve(newClip, curveBinding, null);
                    }
                }
                var curve = AnimationUtility.GetEditorCurve(clip, curveBinding);
                if (curve == null || curve.keys == null) continue;
                var keyFrames = curve.keys;
                for (var j = 0; j < keyFrames.Length; j++)
                {
                    var key = keyFrames[j];

                    if (lowerPrecison)
                    {
                        float newValue;
                        if (TryConvertToStandardNumericFormatFloat(key.value, out newValue))
                        { key.value = newValue; }
                        if (TryConvertToStandardNumericFormatFloat(key.inTangent, out newValue))
                        { key.inTangent = newValue; }
                        if (TryConvertToStandardNumericFormatFloat(key.outTangent, out newValue))
                        { key.outTangent = newValue; }
                    }
                    keyFrames[j] = key;
                }

                curve.keys = keyFrames;
                newClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, curve);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("CompressAnimationClip Failed !!! animationPath : {0} error: {1}", path, e.StackTrace));
        }

        return newClip;
    }

    private static bool TryConvertToStandardNumericFormatFloat(float value, out float result, string numericFormat = "f3")
    {
        result = float.Parse(value.ToString(numericFormat));
        return result != value;
    }


    #region LogInfo

    private static MethodInfo getAnimationClipStats;
    private static FieldInfo sizeInfo;
    private struct AnimFileInfo
    {
        public long animationClipStatsSize;
        public long fileSize;
        public long memorySize;
    }

    private static void Init()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        getAnimationClipStats = typeof(AnimationUtility).GetMethod("GetAnimationClipStats", BindingFlags.NonPublic | BindingFlags.Static);
        var animclipStats = assembly.GetType("UnityEditor.AnimationClipStats");
        sizeInfo = animclipStats.GetField("size", BindingFlags.Public | BindingFlags.Instance);
    }

    private static void LogOptimizedInfo(AnimationClip clip, string filPath)
    {
        if (sizeInfo == null) Init();
        Debug.LogFormat("{0} FileSize: <color=#e64d32>{1}</color>, MemorySize: <color=#64cc98>{2}</color>, InspectorSize: <color=#66d4ff>{3}</color>", clip.name, GetFileSize(filPath), GetMemorySize(clip),
            GetAnimationClipStatsSize(clip));
    }

    private static void LogOptimizedInfo(AnimationClip clip, AnimationClip newClip, string filPath, string newFilePath)
    {
        if (sizeInfo == null) Init();
        Debug.LogFormat("{0} FileSize: <color=#e64d32>{1} -> {2}</color>, MemorySize: <color=#64cc98>{3} -> {4}</color>, InspectorSize: <color=#66d4ff>{5} -> {6}</color>",
        clip.name,
        GetFileSize(filPath), GetFileSize(newFilePath),
        GetMemorySize(clip), GetMemorySize(newClip),
        GetAnimationClipStatsSize(clip), GetAnimationClipStatsSize(newClip)
        );
    }

    /// <summary>
    /// InspectorSize 
    /// </summary>
    /// <returns></returns>
    private static long GetAnimationClipStatsSize(AnimationClip clip)
    {
        var stats = getAnimationClipStats.Invoke(null, new object[] { clip });

        var fileSize = stats.GetType().GetField("size", BindingFlags.Public | BindingFlags.Instance);
        return (int)fileSize.GetValue(stats);
    }

    private static long GetFileSize(string filePath)
    {
        var filInfo = new FileInfo(filePath);
        return filInfo.Length;
    }

    private static long GetMemorySize(AnimationClip clip) => Profiler.GetRuntimeMemorySizeLong(clip);

    #endregion

}

