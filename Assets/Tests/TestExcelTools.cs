using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;

public class TestExcelTools : IPrebuildSetup
{
    private static string FilePath = @"Cr7Example.xlsx";
    private string fileRelativePath = @"Cr7Example.xlsx";
    private List<int> datas = new List<int>();
    private int length = 9;

    public void Setup()
    {
        FilePath = EditorUtil.Instance.GetAssetAbsolutePath(fileRelativePath);

        InitDatas();
    }

    private void InitDatas()
    {
        datas.Clear();
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                datas.Add(row * length + col);
            }
        }
    }

    // A Test behaves as an ordinary method
    [Test]
    public void CreateOneExcel()
    {
        var excelWriter = new NewExcelWriter(FilePath, 0, 1);

        var table = excelWriter.CreateTable("west", true, true);
        var sb = new StringBuilder();
        for (int col = 0; col < length - 1; col++) sb.Append($"Num_{col},");
        sb.Append($"Num_{length - 1}");
        table.InitHeaders(sb.ToString().Split(",")); //last

        for (int i = 0; i < datas.Count; i++)
        {
            int row = i / length;
            int col = i % length;
            table.SetValue(row, col, datas[i]);
        }

        excelWriter.SaveExcels();
    }

    // A Test behaves as an ordinary method
    [Test]
    public void CreateMulitpleExcel()
    {
        var excelWriter = new NewExcelWriter(FilePath, 0, 1);

        int length = 9;
        var table = excelWriter.CreateTable("west", true, true);
        var sb = new StringBuilder();
        for (int col = 0; col < length - 1; col++) sb.Append($"Num_{col},");
        sb.Append($"Num_{length - 1}");
        table.InitHeaders(sb.ToString().Split(",")); //last
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                table.SetValue(row, col, row * length + col);
            }
        }

        length = 19;
        var table2 = excelWriter.CreateTable("west2", false, false);
        sb = new StringBuilder();
        for (int col = 0; col < length - 1; col++) sb.Append($"Num_{col},");
        sb.Append($"Num_{length - 1}");
        table2.InitHeaders(sb.ToString().Split(","));

        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                table2.SetValue(row, col, row * length + col);
            }
        }

        excelWriter.SaveExcels();
    }

    [Test]
    public void ReadExcel()
    {

        var excelReader = new ExcelReader(FilePath, "west", 0, 1);
        int rowIndex = 2;
        var list = excelReader.GetRowsByID(rowIndex);
        for (int i = 0; i < list.Count; i++)
        {
            object item = list[i];
            if (datas[rowIndex * length + i] != (int)list[i]) // has ID column
                Debug.LogError($"{i}-{item}");
        }
    }

}
