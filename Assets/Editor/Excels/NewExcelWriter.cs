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



    public class NewExcelWriter : ExcelQuery
    {

        Dictionary<string, TableWriter> tableWriters = new Dictionary<string, TableWriter>();

        public NewExcelWriter(string path, int headerIndex = 0, int contentIndex = 0,
            char delimiter = ';') : base(path, headerIndex, contentIndex, delimiter)
        {
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

                    foreach (var item in tableWriters)
                    {

                        item.Value.WriteDatas(workbook);
                    }

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
            Application.OpenURL(filePath);
        }



        public TableWriter AddTable(string sheetName)
        {
            if (!tableWriters.ContainsKey(sheetName)) tableWriters.Add(sheetName, new TableWriter(filePath, sheetName, headerStartIndex, contentStartIndex, delimiter));
            else Debug.LogError($"Already exist {sheetName}");
            return tableWriters[sheetName];
        }


    }
}