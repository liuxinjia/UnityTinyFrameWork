using System;
using System.Data;
using System.IO;
using System.Text;
using Cr7Sund.Editor.Excels;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;
using UnityQuickSheet;

namespace Cr7Sund.Editor
{
    public class ExcelDataHelper
    {
        private const string FilePath = @"E:\UnityProject\UnityTinyFrameWork\Assets\Example.xlsx";
        private const string NewFilePath = @"E:\UnityProject\UnityTinyFrameWork\Assets\NewExample.xlsx";

        private const string SheetName = "ExcelExample";
        private static ExcelDataHelper instance;

        public static ExcelDataHelper Instance
        {
            get
            {
                if (instance == null) instance = new ExcelDataHelper();
                return instance;
            }
        }

        [MenuItem("Tools/Test")]
        public static void Test()
        {
            ExcelDataHelper.Instance.ReadAllTables(FilePath);
        }

        [MenuItem("Tools/Test1")]
        public static void Test1()
        {
            ExcelDataHelper.Instance.ReadTable(FilePath, SheetName, "");
        }

        [MenuItem("Tools/Test_Read")]
        public static void Test_Read()
        {
            var excelReader = new ExcelReader(FilePath, SheetName, 0, 2);
            var list = excelReader.GetRowsByID(9);
            foreach (var item in list)
            {
                Debug.Log(item);
            }
        }

        [MenuItem("Tools/Test_ReadAll")]
        public static void Test_Read_All()
        {
            var excelReader = new ExcelReader(NewFilePath, SheetName, 0, 2);
            var list = excelReader.GetAllCells();
            foreach (var item in list)
            {
                Debug.Log(item);
            }
        }

        [MenuItem("Tools/Test_Write %t")]
        public static void Test_Write()
        {
            var excelWriter = new ExcelReplacer(FilePath, 0, 2);
            excelWriter.AddNewRow(SheetName, "15", "Cindy15", "24", "Very Hard");
            excelWriter.InsertNewRow(SheetName, 2, "10", "Cindy", "24", "Very Hard");
            // excelWriter.InsertNewRow(SheetName, 6, "12", "Cindy12", "24", "Very Hard");
            // excelWriter.InsertNewRow(SheetName, 6, "13", "Cindy13", "24", "Very Hard");
            // excelWriter.InsertNewRow(SheetName, 9, "19", "Cindy19", "24", "Very Hard");
            // excelWriter.AddNewRow(SheetName, "115", "Cindy_Last", "24", "Very Hard");
            excelWriter.ReplaceRow(SheetName, 9, ("Name", "Candy"), ("Difficulty", "Easy"));
            excelWriter.WriteTo(NewFilePath);
        }

        public void ReadAllTables(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();
                    foreach (DataTable table in result.Tables)
                        foreach (DataRow row in table.Rows)
                            foreach (DataColumn column in table.Columns)
                                if (row[column] != null)
                                    Debug.Log(row[column]);
                }
            }
        }

        public void ReadTable(string filePath, string tableName, string nameSpace)
        {
            DataTable dataTable;
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();
                    dataTable = result.Tables[tableName];
                    var row = dataTable.Rows[0];
                    var column_0 = dataTable.Columns[0];
                    var column_1 = dataTable.Columns[1];
                    var column_2 = dataTable.Columns[2];
                    var column_3 = dataTable.Columns[3];
                    var newRow = dataTable.NewRow();
                    newRow[column_1.ColumnName] = 1400;
                    newRow[column_2.ColumnName] = 1;
                    newRow[column_3.ColumnName] = "SectionName1400";


                    var column = new DataColumn();
                    column.DataType = System.Type.GetType("System.Int32");
                    column.ColumnName = "id";
                    dataTable.Columns.Add(column);

                    dataTable.Rows.Add(newRow);

                    column_2.ReadOnly = false;
                    dataTable.Rows[3][column_2] = "west";

                    dataTable.AcceptChanges();
                }
            }

            var excelWriter = new ExcelWriterHelper(FilePath);
            File.Delete(FilePath);
            excelWriter.WriteExcel(dataTable, SheetName);

            dataTable.Clear();
            dataTable = null;
            GC.Collect();
        }
    }
}