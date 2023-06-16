
namespace Cr7Sund.Async
{
    public class ResultAsync<TResult> : Async, IResultAsync<TResult>
    {

        public TResult Result { get; set; }

        public TResult GetResult()
        {
            return (TResult)Result;
        }
    }
}
