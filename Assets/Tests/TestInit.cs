using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;
public class TestInit : IPrebuildSetup
{
    public static string FilePath = @"Cr7Example.xlsx";
    public static string fileRelativePath = @"Cr7Example.xlsx";

    public static List<int> datas = new List<int>();
    public static int length = 9;


    public void Setup()
    {
        InitExcelDatas();
    }

    public void InitExcelDatas()
    {
        FilePath = EditorUtil.GetAssetAbsolutePath(fileRelativePath);

        datas.Clear();
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                datas.Add(row * length + col);
            }
        }
    }
}