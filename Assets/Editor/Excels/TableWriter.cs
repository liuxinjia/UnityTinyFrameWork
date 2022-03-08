using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using ExcelDataReader.Log;
using NPOI.SS.UserModel;
using System.Text;

namespace Editor.Excels
{
    public class MyCell
    {
        public CellType CellType;
        public string StringCellValue;
        public double NumericCellValue;
        public bool BooleanCellValue;
    }
    public class MyRow
    {
        private List<MyCell> cells = new List<MyCell>();
        public int Count => cells.Count;

        internal MyCell GetCell(int k) => cells[k];
        public void AddCell(object obj, Type objType)
        {
            MyCell item = new MyCell();
            if (objType == typeof(string))
            {
                item.CellType = CellType.String;
                item.StringCellValue = obj.ToString();
            }
            else if (objType == typeof(bool))
            {
                item.CellType = CellType.Boolean;
                item.BooleanCellValue = Convert.ToBoolean(obj);
            }
            else if (double.TryParse(obj.ToString(), out var result))
            {
                item.CellType = CellType.Numeric;
                item.NumericCellValue = result;
            }
            else
            {
                item.CellType = CellType.Error;
                Debug.LogError("Unkown Type ");
            }
            cells.Add(item);
        }
    }


    public class TableWriter : ExcelQuery
    {

        private List<(string header, Type type)> headerList;
        private List<MyRow> rows;
        private string sheetName;

        public TableWriter(string path, string sheetName, int headerIndex = 0, int contentIndex = 0,
            char delimiter = ';') : base(path, headerIndex, contentIndex, delimiter)
        {
            this.sheetName = sheetName;
        }

        public void WriteTo(string newPath = null)
        {

            if (string.IsNullOrEmpty(newPath))
            {
                try
                {
                    File.Delete(filePath);
                    EditorUtility.DisplayProgressBar(" Excel writer", "Write Excel", 0.4f);
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    throw new Exception("Delete File: ", e);
                }
                newPath = filePath;
            }

            try
            {
                using (FileStream fileStream = new FileStream(newPath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                           FileShare.ReadWrite))
                {
                    fileStream.Position = 0;
                    IWorkbook workbook = null;

                    string extension = UnityQuickSheet.ExcelQuery.GetSuffix(newPath);

                    if (extension == "xls")
                        workbook = new HSSFWorkbook();
                    else if (extension == "xlsx")
                    {
#if UNITY_EDITOR_OSX
                        throw new Exception("xlsx is not supported on OSX.");
#else
                        workbook = new XSSFWorkbook();
#endif
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                        throw new Exception("Wrong file.");
                    }

                    WriteDatas(workbook);

                    workbook.Write(fileStream);
                    EditorUtility.DisplayProgressBar(" Excel writer", "Write Excel", 1.0f);
                    Debug.Log("Wirte to excel successfully");
                }

            }
            catch (Exception e)
            {
                Debug.Log(e + ", " + e.StackTrace);
                EditorUtility.ClearProgressBar();
            }
            EditorUtility.ClearProgressBar();
        }

        public void WriteDatas(IWorkbook workbook)
        {
            var newSheet = workbook.CreateSheet(sheetName);

            //Iterate through each sheet, sheet -> rows
            for (int j = 0; j < rows.Count; j++)
            {
                var newRow = newSheet.CreateRow(j);
                var sheetRows = rows[j];

                // Iterate through eachRow, row -> cells 
                for (int k = 0; k < sheetRows.Count; k++)
                {
                    var newCell = newRow.CreateCell(k);
                    var oldCell = sheetRows.GetCell(k);

                    var type = headerList[k].type;
                    bool isComment = headerList[k].header.Contains('#');

                    if (oldCell == null || oldCell.CellType == CellType.Blank) continue;
                    if (type.IsArray
                        || isComment
                        || j < contentStartIndex)
                    {
                        newCell.SetCellValue(oldCell.StringCellValue);
                    }
                    else
                    {
                        ConvertTo(type, oldCell, newCell);
                    }
                }
            }
        }

        #region Public Methods
        public void InitHeaders(List<(string header, Type type)> headers)
        {
            foreach (var item in headers)
            {
                headerList.Add(item);
            }
        }

        public void Add(params object[] cells)
        {
            if (cells.Length != headerList.Count) { Debug.LogError("row datas don not match headers"); return; }
            var newRow = new MyRow();
            rows.Add(newRow);

            for (int i = 0; i < cells.Length; i++)
            {
                object cell = cells[i];
                newRow.AddCell(cell, headerList[i].type);
            }
        }


        #endregion
    }
}