using System;
using System.Linq;

namespace Cr7Sund.Editor.Excels
{
    public class TypeConverter
    {
        private readonly char DELIMITER = ',';

        public TypeConverter(char delimiter = ',')
        {
            DELIMITER = delimiter;
        }

        /// <summary>
        /// 
        /// </summary>
        public object[] Split(string value)
        {
            string str = value as string;

            // remove whitespace between each of element
            str = new string(str.ToCharArray()
                .Where(ch => !System.Char.IsWhiteSpace(ch))
                .ToArray());

            // remove DELIMITER, if it is found at the end.
            char[] charToTrim = { DELIMITER, ' ' };
            str = str.TrimEnd(charToTrim);

            // split by DELIMITER
            object[] temp = str.Split(DELIMITER);
            return temp;
        }

        /// <summary>
        /// Convert the given string to array of float. 
        /// Note the string should contain DELIMITER to separate each of array element.
        /// </summary>
        public float[] ToSingleArray(string value)
        {
            object[] temp = Split(value);
            float[] result = temp.Select(e => Convert.ChangeType(e, typeof(float)))
                .Select(e => (float)e).ToArray();
            //ERROR: InvalidCastException: Cannot cast from source type to destination type.
            //float[] result = temp.Select(e => (float)e).ToArray();
            return result;
        }

        /// <summary>
        /// Convert the given string to array of double. 
        /// </summary>
        public double[] ToDoubleArray(string value)
        {
            object[] temp = Split(value);
            double[] result = temp.Select(e => Convert.ChangeType(e, typeof(double)))
                .Select(e => (double)e).ToArray();
            return result;
        }

        /// <summary>
        /// Convert the given string to array of short. 
        /// </summary>
        public short[] ToInt16Array(string value)
        {
            object[] temp = Split(value);
            short[] result = temp.Select(e => Convert.ChangeType(e, typeof(short)))
                .Select(e => (short)e).ToArray();
            return result;
        }

        /// <summary>
        /// Convert the given string to array of int. 
        /// </summary>
        public int[] ToInt32Array(string value)
        {
            object[] temp = Split(value);
            int[] result = temp.Select(e => Convert.ChangeType(e, typeof(int)))
                .Select(e => (int)e).ToArray();
            return result;
        }

        /// <summary>
        /// Convert the given string to array of long. 
        /// </summary>
        public long[] ToInt64Array(string value)
        {
            object[] temp = Split(value);
            long[] result = temp.Select(e => Convert.ChangeType(e, typeof(long)))
                .Select(e => (long)e).ToArray();
            return result;
        }

        /// <summary>
        /// Convert the given string to array of long. 
        /// </summary>
        public string[] ToStringArray(string value)
        {
            object[] temp = Split(value);
            string[] result = temp.Select(e => Convert.ChangeType(e, typeof(string)))
                .Select(e => (string)e).ToArray();
            return result;
        }
    }
}