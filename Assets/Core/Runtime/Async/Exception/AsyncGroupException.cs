using System;
using System.Collections.Generic;

namespace Cr7Sund.Async
{
    public class AsyncGroupException : Exception
    {
        public List<Exception> elementExceptions = new List<Exception>();

        public void AddException(Exception exception)
        {
            elementExceptions.Add(exception);
        }

        public void RemoveException(Exception exception)
        {
            elementExceptions.Remove(exception);
        }
    }
}