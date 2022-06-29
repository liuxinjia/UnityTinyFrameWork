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

    public class ExcelReader
    {
        private Dictionary<string, TableReader> tableReaders = new Dictionary<string, TableReader>();
        private string filePath = string.Empty;

        private char delimiter;
        public List<string> sheetNames = new List<string>();

        public ExcelReader(string path)
        {
            this.filePath = path;



            // need get all sheetnames
            try
            {
                using (FileStream fileStream =
                       new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fileStream.Position = 0;
                    IWorkbook workbook = null;

                    string extension = EditorUtil.GetSuffix(path);
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
                        workbook = new XSSFWorkbook(fileStream);
#endif
                    }
                    else
                    {
                        throw new Exception("Wrong file.");
                    }


                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                    {
                        sheetNames.Add(workbook.GetSheetName(i));
                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        public TableReader GetTableReader(string sheetName)
        {
            if (!tableReaders.ContainsKey(sheetName)) tableReaders.Add(sheetName, new TableReader(filePath, sheetName));
            return tableReaders[sheetName];
        }
    }


}