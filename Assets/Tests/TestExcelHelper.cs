using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;

public class TestExcelHelper
{
    private const string FilePath = @"C:\Users\liux4\Documents\UnityProjects\UnityTinyFrameWork\Assets\Cr7Example.xlsx";
    private const string SheetName = "west";

    // A Test behaves as an ordinary method
    [Test]
    public void CreateOneExcel()
    {
        var excelWriter = new NewExcelWriter(FilePath, 0, 2);

        int length = 9;
        var table = excelWriter.AddTable("west");
        var sb = new StringBuilder();
        for (int col = 0; col < length; col++) sb.Append($"Num_{col},");
        table.InitHeaders(sb.ToString().Split(",")); //last
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                table.SetValue(row, col, row * length + col);
            }
        }

        excelWriter.WriteTo(FilePath);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void CreateMulitpleExcel()
    {
        var excelWriter = new NewExcelWriter(FilePath, 0, 2);

        int length = 9;
        var table = excelWriter.AddTable("west");
        var sb = new StringBuilder();
        for (int col = 0; col < length; col++) sb.Append($"Num_{col},");
        table.InitHeaders(sb.ToString().Split(",")); //last
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                table.SetValue(row, col, row * length + col);
            }
        }

        length = 19;
        var table2 = excelWriter.AddTable("west2");
        sb = new StringBuilder();
        for (int col = 0; col < length; col++) sb.Append($"Num_{col},");
        table2.InitHeaders(sb.ToString().Split(","));
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                table2.SetValue(row, col, row * length + col);

            }
        }

        excelWriter.WriteTo(FilePath);
    }

    [Test]
    public void CreateFail()
    {
        Debug.LogError("west");
    }

}
