using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NPOI.SS.UserModel;

namespace Cr7Sund.Editor.Excels
{
    public class TableReader : ExcelQuery, IExcelReader
    {
        private readonly ISheet Sheet = null;
        protected HashSet<int> excludeNoteSet;
        protected Dictionary<int, int> idDict;



        public TableReader(string path, string sheetName, int headerIndex = 0, int contentIndex = 1,
            char delimiter = ';') : base(path, sheetName, headerIndex, contentIndex, delimiter)
        {
            try
            {
                using (FileStream fileStream =
                       new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileStream.Position = 0;
                    IWorkbook workbook = null;

                    string extension = EditorUtil.GetSuffix(path);

                    if (extension == "xls")
                        workbook = new HSSFWorkbook(fileStream);
                    else if (extension == "xlsx")
                    {
#if UNITY_EDITOR_OSX
                        throw new Exception("xlsx is not supported on OSX.");
#else
                        workbook = new XSSFWorkbook(fileStream);
#endif
                    }
                    else
                    {
                        throw new Exception("Wrong file.");
                    }

                    if (!string.IsNullOrEmpty(sheetName))
                    {
                        Sheet = workbook.GetSheet(sheetName);
                        if (Sheet == null)
                            Debug.LogErrorFormat("Cannot find sheet '{0}'.", sheetName);


                        InitHeaders(Sheet);
                        if (showID) InitSheetIDs(Sheet);
                        InitContents(Sheet);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e + ", " + e.StackTrace);
            }
        }


        #region Private Methods

        /// <summary>
        /// Get header Info
        /// </summary>
        /// <param name="sheet"></param>
        private void InitHeaders(ISheet sheet)
        {
            if (headerList == null)
            {
                headerList = new List<(string, Type)>(sheet.LastRowNum + 1);
            }

            if (excludeNoteSet == null)
            {
                excludeNoteSet = new HashSet<int>();
            }

            var headers = new List<string>();
            var dataTypes = new List<string>();
            var tmpObj = new List<object>();

            string firstCell = sheet.GetRow(0).Cells[0].StringCellValue.ToLower();
            if (firstCell == "id") showID = true;

            GetHeaderCellValuesByColIndex(sheet, ref tmpObj, headerStartIndex);
            foreach (var item in tmpObj) headers.Add(Convert.ToString(item));
            GetHeaderCellValuesByColIndex(sheet, ref tmpObj, headerStartIndex + 1);
            foreach (var item in tmpObj) { dataTypes.Add(item == null ? null : Convert.ToString(item)); }
            showColumnType = tmpObj.Count != 0;


            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                headerList.Add((header, dataTypes.Count == 0 ? typeof(string) : dataTypes[i] == null ? typeof(string) :
                GetMapDataTypes(dataTypes[i])));
            }
        }

        /// <summary>
        /// Get Row ID Relationship
        /// </summary>
        /// <param name="sheet"></param>
        private void InitSheetIDs(ISheet sheet)
        {
            int current = 0;

            if (idDict == null)
            {
                idDict = new Dictionary<int, int>(sheet.LastRowNum + 1);
            }

            foreach (IRow row in sheet)
            {
                if (current <= contentStartIndex)
                {
                    current++; // skip header column.
                    continue;
                }

                ICell cell = row.GetCell(0);

                idDict.Add((int)cell.NumericCellValue, current);
                current++;
            }
        }

        /// <summary>
        /// Get Cells via column index
        /// </summary>
        /// <param name="sheet">Search Sheet</param>
        /// <param name="result">cell results</param>
        /// <param name="rowIndex">row index</param>
        private void GetContentCellValuesByColIndex(ISheet sheet, ref List<object> result, int rowIndex)
        {
            result.Clear();
            IRow row = sheet.GetRow(rowIndex);
            int headerIndex = showID ? 1 : 0;
            for (int i = showID ? 1 : 0; i < row.LastCellNum -( showID ? 1 : 0); i++)
            {
                var cell = row.GetCell(i);
                if (cell == null)
                {
                    // null or empty column is found. Note column index starts from 0.
                    Debug.LogWarningFormat("Null or empty column is found at {0}-{1}.\n", rowIndex, i);
                    continue;
                }
                else if (cell.CellType == CellType.Blank)
                {
                    continue;
                }

                if (excludeNoteSet.Contains(i)) continue;
                result.Add(ConvertFrom(cell, headerList[headerIndex++].type));
            }
        }

        /// <summary>
        /// Get Header Cells via column index
        /// </summary>
        /// <param name="sheet">Search Sheet</param>
        /// <param name="result">cell results</param>
        /// <param name="rowIndex">row index</param>
        private void GetHeaderCellValuesByColIndex(ISheet sheet, ref List<object> result, int rowIndex)
        {
            result.Clear();
            IRow row = sheet.GetRow(rowIndex);
            if (row == null) return;
            for (int i = 0; i < row.LastCellNum; i++)
            {
                var cell = row.GetCell(i);
                if (cell == null)
                {
                    // null or empty column is found. Note column index starts from 0.
                    Debug.LogWarningFormat("Null or empty column is found at {0}-{1}.\n", rowIndex, i);
                    continue;
                }

                if (cell.CellType == CellType.String && cell.StringCellValue.Contains("#"))
                {
                    excludeNoteSet.Add(i);
                }

                // if (excludeNoteSet.Contains(i)) continue;
                if (cell.CellType == CellType.Blank)
                {
                    result.Add(null);
                    continue;
                }
                result.Add(cell.StringCellValue);
            }
        }

        private List<IRow> GetAllRows(ISheet sheet)
        {
            var result = new List<IRow>();
            //  sheet.LastRowNum is one more than the real rows number 
            for (var i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                result.Add(sheet.GetRow(i));
            }
            return result;
        }

        private void InitContents(ISheet sheet)
        {
            for (var i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                var newRow = MyRow.ToMyRow(sheet.GetRow(i));
                rows.Add(newRow);
            }
        }

        #endregion

        #region internal Methods


        /// <summary>
        /// Can not used by Excel Writer again since we insert the new rows, but the id dictionary is not the same
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<object> GetRowsByID(int id)
        {
            if (idDict == null) { Debug.LogError("Don't exist ids"); return null; }
            var result = new List<object>();

            if (idDict.TryGetValue(id, out int rowIndex))
            {
                GetContentCellValuesByColIndex(Sheet, ref result, rowIndex);
            }
            else
            {
                Debug.LogError($"Don't exist {id}");
            }

            return result;
        }

        public List<object> GetAllCells()
        {
            var rows = GetAllRows(Sheet);
            if (rows.Count < 1) return null;
            var result = new List<object>(rows.Count * (rows[0].LastCellNum - rows[0].FirstCellNum));
            foreach (var row in rows)
            {
                for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
                {
                    result.Add(row.GetCell(i));
                }
            }

            return result;
        }

        #endregion
    }
}