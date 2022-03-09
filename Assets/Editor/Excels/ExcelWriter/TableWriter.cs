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

namespace Cr7Sund.Editor.Excels
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

        public void AddCell(object obj, Type objType, bool skipTypeJudge = false)
        {
            MyCell item = new MyCell();
            cells.Add(item);
            SetCell(obj, objType, cells.Count - 1, skipTypeJudge);
        }

        public void SetCell(object obj, Type objType, int index, bool skipTypeJudge = false)
        {
            MyCell item = cells[index];

            if (objType == typeof(string) || skipTypeJudge)
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
        }
    }


    public class TableWriter : ExcelQuery
    {

        private List<(string header, Type type)> headerList = new List<(string header, Type)>();
        private List<MyRow> rows = new List<MyRow>();
        private string sheetName;
        private bool showColumnType;
        private bool showID;

        public TableWriter(string path, string sheetName, int headerIndex = 0, int contentIndex = 1, bool showType = false,
           bool showID = true, char delimiter = ';') : base(path, headerIndex, contentIndex, delimiter)
        {
            this.sheetName = sheetName;
            this.showID = showID;
            this.showColumnType = showType;
        }

        internal void WriteDatas(IWorkbook workbook)
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
                    var headerInfo = headerList[k];

                    if (!showColumnType && j == this.contentStartIndex) continue;

                    var type = headerInfo.type;
                    bool isComment = headerInfo.header.Contains('#');

                    if (oldCell == null || oldCell.CellType == CellType.Blank) continue;
                    if (type.IsArray
                        || isComment
                        || j < contentStartIndex)
                    {
                        newCell.SetCellValue(oldCell.StringCellValue);
                    }
                    else if (showColumnType && j == this.contentStartIndex)
                    {
                        if (k == 0)
                            newCell.SetCellType(CellType.Blank);
                        else
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

        public void InitHeaders(params string[] headers)
        {
            var tList = new List<Type>();
            var hList = new List<string>();
            Type type = typeof(int);

            foreach (string item in headers)
            {
                tList.Add(type);
                hList.Add(item);
            }

            InitHeaders(tList, hList);
        }

        public void InitHeaders(List<(string header, Type type)> headers)
        {
            var hList = new List<string>();
            var tList = new List<Type>();
            foreach (var item in headers)
            {
                hList.Add(item.header);
                tList.Add(item.type);
            }

            InitHeaders(tList, hList);
        }

        public void InitHeaders(List<Type> tList, List<string> hList)
        {
            if (showID)
            {
                hList.Insert(0, "Id");
                tList.Insert(0, typeof(int));
            }

            for (int i = 0; i < tList.Count; i++)
            {
                headerList.Add((hList[i], tList[i]));
            }


            AddRowList<string>(hList, true);
            hList.Clear();
            tList.ForEach((t) => hList.Add(GetSimplifyTypes(t)));
            AddRowList<string>(hList, true);
        }

        public void AddRowList<T>(List<T> cells, bool skipTypeJudge = false)
        {
            if (cells.Count != headerList.Count) { Debug.LogError("row datas don not match headers"); return; }
            var newRow = new MyRow();
            rows.Add(newRow);
            for (int i = 0; i < cells.Count; i++)
            {
                object cell = cells[i];
                newRow.AddCell(cell, headerList[i].type, skipTypeJudge);
            }
        }

        public void SetValue(int rowIndex, int columnIndex, object value)
        {
            MyRow row = null;
            rowIndex = this.contentStartIndex + rowIndex + 1;
            columnIndex += (showID ? 1 : 0);

            if (rows.Count <= rowIndex)
            {
                row = new MyRow();
                rows.Add(row);
            }
            else
            {
                row = rows[rowIndex];
            }

            if (showID && (row.Count == 0))
            {
                row.AddCell(rowIndex - 1 - this.contentStartIndex, typeof(int));
            }

            if (row.Count <= columnIndex)
            {
                row.AddCell(value, headerList[columnIndex].type);
            }
            else
            {
                row.SetCell(value, headerList[columnIndex].type, columnIndex);
            }
        }

        #endregion
    }
}