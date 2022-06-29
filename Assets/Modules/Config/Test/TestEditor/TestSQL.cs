using System.Collections;
using System.Collections.Generic;
using Cr7Sund.Editor.Excels;
using Cr7Sund.Core.Persistance;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Core;
using NPOI.SS.UserModel;
using Cr7Sund.Editor;
using Mono.Data.Sqlite;
using System;
using System.Text;

public class TestSQL
{
    #region  TestMethods
    [Test, Order(0)]
    public void TestCreateTable()
    {
        TestExcelInit.FilePath = EditorUtil.GetProjectFolderAbsPath(TestExcelInit.fileRelativePath);
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        tableReader.ConvertToRowDatas();

        string tableName = tableReader.SheetName;

        using (var sqlCtrl = new SQLController())
        {
            sqlCtrl.OpenConnection("TestDB.sqlite");

            var columnValues = new List<string>();
            var columnSqlTypes = new List<string>();

            tableReader.GetHeaderDatas(out var columnNames, out var columnTypes);
            foreach (var type in columnTypes)
            {
                columnSqlTypes.Add(TypeUtil.ToSQLDataType(type));
            }

            sqlCtrl.CreateOrOpenTable(tableName, columnNames.ToArray(), columnSqlTypes.ToArray());

            for (int i = TableData.ContentStartIndex; i <= tableReader.RowLength; i++)
            {
                int id = i - TableData.ContentStartIndex;
                var rowData = tableReader.GetRow(i);
                columnValues.Clear();
                for (int j = rowData.FirstCellNum; j < rowData.LastCellNum; j++)
                {
                    var cell = rowData.GetCell(j);
                    if (cell.CellType == CellType.Numeric) columnValues.Add(cell.NumericCellValue.ToString());
                    else if (cell.CellType == CellType.Boolean) columnValues.Add(cell.BooleanCellValue.ToString());
                    else if (cell.CellType == CellType.String) columnValues.Add($"'{cell.StringCellValue}'");
                    else throw new System.Exception("No supported");
                }

                // sqlCtrl.InsertValues(tableName, columnValues.ToArray());
                // sqlCtrl.UpdateValues(tableName, columnNames.ToArray(), columnValues.ToArray(), $"ID = {id}");
                // sqlCtrl.DeleteValue(tableName, $"ID = {id}");
            }
        }

    }

    // A Test behaves as an ordinary method
    [Test, Order(2)]
    public void TestInsertDatas()
    {
        TestExcelInit.FilePath = EditorUtil.GetProjectFolderAbsPath(TestExcelInit.fileRelativePath);
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        tableReader.ConvertToRowDatas();

        string tableName = tableReader.SheetName;

        using (var sqlCtrl = new SQLController())
        {
            sqlCtrl.OpenConnection("TestDB.sqlite");

            var columnValues = new List<string>();
            var columnSqlTypes = new List<string>();

            tableReader.GetHeaderDatas(out var columnNames, out var columnTypes);
            foreach (var type in columnTypes)
            {
                columnSqlTypes.Add(TypeUtil.ToSQLDataType(type));
            }


            for (int i = TableData.ContentStartIndex; i <= tableReader.RowLength; i++)
            {
                int id = i - TableData.ContentStartIndex;
                var rowData = tableReader.GetRow(i);
                columnValues.Clear();
                for (int j = rowData.FirstCellNum; j < rowData.LastCellNum; j++)
                {
                    var cell = rowData.GetCell(j);
                    if (cell.CellType == CellType.Numeric) columnValues.Add(cell.NumericCellValue.ToString());
                    else if (cell.CellType == CellType.Boolean) columnValues.Add(cell.BooleanCellValue.ToString());
                    else if (cell.CellType == CellType.String) columnValues.Add($"'{cell.StringCellValue}'");
                    else throw new System.Exception("No supported");
                }

                sqlCtrl.InsertValues(tableName, columnValues.ToArray());
                // sqlCtrl.UpdateValues(tableName, columnNames.ToArray(), columnValues.ToArray(), $"ID = {id}");
                // sqlCtrl.DeleteValue(tableName, $"ID = {id}");
            }
        }

        ValidateDatas(tableReader, tableName);

    }

    // A Test behaves as an ordinary method
    [Test, Order(3)]
    public void TestUpdateDatas()
    {
        TestExcelInit.FilePath = EditorUtil.GetProjectFolderAbsPath(TestExcelInit.fileRelativePath);
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        tableReader.ConvertToRowDatas();

        string tableName = tableReader.SheetName;

        using (var sqlCtrl = new SQLController())
        {
            sqlCtrl.OpenConnection("TestDB.sqlite");

            var columnValues = new List<string>();
            var columnSqlTypes = new List<string>();

            tableReader.GetHeaderDatas(out var columnNames, out var columnTypes);
            foreach (var type in columnTypes)
            {
                columnSqlTypes.Add(TypeUtil.ToSQLDataType(type));
            }

            sqlCtrl.CreateOrOpenTable(tableName, columnNames.ToArray(), columnSqlTypes.ToArray());

            for (int i = TableData.ContentStartIndex; i <= tableReader.RowLength; i++)
            {
                int id = i - TableData.ContentStartIndex;
                var rowData = tableReader.GetRow(i);
                columnValues.Clear();
                for (int j = rowData.FirstCellNum; j < rowData.LastCellNum; j++)
                {
                    var cell = rowData.GetCell(j);
                    if (cell.CellType == CellType.Numeric) columnValues.Add(cell.NumericCellValue.ToString());
                    else if (cell.CellType == CellType.Boolean) columnValues.Add(cell.BooleanCellValue.ToString());
                    else if (cell.CellType == CellType.String) columnValues.Add($"'{cell.StringCellValue}'");
                    else throw new System.Exception("No supported");
                }

                sqlCtrl.UpdateValues(tableName, columnNames.ToArray(), columnValues.ToArray(), $"ID = {id}");
                // sqlCtrl.DeleteValue(tableName, $"ID = {id}");
            }
        }

        ValidateDatas(tableReader, tableName);

    }

    // A Test behaves as an ordinary method
    [Test, Order(1)]
    public void TestDeleteDatas()
    {
        TestExcelInit.FilePath = EditorUtil.GetProjectFolderAbsPath(TestExcelInit.fileRelativePath);
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);
        tableReader.ConvertToRowDatas();

        string tableName = tableReader.SheetName;

        int curTableCount = -1;

        using (var sqlCtrl = new SQLController())
        {
            sqlCtrl.OpenConnection("TestDB.sqlite");

            var columnValues = new List<string>();
            var columnSqlTypes = new List<string>();

            tableReader.GetHeaderDatas(out var columnNames, out var columnTypes);
            foreach (var type in columnTypes)
            {
                columnSqlTypes.Add(TypeUtil.ToSQLDataType(type));
            }

            sqlCtrl.CreateOrOpenTable(tableName, columnNames.ToArray(), columnSqlTypes.ToArray());

            for (int i = TableData.ContentStartIndex; i <= tableReader.RowLength; i++)
            {
                int id = i - TableData.ContentStartIndex;
                var rowData = tableReader.GetRow(i);
                columnValues.Clear();
                for (int j = rowData.FirstCellNum; j < rowData.LastCellNum; j++)
                {
                    var cell = rowData.GetCell(j);
                    if (cell.CellType == CellType.Numeric) columnValues.Add(cell.NumericCellValue.ToString());
                    else if (cell.CellType == CellType.Boolean) columnValues.Add(cell.BooleanCellValue.ToString());
                    else if (cell.CellType == CellType.String) columnValues.Add($"'{cell.StringCellValue}'");
                    else throw new System.Exception("No supported");
                }

                curTableCount = sqlCtrl.DeleteValue(tableName, $"ID = {id}");
            }

        }

        Assert.Zero(curTableCount);
    }

   

    [Test, Order(4)]
    public void TestDropTables()
    {
        TestExcelInit.FilePath = EditorUtil.GetProjectFolderAbsPath(TestExcelInit.fileRelativePath);
        var excelReader = new ExcelReader(TestExcelInit.FilePath);
        var tableReader = excelReader.GetTableReader(TestExcelInit.TableName_1);

        string tableName = tableReader.SheetName;

        using (var sqlCtrl = new SQLController())
        {
            sqlCtrl.OpenConnection("TestDB.sqlite");
            sqlCtrl.DropTable(tableName);
        }
    }

    #endregion

    private void ValidateDatas(TableReader tableReader, string tableName)
    {
        using (var sqlCtrl = new SQLController())
        {
            sqlCtrl.OpenConnection("TestDB.sqlite");

            var columnValues = new List<string>();
            tableReader.GetHeaderDatas(out var columnNames, out var columnTypes);

            for (int i = TableData.ContentStartIndex; i <= tableReader.RowLength; i++)
            {
                int id = i - TableData.ContentStartIndex;
                var rowData = tableReader.GetRowsByID(id);

                for (int j = 1; j < columnNames.Count; j++)
                {
                    var query = $"SELECT {columnNames[j]} FROM {tableName} WHERE ID = {id}";

                    if (columnTypes[j] == typeof(short))
                    {
                        Assert.AreEqual(Convert.ToInt16(rowData[j - 1]), sqlCtrl.QueryShort(query));
                    }
                    else if (columnTypes[j] == typeof(int))
                    {
                        Assert.AreEqual(Convert.ToInt32(rowData[j - 1]), sqlCtrl.QueryInt(query));
                    }
                    else if (columnTypes[j] == typeof(long))
                    {
                        Assert.AreEqual(Convert.ToInt64(rowData[j - 1]), sqlCtrl.QueryLong(query));
                    }
                    else if (columnTypes[j] == typeof(float))
                    {
                        Assert.AreEqual(Convert.ToSingle(rowData[j - 1]), sqlCtrl.QueryFloat(query));
                    }
                    else if (columnTypes[j] == typeof(double))
                    {
                        Assert.AreEqual(Convert.ToDouble(rowData[j - 1]), sqlCtrl.QueryDouble(query));
                    }
                    else if (columnTypes[j] == typeof(string))
                    {
                        Assert.AreEqual(Convert.ToString(rowData[j - 1]), sqlCtrl.QueryString(query));
                    }
                    if (columnTypes[j] == typeof(bool))
                    {
                        Assert.AreEqual(Convert.ToBoolean(rowData[j - 1]), sqlCtrl.QueryBoolean(query));
                    }
                    else if (columnTypes[j].IsArray)
                    {
                        // Assert.AreEqual(Convert.ToString(rowData[j - 1]), sqlCtrl.QueryString(query));
                    }
                }
            }
        }
    }

    public void TestDataTypes()
    {

        var sqlTypeMap = new Dictionary<Type, string>();
        sqlTypeMap.Add(typeof(bool), "BOOLEAN");
        sqlTypeMap.Add(typeof(byte), "TINYINT");
        sqlTypeMap.Add(typeof(short), "SMALLINT");
        sqlTypeMap.Add(typeof(int), "INT");
        sqlTypeMap.Add(typeof(long), "MEDIUMINT");

        sqlTypeMap.Add(typeof(float), "REAL");
        sqlTypeMap.Add(typeof(double), "DOUBLE");
        sqlTypeMap.Add(typeof(decimal), "NUMERIC");


        sqlTypeMap.Add(typeof(string), "TEXT");
        sqlTypeMap.Add(typeof(byte[]), "BLOB");
        sqlTypeMap.Add(typeof(object), "");

        sqlTypeMap.Add(typeof(DateTime), "DATETIME");



    }
}


