using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using System.Text;

namespace Cr7Sund.Editor.Excels
{

    public class TableWriter : ExcelQuery
    {

        public TableWriter(string path, string sheetName, int headerIndex = 0, int contentIndex = 1, bool showType = false,
           bool showID = true, char delimiter = ';') : base(path, sheetName, headerIndex, contentIndex, delimiter)
        {
            this.showID = showID;
            this.showColumnType = showType;
        }


        #region Public Methods

        public void InitHeaders(Type type, params string[] headers)
        {
            var tList = new List<Type>();
            var hList = new List<string>();

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

        #region 

        #endregion

    }
}