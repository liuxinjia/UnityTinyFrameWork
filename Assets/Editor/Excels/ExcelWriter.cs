using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace Editor.Excels
{
    public class ExcelWriter : ExcelQuery, IExcelWriter
    {

        Dictionary<string, List<IRow>> oldWorkBookRows;
        Dictionary<string, ExcelReader> readerDict;
        Queue<IRow> newRows = new Queue<IRow>(16);

        public ExcelWriter(string path, int headerIndex = 0, int contentIndex = 0,
            char delimiter = ';') : base(path, headerIndex, contentIndex, delimiter)
        {
            var sheetNames = new List<string>();
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileStream.Position = 0;
                    IWorkbook workbook = null;

                    string extension = UnityQuickSheet.ExcelQuery.GetSuffix(path);

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


            for (int i = 0; i < sheetNames.Count; i++)
            {
                readerDict.Add(sheetNames[i], new ExcelReader(path, sheetNames[i], headerIndex, contentIndex, delimiter));
            }
        }


        public void WriteTo(string newPath = null)
        {

            if (string.IsNullOrEmpty(newPath))
            {
                try
                {
                    File.Delete(filepath);
                    EditorUtility.DisplayProgressBar(" Excel writer", "Write Excel", 0.4f);
                }
                catch (Exception e)
                {
                    EditorUtility.ClearProgressBar();
                    throw new Exception("Delete File: ", e);
                }
                newPath = filepath;
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
                    int header = 0;
                    var newRow = newSheet.CreateRow(j);
                    var sheetRows = oldSheetDatas.Value[j];

                    // Iterate through eachRow, row -> cells 
                    for (int k = sheetRows.FirstCellNum; k < sheetRows.LastCellNum; k++)
                    {
                        var newCell = newRow.CreateCell(k);
                        var oldCell = sheetRows.GetCell(k);
                        int prevHeader = header;

                        var type = excelReader.GetColumnType(prevHeader);
                        if (!excelReader.IsComment(k))
                        {
                            header++;
                        }

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

        public void InsertNewRow(string sheetName, int id, List<string> values)
        {
            // var newSheet = workbook.CreateSheet("sheetName");
            var list = new List<object>();
            if (readerDict.TryGetValue(sheetName, out var reader))
            {
                if (oldWorkBookRows.TryGetValue(sheetName, out var item))
                {
                    var newRow = item[0];
                    item.Add(newRow);

                    for (var j = 0; j < values.Count; j++)
                    {
                        var cell = newRow.GetCell(j);
                        ConvertTo(reader.GetColumnType(j), cell, values[j]);
                    }
                }
            }
        }

        public void ReplaceRow(string sheetName, int id, params (string, string)[] newValues)
        {
            var values = new List<(string, string)>(newValues);

            if (readerDict.TryGetValue(sheetName, out var reader))
            {
                if (oldWorkBookRows.TryGetValue(sheetName, out var item))
                {
                    foreach (var row in item)
                    {
                        int header = 0;
                        for (var i = row.FirstCellNum; i < row.LastCellNum; i++)
                        {
                            int prevHeader = header;
                            if (!reader.IsComment(i)) header++;

                            var cell = row.GetCell(i);
                            if (cell == null || cell.CellType == CellType.Blank) continue;
                            int count = values.Count;
                            for (int j = 0; j < count; j++)
                            {
                                var value = values[j];
                                if (reader.IsComment(i) && value.Item2.Contains('#'))
                                {
                                    ConvertTo(typeof(string), cell, value.Item2);
                                    values.RemoveAt(j);
                                }
                                else if (value.Item1 == reader.GetColumnHeader(prevHeader))
                                {
                                    ConvertTo(reader.GetColumnType(prevHeader), cell, value.Item2);
                                    values.RemoveAt(j);
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