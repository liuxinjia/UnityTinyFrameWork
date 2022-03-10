using System.Collections.Generic;

namespace Cr7Sund.Editor.Excels
{
    public interface IExcelWriter
    {
        void SaveExcels(bool openURL);
        TableWriter CreateTable(string sheetName, bool showColumnType = false, bool shoID = true);
    }
}