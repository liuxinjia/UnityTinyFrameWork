using System;

namespace Cr7Sund.Core
{
    public static class TypeUtil
    {
        public  static string ToSQLDataType( Type type)
        {
            if (typeof(string).IsAssignableFrom(type))
            {
                return "TEXT";
            }
            else if (typeof(int).IsAssignableFrom(type))
            {
                return "INT";
            }
            else if (typeof(float).IsAssignableFrom(type)
            || typeof(double).IsAssignableFrom(type))
            {
                return "NUMERIC";
            }
            return string.Empty;
        }
    }
}