using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;
using Unity.PerformanceTesting;

[PrebuildSetup("TestInit")]
public class TestExcelTools
{
    [Test, Performance]
    public void CreateExcel()
    {

        Measure.Method(() =>
        {
            var excelWriter = new ExcelWriter(TestInit.FilePath, 0, 1);

            var table = excelWriter.CreateTable("Test", true, true);
            var sb = new StringBuilder();
            for (int col = 0; col < TestInit.length - 1; col++) sb.Append($"ID_{col},");
            sb.Append($"Num_{TestInit.length - 1}");
            table.InitHeaders(typeof(int), sb.ToString().Split(",")); //last

            for (int i = 0; i < TestInit.datas.Count; i++)
            {
                int row = i / TestInit.length;
                int col = i % TestInit.length;
                table.SetValue(row, col, TestInit.datas[i]);
            }
            excelWriter.SaveExcels();

        })
            .MeasurementCount(20)
            .GC()
            .Run();

    }

    [Test]
    public void CreateExcel_WithoutID()
    {
        var excelWriter = new ExcelWriter(TestInit.FilePath, 0, 1);

        var table = excelWriter.CreateTable("Test_WithoutID", false, false);
        var sb = new StringBuilder();
        for (int col = 0; col < TestInit.length - 1; col++) sb.Append($"Num_{col},");
        sb.Append($"Num_{TestInit.length - 1}");
        table.InitHeaders(typeof(int), sb.ToString().Split(",")); //last

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
        var excelReader = new ExcelReader(TestInit.FilePath, 0, 1);
        var tableReader = excelReader.GetTableReader("Test");
        int rowIndex = 2;
        var list = tableReader.GetRowsByID(rowIndex);
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
        var table = excelWriter.CreateTable("Test", true, true);
        var sb = new StringBuilder();
        for (int col = 0; col < length - 1; col++) sb.Append($"Num_{col},");
        sb.Append($"Num_{length - 1}");
        table.InitHeaders(typeof(int), sb.ToString().Split(",")); //last
        for (int row = 0; row < length; row++)
        {
            for (int col = 0; col < length; col++)
            {
                table.SetValue(row, col, row * length + col);
            }
        }

        excelWriter.SaveExcels();

        excelWriter = new ExcelWriter(TestInit.FilePath, 0, 1);
        length = 19;
        var table2 = excelWriter.CreateTable("Test2", false, false);
        sb = new StringBuilder();
        for (int col = 0; col < length - 1; col++) sb.Append($"Num_{col},");
        sb.Append($"Num_{length - 1}");
        table2.InitHeaders(typeof(int), sb.ToString().Split(","));

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
