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

        public TableReader(string path, string sheetName, char delimiter = ';') : base(path, sheetName, delimiter)
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
                        var sheet = workbook.GetSheet(sheetName);
                        if (sheet == null)
                            Debug.LogErrorFormat("Cannot find sheet '{0}'.", sheetName);

                        InitRowDatas(sheet);
                        InitHeaders(sheet);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e + ", " + e.StackTrace);
            }
        }


        #region Init Datas 

        /// <summary>
        /// Get header Info
        /// </summary>
        /// <param name="sheet"></param>
        private void InitHeaders(ISheet sheet)
        {
            string firstCell = sheet.GetRow(0).Cells[0].StringCellValue.ToLower();

            var headerRowData = rowDatas[HeaderStartIndex];
            var typeRowData = rowDatas[HeaderStartIndex + 1];
            for (int i = 0; i < headerRowData.Count; i++)
            {
                var header = headerRowData.GetCell(i).StringCellValue;
                var typeCell = typeRowData.GetCell(i);
                headerList.Add(
                    new HeaderData()
                    {
                        header = header,
                        type = typeCell.CellType == CellType.Blank ? typeof(int) :
                         GetMapDataTypes(typeCell.StringCellValue)
                    }
                );
            }
        }

        private void InitRowDatas(ISheet sheet)
        {
            ///Used for 
            /// 1. search cache
            /// 2. keep this table while modifying other tables
            for (var i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                var newRow = RowData.GetRowData(sheet.GetRow(i));
                rowDatas.Add(newRow);
            }
        }




        #endregion

        #region public Methods


        /// <summary>
        /// Can not used by Excel Writer again since we insert the new rows, but the id dictionary is not the same
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<object> GetRowsByID(int id)
        {
            var result = new List<object>();

            var rowCounts = rowDatas.Count;

            if (id < 0 || id >= rowCounts - ContentStartIndex) throw new ArgumentOutOfRangeException($"{id} is out of index");

            for (int i =  1 ; i < rowDatas[id].Count; i++) // start form id column
            {
                result.Add(rowDatas[id + ContentStartIndex].GetCell(i).GetValue());
            }

            return result;
        }

        public object GetCell(int id, int title)
        {
            var result = GetRowsByID(id);

            for (int i = 0; i < headerList.Count; i++)
            {
                var item = headerList[i];
                if (item.header.Equals(title))
                {
                    return result[i];
                }
            }

            return null;
        }


        #endregion
    }


}