namespace Cr7Sund.Async
{
    public class AwaiterResultable<TResult> : Awaiter, IAsyncResultable<TResult>
    {
        TResult _Result;
        public new TResult GetResult()
        {
            return _Result;
        }

        public override void Completed(IAsync result)
        {
            if (result is IAsyncResultable<TResult>)
                _Result = (result as IAsyncResultable<TResult>).GetResult();
            base.Completed(result);
        }
    }
}