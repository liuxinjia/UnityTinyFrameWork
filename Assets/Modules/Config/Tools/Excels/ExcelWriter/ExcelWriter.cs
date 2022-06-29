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

    public class ExcelWriter : IExcelWriter
    {
        private readonly Dictionary<string, TableData> tableWriters = new();

        protected string filePath = string.Empty;

        protected char delimiter;

        public ExcelWriter(string path)
        {
            this.filePath = path;

        }

        public void SaveExcels(bool openURL = false)
        {

            if (File.Exists(filePath))
            {
                try
                {
                    var reader = new ExcelReader(filePath);
                    for (int i = 0; i < reader.sheetNames.Count; i++)
                    {
                        if (!tableWriters.ContainsKey(reader.sheetNames[i]))
                        {
                            TableReader existedTables = reader.GetTableReader(reader.sheetNames[i]);
                            existedTables.ConvertToRowDatas(); // convert original excel datas to custom datas
                            tableWriters.Add(reader.sheetNames[i], existedTables);
                        }
                        else
                        {
                            //overwrite table by default
                        }
                    }
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    throw new Exception($"Delete File: ", e);
                }
            }

            try
            {
                using FileStream fileStream = new(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                           FileShare.ReadWrite);
                fileStream.Position = 0;
                IWorkbook workbook = null;

                string extension = EditorUtil.GetSuffix(filePath);

                if (extension == "xls")
                {
                    throw new Exception(".xls format is obsolete");
                    // workbook = new HSSFWorkbook();
                }
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

                foreach (var item in tableWriters)
                {
                    item.Value.ConvertToTable(workbook);
                }

                workbook.Write(fileStream);

            }
            catch (Exception e)
            {
                throw new Exception($"Write File: ", e);
            }
            if (openURL) Application.OpenURL(filePath);
        }

        public TableWriter CreateTable(string sheetName)
        {
            tableWriters.Add(sheetName, new TableWriter(sheetName));
            return tableWriters[sheetName] as TableWriter;
        }


    }
}