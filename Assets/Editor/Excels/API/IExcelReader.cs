using System.Collections.Generic;

namespace Editor.Excels
{
    public interface IExcelReader
    {
        List<object> GetRowsByID(int id);
    }
}