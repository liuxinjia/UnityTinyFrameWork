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
    using Object = System.Object;

    public class TableWriter : TableData
    {

        public TableWriter(string sheetName) : base(sheetName)
        {

        }


        #region Public Methods


        public void InitHeaders(List<Type> tList, List<string> hList, List<string> cList = null)
        {
            hList.Insert(0, "");// [0][0] will be empty, but we put it in ConvertDatasToTable together
            tList.Insert(0, typeof(int));

            var emptyStrList = new List<string>();
            for (int i = 0; i < tList.Count; i++)
            {
                headerList.Add(
                   new HeaderData()
                   {
                       header = hList[i],
                       type = tList[i],
                       comment = cList != null ? cList[i] : string.Empty
                   }
               );
                emptyStrList.Add(string.Empty);
            }

            var tmpList = new List<Object>();

            tmpList.AddRange(hList);
            AddRowList(tmpList, true);

            tmpList.Clear();
            tList.ForEach((t) => tmpList.Add(GetSimplifyTypes(t)));
            AddRowList(tmpList, true);


            // Empty Line
            // Empty Line will have one zero value, but the next line  have the same things
            tmpList.Clear();
            tmpList.AddRange(emptyStrList);
            AddRowList(tmpList, true);
            AddRowList(tmpList, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="isString">consider as string if true</param>
        public void AddRowList(List<Object> cells, bool isString = false)
        {
            if (cells.Count != headerList.Count) { Debug.LogError("row datas don not match headers"); return; }
            var newRow = new RowData();
            rowDatas.Add(newRow);
            for (int i = 0; i < cells.Count; i++)
            {
                object cell = cells[i];
                newRow.AddCell(cell, headerList[i].type, delimiter, isString);
            }
        }

        /// <summary>
        /// set existed value or add new cell
        /// </summary>
        /// <param name="rowIndex">start from 0</param>
        /// <param name="columnIndex">start from 0</param>
        /// <param name="value"></param>
        public void SetValue(int rowIndex, int columnIndex, object value)
        {
            RowData row = null;
            rowIndex = ContentStartIndex + rowIndex;//include the title indexs

            if (value == null) throw new NullReferenceException("Can not set null value"); // there is a problem when modifying excel manually with null value
            if (rowDatas.Count <= rowIndex)
            {
                row = new RowData();
                rowDatas.Add(row);

                row.AddCell(rowIndex - ContentStartIndex, typeof(int), delimiter); // Add ID Column when create new row
            }
            else
            {
                row = rowDatas[rowIndex];
            }

            columnIndex += 1;
            if (row.Count <= columnIndex)
            {
                row.AddCell(value, headerList[columnIndex].type, delimiter);
            }
            else
            {
                row.SetCell(value, headerList[columnIndex].type, columnIndex, delimiter);
            }
        }

        #endregion


    }

    public static class TableWriterExtender
    {
        public static void InitHeaders(this TableWriter tableWriter, Type type, params string[] headers)
        {
            var tList = new List<Type>();
            var hList = new List<string>();

            foreach (string item in headers)
            {
                tList.Add(type);
                hList.Add(item);
            }

            tableWriter.InitHeaders(tList, hList);
        }

        public static void InitHeaders(this TableWriter tableWriter, List<(string header, Type type)> headers)
        {
            var hList = new List<string>();
            var tList = new List<Type>();
            foreach (var item in headers)
            {
                hList.Add(item.header);
                tList.Add(item.type);
            }

            tableWriter.InitHeaders(tList, hList);
        }


        public static void InitHeaders(this TableWriter tableWriter, Type type, List<string> headers, List<string> comments)
        {
            var hList = new List<string>();
            var tList = new List<Type>();
            foreach (var item in headers)
            {
                hList.Add(item);
                tList.Add(type);
            }

            tableWriter.InitHeaders(tList, hList, comments);
        }
    }
}