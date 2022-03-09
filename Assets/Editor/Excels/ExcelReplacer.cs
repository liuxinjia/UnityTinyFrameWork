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
    public class ExcelReplacer : ExcelQuery
    {

        Dictionary<string, List<IRow>> oldWorkBookRows;
        Dictionary<string, ExcelReader> readerDict;
        Queue<IRow> newRows = new Queue<IRow>(16);

        public ExcelReplacer(string oldFilePath, int headerIndex = 0, int contentIndex = 0,
            char delimiter = ';') : base(oldFilePath, headerIndex, contentIndex, delimiter)
        {
            var sheetNames = new List<string>();
            if (string.IsNullOrEmpty(oldFilePath)) return;
            try
            {
                using (FileStream fileStream = new FileStream(oldFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileStream.Position = 0;
                    IWorkbook workbook = null;

                    string extension = UnityQuickSheet.ExcelQuery.GetSuffix(oldFilePath);

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

                    if (workbook == null) return;
                    for (var i = 0; i < workbook.NumberOfSheets; i++)
                    {
                        sheetNames.Add(workbook.GetSheetAt(i).SheetName);
                    }

                    oldWorkBookRows = GetAllSheetRows(workbook);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e + ", " + e.StackTrace);
            }


            readerDict = new Dictionary<string, ExcelReader>(sheetNames.Count);
            for (int i = 0; i < sheetNames.Count; i++)
            {
                readerDict.Add(sheetNames[i], new ExcelReader(oldFilePath, sheetNames[i], headerIndex, contentIndex, delimiter));
            }
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

                    CopyFromOldSheets(workbook);

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


        private Dictionary<string, List<IRow>> GetAllSheetRows(IWorkbook workBook)
        {
            var result = new Dictionary<string, List<IRow>>();
            for (var i = 0; i < workBook.NumberOfSheets; i++)
            {
                result.TryAdd(workBook.GetSheetAt(i).SheetName, GetAllRows(workBook.GetSheetAt(i)));
            }
            return result;
        }

        private void CopyFromOldSheets(IWorkbook workbook)
        {
            //Iterate through all Sheets, sheets -> sheet
            foreach (var oldSheetDatas in oldWorkBookRows)
            {
                // var oldSheetDatas = oldWorkBookRows[i];
                var newSheet = workbook.CreateSheet(oldSheetDatas.Key);
                var excelReader = readerDict[oldSheetDatas.Key];

                //Iterate through each sheet, sheet -> rows
                for (int j = 0; j < oldSheetDatas.Value.Count; j++)
                {
                    var newRow = newSheet.CreateRow(j);
                    var sheetRows = oldSheetDatas.Value[j];

                    // Iterate through eachRow, row -> cells 
                    for (int k = sheetRows.FirstCellNum; k < sheetRows.LastCellNum; k++)
                    {
                        var newCell = newRow.CreateCell(k);
                        var oldCell = sheetRows.GetCell(k);

                        var type = excelReader.GetColumnType(k);

                        if (oldCell == null || oldCell.CellType == CellType.Blank) continue;
                        if (type.IsArray
                            || excelReader.IsComment(k)
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
        }


        #region Public Methods

        // public List<object> GetRowsByID(int id)
        // {

        // }


        public void ChangeCellValue(int id, params (string, object)[] values)
        {
            // foreach (var item in values)    
            // {
            //     item.Item1
            // }
        }

        /// <summary>
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="id">Insert before Id, zero-based index</param>
        /// <param name="values">even if the value is numeric value, since we  can covnert string to specfic type we need</param>
        public void AddNewRow(string sheetName, params string[] values)
        {
            this.InsertNewRow(sheetName, -1, values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="id">Insert before Id, zero-based index</param>
        /// <param name="values">even if the value is numeric value, since we  can covnert string to specfic type we need</param>
        public void InsertNewRow(string sheetName, int id = -1, params string[] values)
        {
            var list = new List<object>();

            if (readerDict.TryGetValue(sheetName, out var reader))
            {
                bool unMatchParams = values.Length != reader.GetSheetColumnLengh();
                var unMatchLogSb = new StringBuilder();

                if (unMatchParams)
                {
                    unMatchLogSb.Append("params dont match row values: ");
                    Debug.LogError(unMatchLogSb);
                }

                int newId = Int32.Parse(values[0]);
                if (reader.GetMappedColumn(newId) != -1)
                {
                    Debug.LogError($"Already has {newId}");
                    return;
                }
                if (oldWorkBookRows.TryGetValue(sheetName, out var item))
                {
                    int column = id < 0 ? reader.GetSheetRowLength() + 1 : reader.GetMappedColumn(id);
                    var newRow = reader.InsertNewRow(id, newId);

                    for (var j = 0; j < reader.GetSheetColumnLengh(); j++)
                    {
                        if (values.Length <= j)
                        {
                            unMatchLogSb.Append($"{reader.GetColumnHeader(j)}, ");
                        }
                        else
                        {
                            var cell = newRow.GetCell(j);
                            if (cell == null)
                            {
                                cell = newRow.CreateCell(j);
                            }
                            else
                            {
                                ConvertTo(reader.GetColumnType(j), cell, values[j]);
                            }
                        }
                    }

                    if (unMatchParams) Debug.LogError(unMatchLogSb.ToString());
                    else item.Insert(column, newRow);
                }
            }
        }

        public void ReplaceRow(string sheetName, int id, params (string, string)[] newValues)
        {
            var valueDict = new Dictionary<string, string>(newValues.Length);
            foreach (var item in newValues)
            {
                valueDict.Add(item.Item1, item.Item2);
            }

            if (readerDict.TryGetValue(sheetName, out var reader))
            {
                if (oldWorkBookRows.TryGetValue(sheetName, out var item))
                {
                    var row = item[reader.GetMappedColumn(id)];
                    {
                        for (var i = row.FirstCellNum; i < row.LastCellNum; i++)
                        {
                            var cell = row.GetCell(i);

                            if (cell == null || cell.CellType == CellType.Blank) continue;

                            if (valueDict.TryGetValue(reader.GetColumnHeader(i), out var value))
                            {
                                if (reader.IsComment(i))
                                {
                                    ConvertTo(typeof(string), cell, value);
                                }
                                else
                                {
                                    ConvertTo(reader.GetColumnType(i), cell, value);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void DeleteRow(int id)
        {
            throw new NotImplementedException();
        }

        #endregion



    }
}