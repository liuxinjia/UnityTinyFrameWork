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
    public class TableReader : TableData, IExcelReader
    {
        private ISheet _sheet; //instead convert all table datas to custom datas(save) , we keep the original sheet to acess directlry 
        public int RowLength => _sheet.LastRowNum;
        public int ColumnLength => _sheet.GetRow(HeaderStartIndex).LastCellNum;
        public TableReader(string path, string sheetName) : base(sheetName)
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

                        _sheet = sheet;

                        InitHeaders(sheet);

                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
                throw e;
            }
        }


        #region Init Datas 


        /// <summary>
        /// Get header Info
        /// </summary>
        /// <param name="sheet"></param>
        private void InitHeaders(ISheet sheet)
        {
            var headerRowData = sheet.GetRow(HeaderStartIndex);
            var typeRowData = sheet.GetRow(HeaderStartIndex + 1);
            for (int i = 0; i < ColumnLength; i++)
            {
                var header = headerRowData.GetCell(i).StringCellValue;
                var typeCell = typeRowData.GetCell(i);
                headerList.Add(
                    new HeaderData()
                    {
                        header = header,
                        type = i == 0 ? typeof(int) : GetMapDataTypes(typeCell.StringCellValue), // the id columns is typeof(int)
                        comment = headerRowData.GetCell(i).CellComment == null ? string.Empty : headerRowData.GetCell(i).CellComment.String.String
                    }
                );
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

            if (id < 0 || id > RowLength - ContentStartIndex) throw new ArgumentOutOfRangeException($"{id} is out of index");

            for (int i = 1; i < ColumnLength; i++) // start form id column
            {
                result.Add(GetOriginalCellValue(id + ContentStartIndex, i));
            }

            return result;
        }

        public IRow GetRow(int rowIndex) => _sheet.GetRow(rowIndex);

        public object GetCell(int id, string title)
        {
            var result = GetRowsByID(id);

            for (int i = 1; i < headerList.Count; i++) // exclude from id
            {
                var item = headerList[i];
                if (item.header.Equals(title))
                {
                    return result[i - 1];
                }
            }

            return null;
        }


        #endregion

        #region private Methods

        private object GetOriginalCellValue(int rowIndex, int colIndex)
        {
            ICell cell = _sheet.GetRow(rowIndex).GetCell(colIndex);
            var cellData = CellData.GetCellData(cell);
            object cellValue = cellData.GetValue();

            if (this.headerList[colIndex].type.IsArray && cellValue is string arrayValue)
            {
                Type elementType = this.headerList[colIndex].type.GetElementType();
                return GetCellArrayValue(elementType, arrayValue);
            }
            else
                return cellValue;
        }

        public void ConvertToRowDatas()
        {
            ///Used for 
            /// 1. search cache
            /// 2. keep this table while modifying other tables
            for (var i = _sheet.FirstRowNum; i <= _sheet.LastRowNum; i++)
            {
                var newRow = RowData.GetRowData(_sheet.GetRow(i));
                rowDatas.Add(newRow);
            }
        }


        #endregion
    }


}