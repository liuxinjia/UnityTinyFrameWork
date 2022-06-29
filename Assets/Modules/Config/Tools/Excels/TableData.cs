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
using System.Text;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;

namespace Cr7Sund.Editor.Excels
{
    public class TableData
    {

        #region  Fields

        #region Public Fields
        public string SheetName { get => sheetName; }

        #endregion

        #region  Private Properties
        protected List<HeaderData> headerList = new();
        protected List<RowData> rowDatas = new();
        private string sheetName;


        #endregion

        #region  Const Fields
        public const int HeaderStartIndex = 0;
        public const int ContentStartIndex = 4;
        public const char delimiter = ';';


        #endregion

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public TableData(string sheetName)
        {
            this.sheetName = sheetName;
        }


        #region Convert Datas
        internal void ConvertToTable(IWorkbook workbook)
        {
            var newSheet = workbook.CreateSheet(SheetName);

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

                    if (rowIndex < ContentStartIndex) // handle not -content type
                    {
                        if (rowIndex == HeaderStartIndex + 1 && columnIndex == 0)
                        {
                            cellData.StringCellValue = "Type";
                        }
                        else if (rowIndex == HeaderStartIndex && columnIndex == 0)
                        {
                            cellData.StringCellValue = "ID";
                        }
                        if (cellData.CellType != CellType.Blank && cellData.StringCellValue != null)
                            newCell.SetCellValue(cellData.StringCellValue);
                    }
                    else
                    {
                        ConvertTo(type, cellData, newCell);
                    }

                    newCell.CellComment = cellData.CellComment;

                    if (rowIndex == 0)
                    {
                        newCell.CreateComment(newSheet, headerInfo.comment);
                        newCell.SetCellStyle(workbook);
                    }

                }

                if (rowIndex == 1)
                {
                    newRow.SetRowStyle(workbook);
                }
            }
        }

        public DataTable ConvertToDataTable()
        {
            DataTable newDatable = new DataTable(SheetName);

            //Create columns
            foreach (var header in headerList)
            {
                if (header.type.IsArray) newDatable.Columns.Add(header.header, typeof(string));
                else newDatable.Columns.Add(header.header, header.type);
            }
            //Iterate through each sheet, sheet -> rows
            for (int rowIndex = ContentStartIndex; rowIndex < this.rowDatas.Count; rowIndex++)
            {
                var newRow = newDatable.NewRow();
                var rowData = this.rowDatas[rowIndex];

                for (int i = 0; i < headerList.Count; i++)
                {
                    HeaderData header = headerList[i];
                    newRow[header.header] = rowData.GetCell(i).GetValue();
                }

                newDatable.Rows.Add(newRow);

            }
            return newDatable;
        }

        /// <summary>
        /// 转换为Json
        /// </summary>
        /// <param name="JsonPath">Json文件路径</param>
        /// <param name="Header">表头行数</param>
        public void ConvertToJson(string JsonPath, Encoding encoding)
        {
            //默认读取第一个数据表
            DataTable mSheet = ConvertToDataTable();

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
                return;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < colCount; j++)
                {
                    //读取第1行数据作为表头字段
                    string field = mSheet.Columns[j].ToString();
                    //Key-Value对应
                    Type dataType = headerList[j].type;//  mSheet.Columns[j].DataType is string
                    if (dataType.IsArray)
                    {
                        row[field] = GetCellArrayValue(dataType.GetElementType(), mSheet.Rows[i][j].ToString());
                    }
                    else
                    {
                        row[field] = mSheet.Rows[i][j];
                    }

                }

                //添加到表数据中
                table.Add(row);
            }

            //生成Json字符串
            string json = JsonConvert.SerializeObject(table, Newtonsoft.Json.Formatting.Indented);
            //写入文件
            using (FileStream fileStream = new FileStream(JsonPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(json);
                }
            }
        }

        /// <summary>
        /// 导出为Xml
        /// </summary>
        public void ConvertToXml(string XmlFile)
        {
            //默认读取第一个数据表
            DataTable mSheet = ConvertToDataTable();

            // .Net.6 new feature
            // System.IO.StringWriter writer = new System.IO.StringWriter();
            // mSheet.WriteXML(writer,true);

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
                return;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //创建一个StringBuilder存储数据
            StringBuilder stringBuilder = new StringBuilder();
            //创建Xml文件头
            stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringBuilder.Append("\r\n");
            //创建根节点
            stringBuilder.Append("<Table>");
            stringBuilder.Append("\r\n");
            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //创建子节点
                stringBuilder.Append("  <Row>");
                stringBuilder.Append("\r\n");
                for (int j = 0; j < colCount; j++)
                {
                    stringBuilder.Append("   <" + mSheet.Columns[j].ToString() + ">");
                    string value = mSheet.Rows[i][j].ToString();
                    Type dataType = headerList[j].type;//  mSheet.Columns[j].DataType is string
                    if (dataType.IsArray)
                    {
                        value = $"[{value.Replace(delimiter, ',')}]";
                    }

                    stringBuilder.Append(value);

                    stringBuilder.Append("</" + mSheet.Columns[j].ToString() + ">");
                    stringBuilder.Append("\r\n");
                }
                //使用换行符分割每一行
                stringBuilder.Append("  </Row>");
                stringBuilder.Append("\r\n");
            }
            //闭合标签
            stringBuilder.Append("</Table>");
            //写入文件
            using (FileStream fileStream = new FileStream(XmlFile, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.GetEncoding("utf-8")))
                {
                    textWriter.Write(stringBuilder.ToString());
                }
            }
        }

        public void ConvertToSQL(string dbName){

        }
        
        #endregion

        #region HelpMethods

        /// <summary>
        /// Get Data type via string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Type GetMapDataTypes(string value)
        {
            if (value == "short") return typeof(short);
            if (value == "int") return typeof(int);
            if (value == "string") return typeof(string);
            if (value == "float") return typeof(float);
            // if (value == "double") return typeof(double);
            if (value == "string") return typeof(string);
            if (value == "boolean") return typeof(bool);

            if (value == "short[;]") return typeof(short[]);
            if (value == "int[;]") return typeof(int[]);
            if (value == "string[;]") return typeof(string[]);
            if (value == "float[;]") return typeof(float[]);
            // if (value == "double[;]") return typeof(double[]);
            if (value == "boolean[;]") return typeof(bool[]);
            return typeof(string);
        }

        public string GetSimplifyTypes(Type type)
        {
            if (type == typeof(short)) return "short";
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";

            if (type.IsArray && type.GetElementType() == typeof(short)) return "short[;]";
            if (type.IsArray && type.GetElementType() == typeof(int)) return "int[;]";
            if (type.IsArray && type.GetElementType() == typeof(float)) return "float[;]";
            if (type.IsArray && type.GetElementType() == typeof(bool)) return "boolean[;]";
            if (type.IsArray && type.GetElementType() == typeof(string)) return "string[;]";

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
        protected void ConvertTo(Type t, CellData oldCell, ICell newCell)
        {

            if (t == typeof(float) || t == typeof(double) || t == typeof(short) || t == typeof(int) || t == typeof(long))
            {
                newCell.SetCellType(CellType.Numeric);
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
            else if (t == typeof(string) || t.IsArray || t.IsEnum || t == typeof(Array))
            {
                newCell.SetCellType(CellType.String);

                // HACK: handles the case that a cell contains numeric value
                //       but a member field in a data class is defined as string type.
                //       e.g. string s = "123"
                if (oldCell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    newCell.SetCellValue(oldCell.NumericCellValue);
                else
                    newCell.SetCellValue(oldCell.StringCellValue);
            }
            else if (t == typeof(bool))
            {
                newCell.SetCellType(CellType.Boolean);
                newCell.SetCellValue(oldCell.BooleanCellValue);
            }
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

        protected object[] GetCellArrayValue(Type elementType, string arrayValue)
        {
            var arrayList = new List<object>();
            var arrays = arrayValue.Split(delimiter);
            foreach (var arrayItem in arrays)
            {
                arrayList.Add(Convert.ChangeType(arrayItem, elementType));
            }

            return arrayList.ToArray();
        }

        public void GetHeaderDatas(out List<string> columnNames, out List<Type> columnTypes)
        {
            columnNames = new List<string>();
            columnTypes = new List<Type>();
            foreach (var header in headerList)
            {
                columnNames.Add(header.header);
                columnTypes.Add(header.type);
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
        public IComment CellComment;

        public static CellData GetCellData(ICell cell)
        {
            var newCell = new CellData();
            newCell.CellType = cell.CellType;
            newCell.CellComment = cell.CellComment;
            // newCell.cellStyle = cell.CellStyle; // Can't  do that babay  https://github.com/dromara/hutool/issues/432

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

        public void AddCell(object obj, Type objType, char delimiter, bool isString = false)
        {
            CellData item = new CellData();
            cells.Add(item);
            SetCell(obj, objType, cells.Count - 1, delimiter, isString);
        }


        public void SetCell(object obj, Type objType, int index, char delimiter, bool isString = false)
        {
            CellData item = cells[index];

            if (objType == typeof(string) || isString)
            {
                item.CellType = CellType.String;
                item.StringCellValue = obj.ToString();
            }
            else if (objType.IsArray)
            {
                item.CellType = CellType.String;
                item.StringCellValue = ConvertArrayToStrs(obj, delimiter);
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

        public static string ConvertArrayToStrs(object obj, char delimiter)
        {
            var sb = new StringBuilder();
            System.Collections.IList list = (Array)obj;
            for (int i = 0; i < list.Count; i++)
            {
                object arrayItem = list[i];
                string k = i == list.Count - 1 ? "" : delimiter.ToString();
                sb.Append($"{arrayItem}{k}");
            }

            return sb.ToString();
        }
    }

    public class HeaderData
    {
        public string header;
        public Type type;
        public string comment;
    }

}