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
    public class ExcelReader : ExcelQuery, IExcelReader
    {
        private readonly ISheet sheet = null;
        protected List<(string header, Type type)> headerList;
        protected HashSet<int> excludeNoteSet;
        protected Dictionary<int, int> idDict;
        private List<IRow> newRow;
        private bool haveBeenModified = false;

        private bool hasIDColumn = false;

        internal ISheet Sheet
        {
            get
            {
                if (haveBeenModified)
                {
                    Debug.LogError("Already have been modified, e.g. Insert");
                    return null;
                }
                else return sheet;
            }
        }

        public ExcelReader(string path, string sheetName, int headerIndex = 0, int contentIndex = 1,
            char delimiter = ';') : base(path, headerIndex, contentIndex, delimiter)
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
                        sheet = workbook.GetSheet(sheetName);
                        if (sheet == null)
                            Debug.LogErrorFormat("Cannot find sheet '{0}'.", sheetName);


                        InitHeaders(Sheet);
                        InitSheetIDs(Sheet);
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
            if (firstCell == "id") hasIDColumn = true;

            GetHeaderCellValuesByColIndex(sheet, ref tmpObj, headerStartIndex, hasIDColumn);
            foreach (var item in tmpObj) headers.Add(Convert.ToString(item));
            GetHeaderCellValuesByColIndex(sheet, ref tmpObj, headerStartIndex + 1, hasIDColumn);
            foreach (var item in tmpObj) dataTypes.Add(Convert.ToString(item));


            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                headerList.Add((header, GetMapDataTypes(dataTypes[i])));
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
            int headerIndex = 0;
            for (int i = hasIDColumn ? 1 : 0; i < row.LastCellNum; i++)
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
        private void GetHeaderCellValuesByColIndex(ISheet sheet, ref List<object> result, int rowIndex, bool hasIDColumn)
        {
            result.Clear();
            IRow row = sheet.GetRow(rowIndex);

            for (int i = hasIDColumn ? 1 : 0; i < row.LastCellNum; i++)
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

        #endregion

        #region internal Methods


        /// <summary>
        /// Can not used by Excel Writer again since we insert the new rows, but the id dictionary is not the same
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<object> GetRowsByID(int id)
        {
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