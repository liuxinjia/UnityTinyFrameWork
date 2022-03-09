using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;

[PrebuildSetup("TestInit")]
public class TestExcelTools
{
    [Test]
    public void CreateExcel()
    {
        var excelWriter = new ExcelWriter(TestInit.FilePath, 0, 1);

        var table = excelWriter.CreateTable("west", true, true);
        var sb = new StringBuilder();
        for (int col = 0; col < TestInit.length - 1; col++) sb.Append($"Num_{col},");
        sb.Append($"Num_{TestInit.length - 1}");
        table.InitHeaders(sb.ToString().Split(",")); //last

        for (int i = 0; i < TestInit.datas.Count; i++)
        {
            int row = i / TestInit.length;
            int col = i % TestInit.length;
            table.SetValue(row, col, TestInit.datas[i]);
        }

        excelWriter.SaveExcels();
    }


    [Test]
    public void ReadExcel()
    {

        var excelReader = new ExcelReader(TestInit.FilePath, "west", 0, 1);
        int rowIndex = 2;
        var list = excelReader.GetRowsByID(rowIndex);
        for (int i = 0; i < list.Count; i++)
        {
            object item = list[i];
            if (TestInit.datas[rowIndex * TestInit.length + i] != (int)list[i]) // has ID column
                Debug.LogError($"{i}-{item}");
        }
    }

    [Test]
    public void CreateMulitpleExcels()
    {
        var excelWriter = new ExcelWriter(TestInit.FilePath, 0, 1);

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

}
