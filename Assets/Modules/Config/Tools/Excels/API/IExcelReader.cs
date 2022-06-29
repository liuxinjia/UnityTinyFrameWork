using System.Collections.Generic;

namespace Cr7Sund.Editor.Excels
{
    public interface IExcelReader
    {
        List<object> GetRowsByID(int id);
        object GetCell(int id, string title);
    }
}