using System;

namespace Cr7Sund.Logger
{
    internal class MMFileOverflowException : Exception
    {
        public int Length;

        public MMFileOverflowException(int length, string message) : base(message)
        {
            this.Length = length;
        }
    }
}
