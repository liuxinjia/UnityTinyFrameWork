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
        private Dictionary<string, ExcelQuery> tableWriters = new Dictionary<string, ExcelQuery>();

        protected string filePath = string.Empty;
        protected int headerStartIndex = 0;
        protected int contentStartIndex = 2;
        protected char delimiter;

        public ExcelWriter(string path, int headerIndex = 0, int contentIndex = 1,
            char delimiter = ';')
        {
            this.filePath = path;
            this.headerStartIndex = headerIndex;
            this.contentStartIndex = contentIndex;
            this.delimiter = delimiter;
        }

        public void SaveExcels(bool openURL = false)
        {

            if (File.Exists(filePath))
            {
                try
                {
                    var reader = new ExcelReader(filePath, headerStartIndex, contentStartIndex, delimiter);
                    for (int i = 0; i < reader.sheetNames.Count; i++)
                    {
                        if (!tableWriters.ContainsKey(reader.sheetNames[i]))
                        {
                            tableWriters.Add(reader.sheetNames[i], reader.GetTableReader(reader.sheetNames[i]));
                        }

                    }
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    throw new Exception("Delete File: ", e);
                }
            }

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                           FileShare.ReadWrite))
                {
                    fileStream.Position = 0;
                    IWorkbook workbook = null;

                    string extension = EditorUtil.GetSuffix(filePath);

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

                    foreach (var item in tableWriters)
                    {
                        item.Value.WriteDatas(workbook);
                    }

                    workbook.Write(fileStream);
                }

            }
            catch (Exception e)
            {
                Debug.Log(e + ", " + e.StackTrace);
            }
            if (openURL) Application.OpenURL(filePath);
        }

        public TableWriter CreateTable(string sheetName, bool showColumnType = false, bool showID = true)
        {
            if (!tableWriters.ContainsKey(sheetName)) tableWriters.Add(sheetName, new TableWriter(filePath, sheetName, headerStartIndex, contentStartIndex, showColumnType, showID, delimiter));
            else Debug.LogError($"Already exist {sheetName}");
            return tableWriters[sheetName] as TableWriter;
        }


    }
}