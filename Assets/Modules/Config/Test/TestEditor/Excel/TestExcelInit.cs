using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;
using Cr7Sund.Editor;

public class TestExcelInit : IPrebuildSetup
{
    public static string FilePath = @"ConfigOutputs/Excels/Cr7TestExample.xlsx";
    public static string JsonPath = @"ConfigOutputs/Jsons/Cr7TestExample.json";
    public static string XMLPath = @"ConfigOutputs/XMLs/Cr7TestExample.xml";
    public static string fileRelativePath = @"ConfigOutputs/Excels/Cr7Example.xlsx";
    private static readonly string jsonRelativePath = @"ConfigOutputs/Jsons/Cr7TestExample.json";
    public static string XMLRelativePath = @"ConfigOutputs/XMLs/Cr7TestExample.xml";

    public const string TableName_1 = "Test1";
    public const string TableName_2 = "Test2";
    public const string TableName_3 = "Test3";
    public const string TableName_4 = "Test4";
    public const string TableName_performance = "TableName_performance";
    public static List<object> datas = new();
    public static int Length => shortLength + intLength + floatLength + stringLength + boolLength + intArrayLength + stringArrayLength;

    public static int shortLength = 3;
    public static int intLength = 3;
    public static int floatLength = 3;

    public static int stringLength = 3;
    public static int boolLength = 3;

    public static int intArrayLength = 3;
    public static int stringArrayLength = 3;
    public void Setup()
    {
        InitExcelDatas();
    }

    public void InitExcelDatas()
    {
        FilePath = EditorUtil.GetProjectFolderAbsPath(fileRelativePath);
        JsonPath = EditorUtil.GetProjectFolderAbsPath(jsonRelativePath);
        XMLPath = EditorUtil.GetProjectFolderAbsPath(XMLRelativePath);
        
        datas.Clear();
        for (int row = 0; row < Length; row++)
        {

            for (int col = 0; col < shortLength; col++)
            {
                int v = ((row + 1) * shortLength + (col + 1));
                datas.Add((short)v);
            }

            for (int col = 0; col < intLength; col++)
            {
                int v = ((row + 1) * intLength + (col + 1));
                datas.Add((int)v);
            }

            for (int col = 0; col < floatLength; col++)
            {
                int v = ((row + 1) * floatLength + (col + 1));
                datas.Add((float)v);
            }

            for (int col = 0; col < stringLength; col++)
            {
                int v = ((row + 1) * stringLength + (col + 1));
                datas.Add(v.ToString());
            }

            for (int col = 0; col < boolLength; col++)
            {
                datas.Add(col % 2 == 0);
            }

            for (int col = 0; col < intArrayLength; col++)
            {
                int v = ((row + 1) * intArrayLength + (col + 1));
                datas.Add(new int[] { v + 1, v + 2, v + 3 });
            }

            for (int col = 0; col < stringArrayLength; col++)
            {
                int v = ((row + 1) * stringArrayLength + (col + 1));
                datas.Add(new string[] { $"{v + 1}", $"{v + 2}", $"{v + 3}" });
            }
        }
    }
}