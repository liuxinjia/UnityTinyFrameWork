using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ExcelDataHelper
    {
        private const string FilePath = @"D:\Phoenix_Android\excel\Taiwan\0.8.0\obt\item\CainBook.xlsx";
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
            ExcelDataHelper.Instance.ReadTable(FilePath, "CainBook", "");
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
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet();
                    var dataTable = result.Tables[tableName];
                    var row = dataTable.Rows[0];
                    var column_0 = dataTable.Columns[0];
                    var column_1 = dataTable.Columns[1];
                    var column_2 = dataTable.Columns[2];
                    var column_3 = dataTable.Columns[3];
                  var newRow =  dataTable.NewRow();
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
        }
    }
}