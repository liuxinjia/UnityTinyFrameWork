using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Editor.Excels;
using System.Text;
using Unity.PerformanceTesting;
using System;

[PrebuildSetup(nameof(TestExcelInit))]
public class TestExcelTools
{
    #region Test Entries



    [Test, Performance, Order(0)]
    public void Profile_WriteExcel()
    {

        Measure.Method(() =>
        {
            Write(TestExcelInit.TableName_performance);
        })
            .MeasurementCount(10)
            .GC()
            .Run();

    }

    [Test, Performance, Order(1)]
    public void Profile_ReadExcel()
    {

        Measure.Method(() =>
        {
            Read(TestExcelInit.TableName_performance);
        })
            .MeasurementCount(10)
            .GC()
            .Run();

    }


    [Test, Order(2)]
    public void TestValidateExcel()
    {
        Write(TestExcelInit.TableName_1);
        Read(TestExcelInit.TableName_1);
    }


    [Test, Order(3)]
    public void TestConvertToDataTables()
    {
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        tableReader.ConvertToDataTable();
    }
    [Test, Order(4)]
    public void TestConvertToJson()
    {
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        tableReader.ConvertToJson(TestExcelInit.JsonPath, Encoding.UTF8);
    }
    [Test, Order(5)]
    public void TestConvertToXml()
    {
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        tableReader.ConvertToXml(TestExcelInit.XMLPath);
    }


    [Test]
    public void TestExcel_ReadyBykey()
    {
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        Assert.AreEqual(Convert.ToInt16(tableReader.GetCell(0, "Short_0")), 4);
        Assert.AreEqual(Convert.ToInt32(tableReader.GetCell(0, "Int_0")), 4);
        Assert.AreEqual(Convert.ToSingle(tableReader.GetCell(5, "Float_1")), 20.0f);
        Assert.AreEqual(Convert.ToString(tableReader.GetCell(6, "String_0")), "22");
        Assert.AreEqual(Convert.ToBoolean(tableReader.GetCell(3, "Bool_0")), true);

        IList actualList = (Array)tableReader.GetCell(14, "IntArray_2");
        for (int j = 0; j < actualList.Count; j++)
        {
            object arrayNum = actualList[j];
            Assert.AreEqual(49 + j, arrayNum);
        }
    }

    [Test]
    public void TestCreateMulitpleExcels()
    {
        Write(TestExcelInit.TableName_1);
        Read(TestExcelInit.TableName_1);
        Write(TestExcelInit.TableName_2);
        Read(TestExcelInit.TableName_2);

        Write(TestExcelInit.TableName_3);
        Read(TestExcelInit.TableName_3);
        Write(TestExcelInit.TableName_4);
        Read(TestExcelInit.TableName_4);
    }


    #endregion

    #region Test Methods
    private static void Write(string tableName)
    {
        var excelWriter = new ExcelWriter(TestExcelInit.FilePath);

        var table = excelWriter.CreateTable(tableName);
        //header Titles and types
        var headers = new List<string>();
        var types = new List<Type>();
        for (int col = 0; col < TestExcelInit.shortLength; col++) { headers.Add($"Short_{col}"); types.Add(typeof(short)); }
        for (int col = 0; col < TestExcelInit.intLength; col++) { headers.Add($"Int_{col}"); types.Add(typeof(int)); }
        for (int col = 0; col < TestExcelInit.floatLength; col++) { headers.Add($"Float_{col}"); types.Add(typeof(float)); }
        for (int col = 0; col < TestExcelInit.stringLength; col++) { headers.Add($"String_{col}"); types.Add(typeof(string)); }
        for (int col = 0; col < TestExcelInit.boolLength; col++) { headers.Add($"Bool_{col}"); types.Add(typeof(bool)); }
        for (int col = 0; col < TestExcelInit.intArrayLength; col++) { headers.Add($"IntArray_{col}"); types.Add(typeof(int[])); }
        for (int col = 0; col < TestExcelInit.stringArrayLength; col++) { headers.Add($"StringArray_{col}"); types.Add(typeof(string[])); }
        //Comments
        List<string> comments = new()
        {
            "Comment"
        };
        
        for (int col = 0; col < TestExcelInit.Length; col++) comments.Add($"#Comment_{col}");

        table.InitHeaders(types, headers, comments);


        for (int i = 0; i < TestExcelInit.datas.Count; i++)
        {
            int row = i / TestExcelInit.Length;
            int col = i % TestExcelInit.Length;
            table.SetValue(row, col, TestExcelInit.datas[i]);
        }
        excelWriter.SaveExcels();
    }

    private static void Read(string tableName)
    {
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(tableName);
        for (int i = 0; i < TestExcelInit.Length; i++)
        {
            int rowId = i;
            var list = tableReader.GetRowsByID(rowId);

            int col = 0;
            int length = TestExcelInit.shortLength;
            for (; col < length; col++) { Assert.AreEqual(TestExcelInit.datas[rowId * TestExcelInit.Length + col], System.Convert.ToInt16(list[col])); }
            length += TestExcelInit.intLength;
            for (; col < length; col++) { Assert.AreEqual(TestExcelInit.datas[rowId * TestExcelInit.Length + col], System.Convert.ToInt32(list[col])); }
            length += TestExcelInit.floatLength;
            for (; col < length; col++) { Assert.AreEqual(TestExcelInit.datas[rowId * TestExcelInit.Length + col], System.Convert.ToSingle(list[col])); }
            length += TestExcelInit.stringLength;
            for (; col < length; col++) { Assert.AreEqual(TestExcelInit.datas[rowId * TestExcelInit.Length + col].ToString(), list[col].ToString()); }
            length += TestExcelInit.boolLength;
            for (; col < length; col++) { Assert.AreEqual(TestExcelInit.datas[rowId * TestExcelInit.Length + col], System.Convert.ToBoolean(list[col])); }
            length += TestExcelInit.intArrayLength;
            for (; col < length; col++)
            {
                var expectedList = new List<int>();
                foreach (var arrayNum in (Array)TestExcelInit.datas[rowId * TestExcelInit.Length + col])
                {
                    expectedList.Add((int)arrayNum);
                }
                IList actualList = (Array)list[col];
                for (int j = 0; j < actualList.Count; j++)
                {
                    object arrayNum = actualList[j];
                    Assert.AreEqual(expectedList[j], arrayNum);
                }
            }

            length += TestExcelInit.stringArrayLength;
            for (; col < length; col++)
            {
                var expectedList = new List<string>();
                foreach (var arrayNum in (Array)TestExcelInit.datas[rowId * TestExcelInit.Length + col])
                {
                    expectedList.Add(arrayNum.ToString());
                }
                IList actualList = (Array)list[col];
                for (int j = 0; j < actualList.Count; j++)
                {
                    object arrayNum = actualList[j];
                    Assert.AreEqual(expectedList[j], arrayNum.ToString());
                }
            }
        }
    }

    #endregion

}
