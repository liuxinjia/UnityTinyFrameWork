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

    public class NewExcelWriter : ExcelQuery, IExcelWriter
    {
        private Dictionary<string, TableWriter> tableWriters = new Dictionary<string, TableWriter>();

        public NewExcelWriter(string path, int headerIndex = 0, int contentIndex = 1,
            char delimiter = ';') : base(path, headerIndex, contentIndex, delimiter)
        {
        }

        public void SaveExcels()
        {
            EditorUtil.Instance.ClearConsoleLog();
            
            if (File.Exists(filePath))
            {
                try
                {
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

                    string extension = UnityQuickSheet.ExcelQuery.GetSuffix(filePath);

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
                    Debug.Log("Wirte to excel successfully");
                }

            }
            catch (Exception e)
            {
                Debug.Log(e + ", " + e.StackTrace);
            }
            Application.OpenURL(filePath);
        }

        public TableWriter CreateTable(string sheetName, bool showColumnType = false, bool showID = true)
        {
            if (!tableWriters.ContainsKey(sheetName)) tableWriters.Add(sheetName, new TableWriter(filePath, sheetName, headerStartIndex, contentStartIndex, showColumnType, showID, delimiter));
            else Debug.LogError($"Already exist {sheetName}");
            return tableWriters[sheetName];
        }


    }
}