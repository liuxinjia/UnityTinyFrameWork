using System;
using System.Collections.Generic;

namespace Cr7Sund.Core
{
    public static class TypeUtil
    {
        public static string ToSQLDataType(Type type)
        {
            var sqlTypeMap = new Dictionary<Type, string>();
            sqlTypeMap.Add(typeof(bool), "BOOLEAN");
            sqlTypeMap.Add(typeof(byte), "TINYINT");
            sqlTypeMap.Add(typeof(short), "SMALLINT");
            sqlTypeMap.Add(typeof(int), "INT");
            sqlTypeMap.Add(typeof(long), "MEDIUMINT");

            sqlTypeMap.Add(typeof(float), "REAL");
            sqlTypeMap.Add(typeof(double), "DOUBLE");
            sqlTypeMap.Add(typeof(decimal), "NUMERIC");


            sqlTypeMap.Add(typeof(string), "TEXT");
            sqlTypeMap.Add(typeof(byte[]), "BLOB");
            sqlTypeMap.Add(typeof(object), string.Empty);

            sqlTypeMap.Add(typeof(DateTime), "DATETIME");

            if (sqlTypeMap.TryGetValue(type, out var sql)) return sql;
            else if (type.IsArray) return string.Empty;
            return string.Empty;
        }
    }
}