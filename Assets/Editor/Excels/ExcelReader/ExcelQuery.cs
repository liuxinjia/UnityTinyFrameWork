using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using NPOI.SS.Util;

namespace Cr7Sund.Editor.Excels
{
    public class ExcelQuery
    {

        protected List<HeaderData> headerList = new List<HeaderData>();
        protected List<RowData> rowDatas = new List<RowData>();
        protected string sheetName;

        protected string filePath = string.Empty;

        protected char delimiter;


        protected const int HeaderStartIndex = 0;
        protected const int ContentStartIndex = 4;

        protected readonly TypeConverter converter;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExcelQuery(string path, string sheetName, char delimiter = ';')
        {
            this.filePath = path;
            this.sheetName = sheetName;

            this.delimiter = delimiter;
            this.converter = new TypeConverter(delimiter);
        }


        internal void WriteDatas(IWorkbook workbook)
        {
            var newSheet = workbook.CreateSheet(sheetName);

            //Iterate through each sheet, sheet -> rows
            for (int rowIndex = 0; rowIndex < this.rowDatas.Count; rowIndex++)
            {
                var newRow = newSheet.CreateRow(rowIndex);
                var rowData = this.rowDatas[rowIndex];

                // Iterate through eachRow, row -> cells 
                for (int columnIndex = 0; columnIndex < rowData.Count; columnIndex++)
                {
                    var newCell = newRow.CreateCell(columnIndex);
                    var cellData = rowData.GetCell(columnIndex);
                    var headerInfo = headerList[columnIndex];
                    var type = headerInfo.type;

                    if (cellData == null || cellData.CellType == CellType.Blank) continue; // skip empty type

                    if (type.IsArray
                        || rowIndex == HeaderStartIndex)
                    {
                        newCell.SetCellValue(cellData.StringCellValue);
                    }
                    else if (rowIndex < ContentStartIndex) // handle not -content type
                    {
                        if (columnIndex == 0)
                            newCell.SetCellType(CellType.Blank);
                        else if (cellData.CellType != CellType.Blank && cellData.StringCellValue != null)
                            newCell.SetCellValue(cellData.StringCellValue);
                    }
                    else
                    {
                        ConvertTo(type, cellData, newCell);
                    }

                }
            }
        }


        #region HelpMethods


        /// <summary>
        /// Get Data type via string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Type GetMapDataTypes(string value)
        {
            if (value == "int") return typeof(int);
            if (value == "string") return typeof(string);
            if (value == "float") return typeof(float);
            if (value == "double") return typeof(double);
            if (value == "string") return typeof(string);

            if (value == "int[;]") return typeof(int[]);
            if (value == "string[;]") return typeof(string[]);
            if (value == "float[;]") return typeof(float[]);
            if (value == "double[;]") return typeof(double[]);
            return typeof(Array);
        }

        public string GetSimplifyTypes(Type type)
        {
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";

            if (type.IsArray && type.GetElementType() == typeof(int)) return "int[;]";
            if (type.IsArray && type.GetElementType() == typeof(float)) return "float[;]";

            var typeStrs = type.ToString().Split('.');
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return $"{GetSimplifyTypes(elementType)}[;]";
            }
            else if (type.IsGenericType)
            {
                var elementType = type.GetGenericArguments()[0];
                return GetSimplifyTypes(elementType);
            }
            else return typeStrs[typeStrs.Length - 1].ToLower();
        }



        /// <summary>
        /// Convert type of cell value to its predefined type which is specified in the sheet's ScriptMachine setting file.
        /// </summary>
        protected object ConvertFrom(ICell cell, Type t)
        {
            object value = null;

            if (t == typeof(float) || t == typeof(double) || t == typeof(short) || t == typeof(int) || t == typeof(long))
            {
                if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                {
                    value = cell.NumericCellValue;
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.String)
                {
                    //Get correct numeric value even the cell is string type but defined with a numeric type in a data class.
                    if (t == typeof(float))
                        value = Convert.ToSingle(cell.StringCellValue);
                    if (t == typeof(double))
                        value = Convert.ToDouble(cell.StringCellValue);
                    if (t == typeof(short))
                        value = Convert.ToInt16(cell.StringCellValue);
                    if (t == typeof(int))
                        value = Convert.ToInt32(cell.StringCellValue);
                    if (t == typeof(long))
                        value = Convert.ToInt64(cell.StringCellValue);
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.Formula)
                {
                    // Get value even if cell is a formula
                    if (t == typeof(float))
                        value = Convert.ToSingle(cell.NumericCellValue);
                    if (t == typeof(double))
                        value = Convert.ToDouble(cell.NumericCellValue);
                    if (t == typeof(short))
                        value = Convert.ToInt16(cell.NumericCellValue);
                    if (t == typeof(int))
                        value = Convert.ToInt32(cell.NumericCellValue);
                    if (t == typeof(long))
                        value = Convert.ToInt64(cell.NumericCellValue);
                }
            }
            else if (t == typeof(string) || t.IsArray)
            {
                // HACK: handles the case that a cell contains numeric value
                //       but a member field in a data class is defined as string type.
                //       e.g. string s = "123"
                if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    value = cell.NumericCellValue;
                else
                    value = cell.StringCellValue;
            }
            else if (t == typeof(bool))
                value = cell.BooleanCellValue;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nc = new NullableConverter(t);
                return nc.ConvertFrom(value);
            }

            if (t.IsEnum)
            {
                // for enum type, first get value by string then convert it to enum.
                value = cell.StringCellValue;
                return Enum.Parse(t, value.ToString(), true);
            }
            else if (t.IsArray)
            {
                if (t.GetElementType() == typeof(float))
                    return converter.ToSingleArray((string)value);

                if (t.GetElementType() == typeof(double))
                    return converter.ToDoubleArray((string)value);

                if (t.GetElementType() == typeof(short))
                    return converter.ToInt16Array((string)value);

                if (t.GetElementType() == typeof(int))
                    return converter.ToInt32Array((string)value);

                if (t.GetElementType() == typeof(long))
                    return converter.ToInt64Array((string)value);

                if (t.GetElementType() == typeof(string))
                    return converter.ToStringArray((string)value);
            }

            // for all other types, convert its corresponding type.
            return Convert.ChangeType(value, t);
        }

        /// <summary>
        /// Convert type of cell value to its predefined type which is specified in the sheet's ScriptMachine setting file.
        /// </summary>
        protected void ConvertTo(Type t, ICell oldCell, ICell newCell)
        {

            if (t == typeof(float) || t == typeof(double) || t == typeof(short) || t == typeof(int) || t == typeof(long))
            {
                if (oldCell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                {
                    newCell.SetCellValue(oldCell.NumericCellValue);
                }
                else if (oldCell.CellType == NPOI.SS.UserModel.CellType.String)
                {
                    //Get correct numeric value even the cell is string type but defined with a numeric type in a data class.
                    if (t == typeof(float))
                        newCell.SetCellValue(Convert.ToSingle(oldCell.StringCellValue));
                    if (t == typeof(double))
                        newCell.SetCellValue(Convert.ToDouble(oldCell.StringCellValue));
                    if (t == typeof(short))
                        newCell.SetCellValue(Convert.ToInt16(oldCell.StringCellValue));
                    if (t == typeof(int))
                        newCell.SetCellValue(Convert.ToInt32(oldCell.StringCellValue));
                    if (t == typeof(long))
                        newCell.SetCellValue(Convert.ToInt64(oldCell.StringCellValue));
                }
                else if (oldCell.CellType == NPOI.SS.UserModel.CellType.Formula)
                {
                    // Get value even if cell is a formula
                    if (t == typeof(float))
                        newCell.SetCellValue(Convert.ToSingle(oldCell.NumericCellValue));
                    if (t == typeof(double))
                        newCell.SetCellValue(Convert.ToDouble(oldCell.NumericCellValue));
                    if (t == typeof(short))
                        newCell.SetCellValue(Convert.ToInt16(oldCell.NumericCellValue));
                    if (t == typeof(int))
                        newCell.SetCellValue(Convert.ToInt32(oldCell.NumericCellValue));
                    if (t == typeof(long))
                        newCell.SetCellValue(Convert.ToInt64(oldCell.NumericCellValue));
                }
            }
            else if (t == typeof(string) || t.IsArray || t.IsEnum)
            {
                // HACK: handles the case that a cell contains numeric value
                //       but a member field in a data class is defined as string type.
                //       e.g. string s = "123"
                if (oldCell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    newCell.SetCellValue(oldCell.NumericCellValue);
                else
                    newCell.SetCellValue(oldCell.StringCellValue);
            }
            else if (t == typeof(bool))
                newCell.SetCellValue(oldCell.BooleanCellValue);

            else if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                Debug.LogError("Don not support Generic Type Write");
                newCell.SetCellType(CellType.Blank);
                return;
            }

            else
            {
                Debug.LogError($"Dont not Support Type {t}");
                newCell.SetCellType(CellType.Blank);
            }


        }

        /// <summary>
        /// Convert type of cell value to its predefined type which is specified in the sheet's ScriptMachine setting file.
        /// </summary>
        protected void ConvertTo(Type t, ICell cell, string newValue)
        {

            if (t == typeof(float) || t == typeof(double) || t == typeof(short) || t == typeof(int) || t == typeof(long))
            {
                cell.SetCellValue(Convert.ToDouble(newValue));
            }
            else if (t == typeof(string) || t.IsArray || t.IsEnum)
            {
                cell.SetCellValue(Convert.ToString(newValue));
            }
            else if (t == typeof(bool))
                cell.SetCellValue(Convert.ToBoolean(newValue));

            else
            {
                Debug.LogError($"Dont not Support Type {t}");
                cell.SetCellType(CellType.Blank);
            }


        }

        /// <summary>
        /// Convert type of cell value to its predefined type which is specified in the sheet's ScriptMachine setting file.
        /// </summary>
        protected void ConvertTo(Type t, CellData oldCell, ICell newCell)
        {

            if (t == typeof(float) || t == typeof(double) || t == typeof(short) || t == typeof(int) || t == typeof(long))
            {
                if (oldCell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                {
                    newCell.SetCellValue(oldCell.NumericCellValue);
                }
                else if (oldCell.CellType == NPOI.SS.UserModel.CellType.String)
                {
                    //Get correct numeric value even the cell is string type but defined with a numeric type in a data class.
                    if (t == typeof(float))
                        newCell.SetCellValue(Convert.ToSingle(oldCell.StringCellValue));
                    if (t == typeof(double))
                        newCell.SetCellValue(Convert.ToDouble(oldCell.StringCellValue));
                    if (t == typeof(short))
                        newCell.SetCellValue(Convert.ToInt16(oldCell.StringCellValue));
                    if (t == typeof(int))
                        newCell.SetCellValue(Convert.ToInt32(oldCell.StringCellValue));
                    if (t == typeof(long))
                        newCell.SetCellValue(Convert.ToInt64(oldCell.StringCellValue));
                }
                else if (oldCell.CellType == NPOI.SS.UserModel.CellType.Formula)
                {
                    // Get value even if cell is a formula
                    if (t == typeof(float))
                        newCell.SetCellValue(Convert.ToSingle(oldCell.NumericCellValue));
                    if (t == typeof(double))
                        newCell.SetCellValue(Convert.ToDouble(oldCell.NumericCellValue));
                    if (t == typeof(short))
                        newCell.SetCellValue(Convert.ToInt16(oldCell.NumericCellValue));
                    if (t == typeof(int))
                        newCell.SetCellValue(Convert.ToInt32(oldCell.NumericCellValue));
                    if (t == typeof(long))
                        newCell.SetCellValue(Convert.ToInt64(oldCell.NumericCellValue));
                }
            }
            else if (t == typeof(string) || t.IsArray || t.IsEnum)
            {
                // HACK: handles the case that a cell contains numeric value
                //       but a member field in a data class is defined as string type.
                //       e.g. string s = "123"
                if (oldCell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    newCell.SetCellValue(oldCell.NumericCellValue);
                else
                    newCell.SetCellValue(oldCell.StringCellValue);
            }
            else if (t == typeof(bool))
                newCell.SetCellValue(oldCell.BooleanCellValue);

            else if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                Debug.LogError("Don not support Generic Type Write");
                newCell.SetCellType(CellType.Blank);
                return;
            }
            else
            {
                Debug.LogError($"Dont not Support Type {t}");
                newCell.SetCellType(CellType.Blank);
            }


        }


        #endregion
    }


    public class CellData
    {
        public CellType CellType;
        public string StringCellValue;
        public double NumericCellValue;
        public bool BooleanCellValue;

        public static CellData GetCellData(ICell cell)
        {
            var newCell = new CellData();
            newCell.CellType = cell.CellType;

            if (cell.CellType == CellType.String)
                newCell.StringCellValue = cell.StringCellValue;
            else if (cell.CellType == CellType.Numeric)
                newCell.NumericCellValue = cell.NumericCellValue;
            else if (cell.CellType == CellType.Boolean)
                newCell.BooleanCellValue = cell.BooleanCellValue;
            return newCell;
        }

        public object GetValue()
        {
            if (CellType == CellType.String)
                return StringCellValue;
            else if (CellType == CellType.Numeric)
                return NumericCellValue;
            else if (CellType == CellType.Boolean)
                return BooleanCellValue;
            return null;
        }
    }

    public class RowData
    {
        private List<CellData> cells = new List<CellData>();
        public int Count => cells.Count;

        internal CellData GetCell(int k) => cells[k];

        public void AddCell(object obj, Type objType, bool isString = false)
        {
            CellData item = new CellData();
            cells.Add(item);
            SetCell(obj, objType, cells.Count - 1, isString);
        }

        public void SetCell(object obj, Type objType, int index, bool isString = false)
        {
            CellData item = cells[index];

            if (objType == typeof(string) || isString)
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


        public static RowData GetRowData(IRow row)
        {
            var newRow = new RowData();
            if (row != null)
            {
                for (var j = row.FirstCellNum; j < row.LastCellNum; j++)
                {
                    newRow.cells.Add(CellData.GetCellData(row.GetCell(j)));
                }
            }
            return newRow;
        }
    }

    public class HeaderData
    {
        public string header;
        public Type type;
        public string comment;
    }

}