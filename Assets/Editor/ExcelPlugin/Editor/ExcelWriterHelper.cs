using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

namespace UnityQuickSheet
{
    public partial class ExcelWriterHelper
    {
        private IWorkbook workbook;
        private string fielPath;

        public ExcelWriterHelper(string fielPath)
        {
            string extension = ExcelQuery.GetSuffix(fielPath);

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
                throw new Exception("Wrong file.");
            }

            this.fielPath = fielPath;
        }

        public void WriteExcel(DataTable table,  string sheetName)
        {
            using (FileStream fs = new FileStream(fielPath, FileMode.Create, FileAccess.Write))
            {
                var excelSheet = workbook.CreateSheet(sheetName);
                List<String> columns = new List<string>();
                IRow row = excelSheet.CreateRow(0);
                int columnIndex = 0;

                foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    row.CreateCell(columnIndex).SetCellValue(column.ColumnName);
                    columnIndex++;
                }

                int rowIndex = 1;
                foreach (DataRow dsrow in table.Rows)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    foreach (String col in columns)
                    {
                        row.CreateCell(cellIndex).SetCellValue(dsrow[col].ToString());
                        cellIndex++;
                    }

                    rowIndex++;
                }

                workbook.Write(fs);
                
                workbook.Close();
            }
        }
    }
}