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
    #region Test Entries

    [Test]
    public void TestValidateExcel()
    {
        Write(TestInit.TableName_1);
        Read(TestInit.TableName_1);
    }

    [Test]
    public void TestCreateMulitpleExcels()
    {
        Write(TestInit.TableName_1);
        Read(TestInit.TableName_1);
        Write(TestInit.TableName_2);
        Read(TestInit.TableName_2);

        Write(TestInit.TableName_3);
        Read(TestInit.TableName_3);
        Write(TestInit.TableName_4);
        Read(TestInit.TableName_4);
    }


    [Test, Performance]
    public void Profile_WriteExcel()
    {

        Measure.Method(() =>
        {
            Write(TestInit.TableName_performance);
        })
            .MeasurementCount(10)
            .GC()
            .Run();

    }

    [Test, Performance]
    public void Profile_ReadExcel()
    {

        Measure.Method(() =>
        {
            Read(TestInit.TableName_performance);
        })
            .MeasurementCount(10)
            .GC()
            .Run();

    }

    #endregion

    #region Test Methods
    private static void Write(string tableName)
    {
        var excelWriter = new ExcelWriter(TestInit.FilePath);

        var table = excelWriter.CreateTable(tableName);
        //headerTitles
        var headers = new List<string>();
        for (int col = 0; col < TestInit.Length; col++) headers.Add($"Num_{col}");
        //Comments
        var comments = new List<string>();
        comments.Add("Comment");
        for (int col = 0; col < TestInit.Length; col++) comments.Add($"#Comment_{col}");
        table.InitHeaders(typeof(int), headers, comments);


        for (int i = 0; i < TestInit.datas.Count; i++)
        {
            int row = i / TestInit.Length;
            int col = i % TestInit.Length;
            table.SetValue(row, col, TestInit.datas[i]);
        }
        excelWriter.SaveExcels();
    }

    private static void Read(string tableName)
    {
        var excelReader = new ExcelReader(TestInit.FilePath);
        var tableReader = excelReader.GetTableReader(tableName);
        for (int i = 0; i < TestInit.Length; i++)
        {
            int rowId = i;
            var list = tableReader.GetRowsByID(rowId);
            for (int j = 0; j < list.Count; j++)
            {
                Assert.AreEqual(TestInit.datas[rowId * TestInit.Length + j], System.Convert.ToInt32(list[j]));
            }
        }
    }

    #endregion

}
