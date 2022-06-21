using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;
public class TestInit : IPrebuildSetup
{
    public static string FilePath = @"ExcelOutputs/Test/Cr7TestExample.xlsx";
    public static string fileRelativePath = @"ExcelOutputs/Test/Cr7Example.xlsx";
    public const string TableName_1 = "Test1";
    public const string TableName_2 = "Test2";
    public const string TableName_3 = "Test3";
    public const string TableName_4 = "Test4";
    public const string TableName_performance = "TableName_performance";
    public static List<int> datas = new List<int>();
    public static int Length = 9;


    public void Setup()
    {
        InitExcelDatas();
    }

    public void InitExcelDatas()
    {
        FilePath = EditorUtil.GetProjectFolderAbsPath(fileRelativePath);

        datas.Clear();
        for (int row = 0; row < Length; row++)
        {
            for (int col = 0; col < Length; col++)
            {
                datas.Add((row + 1) * Length + (col + 1));
            }
        }
    }
}